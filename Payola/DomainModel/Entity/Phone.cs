using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("Phones")]
    public class Phone : Entity
    {
        #region Constructors

        public Phone ()
            : base ()
        {
            AnteNumber = 420;
        }

        public Phone (string identification)
            : this ()
        {
            int number = 0;
            if (Int32.TryParse (identification, out number))
            {
                Number = number;
            }
        }

        #endregion

        #region Properties

        [Required]
        [DisplayFormat (DataFormatString = "{0:000}")]
        public int AnteNumber { get; set; }

        [Required, DataType (DataType.PhoneNumber)]
        [DisplayFormat (DataFormatString = "{0:000000000}")]
        public int Number { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                return String.Format (CultureInfo.CurrentCulture, "+{0:000} {1:000000000}", AnteNumber, Number);
            }
        }

        #endregion
    }
}
