using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Payola.DomainModel
{
    [NotMapped]
    public class PredefinedRelationType : RelationType
    {
        #region Constructors

        public PredefinedRelationType (PropertyInfo objectiveEntitiesProperty)
        {
            ObjectiveEntitiesProperty = objectiveEntitiesProperty;
            Properties = RelationProperties.None | RelationProperties.AntiSymmetric;
        }

        #endregion

        #region Properties

        public PropertyInfo ObjectiveEntitiesProperty
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public IEnumerable<Entity> GetRelatedEntities (Entity entity)
        {
            return (IEnumerable<Entity>) ObjectiveEntitiesProperty.GetValue (entity, null);
        }

        #endregion
    }
}
