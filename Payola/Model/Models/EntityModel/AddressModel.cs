using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class AddressModel : EntityModel<Address>
    {
        #region Constructors

        public AddressModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Address> Methods

        protected override IQueryable<Address> FilterEntitiesByNeedle (IQueryable<Address> entities, string needle)
        {
            return entities.Where (a => a.Street.Contains (needle) || a.City.Contains (needle) || a.Note.Contains (needle));
        }

        protected override IQueryable<Address> FilterEntitiesBySimilarity (IQueryable<Address> entities, Address patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Street);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.City);
        }

        #endregion
    }
}
