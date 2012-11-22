
namespace Payola.Model
{
    public class ModelBase
    {
        #region Constructors

        public ModelBase (PayolaContext db)
        {
            Db = db;
        }

        #endregion

        #region Properties

        internal PayolaContext Db 
        { 
            get; 
            set;
        }

        #endregion
    }
}
