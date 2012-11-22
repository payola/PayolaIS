using Payola.DomainModel;

namespace Payola.Intranet.Models.ViewModels
{
    public class CreateAndAddRelationViewModel : EntityViewModel
    {
        public Entity NewEntity { get; set; }

        public long RelationTypeId { get; set; }
    }
}
