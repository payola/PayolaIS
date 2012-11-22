using Payola.DomainModel;

namespace Payola.Intranet.Models.ViewModels
{
    public class RelationEditViewModel
    {
        public long RelationTypeId { get; set; }

        public Relation Relation { get; set; }

        public long EntityId { get; set; }
    }
}
