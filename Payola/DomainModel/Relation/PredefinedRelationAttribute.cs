using System;

namespace Payola.DomainModel
{
    [AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PredefinedRelationAttribute : Attribute
    {
        #region Constructors

        public PredefinedRelationAttribute (PredefinedRelationTypeId relationTypeValue)
            : base ()
        {
            RelationTypeValue = relationTypeValue;
        }

        #endregion

        #region Properties

        public PredefinedRelationTypeId RelationTypeValue
        {
            get;
            private set;
        }

        #endregion
    }
}
