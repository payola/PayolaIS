using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class ReportModel : InformativeEntityModel<Report>
    {
        #region Constructors

        public ReportModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region Methods

        public void OpenReport (Report report)
        {
            if (!report.IsDeleted && report.State != ReportState.Concept)
            {
                report.State = ReportState.Concept;
                SaveEntity (report);
            }
        }

        public void CloseReport (Report report)
        {
            if (report.IsEditable)
            {
                report.State = ReportState.Closed;
                SaveEntity (report);
            }
        }

        #endregion

        #region InformativeEntityModel<Report> Methods

        protected override IQueryable<Report> FilterEntitiesByNeedle (IQueryable<Report> entities, string needle)
        {
            return entities.Where (e => e.Annotation.Contains (needle) || e.Text.Contains (needle) ||
                e.Comment.Contains (needle) || e.Name.Contains (needle) || e.Note.Contains (needle));
        }

        protected override IQueryable<Report> FilterEntitiesBySimilarity (IQueryable<Report> entities, Report patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Text);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Comment);
        }

        #endregion
    }
}
