using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Payola.DomainModel
{
    [Table ("Persons")]
    public class Person : Entity
    {
        #region Constructors

        public Person ()
            : base ()
        {
            Sex = Sex.Male;
            Nationality = Nationality.Czech;
            MaritalStatus = MaritalStatus.Single;
        }

        public Person (string identification)
            : this ()
        {
            if (!String.IsNullOrWhiteSpace (identification))
            {
                string[] names = identification.Split (new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (names.Length > 1)
                {
                    FirstName = names[0];
                    Surname = names[1];
                }
                else
                {
                    Surname = names[0];
                }
            }
        }

        #endregion

        #region Properties

        [ScaffoldColumn (false)]
        public int SexValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (Sex))]
        public Sex Sex
        {
            get { return (Sex) SexValue; }
            set { SexValue = (int) value; }
        }

        public long? PersonalIdentificationNumber { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string BirthDate { get; set; }

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string FirstName { get; set; }

        [Required, DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string Surname { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string BirthSurname { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string DegreesAheadName { get; set; }

        [DataType (DataType.Text), MaxLength (IdentificatorMaxLength)]
        public string DegreesBehindName { get; set; }

        [ScaffoldColumn (false)]
        public int NationalityValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (Nationality))]
        public Nationality Nationality
        {
            get { return (Nationality) NationalityValue; }
            set { NationalityValue = (int) value; }
        }

        [NotMapped]
        [DataType ("MultipleEnumeration")]
        public IEnumerable<Country> Citizenships
        {
            get
            {
                if (PersonCitizenships == null)
                {
                    // TODO review. Behaves as the default value. When the PersonCitizenships property is null and getter is 
                    // called, we know that the client is working with a new instance which didn't come from the database. 
                    // If it came from the database the PersonCitizenships property is an empty enumerable, not null.
                    return new List<Country> { Country.CzechRepublic };
                }
                return PersonCitizenships.Select<PersonCitizenship, Country> (p => p.Country).ToList ();
            }

            set
            {
                if (PersonCitizenships == null)
                {
                    PersonCitizenships = new HashSet<PersonCitizenship> ();
                }

                PersonCitizenships.Clear ();
                foreach (Country country in value)
                {
                    PersonCitizenships.Add (new PersonCitizenship ()
                    {
                        Country = country,
                        Person = this
                    });
                }
            }
        }

        [ScaffoldColumn (false)]
        public int MaritalStatusValue { get; set; }

        [NotMapped]
        [EnumDataType (typeof (MaritalStatus))]
        public MaritalStatus MaritalStatus
        {
            get { return (MaritalStatus) MaritalStatusValue; }
            set { MaritalStatusValue = (int) value; }
        }

        /// <summary>
        /// The citizenships of the person.
        /// </summary>
        [ScaffoldColumn (false)]
        public virtual ICollection<PersonCitizenship> PersonCitizenships { get; set; }

        #endregion

        #region Entity Properties

        [DataMember]
        public override string Identification
        {
            get
            {
                return String.Format (CultureInfo.CurrentCulture, "{0} {1}", FirstName, Surname);
            }
        }

        #endregion
    }
}
