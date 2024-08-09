using Server.Context;
namespace Util;


/* Denne er byttet ut med ConflictHandler */
public class Entity
{
    public static bool Exists<T>(T entity, BtdbContext context) where T : class
    {
        var trackedEntity = context.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == entity);
        return trackedEntity != null;
    }
}