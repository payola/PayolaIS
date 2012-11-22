using System.Data.Objects.SqlClient;
using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class PhoneModel : EntityModel<Phone>
    {
        #region Constructors

        public PhoneModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Phone> Methods

        protected override IQueryable<Phone> FilterEntitiesByNeedle (IQueryable<Phone> entities, string needle)
        {
            return entities.Where (p => SqlFunctions.StringConvert ((decimal) p.Number).Contains (needle));
        }

        #endregion
    }
}
