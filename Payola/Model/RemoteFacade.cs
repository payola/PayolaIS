using System;
using System.Diagnostics.CodeAnalysis;

namespace Payola.Model
{
    public class RemoteFacade
    {
        #region Fields

        private static RemoteFacade instance = new RemoteFacade ();

        #endregion

        #region Properties

        public static RemoteFacade Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #region Methods

        public TModel GetModel<TModel> (PayolaContext db)
            where TModel : ModelBase
        {
            return (TModel) GetModel (typeof (TModel), db);
        }

        public ModelBase GetModel (Type modelType, PayolaContext db)
        {
            return (ModelBase) modelType.GetConstructor (new Type[] { typeof (PayolaContext) }).Invoke (new object[] { db });
        }

        public IEntityModel GetEntityModel (Type entityType, PayolaContext db)
        {
            Type modelType = Type.GetType (typeof (EntityModel<>).Namespace + "." + entityType.Name + "Model");
            return (IEntityModel) GetModel (modelType, db);
        }

        #endregion
    }
}