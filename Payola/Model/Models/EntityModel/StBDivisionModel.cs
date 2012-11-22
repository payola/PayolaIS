using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class StbDivisionModel : EntityModel<StbDivision>
    {
        #region Constructors

        public StbDivisionModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<StbCommandCenter> Methods

        protected override IQueryable<StbDivision> FilterEntitiesByNeedle (IQueryable<StbDivision> entities, string needle)
        {
            return entities.Where (d => d.Name.Contains (needle) || d.Note.Contains (needle));
        }

        protected override IQueryable<StbDivision> FilterEntitiesBySimilarity (IQueryable<StbDivision> entities, StbDivision patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Name);
        }

        #endregion
    }
}
