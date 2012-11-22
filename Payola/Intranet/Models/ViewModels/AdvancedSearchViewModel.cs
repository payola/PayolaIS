using System.Collections.Generic;
using Payola.DomainModel;

namespace Payola.Intranet.Models.ViewModels
{
    public class AdvancedSearchViewModel : EntityViewModel
    {
        public IEnumerable<Entity> FoundEntities { get; set; }
    }
}
