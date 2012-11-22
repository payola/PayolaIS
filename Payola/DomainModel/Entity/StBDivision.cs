using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("StbDivisions")]
    public class StbDivision : Entity
    {
        #region Constructors

        public StbDivision ()
            : base () 
        {
            // NOOP
        }

        public StbDivision (string name)
            : this ()
        {
            Name = name;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Name { get; set; }

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
