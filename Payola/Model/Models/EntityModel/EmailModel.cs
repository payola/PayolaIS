using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class EmailModel : EntityModel<Email>
    {
        #region Constructors

        public EmailModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Email> Methods

        protected override IQueryable<Email> FilterEntitiesByNeedle (IQueryable<Email> entities, string needle)
        {
            return entities.Where (e => e.Address.Contains (needle) || e.Note.Contains (needle));
        }

        protected override IQueryable<Email> FilterEntitiesBySimilarity (IQueryable<Email> entities, Email patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Address);
        }

        #endregion
    }
}
