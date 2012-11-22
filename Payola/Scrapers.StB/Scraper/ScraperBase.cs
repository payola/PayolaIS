using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HtmlAgilityPack;
using Payola.DomainModel;
using Payola.Model;

namespace Payola.Scrapers.Stb
{
    public abstract class ScraperBase
    {

        /// <summary>
        ///     Used for converting strings to Country
        /// </summary>
        static private Dictionary<string, Country> _countryConversionDictionary = new Dictionary<string, Country> ()
        { 
            { "Afghanistan", Country.Afghanistan },
            { "Albanie", Country.Albania },
            { "Alzirsko", Country.Algeria },
            { "Angola", Country.Angola },
            { "Argentina", Country.Argentina },
            { "Australie", Country.Australia },
            { "Banglades", Country.Bangladesh },
            { "Barma", Country.Myanmar },
            { "Belgie", Country.Belgium },
            { "Benin (dahome)", Country.Benin },
            { "Bolivie", Country.Bolivia },
            { "Brazilie", Country.Brazil },
            { "Bulharsko", Country.Bulgaria },
            { "Burundi", Country.Burundi },
            { "Ceskoslovensko", Country.CzechRepublic },
            { "Chile", Country.Chile },
            { "Cina", Country.China },
            { "Dansko", Country.Denmark },
            { "Dominikanska Rep.", Country.DominicanRepublic },
            { "Egypt", Country.Egypt },
            { "Ekvador", Country.Ecuador },
            { "Etiopie", Country.Ethiopia },
            { "Filipiny", Country.Philippines },
            { "Finsko", Country.Finland },
            { "Francie", Country.France },
            { "Ghana", Country.Ghana },
            { "Guayana", Country.Guyana },
            { "Guinea", Country.Guinea },
            { "Guinea-bissau", Country.GuineaBissau },
            { "Haiti", Country.Haiti },
            { "Honduras", Country.Honduras },
            { "Indie", Country.India },
            { "Indonesie", Country.Indonesia },
            { "Irak", Country.Iraq },
            { "Iran", Country.Iran },
            { "Irsko", Country.Ireland },
            { "Island", Country.Iceland },
            { "Italie", Country.Italy },
            { "Izrael", Country.Israel },
            { "Japonsko", Country.Japan },
            { "Jemen", Country.Yemen },
            { "Jemenska Lid.dem.republika", Country.Yemen },
            { "Jizni Afrika", Country.SouthAfrica },
            { "Jordansko", Country.Jordan },
            { "Jugoslavia", Country.Yougoslavia },
            { "Jugoslavie", Country.Yougoslavia },
            { "Kamerun", Country.Cameroon },
            { "Kampucska Lidova Republika", Country.Cambodia },
            { "Kanada", Country.Canada },
            { "Kena", Country.Kenya },
            { "Kolumbie", Country.Colombia },
            { "Kongo", Country.Congo },
            { "Korejska Republika", Country.SouthKorea },
            { "Kostarika", Country.CostaRica },
            { "Kuba", Country.Cuba },
            { "Kuwajt", Country.Kuwait },
            { "Kypr", Country.Cyprus },
            { "Laos", Country.Laos },
            { "Lesotho", Country.Lesotho },
            { "Libanon", Country.Lebanon },
            { "Libye (libyjska Dzamahirie)", Country.Libya },
            { "Lichtenstejnsko", Country.Liechtenstein },
            { "Lucembursko", Country.Luxembourg },
            { "Madagaskar", Country.Madagascar },
            { "Madarsko", Country.Hungary },
            { "Malawi", Country.Malawi },
            { "Mali", Country.Mali },
            { "Malta", Country.Malta },
            { "Maroko", Country.Morocco },
            { "Mexiko", Country.Mexico },
            { "Mongolsko", Country.Mongolia },
            { "Mosambik", Country.Mozambique },
            { "Namibie", Country.Namibia },
            { "Nemecka Spolk.republika", Country.Nsr },
            { "Nemecka Dem.republika", Country.Ndr },
            { "Nepal", Country.Nepal },
            { "Novy Zeland", Country.NewZealand },
            { "Niger", Country.Niger },
            { "Nigerie", Country.Nigeria },
            { "Nikaragua", Country.Nicaragua },
            { "Nizozemi", Country.Netherlands },
            { "Norsko", Country.Norway },
            { "Pakistan", Country.Pakistan },
            { "Panama", Country.Panama },
            { "Papua - Nova Guinea", Country.PapuaNewGuinea },
            { "Polsko", Country.Poland },
            { "Peru", Country.Peru },
            { "Rakousko", Country.Austria },
            { "Recko", Country.Greece },
            { "Spanelsko", Country.Spain },
            { "Spojene Arabske Emiraty", Country.UnitedArabEmirates },
            { "Spojene Staty Americke", Country.UnitedStates },
            { "Sri Lanka", Country.SriLanka },
            { "Svycarsko", Country.Switzerland },
            { "Tanzanie", Country.Tanzania },
            { "Thajsko", Country.Thailand },
            { "Tchaj Wan", Country.Taiwan },
            { "Togo", Country.Togo },
            { "Trinidad A Tobago", Country.TrinidadAndTobago },
            { "Tunisko", Country.Tunisia },
            { "Turecko", Country.Turkey },
            { "Uganda", Country.Uganda },
            { "Uruguay", Country.Uruguay },
            { "Vatikan", Country.VaticanCity },
            { "Velka Britanie", Country.UnitedKingdom },
            { "Venezuela", Country.Venezuela },
            { "Vietnam", Country.Vietnam },
            { "Zair", Country.Zair },
            { "Zambie", Country.Zambia },
            { "Zimbabwe", Country.Zimbabwe },
        };

