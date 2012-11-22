using System.Collections.Generic;
using Payola.Model;

namespace Payola.Intranet.Models.ViewModels
{
    public class EntityDetailViewModel : EntityViewModel
    {
        public IEnumerable<TypedRelations> Relations { get; set; }
    }
}
