namespace Util;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Server.Models;
public class ConflictHandler
{
    /// <summary>
    /// Creates a lamdafunction to find and update values in a potential duplicate tracked by EF.
    /// </summary>
    ///<param name="context">
    ///Database Context
    ///</param>
    ///<param name="entity">
    ///An entity that potentially caused a DbUpdateException
    ///</param>
    ///<param name="primaryKeys">
    ///A list of property names representing the primary keys of the entity in EF
    ///</param>
    ///<param name="equalityKeys">
    ///A list of property names representing the keys that should potentially be updated in an existing entity
    ///</param>
    ///<exception cref="NullReferenceException">
    ///If it fails to create a a Lamda predicate based on the primary keys in question on class T
    ///</exception>
    public static async Task HandleConflicts<T>(BtdbContext context, T entity, List<string> primaryKeys, List<string>? equalityKeys) where T : class
    {
        if (equalityKeys == null || equalityKeys.Count == 0) return;
        context.Entry(entity).State = EntityState.Detached;
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? predicate = null;
        foreach (var key in primaryKeys)
        {
            var property = Expression.Property(parameter, key);
            var value = Expression.Constant(typeof(T)?.GetProperty(key)?.GetValue(entity));
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
            catch (Exception ex)
            {
                Console.WriteLine($"Something else than duplicate data happened, {ex.Message}");
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
                }
            }
        }
        await context.SaveChangesAsync();
        return;
    }
}