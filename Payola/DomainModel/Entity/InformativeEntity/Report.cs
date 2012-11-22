using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    [Table ("Reports")]
    public class Report : InformativeEntity
    {
        #region Constructors

        public Report ()
            : base ()
        {
            InsertionDate = DateTime.Now;
            Kind = ReportKind.Overview;
            IsPublishable = false;
            State = ReportState.Concept;
        }

        public Report (string identification)
            : this ()
        {
            Name = identification;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.MultilineText), MaxLength (TextMaxLength)]
        public string Text { get; set; }

        [DataType (DataType.Date)]
        [Editable (false)]
        public DateTime InsertionDate { get; set; }

        [ScaffoldColumn (false)]
        public int KindValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (ReportKind))]
        public ReportKind Kind
        {
            get { return (ReportKind) KindValue; }
            set { KindValue = (int) value; }
        }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        public string Comment { get; set; }

        [ScaffoldColumn (false)]
        public int StateValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (ReportState))]
        [Editable (false)]
        public ReportState State
        {
            get { return (ReportState) StateValue; }
            set { StateValue = (int) value; }
        }

        /// <summary>
        /// The Incidents that are described in the report.
        /// </summary>
        [ScaffoldColumn (false)]
        [PredefinedRelation (PredefinedRelationTypeId.ReportDescribesIncident)]
        public virtual ICollection<Incident> DescribedIncidents { get; set; }

        /// <summary>
        /// The information that are described in the report.
        /// </summary>
        [ScaffoldColumn (false)]
        [PredefinedRelation (PredefinedRelationTypeId.ReportDescribesInformation)]
        public virtual ICollection<Information> DescribedInformation { get; set; }

        #endregion

        #region InformativeEntity Properties

        [NotMapped]
        [ScaffoldColumn (false)]
        public override bool IsEditable
        {
            get
            {
                return base.IsEditable && State != ReportState.Closed;
            }
        }

        #endregion
    }
}
