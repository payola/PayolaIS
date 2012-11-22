using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class CompanyModel : EntityModel<Company>
    {
        #region Constructors

        public CompanyModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Company> Methods

        protected override IQueryable<Company> FilterEntitiesByNeedle (IQueryable<Company> entities, string needle)
        {
            return entities.Where (c => c.Name.Contains (needle) || c.Note.Contains (needle));
        }

        protected override IQueryable<Company> FilterEntitiesBySimilarity (IQueryable<Company> entities, Company patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Name);
        }

        #endregion
    }
}
