using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.Database.Util
{
    internal static class DbSetExtensions
    {
        internal static TEntity AddOrUpdate<TDbContext, TEntity>(this DbSet<TEntity> dbSet, TDbContext context, TEntity entity) where TDbContext : DbContext where TEntity : class
        {
            var primaryKeyName = context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties
                .Select(x => x.Name).Single();

            var primaryKeyField = entity.GetType().GetProperty(primaryKeyName);

            var t = typeof(TEntity);
            if (primaryKeyField == null)
                throw new Exception($"{t.FullName} does not have a primary key specified. Unable to AddOrUpdate.");

            var keyVal = primaryKeyField.GetValue(entity);
            var dbVal = dbSet.Find(keyVal);

            if (dbVal == null)
            {
                dbSet.Add(entity);
                return entity;
            }

            context.Entry(dbVal).CurrentValues.SetValues(entity);
            dbSet.Update(dbVal);
            return dbVal;
        }
    }
}