        private HashSet<string> unknownCountries = new HashSet<string> ()
        {
            "Bez Statni Prislusnosti", 
            "Nezjistena", 
            "Ostatni", 
            "Stpt Nezjistena"
        };
 
        /// <summary>
        ///     Number of columns in the table.
        /// </summary>
        const int kNumberOfColumns = 14;

        /// <summary>
        ///     Column indexes.
        /// </summary>
        const int kLastNameColumnIndex = 0;
        const int kFirstNameColumnIndex = 1;
        const int kBirthYearColumnIndex = 2;
        const int kBirthPlaceColumnIndex = 3;
        const int kStateFirstColumnIndex = 4;
        const int kStateSecondColumnIndex = 5;
        const int kAddedToDatabaseYearColumnIndex = 6;
        const int kSexColumnIndex = 7;
        const int kDepartmentColumnIndex = 8;
        const int kDepartmentCommentColumnIndex = 9;
        const int kAdministrationColumnIndex = 10;
        const int kAdministrationCommentColumnIndex = 11;
        const int kPozorkaFirstColumnIndex = 12;
        const int kPozorkaSecondColumnIndex = 13;

        /// <summary>
        ///     Used in GetRelationType
        /// </summary>
        const int kRelationAdministrativeType = 0;
        const int kRelationDepartmentType = 1;

        /// <summary>
        ///     Male and female identifier used in the database.
        /// </summary>
        const string kMaleSexIdentifier = "muž";
        const string kFemaleSexIdentifier = "žena";

        /// <summary>
        ///     These strings are used in the note for fields in the database
        ///     that have no equivalent in Person's fields.
        /// </summary>
        const string kPlaceOfBirthNoteCaption = "Misto narozeni:";
        const string kPozorka1NoteCaption = "Pozorka 1:";
        const string kPozorka2NoteCaption = "Pozorka 2:";
        const string kAddedToStbDatabaseNoteCaption = "Pridani do Stb database:";



        /// <summary>
        ///     A string identifying the resource from which to scrape.
        ///     Can be a local path or a URL.
        /// </summary>
        protected string _path;

