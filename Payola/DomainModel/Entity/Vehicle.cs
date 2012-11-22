using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Payola.DomainModel
{
    [Table ("Vehicles")]
    public class Vehicle : Entity, IValidatableObject
    {
        #region Constructors

        public Vehicle () 
            : base () 
        { 
            // NOOP
        }

        public Vehicle (string identification)
            : this ()
        {
            LicenseNumber = identification;
        }

        #endregion

        #region Properties

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string LicenseNumber { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Color { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Brand { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Manufacturer { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                StringBuilder builder = new StringBuilder ();
                if (!String.IsNullOrWhiteSpace (LicenseNumber))
                {
                    builder.Append (LicenseNumber + " ");
                }
                if (!String.IsNullOrWhiteSpace (Manufacturer))
                {
                    builder.Append (Manufacturer + " ");
                }
                if (!String.IsNullOrWhiteSpace (Brand))
                {
                    builder.Append (Brand);
                }
                return builder.ToString ();
            }
        }

        #endregion

        #region IValidatableObject Methods

        public IEnumerable<ValidationResult> Validate (ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace (LicenseNumber) && String.IsNullOrWhiteSpace (Color) &&
                String.IsNullOrWhiteSpace (Brand) && String.IsNullOrWhiteSpace (Manufacturer))
            {
                yield return new ValidationResult (Resources.Resources.AllPropertiesEmptyErrorMessage);
            }
        }

        #endregion
    }
}
