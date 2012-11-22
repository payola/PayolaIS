using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class RelationType
    {
        #region Properties

        public long Id { get; set; }

        [Required, DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Name of the entity type (without namespaces) that may act as a subject in the relation.
        /// </summary>
        [Required, DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string SubjectiveEntityTypeName { get; set; }

        [NotMapped]
        public Type SubjectiveEntityType
        {
            get
            {
                return Entity.GetEntityType (SubjectiveEntityTypeName);
            }

            set
            {
                SubjectiveEntityTypeName = value.Name;
            }
        }

        /// <summary>
        /// Name of the entity type (without namespaces) that may act as an object in the relation.
        /// </summary>
        [Required, DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string ObjectiveEntityTypeName { get; set; }

        [NotMapped]
        public Type ObjectiveEntityType
        {
            get
            {
                return Entity.GetEntityType (ObjectiveEntityTypeName);
            }

            set
            {
                ObjectiveEntityTypeName = value.Name;
            }
        }

        public int PropertiesValue { get; set; }

        [NotMapped]
        public RelationProperties Properties
        {
            get { return (RelationProperties) PropertiesValue; }
            set { PropertiesValue = (int) value; }
        }

        /// <summary>
        /// The relations with the relation type.
        /// </summary>
        public virtual ICollection<Relation> Relations { get; set; }

        [NotMapped]
        public bool IsSymmetric
        {
            get
            {
                return (Properties & RelationProperties.Symmetric) == RelationProperties.Symmetric;
            }
        }

        [NotMapped]
        public bool IsInverse
        {
            get
            {
                return Id < 0;
            }
        }

        [NotMapped]
        public RelationType InverseRelationType
        {
            get
            {
                return new RelationType ()
                {
                    Id = -Id,
                    Name = Name + "Inverse",
                    SubjectiveEntityType = ObjectiveEntityType,
                    ObjectiveEntityType = SubjectiveEntityType,
                    Properties = Properties
                };
            }
        }

        #endregion
    }
}
