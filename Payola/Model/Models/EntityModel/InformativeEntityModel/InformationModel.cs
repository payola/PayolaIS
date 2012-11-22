using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class InformationModel : InformativeEntityModel<Information>
    {
        #region Constructors

        public InformationModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region InformativeEntityModel<Information> Methods

        protected override IQueryable<Information> FilterEntitiesByNeedle (IQueryable<Information> entities, string needle)
        {
            return entities.Where (e => e.Annotation.Contains (needle) || e.Text.Contains (needle) ||
                e.Comment.Contains (needle) || e.Name.Contains (needle) || e.Note.Contains (needle));
        }

        protected override IQueryable<Information> FilterEntitiesBySimilarity (IQueryable<Information> entities, Information patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Text);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Comment);
        }

        #endregion
    }
}
