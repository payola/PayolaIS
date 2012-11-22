using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Payola.DomainModel
{
    [Table ("Incidents")]
    public class Incident : InformativeEntity
    {
        #region Constructors

        public Incident ()
            : base ()
        {
            OriginYear = DateTime.Now.Year;
            State = IncidentState.Open;
        }

        public Incident (string identification)
            : this ()
        {
            Name = identification;
        }

        #endregion

        #region Properties

        [Editable (false)]
        public int OriginYear { get; set; }

        [NotMapped]
        [DataType ("MultipleEnumeration")]
        public IEnumerable<Field> Fields
        {
            get
            {
                if (IncidentFields == null)
                {
                    // Behaves like the default value.
                    return new List<Field> ();
                }
                return IncidentFields.Select<IncidentField, Field> (e => e.Field).ToList ();
            }

            set
            {
                if (IncidentFields == null)
                {
                    IncidentFields = new HashSet<IncidentField> ();
                }

                IncidentFields.Clear ();
                foreach (Field field in value)
                {
                    IncidentFields.Add (new IncidentField ()
                    {
                        Field = field,
                        Incident = this
                    });
                }
            }
        }

        [NotMapped]
        [DataType ("MultipleEnumeration")]
        public IEnumerable<Region> Regions
        {
            // TODO refactor
            get
            {
                if (IncidentRegions == null)
                {
                    // Behaves like the default value.
                    return new List<Region> ();
                }
                return IncidentRegions.Select<IncidentRegion, Region> (e => e.Region).ToList ();
            }

            set
            {
                if (IncidentRegions == null)
                {
                    IncidentRegions = new HashSet<IncidentRegion> ();
                }

                IncidentRegions.Clear ();
                foreach (Region region in value)
                {
                    IncidentRegions.Add (new IncidentRegion ()
                    {
                        Region = region,
                        Incident = this
                    });
                }
            }
        }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        [Editable (false)]
        public string EnclosureDescription { get; set; }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        public string Comment { get; set; }

        [ScaffoldColumn (false)]
        public int StateValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (IncidentState))]
        [Editable (false)]
        public IncidentState State
        {
            get { return (IncidentState) StateValue; }
            set { StateValue = (int) value; }
        }

        /// <summary>
        /// The information that are somehow related to the Incident.
        /// </summary>
        [ScaffoldColumn (false)]
        [PredefinedRelation (PredefinedRelationTypeId.IncidentHasRelatedInformation)]
        public virtual ICollection<Information> RelatedInformation { get; set; }

        /// <summary>
        /// The reports where the Incident is being described. In other words influenced reports.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<Report> InfluencedReports { get; set; }

        /// <summary>
        /// The locations that the Incident is related with.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<Location> Locations { get; set; }

        /// <summary>
        /// The fields of the Incident.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<IncidentField> IncidentFields { get; set; }

        /// <summary>
        /// The regions of the Incident.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<IncidentRegion> IncidentRegions { get; set; }

        #endregion

        #region InformativeEntity Properties

        [NotMapped]
        [ScaffoldColumn (false)]
        public override bool IsEditable
        {
            get
            {
                return base.IsEditable && State != IncidentState.Closed;
            }
        }

        #endregion
    }
}
