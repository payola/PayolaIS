using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class Location
    {
        #region Properties

        public long Id { get; set; }

        [Required, DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string Name { get; set; }

        public long? ParentLocationId { get; set; }

        /// <summary>
        /// The parent location.
        /// </summary>
        public virtual Location ParentLocation { get; set; }

        /// <summary>
        /// The child locations.
        /// </summary>
        public virtual ICollection<Location> ChildLocations { get; set; }

        /// <summary>
        /// The Incidents that are related with the location.
        /// </summary>
        public virtual ICollection<Incident> RelatedIncidents { get; set; }

        #endregion
    }
}
