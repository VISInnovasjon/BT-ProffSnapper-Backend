namespace Util;
using System.Linq.Expressions;
using Server.Models;
public class ConflictHandler
{
    /* Wild lamda function to handle duplication errors in incoming data from outside api.

   This takes in your dbContext, the entity that caused a dbUpdateException, a list of primarykeys for the entity, and a list of keys if you want to update the existing data with the new data. 

   You can leave equalityKeys as an empty list or leave it out entirely if you don't want to update conflicting data.   */
    public static void HandleConflicts<T>(BtdbContext context, T entity, List<string> primaryKeys, List<string>? equalityKeys) where T : class
    {
        /* If we don't add any equality keys we don't want to change the existing data, so we return, not bothering to change anything. */
        if (equalityKeys == null || equalityKeys.Count == 0) return;
        /* We begin by setting up an equality lamda expression so we can find the conflicting entity 
        In this first part we define our comparisonparameter. We can call it whatever, but we call it x. Then we say it's of type T, the same class as our entity.
        
        we then define our predicate. */
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? predicate = null;
        /* We begin looping through each of our primary keys. */
        foreach (var key in primaryKeys)
        {
            /* we get the property expression f.ex. x.Id. */
            var property = Expression.Property(parameter, key);
            /* We then get the value it's supposed to be equal to, if we follow the above example it would be entity.Id */
            var value = Expression.Constant(typeof(T)?.GetProperty(key)?.GetValue(entity));
            /* Here we define the equality expression, which if we follow the above example would be x.Id == entity.Id */
            var equals = Expression.Equal(property, value);
            /* Here we add the equality expression to our predicate. */
            predicate = predicate == null ? equals : Expression.AndAlso(predicate, equals);
        }
        /* If we could not build a predicate for our lamda function for whatever reason. We throw an error  */
        if (predicate == null) throw new NullReferenceException("Could not build predicate for the lamda function.");
        /* This leaves us with a function defining all equality expressions in our lamda function
        and allows us to find an entity in our EF core setup that shares primary keys with our conflicting entity.  */
        var lamda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
        var existingEntity = context.Set<T>().SingleOrDefault(lamda);
        /* If none entity is found we throw an error, something else has gone wrong with the search. and it's not a conflict in entities. */
        if (existingEntity == null)
        {
            try
            {
                context.Set<T>().Add(entity);
            }
            catch (Exception secondEx)
            {
                Console.WriteLine($"Something else than duplicate data happened, {secondEx.Message}");
            }
        }
        /* Here we update our existing entity with the new values in the conflicting entity. */
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
        /* At the end we return the updated entity.  */
        return;
    }
}