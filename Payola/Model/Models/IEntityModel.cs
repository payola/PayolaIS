using System.Collections.Generic;
using Payola.DomainModel;

namespace Payola.Model
{
    public interface IEntityModel
    {
        #region Methods

        Entity GetEntityById (long id);

        IEnumerable<Entity> GetEntitiesByIds (IEnumerable<long> ids);

        IEnumerable<Entity> GetEntitiesByIdsAndNeedle (IEnumerable<long> excludeIds, string needle);

        Entity GetNewEntityWithIdentification (string identification);

        Entity AddNewEntity (Entity entity);

        #endregion
    }
}
