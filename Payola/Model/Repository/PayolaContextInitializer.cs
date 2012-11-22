using System;
using System.Data.Entity;
using System.Data.Entity.Design;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Payola.DomainModel;

namespace Payola.Model
{
    /// <summary>
    /// Based on the following post:
    /// http://stackoverflow.com/questions/4031431/entity-framework-ctp-4-code-first-custom-database-initializer/5018062#5018062
    /// </summary>
    public class PayolaContextInitializer : IDatabaseInitializer<PayolaContext>
    {
        #region Fields

        private EdmMetadata edmMetaData;

        #endregion

        #region Methods

        private static void CreateTables (ObjectContext objectContext)
        {
            string dataBaseCreateScript = objectContext.CreateDatabaseScript ();
            objectContext.ExecuteStoreCommand (dataBaseCreateScript);
        }

        private static void DeleteEverythingFromDatabase (ObjectContext objectContext)
        {
            // http://blog.falafel.com/Blogs/AdamAnderson/09-01-06/T-SQL_Drop_All_Objects_in_a_SQL_Server_Database.aspx
            objectContext.ExecuteStoreCommand (@"
                declare @n char(1)
                set @n = char(10)

                declare @stmt nvarchar(max)

                -- procedures
                select @stmt = isnull( @stmt + @n, '' ) +
                    'drop procedure [' + name + ']'
                from sys.procedures

                -- check constraints
                select @stmt = isnull( @stmt + @n, '' ) +
                    'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']'
                from sys.check_constraints

                -- functions
                select @stmt = isnull( @stmt + @n, '' ) +
                    'drop function [' + name + ']'
                from sys.objects
                where type in ( 'FN', 'IF', 'TF' )

                -- views
                select @stmt = isnull( @stmt + @n, '' ) +
                    'drop view [' + name + ']'
                from sys.views

                -- foreign keys
                select @stmt = isnull( @stmt + @n, '' ) +
                    'alter table [' + object_name( parent_object_id ) + '] drop constraint [' + name + ']'
                from sys.foreign_keys

                -- tables
                select @stmt = isnull( @stmt + @n, '' ) +
                    'drop table [' + name + ']'
                from sys.tables

                -- user defined types
                select @stmt = isnull( @stmt + @n, '' ) +
                    'drop type [' + name + ']'
                from sys.types
                where is_user_defined = 1

                exec sp_executesql @stmt");
        }

        private static string GetModelHash (ObjectContext context)
        {
            var csdlXmlString = GetCsdlXmlString (context).ToString ();
            return ComputeSha256Hash (csdlXmlString);
        }

        private static string GetCsdlXmlString (ObjectContext context)
        {
            if (context != null)
            {
                var entityContainerList = context.MetadataWorkspace.GetItems<EntityContainer> (DataSpace.SSpace);
                if (entityContainerList != null)
                {
                    EntityContainer entityContainer = entityContainerList.FirstOrDefault ();
                    var generator = new EntityModelSchemaGenerator (entityContainer);
                    var stringBuilder = new StringBuilder ();
                    var xmlWRiter = XmlWriter.Create (stringBuilder);
                    generator.GenerateMetadata ();
                    generator.WriteModelSchema (xmlWRiter);
                    xmlWRiter.Flush ();
                    return stringBuilder.ToString ();
                }
            }
            return string.Empty;
        }

        private static string ComputeSha256Hash (string input)
        {
            byte[] buffer = new SHA256Managed ().ComputeHash (Encoding.ASCII.GetBytes (input));
            var builder = new StringBuilder (buffer.Length * 2);
            foreach (byte num in buffer)
            {
                builder.Append (num.ToString ("X2", CultureInfo.InvariantCulture));
            }
            return builder.ToString ();
        }

        private void SaveModelHashToDatabase (PayolaContext context, string modelHash, ObjectContext objectContext)
        {
            if (edmMetaData != null)
            {
                objectContext.Detach (edmMetaData);
            }

            edmMetaData = new EdmMetadata ();
            context.Set<EdmMetadata> ().Add (edmMetaData);
            edmMetaData.ModelHash = modelHash;
            context.SaveChanges ();
        }

        private bool CompatibleWithModel (string modelHash, DbContext context, ObjectContext objectContext)
        {
            var isEdmMetaDataInStore = objectContext.ExecuteStoreQuery<int> (@"
                Select COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES T 
                Where T.TABLE_NAME = 'EdmMetaData'").FirstOrDefault ();

            if (isEdmMetaDataInStore == 1)
            {
                edmMetaData = context.Set<EdmMetadata> ().FirstOrDefault ();
                if (edmMetaData != null)
                {
                    return modelHash == edmMetaData.ModelHash;
                }
            }
            return false;
        }

        #endregion

        #region IDatabaseInitializer<PayolaContext> Methods

        public void InitializeDatabase (PayolaContext context)
        {
            ObjectContext objectContext = ((IObjectContextAdapter) context).ObjectContext;
            string modelHash = GetModelHash (objectContext);

            if (!CompatibleWithModel (modelHash, context, objectContext))
            {
                DeleteEverythingFromDatabase (objectContext);
                CreateTables (objectContext);
                SaveModelHashToDatabase (context, modelHash, objectContext);
                context.Seed ();
            }
        }

        #endregion
    }
}