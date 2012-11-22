using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class IncidentField
    {
        #region Properties

        [Key]
        [Column (Order = 0)]
        public int FieldValue { get; set; }

        [NotMapped]
        public Field Field
        {
            get { return (Field) FieldValue; }
            set { FieldValue = (int) value; }
        }

        [Key]
        [Column (Order = 1)]
        public long IncidentId { get; set; }

        /// <summary>
        /// The Incident whose field is represented.
        /// </summary>
        public virtual Incident Incident { get; set; }

        #endregion
    }
}
