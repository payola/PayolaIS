using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("Keywords")]
    public class Keyword : Entity
    {
        #region Constructors

        public Keyword ()
            : base () 
        {
            // NOOP
        }

        public Keyword (string identification)
            : this ()
        {
            Value = identification;
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string Value { get; set; }

        /// <summary>
        /// The informatïve entities that are tagged with the keyword.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<InformativeEntity> TaggedInformativeEntities { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                return Value;
            }
        }

        #endregion
    }
}
