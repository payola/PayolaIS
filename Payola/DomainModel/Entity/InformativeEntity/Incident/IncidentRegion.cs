using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class IncidentRegion
    {
        #region Properties

        [Key]
        [Column (Order = 0)]
        public int RegionValue { get; set; }

        [NotMapped]
        public Region Region
        {
            get { return (Region) RegionValue; }
            set { RegionValue = (int) value; }
        }

        [Key]
        [Column (Order = 1)]
        public long IncidentId { get; set; }

        /// <summary>
        /// The Incident whose region is represented.
        /// </summary>
        public virtual Incident Incident { get; set; }

        #endregion
    }
}
