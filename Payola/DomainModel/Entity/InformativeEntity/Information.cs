using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    [Table ("Information")]
    public class Information : InformativeEntity
    {
        #region Constructors

        public Information ()
            : base ()
        {
            InsertionDate = DateTime.Now;
            Source = Source.Internet;
            Credibility = Credibility.LikelyTrue;
            IsPublishable = false;
            State = InformationState.Unverified;
        }

        public Information (string identification)
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

        [DataType (DataType.Date)]
        public DateTime? OriginDate { get; set; }

        [DataType (DataType.Date)]
        public DateTime? AcquirementDate { get; set; }

        [ScaffoldColumn (false)]
        public int SourceValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (Source))]
        public Source Source
        {
            get { return (Source) SourceValue; }
            set { SourceValue = (int) value; }
        }

        [ScaffoldColumn (false)]
        public int CredibilityValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (Credibility))]
        public Credibility Credibility
        {
            get { return (Credibility) CredibilityValue; }
            set { CredibilityValue = (int) value; }
        }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        public string Comment { get; set; }

        [ScaffoldColumn (false)]
        public int StateValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (InformationState))]
        public InformationState State
        {
            get { return (InformationState) StateValue; }
            set { StateValue = (int) value; }
        }

        /// <summary>
        /// The Incidents that are somehow related to the information.
        /// </summary>
        [ScaffoldColumn (false)]
        [PredefinedRelation (PredefinedRelationTypeId.InformationHasRelatedIncident)]
        public virtual ICollection<Incident> RelatedIncidents { get; set; }

        /// <summary>
        /// The reports where the information is being described. In other words influenced reports.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<Report> InfluencedReports { get; set; }

        #endregion
    }
}
