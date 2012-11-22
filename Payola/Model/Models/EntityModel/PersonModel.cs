using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class PersonModel : EntityModel<Person>
    {
        #region Constructors

        public PersonModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Person> Properties

        protected override IQueryable<Person> EntitiesQueryable
        {
            get
            {
                return Db.Entities.Include ("PersonCitizenships").OfType<Person> ();
            }
        }

        #endregion

        #region EntityModel<Person> Methods

        protected override IQueryable<Person> FilterEntitiesByNeedle (IQueryable<Person> entities, string needle)
        {
            return entities.Where (p => p.FirstName.Contains (needle) || p.Surname.Contains (needle) || 
                p.BirthSurname.Contains (needle) || p.Note.Contains (needle));
        }

        protected override IQueryable<Person> FilterEntitiesBySimilarity (IQueryable<Person> entities, Person patternEntity)
        {
            entities = base.FilterEntitiesBySimilarity (entities, patternEntity);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.FirstName);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.Surname);
            entities = FilterEntitiesByPropertyValue (entities, patternEntity, e => e.BirthSurname);
            return FilterEntitiesByPropertyValue (entities, patternEntity, e => e.BirthDate);
        }

        #endregion
    }
}
