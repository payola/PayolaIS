using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class IncidentModel : InformativeEntityModel<Incident>
    {
        #region Constructors

        public IncidentModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region InformativeEntityModel<Incident> Properties

        protected override IQueryable<Incident> EntitiesQueryable
        {
            get
            {
                return Db.Entities.Include ("IncidentFields").Include ("IncidentRegions").OfType<Incident> ();
            }
        }

        #endregion

        #region Methods

        public void OpenIncident (Incident incidentToOpen)
        {
            if (!incidentToOpen.IsDeleted && incidentToOpen.State != IncidentState.Open)
            {
                incidentToOpen.State = IncidentState.Open;
                SaveEntity (incidentToOpen);
            }
        }

        public void CloseIncident (Incident incidentToClose)
        {
            if (incidentToClose.IsEditable)
            {
                incidentToClose.State = IncidentState.Closed;
                SaveEntity (incidentToClose);
            }
        }

        #endregion

        #region InformativeEntityModel<Incident> Methods

        protected override IQueryable<Incident> FilterEntitiesByNeedle (IQueryable<Incident> entities, string needle)
        {
            return entities.Where (e => e.Annotation.Contains (needle) || e.Comment.Contains (needle) ||
                e.EnclosureDescription.Contains (needle) || e.Name.Contains (needle) || e.Note.Contains (needle));
        }

        protected override IQueryable<Incident> FilterEntitiesBySimilarity (IQueryable<Incident> entities, Incident patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Comment);
        }

        #endregion
    }
}
