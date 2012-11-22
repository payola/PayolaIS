using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class VehicleModel : EntityModel<Vehicle>
    {
        #region Constructors

        public VehicleModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Vehicle> Methods

        protected override IQueryable<Vehicle> FilterEntitiesByNeedle (IQueryable<Vehicle> entities, string needle)
        {
            return entities.Where (v => v.Brand.Contains (needle) || v.Color.Contains (needle) ||
                v.LicenseNumber.Contains (needle) || v.Manufacturer.Contains (needle) || v.Note.Contains (needle));
        }

        protected override IQueryable<Vehicle> FilterEntitiesBySimilarity (IQueryable<Vehicle> entities, Vehicle patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Brand);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Color);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.LicenseNumber);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Manufacturer);
        }

        #endregion
    }
}
