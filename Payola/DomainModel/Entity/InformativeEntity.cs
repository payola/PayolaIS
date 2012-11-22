using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("InformativeEntities")]
    public class InformativeEntity : Entity
    {
        #region Constructors

        public InformativeEntity ()
            : base () 
        {
            // NOOP
        }

        #endregion

        #region Properties

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        [Display (Order = 0)]
        public string Name { get; set; }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        [Display (Order = 1)]
        public string Annotation { get; set; }

        public bool IsPublishable { get; set; }

        /// <summary>
        /// The publications of the entity.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<Publication> Publications { get; set; }

        /// <summary>
        /// The keywords that the informative entity contains.
        /// </summary>
        [ScaffoldColumn (false)]
        [PredefinedRelation (PredefinedRelationTypeId.InformativeEntityContainsKeyword)]
        public virtual ICollection<Keyword> Keywords { get; set; }

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
