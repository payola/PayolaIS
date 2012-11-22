using System;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class Relation
    {
        #region Properties

        [ScaffoldColumn (false)]
        public long Id { get; set; }

        [ScaffoldColumn (false)]
        public long SubjectiveEntityId { get; set; }

        [ScaffoldColumn (false)]
        public long ObjectiveEntityId { get; set; }

        [ScaffoldColumn (false)]
        public long RelationTypeId { get; set; }

        [DataType (DataType.DateTime)]
        public DateTime? ValidFrom { get; set; }

        [DataType (DataType.DateTime)]
        public DateTime? ValidTo { get; set; }

        [DataType (DataType.MultilineText), MaxLength (Entity.ParagraphMaxLength)]
        public string Note { get; set; }

        /// <summary>
        /// The entity that acts as a subject in the relation.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual Entity SubjectiveEntity { get; set; }

        /// <summary>
        /// The entity that acts as an object in the relation.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual Entity ObjectiveEntity { get; set; }

        /// <summary>
        /// Type of the relation.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual RelationType RelationType { get; set; }

        #endregion
    }
}
