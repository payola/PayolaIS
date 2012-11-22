using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("Emails")]
    public class Email : Entity
    {
        #region Constructors

        public Email ()
            : base () 
        {
            // NOOP
        }

        public Email (string identification)
            : this ()
        {
            Address = identification;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.EmailAddress)]
        [Display (ResourceType = typeof (Resources.Resources), Name = "EmailAddress")]
        public string Address { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                return Address;
            }
        }

        #endregion
    }
}
