using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("Companies")]
    public class Company : Entity
    {
        #region Constructors

        public Company ()
            : base ()
        {
            RegistrationDate = DateTime.Now;
        }

        public Company (string identification)
            : this ()
        {
            Name = identification;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Name { get; set; }

        public long RegistrationNumber { get; set; }

        [DataType (DataType.Date)]
        public DateTime? RegistrationDate { get; set; }

        [DataType (DataType.Currency)]
        public decimal? BasicCapital { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                return Name;
            }
        }

        #endregion
    }
}
