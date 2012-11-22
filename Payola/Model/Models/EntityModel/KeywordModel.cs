using System.Data.Objects.SqlClient;
using System.Linq;
using Payola.DomainModel;

namespace Payola.Model
{
    public class KeywordModel : EntityModel<Keyword>
    {
        #region Constructors

        public KeywordModel (PayolaContext db)
            : base (db)
        {
            // NOOP
        }

        #endregion

        #region EntityModel<Keyword> Methods

        public override Keyword AddEntity (Keyword entity)
        {
            Keyword keyword = Db.Keywords.Where (k => !k.IsDeleted && k.Value == entity.Value).FirstOrDefault ();
            if (keyword == null)
            {
                return base.AddEntity (entity);
            }
            return keyword;
        }

        protected override IQueryable<Keyword> FilterEntitiesByNeedle (IQueryable<Keyword> entities, string needle)
        {
            return entities.Where (k => k.Value.Contains (needle));
        }

        #endregion
    }
}
