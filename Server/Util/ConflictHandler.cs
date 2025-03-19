namespace Server.Util;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Context;
public class UpsertHandler
{
    ///  <summary>
    ///  Creates a lamdafunction to find and update values in a potential duplicate tracked by EF.
    ///  This spesifically only works when checking towards the Database, not on EF itself.
    ///  </summary>
    /// <param name="context">
    /// Database Context
    /// </param>
    /// <param name="entity">
    /// An entity that potentially caused a DbUpdateException
    /// </param>
    /// <param name="primaryKeys">
    /// A list of property names representing the primary keys of the entity in EF
    /// </param>
    /// <param name="equalityKeys">
    /// A list of property names representing the keys that should potentially be updated in an existing entity
    /// </param>
    ///  <param name="detachOnly"></param>
    ///  <exception cref="NullReferenceException">
    /// If it fails to create a a Lamda predicate based on the primary keys in question on class T
    /// </exception>
    public static async Task UpsertEntity<T>(BtdbContext context, T entity, bool? detachOnly = false) where T : class
    {
        context.Entry(entity).State = EntityState.Detached;
        if (detachOnly == true) return;
        var entityType = context.Model.FindEntityType(typeof(T)) ?? throw new NullReferenceException($"Could not find an entity of type {entity}");
        var primaryKeys = entityType.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList() ?? throw new NullReferenceException($"Could not find the primary keys of type {entity}");
        var equalityKeys = entityType.GetProperties().Where(p => !primaryKeys.Contains(p.Name)).Select(p => p.Name).ToList();
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? predicate = null;
        foreach (var key in primaryKeys)
        {
            var property = Expression.Property(parameter, key);
            var propertyInfo = typeof(T).GetProperty(key);
            var propertyType = propertyInfo?.PropertyType;
            var value = Expression.Constant(propertyInfo?.GetValue(entity));
            value ??= Expression.Constant(null);
            var equals = Expression.Equal(property, value);
            predicate = predicate == null ? equals : Expression.AndAlso(predicate, equals);
        }
        if (predicate == null) throw new NullReferenceException("Could not build predicate for the lamda function.");
        var lamda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
        var existingEntity = await context.Set<T>().SingleOrDefaultAsync(lamda);
        if (existingEntity == null)
        {
            try
            {
                await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();
                return;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Failed to update reference, detatching entity: {ex.Message}");
            }
        }
        foreach (var key in equalityKeys)
        {
            var propInfo = typeof(T).GetProperty(key);
            if (propInfo == null || !propInfo.CanWrite) continue;
            else
            {
                try
                {
                    propInfo.SetValue(existingEntity, typeof(T)?.GetProperty(key)?.GetValue(entity));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong setting property {key} in {entity}, {ex.Message}");
                    throw;
                }
            }
        }
        try
        {
            await context.SaveChangesAsync();
            return;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception($"Failed to update reference: {ex.Message}");
        }
    }
    /// <summary>
    /// This function makes sure the previous list insert doesn't cause conflicts when trying to insert a handled entity. 
    /// Since all entities in the failed list insert also gets handled by this function it makes sure each situation is handled in order and is added one at a time. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="entity"></param>
    private static void DetachUnwantedEntities<T>(BtdbContext context, T entity) where T : class
    {
        var entries = context.ChangeTracker.Entries().Where(e => e.Entity != entity && e.State != EntityState.Detached).ToList();
        foreach (var entry in entries)
        {
            entry.State = EntityState.Detached;
        }
    }
}