        /// <summary>
        ///     The database context.
        /// </summary>
        protected PayolaContext _ctx;
        private string path;

        /// <summary>
        ///     The only constructor.
        /// </summary>
        /// <param name="path">String identifying the resource from which to scrape (local path, URL, ...).</param>
        public ScraperBase (PayolaContext ctx, string path)
        {
            _ctx = ctx;
            _path = path;
        }

        /// <summary>
        ///     Adds a new relation to the person.
        /// </summary>
        /// <param name="p">Person</param>
        /// <param name="division">Stb division.</param>
        /// <param name="comment">Comment</param>
        private void AddStbRelationToPerson (Person p, StbDivision division, string comment, RelationType type)
        {
            Relation r = new Relation ();
            r.SubjectiveEntity = division;
            r.RelationType = type;
            r.ObjectiveEntity = p;
            r.Note = comment;

            _ctx.Relations.Add (r);

        }

        /// <summary>
        ///     Converts the string names of countries to Country enum objects.
        /// </summary>
        /// <param name="states">String names.</param>
        /// <returns>Array of countries</returns>
        private List<Country> ConvertStringCountriesToCountryArray (string[] states)
        {
            List<Country> countries = new List<Country> ();
            for (int i = 0; i < states.Length; ++i)
            {
                if (states[i] == null || states[i].Equals (""))
                {
                    continue;
                }

                Country c;
                if (!_countryConversionDictionary.TryGetValue (states[i], out c) && !unknownCountries.Contains (states[i]))
                {
                    Console.WriteLine ("WARNING: Unknown country: '{0}'", states[i]);
                }
                else
                {
                    countries.Add (c);
                }
            }

            return countries;
        }

        /// <summary>
        ///     Loads data from resource path.
        /// </summary>
        /// <param name="path">Resource path.</param>
        /// <returns>Loaded data or null if it doesn't exist.</returns>
        protected abstract string GetDataForResourcePath(string path);

        /// <summary>
        ///     Returns the relation type for administration or department or creates one if necessary.
        /// </summary>
        /// <returns></returns>
        private RelationType GetRelationType(int type)
        {
            string relationName;
            if (type == kRelationAdministrativeType)
            {
                relationName = "StbAdministrationRegisteredPerson";
            }
            else if (type == kRelationDepartmentType)
            {
                relationName = "StbDepartmentRegisteredPerson";
            }
            else
            {
                Console.WriteLine ("ERROR: [ScraperBase GetRelationType:] - Unknown relation type {0}", type);
                Program.Exit (1);

                // Shouldn't reach this point anyway
                return null;
            }

            IQueryable<RelationType> types = _ctx.RelationTypes.Where(t => t.Name.Equals(relationName));
            if (types.Count() == 0)
            {
                // Error
                Console.WriteLine("ERROR: Cannot find {0} relation type.", relationName);
                Program.Exit(1);
            }

            return types.First();
        }

        /// <summary>
        ///     Returns the resource path ("{0}{1}", c, i).
        /// </summary>
        /// <param name="c">Char.</param>
        /// <param name="i">Index.</param>
        /// <returns>The resource path.</returns>
        protected abstract string GetResourcePathForCharAndIndex(char c, int i);

        /// <summary>
        ///     Gets a StbDivision named @name or creates one if it doesn't exist. May return null if @name is empty or null.
        /// </summary>
        /// <param name="name">Name of the Stb division.</param>
        /// <returns>StbDivision</returns>
        private StbDivision GetStbDivisionByName (string name)
        {
            if (name == null || name.Equals (""))
            {
                return null;
            }

            IQueryable<StbDivision> divisions = _ctx.Entities.OfType<StbDivision> ();
            IQueryable<StbDivision> selectedDivisions = divisions.Where (p => p.Name.Equals (name));

            if (selectedDivisions.Count () == 0)
            {
                // Center not found, create one
                StbDivision division = new StbDivision ();
                division.Name = name;
                _ctx.Entities.Add (division);
                _ctx.SaveChanges();
                return division;
            }

            return selectedDivisions.First ();
        }

