using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class PersonCitizenship
    {
        #region Properties

        [Key]
        [Column (Order = 0)]
        public int CountryValue { get; set; }

        [NotMapped]
        public Country Country
        {
            get { return (Country) CountryValue; }
            set { CountryValue = (int) value; }
        }

        [Key]
        [Column (Order = 1)]
        public long PersonId { get; set; }

        /// <summary>
        /// The person whose citizenship is represented.
        /// </summary>
        public virtual Person Person { get; set; }

        #endregion
    }
}
