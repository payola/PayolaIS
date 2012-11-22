using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class InformativeEntityModel<TEntity> : EntityModel<TEntity>
        where TEntity : InformativeEntity, new ()
    {
        #region Constructors

        public InformativeEntityModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region Methods

        public void Publish (TEntity entity, Publication publication)
        {
            if (entity.IsPublishable)
            {
                Db.Publications.Add (publication);
                entity.Publications.Add (publication);
                Db.SaveChanges ();
            }
        }

        protected override IQueryable<TEntity> FilterEntitiesBySimilarity (IQueryable<TEntity> entities, TEntity patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Annotation);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Name);
        }

        #endregion
    }
}