        /// <summary>
        ///     Parses loaded data and adds them to the DB.
        /// </summary>
        /// <param name="data">The data string.</param>
        protected void ProcessData (string data)
        {
            HtmlDocument doc = new HtmlDocument ();
            doc.LoadHtml (data);

            XmlDocument document = new XmlDocument ();
            try
            {
                // Convert the HTML document to XHTML and reload it as a XML
                doc.OptionOutputAsXml = true;
                StringWriter writer = new StringWriter ();
                doc.Save (writer);
                document.LoadXml (writer.ToString ());
            }
            catch (Exception e)
            {
                Console.WriteLine ("ERROR: Couldn't parse XML document {0}", data.Substring (data.Length < 50 ? data.Length : 50));
                return;
            }

            XmlNodeList tables = document.SelectNodes ("//table");
            if (tables.Count != 1)
            {
                // Only one table should be on the page
                Console.WriteLine ("ERROR: Invalid table count on page {0}", tables.Count);
                return;
            }

            XmlNode table = tables[0];
            XmlNodeList rows = table.SelectNodes ("tr");
            if (rows.Count == 0)
            {
                // There should be at least one row
                Console.WriteLine ("ERROR: No rows in table {0}", table.ToString ());
                return;
            }


            // Start at 1 as the first row is header
            for (int i = 1; i < rows.Count; ++i)
            {
                XmlNode row = rows[i];
                ProcessRow (row);
            }
        }

        /// <summary>
        ///     Parses the XML node and imports it.
        /// </summary>
        /// <param name="rowNode">The node.</param>
        protected void ProcessRow (XmlNode rowNode)
        {
            XmlNodeList cells = rowNode.ChildNodes;
            if (cells.Count != kNumberOfColumns)
            {
                Console.WriteLine ("ERROR: Wrong number of columns in row {0}", rowNode.ToString ());
                return;
            }


            //------------ Add to DB
            string lastName = cells[kLastNameColumnIndex].InnerText;
            string firstName = cells[kFirstNameColumnIndex].InnerText;
            if (firstName == null || firstName.Equals (""))
            {
                firstName = "Nezname";
            }

            string birthYear = cells[kBirthYearColumnIndex].InnerText;

            string birthPlace = cells[kBirthPlaceColumnIndex].InnerText;

            string[] countryStrings = new string[2];
            countryStrings[0] = cells[kStateFirstColumnIndex].InnerText;
            countryStrings[1] = cells[kStateSecondColumnIndex].InnerText;
            List<Country> countries = ConvertStringCountriesToCountryArray (countryStrings);

            string dbAdditionYear = cells[kAddedToDatabaseYearColumnIndex].InnerText;

            string sexValue = cells[kSexColumnIndex].InnerText;
            Sex sex = Sex.Male;
            if (sexValue != null)
            {
                sex = sexValue.Equals (kMaleSexIdentifier) ? Sex.Male : Sex.Female;
            }
            string department = cells[kDepartmentColumnIndex].InnerText;
            string departmentComment = cells[kDepartmentCommentColumnIndex].InnerText;
            string administration = cells[kAdministrationColumnIndex].InnerText;
            string administrationComment = cells[kAdministrationCommentColumnIndex].InnerText;
            string pozorka1 = cells[kPozorkaFirstColumnIndex].InnerText;
            string pozorka2 = cells[kPozorkaSecondColumnIndex].InnerText;



            Person p = TryToFindPerson (firstName, lastName, birthYear, sex, birthPlace);
            if (p == null)
            {
                // Need to create a new person
                p = new Person ();
                p.FirstName = firstName;
                p.Surname = lastName;
                if (birthYear != null && !birthYear.Equals (""))
                {
                    p.BirthDate = birthYear;
                }
                p.Sex = sex;
                p.Nationality = Nationality.Unknown;
                p.Note = string.Format ("{0} {1}\n{2} {3}\n{4} {5}\n{6} {7}",
                    kPlaceOfBirthNoteCaption, birthPlace,
                    kPozorka1NoteCaption, pozorka1,
                    kPozorka2NoteCaption, pozorka2,
                    kAddedToStbDatabaseNoteCaption, dbAdditionYear);

                if (countries.Count > 0)
                {
                    p.Citizenships = countries;
                }

                _ctx.Entities.Add (p);
            }

            StbDivision StbDepartment = GetStbDivisionByName (department);
            StbDivision StbAdministration = GetStbDivisionByName (administration);

            Relation departmentRelation = null;
            Relation administrationRelation = null;

            ICollection<Relation> relations = p.ObjectiveRelations;
            if (relations != null)
            {
                foreach (Relation r in relations)
                {
                    if (StbDepartment != null && r.SubjectiveEntityId == StbDepartment.Id)
                    {
                        departmentRelation = r;
                    }
                    else if (StbAdministration != null && r.SubjectiveEntityId == StbAdministration.Id)
                    {
                        administrationRelation = r;
                    }
                }
            }

            if (departmentRelation == null && StbDepartment != null)
            {
                // Need to add a relation
                AddStbRelationToPerson (p, StbDepartment, departmentComment, GetRelationType (kRelationDepartmentType));
            }
            if (administrationRelation == null && StbAdministration != null)
            {
                // Need to add a relation
                AddStbRelationToPerson (p, StbAdministration, administrationComment, GetRelationType (kRelationAdministrativeType));
            }

            _ctx.SaveChanges ();
        }

