using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Payola.DomainModel;

namespace Payola.Model
{
    public class RelationModel : ModelBase
    {
        #region Fields

        private Dictionary<Type, Dictionary<long, PredefinedRelationType>> predefinedRelationTypes =
            new Dictionary<Type, Dictionary<long, PredefinedRelationType>> ();

        private Dictionary<Type, Dictionary<long, RelationType>> genericRelationTypes =
            new Dictionary<Type, Dictionary<long, RelationType>> ();

        #endregion

        #region Constructors

        public RelationModel (PayolaContext db)
            : base (db)
        {
            foreach (RelationType relationType in Db.RelationTypes)
            {
                AddGenericRelationType (relationType);

                // If the relation type is symmetric, it represents both the relations and inverse relations. Otherwise
                // the inverse relation type is different from the realtion type so it has to be added.
                if (!relationType.IsSymmetric)
                {
                    AddGenericRelationType (relationType.InverseRelationType);
                }
            }
        }

        #endregion

        #region Methods

        public Entity GetNewObjectiveEntity (RelationType relationType, string identification)
        {
            IEntityModel entityModel = RemoteFacade.Instance.GetEntityModel (relationType.ObjectiveEntityType, Db);
            return entityModel.GetNewEntityWithIdentification (identification);
        }

        public Relation GetRelation (long relationId)
        {
            return Db.Relations.Where (r => r.Id == relationId).FirstOrDefault ();
        }

        public IEnumerable<TypedRelations> GetRelations (Entity entity)
        {
            List<TypedRelations> relations = new List<TypedRelations> ();
            foreach (PredefinedRelationType relationType in GetPredefinedRelationTypes (entity).Values)
            {
                relations.Add (GetPredefinedRelations (relationType, entity));
            }
            foreach (RelationType relationType in GetGenericRelationTypes (entity).Values)
            {
                relations.Add (GetGenericRelations (relationType, entity));
            }
            return relations;
        }

        public TypedRelations GetRelations (long relationTypeId, Entity entity)
        {
            RelationType relationType = GetRelationType (relationTypeId, entity);
            if (relationType == null)
            {
                return null;
            }

            PredefinedRelationType predefinedRelationType = relationType as PredefinedRelationType;
            if (predefinedRelationType != null)
            {
                return GetPredefinedRelations (predefinedRelationType, entity);
            }
            return GetGenericRelations (relationType, entity);
        }

        public IEnumerable<Entity> GetRelatableEntities (long relationTypeId, Entity entity, string needle)
        {
            RelationType relationType = GetRelationType (relationTypeId, entity);
            if (relationType == null)
            {
                return Enumerable.Empty<Entity> ();
            }

            // Reflexive relations aren't supported so the entity id is initially added to the unrelatable entity ids.
            List<long> unrelatableEntityIds = new List<long> (new long[] { entity.Id });

            PredefinedRelationType predefinedRelationType = relationType as PredefinedRelationType;
            if (predefinedRelationType != null)
            {
                unrelatableEntityIds.AddRange (predefinedRelationType.GetRelatedEntities (entity).Select<Entity, long> (e => e.Id));
            }

            // Return the entities that are not unrelatable.
            IEntityModel entityModel = RemoteFacade.Instance.GetEntityModel (relationType.ObjectiveEntityType, Db);
            return entityModel.GetEntitiesByIdsAndNeedle (unrelatableEntityIds, needle);
        }

        public void AddRelation (long relationTypeId, Entity entity, long relatedEntityId)
        {
            RelationType relationType = GetRelationType (relationTypeId, entity);
            if (relationType == null)
            {
                return;
            }
            Entity relatedEntity = RemoteFacade.Instance.GetEntityModel (relationType.ObjectiveEntityType, Db).GetEntityById (relatedEntityId);
            if (relatedEntity == null)
            {
                return;
            }

            PredefinedRelationType predefinedRelationType = relationType as PredefinedRelationType;
            if (predefinedRelationType != null)
            {
                AddPredefinedRelation (predefinedRelationType, entity, relatedEntity);
            }
            else
            {
                AddGenericRelation (relationType, entity, relatedEntity);
            }
        }

