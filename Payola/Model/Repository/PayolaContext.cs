using System;
using System.Data.Entity;
using Payola.DomainModel;

namespace Payola.Model
{
    public class PayolaContext : DbContext
    {
        #region Properties

        public DbSet<Entity> Entities { get; set; }

        public DbSet<PersonCitizenship> PersonCitizenships { get; set; }

        public DbSet<IncidentField> IncidentFields { get; set; }

        public DbSet<IncidentRegion> IncidentRegions { get; set; }

        public DbSet<Relation> Relations { get; set; }

        public DbSet<RelationType> RelationTypes { get; set; }

        public DbSet<Publication> Publications { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<Location> Locations { get; set; }

        #endregion

        #region Methods

        internal void Seed ()
        {
            // Start ids of relation types on 100 to leave some space for predefined relations. TODO find better way to do this.
            for (int i = 0; i < 100; i++)
            {
                AddRelationType ("Noname", typeof (Entity), typeof (Entity), RelationProperties.None);
            }
            SaveChanges ();
            Database.ExecuteSqlCommand ("DELETE RelationTypes");

            AddRelationType ("InformationMentionsAddress", typeof (Information), typeof (Address), RelationProperties.None);
            AddRelationType ("InformationMentionsCompany", typeof (Information), typeof (Company), RelationProperties.None);
            AddRelationType ("InformationMentionsEmail", typeof (Information), typeof (Email), RelationProperties.None);
            AddRelationType ("InformationMentionsPerson", typeof (Information), typeof (Person), RelationProperties.None);
            AddRelationType ("InformationMentionsPhone", typeof (Information), typeof (Phone), RelationProperties.None);
            AddRelationType ("InformationMentionsVehicle", typeof (Information), typeof (Vehicle), RelationProperties.None);

            AddRelationType ("PersonHasResidence", typeof (Person), typeof (Address), RelationProperties.None);
            AddRelationType ("PersonHasBirthAddress", typeof (Person), typeof (Address), RelationProperties.None);
            AddRelationType ("PersonHasWhereabouts", typeof (Person), typeof (Address), RelationProperties.None);
            AddRelationType ("PersonUsesPhone", typeof (Person), typeof (Phone), RelationProperties.None);
            AddRelationType ("PersonUsesEmail", typeof (Person), typeof (Email), RelationProperties.None);
            AddRelationType ("PersonUsesVehicle", typeof (Person), typeof (Vehicle), RelationProperties.None);
            AddRelationType ("PersonHasColleague", typeof (Person), typeof (Person), RelationProperties.Symmetric);
            AddRelationType ("PersonIsInMarriage", typeof (Person), typeof (Person), RelationProperties.Symmetric);
            AddRelationType ("PersonHasPartner", typeof (Person), typeof (Person), RelationProperties.Symmetric);
            AddRelationType ("PersonHasParent", typeof (Person), typeof (Person), RelationProperties.None | RelationProperties.AntiSymmetric);
            AddRelationType ("PersonHasParentInLaw", typeof (Person), typeof (Person), RelationProperties.None);
            AddRelationType ("PersonHasBusinessPartner", typeof (Person), typeof (Person), RelationProperties.Symmetric);

            AddRelationType ("CompanyHasDirector", typeof (Company), typeof (Person), RelationProperties.None);
            AddRelationType ("CompanyHasAssociate", typeof (Company), typeof (Person), RelationProperties.None);
            AddRelationType ("CompanyHasSupervisor", typeof (Company), typeof (Person), RelationProperties.None);
            AddRelationType ("CompanyHasExecutive", typeof (Company), typeof (Person), RelationProperties.None);
            AddRelationType ("CompanyHasAddress", typeof (Company), typeof (Address), RelationProperties.None);
            AddRelationType ("CompanyHasVehicle", typeof (Company), typeof (Vehicle), RelationProperties.None);
            AddRelationType ("CompanyHasPhone", typeof (Company), typeof (Phone), RelationProperties.None);
            AddRelationType ("CompanyHasEmail", typeof (Company), typeof (Email), RelationProperties.None);

            AddRelationType ("VehicleParkedOnAddress", typeof (Vehicle), typeof (Address), RelationProperties.None);

            AddRelationType ("StbDepartmentRegisteredPerson", typeof (StbDivision), typeof (Person), RelationProperties.None);
            AddRelationType ("StbAdministrationRegisteredPerson", typeof (StbDivision), typeof (Person), RelationProperties.None);

            SaveChanges ();
        }

        private void AddRelationType (string name, Type subjectiveEntityType, Type objectiveEntityType, RelationProperties properties)
        {
            RelationTypes.Add (new RelationType ()
            {
                Name = name,
                SubjectiveEntityType = subjectiveEntityType,
                ObjectiveEntityType = objectiveEntityType,
                Properties = properties
            });
        }

        #endregion

        #region DbContext Methods

        protected override void OnModelCreating (DbModelBuilder modelBuilder)
        {
            // Cascade on delete has to be disabled. More info at:
            // http://weblogs.asp.net/manavi/archive/2011/01/23/associations-in-ef-code-first-ctp5-part-3-one-to-one-foreign-key-associations.aspx
            modelBuilder.Entity<Relation> ()
               .HasRequired<Entity> (r => r.ObjectiveEntity)
               .WithMany ()
               .HasForeignKey (r => r.ObjectiveEntityId)
               .WillCascadeOnDelete (false);
        }

        #endregion
    }
}
