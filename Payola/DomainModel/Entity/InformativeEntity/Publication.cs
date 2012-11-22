using System;
using System.ComponentModel.DataAnnotations;

namespace Payola.DomainModel
{
    public class Publication
    {
        #region Constructors

        public Publication ()
        {
            Date = DateTime.Now;
        }

        #endregion

        #region Properties

        [ScaffoldColumn (false)]
        public long Id { get; set; }

        [Required, DataType (DataType.MultilineText), MaxLength (Entity.ParagraphMaxLength)]
        public string Description { get; set; }

        [Required, DataType (DataType.DateTime)]
        public DateTime Date { get; set; }

        [DataType (DataType.Text), MaxLength (Entity.IdentificatorMaxLength)]
        public string Place { get; set; }

        [ScaffoldColumn (false)]
        public long PublishedEntityId { get; set; }

        /// <summary>
        /// The informative entity that is being published.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual InformativeEntity PublishedEntity { get; set; }

        #endregion
    }
}