        public virtual void SaveRelation (Relation relation)
        {
            Db.Entry (relation).State = EntityState.Modified;
            Db.SaveChanges ();
        }

        /// <summary>
        /// Deletes predefined or generic relation.
        /// </summary>
        /// <param name="relationTypeId">Id of the relation type.</param>
        /// <param name="entity">The entity whose relation should be removed</param>
        /// <param name="relationId">
        /// Id of the relation in case of generic relation type. Id of the related entity if the relation is predefined.
        /// </param>
        public void DeleteRelation (long relationTypeId, Entity entity, long relationId)
        {
            RelationType relationType = GetRelationType (relationTypeId, entity);
            if (relationType == null)
            {
                return;
            }

            PredefinedRelationType predefinedRelationType = relationType as PredefinedRelationType;
            if (predefinedRelationType != null)
            {
                IEnumerable<Entity> relatedEntities = predefinedRelationType.GetRelatedEntities (entity);
                long relatedEntityId = relationId;
                Entity relatedEntity = relatedEntities.FirstOrDefault (e => e.Id == relatedEntityId);
                if (relatedEntity != null)
                {
                    // Even though the type of objectiveEntities is IEnumerable<Entity>, it is sure that it is also
                    // ICollection<relationType.ObjectiveEntityType> so the Remove method is surely found.
                    MethodInfo remove = relatedEntities.GetType ().GetMethod ("Remove");
                    remove.Invoke (relatedEntities, new object[] { relatedEntity });
                    Db.SaveChanges ();
                }
            }
            else
            {
                Relation relation = GetGenericRelation (relationType, relationId);
                if (relation != null)
                {
                    Db.Relations.Remove (relation);
                    Db.SaveChanges ();
                }
            }
        }

        private Dictionary<long, PredefinedRelationType> GetPredefinedRelationTypes (Entity entity)
        {
            Type entityType = Entity.GetEntityType (entity);
            if (!predefinedRelationTypes.ContainsKey (entityType))
            {
                predefinedRelationTypes[entityType] = new Dictionary<long, PredefinedRelationType> ();

                // Find all the properties that are marked as predefined relations.
                foreach (PropertyInfo property in entityType.GetProperties ())
                {
                    object[] attributes = property.GetCustomAttributes (typeof (PredefinedRelationAttribute), false);
                    if (attributes.Any ())
                    {
                        // The property represents predefined relation, take just the first attribute.
                        PredefinedRelationAttribute attribute = (PredefinedRelationAttribute) attributes[0];
                        long relationTypeId = (long) attribute.RelationTypeValue;

                        // We know that the attribute is applied on ICollection<TEntity>.
                        Type objectiveEntityType = property.PropertyType.GetGenericArguments ()[0];

                        predefinedRelationTypes[entityType][relationTypeId] = new PredefinedRelationType (property)
                        {
                            Id = relationTypeId,
                            Name = attribute.RelationTypeValue.ToString (),
                            SubjectiveEntityType = entityType,
                            ObjectiveEntityType = objectiveEntityType
                        };
                    }
                }
            }

            return predefinedRelationTypes[entityType];
        }

        private Dictionary<long, RelationType> GetGenericRelationTypes (Entity entity)
        {
            Type entityType = Entity.GetEntityType (entity);
            if (genericRelationTypes.ContainsKey (entityType))
            {
                return genericRelationTypes[entityType];
            }
            return new Dictionary<long, RelationType> ();
        }

        private void AddGenericRelationType (RelationType relationType)
        {
            if (!genericRelationTypes.ContainsKey (relationType.SubjectiveEntityType))
            {
                genericRelationTypes[relationType.SubjectiveEntityType] = new Dictionary<long, RelationType> ();
            }
            genericRelationTypes[relationType.SubjectiveEntityType][relationType.Id] = relationType;
        }

