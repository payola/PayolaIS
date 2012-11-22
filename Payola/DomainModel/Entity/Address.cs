using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Payola.DomainModel
{
    [Table ("Addresses")]
    public class Address : Entity
    {
        #region Constructors

        public Address ()
            : base ()
        {
            Country = Country.CzechRepublic;
        }

        public Address (string identification)
            : this ()
        {
            Street = identification;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Street { get; set; }

        public int? LandRegistryNumber { get; set; }

        public int? HouseNumber { get; set; }

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string City { get; set; }

        public int? PostalCode { get; set; }

        [ScaffoldColumn (false)]
        public int CountryValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (Country))]
        public Country Country
        {
            get { return (Country) CountryValue; }
            set { CountryValue = (int) value; }
        }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                StringBuilder builder = new StringBuilder (Street + " ");
                builder.Append (LandRegistryNumber.HasValue ? LandRegistryNumber.ToString () : "?");
                builder.Append ("/");
                builder.Append (HouseNumber.HasValue ? HouseNumber.ToString () : "?");
                if (!String.IsNullOrWhiteSpace (City))
                {
                    builder.Append (", " + City);
                }
                if (PostalCode.HasValue)
                {
                    builder.Append (", " + PostalCode);
                }
                return builder.ToString ();
            }
        }

        #endregion
    }
}