        /// <summary>
        ///     Begins the scraping process.
        /// </summary>
        public void Scrape ()
        {
            for (char c = 'a'; c <= 'z'; ++c)
            {
                this.ScrapeLetter (c);
            }
        }

        /// <summary>
        ///     Scrapes all people whose last name starts with @c.
        /// </summary>
        /// <param name="c">The letter.</param>
        private void ScrapeLetter (char c)
        {
            int index = 0;
            string resourcePath = GetResourcePathForCharAndIndex (c, index);
            string data = GetDataForResourcePath (resourcePath);
            while (data != null)
            {
                Console.WriteLine ("Processing {0}", resourcePath);
                ProcessData (data);

                ++index;
                resourcePath = GetResourcePathForCharAndIndex (c, index);
                data = GetDataForResourcePath (resourcePath);
            }
        }

        /// <summary>
        ///     Returns a person that matches this description, or null.
        /// </summary>
        /// <param name="firstName">Person's first name.</param>
        /// <param name="lastName">Person's last name.</param>
        /// <param name="birthYear">Person's birth year.</param>
        /// <param name="sex">Person's sex.</param>
        /// <returns>Person or null.</returns>
        private Person TryToFindPerson (string firstName, string lastName, string birthYear, Sex sex, string birthPlace)
        {
            IQueryable<Person> people = _ctx.Entities.OfType<Person> ();

            // We assume the birthYear is four letters at least => The date string of Person should ergo contain it as a string.
            int sexValue = (int) sex;
            // Using the same format as when adding a note to the person.
            string birthPlaceString = string.Format ("{0} {1}", kBirthPlaceColumnIndex, birthPlace);
            IQueryable<Person> selectedPeople = people.Where (p => p.FirstName.Equals (firstName)
                                                               && p.Surname.Equals (lastName)
                                                               && p.SexValue == sexValue
                                                               && (birthYear == null
                                                                            || birthYear.Equals ("")
                                                                            || p.BirthDate.Contains (birthYear))
                                                               && (birthPlace == null
                                                                            || birthPlace.Equals ("")
                                                                            || p.Note.Contains (birthPlaceString)));

            if (selectedPeople.Count () == 0)
            {
                // No people found
                return null;
            }

            return selectedPeople.First ();
        }

    }
}
