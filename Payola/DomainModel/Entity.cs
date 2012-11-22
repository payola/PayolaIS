using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    public class Entity
    {
        #region Fields

        public const int IdentificatorMaxLength = 128;

        public const int ParagraphMaxLength = 5000;

        public const int TextMaxLength = 100000;

        #endregion

        #region Constructors

        public Entity ()
        {
            ModifiactionCount = 0;
            LastModificationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        [DataMember]
        [ScaffoldColumn (false)]
        public long Id { get; set; }

        [DataType (DataType.MultilineText), MaxLength (ParagraphMaxLength)]
        public string Note { get; set; }

        [ScaffoldColumn (false)]
        public int ModifiactionCount { get; set; }

        [ScaffoldColumn (false)]
        public DateTime LastModificationDate { get; set; }

        [ScaffoldColumn (false)]
        public bool IsDeleted { get; set; }

        [ScaffoldColumn (false)]
        public DateTime? DeletionDate { get; set; }

        /// <summary>
        /// The relations in which this entity acts as a subject of the realtion.
        /// </summary>
        [InverseProperty ("SubjectiveEntity")]
        [ScaffoldColumn (false)]
        public virtual ICollection<Relation> SubjectiveRelations { get; set; }

        /// <summary>
        /// The relations in which this entity acts as an object of the relation.
        /// </summary>
        [InverseProperty ("ObjectiveEntity")]
        [ScaffoldColumn (false)]
        public virtual ICollection<Relation> ObjectiveRelations { get; set; }

        /// <summary>
        /// Textual identification of the entity. It may be the name, if defined, or combined values of other 
        /// properties that identify the entity.
        /// </summary>
        [DataMember]
        [NotMapped]
        [ScaffoldColumn (false)]
        public virtual string Identification
        {
            get
            {
                throw new InvalidOperationException ();
            }
        }

        [NotMapped]
        [ScaffoldColumn (false)]
        public virtual bool IsEditable
        {
            get { return !IsDeleted; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the type that corresponds to the specified entity type name.
        /// </summary>
        /// <example>GetEntityType (&quot;Person&quot;) returns equivalent to typeof (Person).</example>
        /// <param name="entityTypeName">Name of the entity type without namespaces.</param>
        /// <returns>Type corresponding to the entity type name.</returns>
        public static Type GetEntityType (string entityTypeName)
        {
            return Type.GetType (typeof (Entity).Namespace + "." + entityTypeName);
        }

        public static Type GetEntityType (Entity entity)
        {
            Type type = entity.GetType ();

            // To avoid EF proxies which contain the underscore in their type name, the closest supertype without underscore
            // in the type name is used.
            while (type.Name.Contains ('_'))
            {
                type = type.BaseType;
            }
            return type;
        }    

        #endregion
    }
}