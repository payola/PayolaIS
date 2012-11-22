using System.Collections.Generic;
using Payola.DomainModel;

namespace Payola.Model
{
    public class TypedRelations
    {
        #region Constructors

        public TypedRelations (RelationType relationType)
        {
            RelationType = relationType;
            Relations = new Dictionary<Relation, Entity> ();
        }

        #endregion

        #region Properties

        public RelationType RelationType { get; set; }

        public Dictionary<Relation, Entity> Relations { get; set; }

        #endregion
    }
}