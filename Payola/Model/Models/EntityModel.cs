using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Payola.DomainModel;

namespace Payola.Model
{
    public class EntityModel<TEntity> : ModelBase, IEntityModel
        where TEntity : Entity, new ()
    {
        #region Constructors

        public EntityModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region Properties

        protected virtual IQueryable<TEntity> EntitiesQueryable
        {
            get
            {
                return Db.Entities.OfType<TEntity> ();
            }
        }

        #endregion

        #region Methods

        public IEnumerable<TEntity> GetEntities ()
        {
            return EntitiesQueryable.Where (e => !e.IsDeleted);
        }

        public IEnumerable<TEntity> GetEntities (IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any ())
            {
                return Enumerable.Empty<TEntity> ();
            }
            return EntitiesQueryable.Where (e => !e.IsDeleted && ids.Contains (e.Id));
        }

        public IEnumerable<TEntity> GetEntities (string needle)
        {
            return GetEntities (Enumerable.Empty<long> (), needle);
        }

        public IEnumerable<TEntity> GetEntities (IEnumerable<long> excludeIds, string needle)
        {
            IEnumerable<string> words = ParseNeedle (needle);
            if (!words.Any ())
            {
                return new List<TEntity> ();
            }

            IQueryable<TEntity> entities = EntitiesQueryable.Where (e => !e.IsDeleted);
            if (excludeIds != null && excludeIds.Any ())
            {
                entities = entities.Where (e => !excludeIds.Contains (e.Id));
            }
            foreach (string word in words)
            {
                entities = FilterEntitiesByNeedle (entities, word);
            }
            return entities;
        }

        public IEnumerable<TEntity> GetEntities (TEntity patternEntity)
        {
            return FilterEntitiesBySimilarity (EntitiesQueryable.Where (e => !e.IsDeleted), patternEntity);
        }

        public TEntity GetEntity (long id)
        {
            return EntitiesQueryable.Where (e => e.Id == id && !e.IsDeleted).FirstOrDefault ();
        }

        public TEntity GetNewEntity ()
        {
            return new TEntity ();
        }

        public TEntity GetNewEntity (string identification)
        {
            return (TEntity) Activator.CreateInstance (typeof (TEntity), identification);
        }

        /// <summary>
        /// Adds the specified entity and returns the added entity. The return value is used to return already existing
        /// entity in case the insertion of the new entity doesen't make sense (adding a new entity that is identical with 
        /// some other entity doesen't make sense, it's better to simply return the existing entity).
        /// </summary>
        /// <param name="entity">The netity to add.</param>
        /// <returns>The added entity or already existing identical entity.</returns>
        public virtual TEntity AddEntity (TEntity entity)
        {
            Db.Entities.Add (entity);
            Db.SaveChanges ();
            return entity;
        }

        public virtual void SaveEntity (TEntity entity)
        {
            entity.ModifiactionCount = entity.ModifiactionCount + 1;
            entity.LastModificationDate = DateTime.Now;
            Db.Entry (entity).State = EntityState.Modified;
            Db.SaveChanges ();
        }

        public void DeleteEntity (long entityId)
        {
            TEntity entity = GetEntity (entityId);
            if (entity != null)
            {
                DeleteEntity (entity);
            }
        }

        public void DeleteEntity (TEntity entity)
        {
            if (!entity.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.DeletionDate = DateTime.Now;
                SaveEntity (entity);
            }
        }

        protected virtual IQueryable<TEntity> FilterEntitiesByNeedle (IQueryable<TEntity> entities, string needle)
        {
            throw new NotImplementedException ();
        }

        protected virtual IQueryable<TEntity> FilterEntitiesBySimilarity (IQueryable<TEntity> entities, TEntity patternEntity)
        {
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Note);
        }

        protected IQueryable<TEntity> FilterEntitiesByPropertyValue (
            IQueryable<TEntity> entities, 
            TEntity patternEntity,
            Expression<Func<TEntity, string>> propertySelector)
        {
            PropertyInfo propertyInfo = (PropertyInfo) ((MemberExpression) propertySelector.Body).Member;
            string propertyValue = (string) propertyInfo.GetValue (patternEntity, null);
            if (!string.IsNullOrWhiteSpace (propertyValue))
            {
                MethodInfo containsInfo = typeof (string).GetMethod ("Contains", new Type[] { typeof (string) });
                ParameterExpression parameter = Expression.Parameter (typeof (TEntity));
                Expression propertyAccess = Expression.Property (parameter, propertyInfo);
                Expression predicate = Expression.Call (propertyAccess, containsInfo, Expression.Constant (propertyValue));
                entities = entities.Where (Expression.Lambda<Func<TEntity, bool>> (predicate, parameter));
            }
            return entities;
        }

        private static IEnumerable<string> ParseNeedle (string needle)
        {
            return needle.Split (new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region IEntityModel Methods

        public Entity GetEntityById (long id)
        {
            return GetEntity (id);
        }

        public IEnumerable<Entity> GetEntitiesByIds (IEnumerable<long> ids)
        {
            return GetEntities (ids);
        }

        public IEnumerable<Entity> GetEntitiesByIdsAndNeedle (IEnumerable<long> excludeIds, string needle)
        {
            return GetEntities (excludeIds, needle);
        }

        public Entity GetNewEntityWithIdentification (string identification)
        {
            return GetNewEntity (identification);
        }

        public Entity AddNewEntity (Entity entity)
        {
            return AddEntity ((TEntity) entity);
        }

        #endregion
    }
}
