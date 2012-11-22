using System;

namespace Payola.DomainModel
{
    [Flags]
    public enum RelationProperties
    {
        None = 0,
        Symmetric = 1,
        AntiSymmetric = 2
    }
}