        private RelationType GetRelationType (long relationTypeId, Entity entity)
        {
            if (GetPredefinedRelationTypes (entity).ContainsKey (relationTypeId))
            {
                return GetPredefinedRelationTypes (entity)[relationTypeId];
            }
            else if (GetGenericRelationTypes (entity).ContainsKey (relationTypeId))
            {
                return GetGenericRelationTypes (entity)[relationTypeId];
            }
            return null;
        }

        private TypedRelations GetPredefinedRelations (PredefinedRelationType relationType, Entity entity)
        {
            IEnumerable<Entity> relatedEntities = relationType.GetRelatedEntities (entity).Where (e => !e.IsDeleted);
            return new TypedRelations (relationType)
            {
                Relations = relatedEntities.ToDictionary<Entity, Relation, Entity> (e => new Relation (), e => e)
            };
        }

        private TypedRelations GetGenericRelations (RelationType relationType, Entity entity)
        {
            Expression<Func<Relation, bool>> relationSelector = r => r.SubjectiveEntityId == entity.Id;
            if (relationType.IsSymmetric)
            {
                relationSelector = r => r.SubjectiveEntityId == entity.Id || r.ObjectiveEntityId == entity.Id;
            }
            else if (relationType.IsInverse)
            {
                relationSelector = r => r.ObjectiveEntityId == entity.Id;
            }

            // The relationType.Id may be nagative in case of inverse relations, but it is positive in the database.
            IQueryable<Relation> relationsQuery = Db.Relations.Where (r => r.RelationTypeId == Math.Abs (relationType.Id));
            relationsQuery = relationsQuery.Where (relationSelector);

            // Retrieve all the relations and store them into the helper structure.
            Dictionary<long, List<Relation>> relationsByRelatedEntityIds = new Dictionary<long, List<Relation>> ();
            foreach (Relation relation in relationsQuery)
            {
                long relatedEntityId = relation.SubjectiveEntityId == entity.Id ? relation.ObjectiveEntityId : relation.SubjectiveEntityId;
                if (!relationsByRelatedEntityIds.ContainsKey (relatedEntityId))
                {
                    relationsByRelatedEntityIds[relatedEntityId] = new List<Relation> ();
                }
                relationsByRelatedEntityIds[relatedEntityId].Add (relation);
            }

            TypedRelations genericRelations = new TypedRelations (relationType);
            IEntityModel entityModel = RemoteFacade.Instance.GetEntityModel (relationType.ObjectiveEntityType, Db);
            foreach (Entity relatedEntity in entityModel.GetEntitiesByIds (relationsByRelatedEntityIds.Keys))
            {
                foreach (Relation relation in relationsByRelatedEntityIds[relatedEntity.Id])
                {
                    genericRelations.Relations[relation] = relatedEntity;
                }
            }
            return genericRelations;
        }

        private Relation GetGenericRelation (RelationType relationType, long relationId)
        {
            return Db.Relations.Where (r => r.Id == relationId && r.RelationTypeId == relationType.Id).FirstOrDefault ();
        }

        private void AddPredefinedRelation (PredefinedRelationType relationType, Entity entity, Entity relatedEntity)
        {
            IEnumerable<Entity> relatedEntities = relationType.GetRelatedEntities (entity);
            if (!relatedEntities.Contains (relatedEntity))
            {
                // Even though the type of objectiveEntities is IEnumerable<Entity>, it is sure that it is also
                // ICollection<relationType.ObjectiveEntityType> so the Add method is surely found.
                MethodInfo add = relatedEntities.GetType ().GetMethod ("Add");
                add.Invoke (relatedEntities, new object[] { relatedEntity });
                Db.SaveChanges ();
            }
        }

        private void AddGenericRelation (RelationType relationType, Entity entity, Entity relatedEntity)
        {
            Db.Relations.Add (new Relation ()
            {
                // The relationType may be inverse so the relation type id may be negative.
                RelationTypeId = Math.Abs (relationType.Id),
                SubjectiveEntity = relationType.IsInverse ? relatedEntity : entity,
                ObjectiveEntity = relationType.IsInverse ? entity : relatedEntity
            });
            Db.SaveChanges ();
        }

        #endregion
    }
}
