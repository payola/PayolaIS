using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Payola.DomainModel;
using Payola.Intranet.Models.ViewModels;
using Payola.Model;

namespace Payola.Intranet.Controllers
{
    public abstract class EntityController<TEntity, TModel> : Controller
        where TEntity : Entity, new ()
        where TModel : EntityModel<TEntity>
    {
        #region Fields

        private const string DEFAULT_INDEX_VIEW = "Entity/Index";

        private const string DEFAULT_ADVANCED_SEARCH_VIEW = "Entity/AdvancedSearch";

        private const string DEFAULT_CREATE_VIEW = "Entity/Create";

        private const string DEFAULT_DETAIL_VIEW = "Entity/Detail";

        private const string DEFAULT_EDIT_VIEW = "Entity/Edit";

        private const string DEFAULT_DELETE_VIEW = "Entity/Delete";

        private const string DEFAULT_RELATIONS_VIEW = "Entity/Relations";

        private const string DEFAULT_EDIT_RELATION_VIEW = "Entity/EditRelation";

        private const string DEFAULT_CREATE_AND_ADD_RELATION_VIEW = "Entity/CreateAndAddRelation";

        private PayolaContext db;

        #endregion

        #region Properties

        protected TModel Model
        {
            get;
            private set;
        }

        protected string IndexView
        {
            get { return DEFAULT_INDEX_VIEW; }
        }

        protected string AdvancedSearchView
        {
            get { return DEFAULT_ADVANCED_SEARCH_VIEW; }
        }

        protected string CreateView
        {
            get { return DEFAULT_CREATE_VIEW; }
        }

        protected virtual string DetailView
        {
            get { return DEFAULT_DETAIL_VIEW; }
        }

        protected string EditView
        {
            get { return DEFAULT_EDIT_VIEW; }
        }

        protected string DeleteView
        {
            get { return DEFAULT_DELETE_VIEW; }
        }

        protected string RelationsView
        {
            get { return DEFAULT_RELATIONS_VIEW; }
        }

        protected string EditRelationView
        {
            get { return DEFAULT_EDIT_RELATION_VIEW; }
        }

        protected string CreateAndAddRelationView
        {
            get { return DEFAULT_CREATE_AND_ADD_RELATION_VIEW; }
        }

        #endregion

        #region Methods

        public ViewResult Index ()
        {
            return View (IndexView, Model.GetEntities ());
        }

        [HttpPost]
        public JsonResult Search (string needle)
        {
            return new JsonResult () { Data = SerializeEntities (Model.GetEntities (needle)) };
        }

        public ActionResult AdvancedSearch (FormCollection formCollection)
        {
            TEntity patternEntity = new TEntity ();
            IEnumerable<TEntity> foundEntities = Enumerable.Empty<TEntity> ();
            if (Request.QueryString.Count > 0)
            {
                TryUpdateModel (patternEntity, "Entity");
                foundEntities = Model.GetEntities (patternEntity);
            }

            return View (AdvancedSearchView, new AdvancedSearchViewModel ()
            {
                Entity = patternEntity,
                FoundEntities = foundEntities
            });
        }

        public ActionResult Create ()
        {
            return View (CreateView, Model.GetNewEntity ());
        }

        [HttpPost]
        public ActionResult Create (TEntity entity)
        {
            if (ModelState.IsValid)
            {
                return RedirectToDetail (Model.AddEntity (entity));
            }
            return View (CreateView, entity);
        }

        public ActionResult Detail (long id)
        {
            TEntity entity = Model.GetEntity (id);
            if (entity == null)
            {
                return RedirectToAction ("Index");
            }

            return View (DetailView, new EntityDetailViewModel ()
            {
                Entity = entity,
                Relations = RemoteFacade.Instance.GetModel<RelationModel> (db).GetRelations (entity)
            });
        }

        public ActionResult Edit (long id)
        {
            TEntity entity = Model.GetEntity (id);
            if (entity == null || !entity.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            return View (EditView, new EntityEditViewModel ()
            {
                Entity = entity,
                Identification = entity.Identification
            });
        }

        [HttpPost]
        public ActionResult Edit (long id, FormCollection formCollection)
        {
            TEntity entity = Model.GetEntity (id);
            if (entity == null || !entity.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            // The identification of the entity has to be remembered because some of the fields, that involve the identification
            // could been filled invalidly. That would cause that invalid identifaction of the entity is displayed.
            string validIdentification = entity.Identification;
            TryUpdateModel (entity, "Entity");
            if (ModelState.IsValid)
            {
                Model.SaveEntity (entity);
                return RedirectToDetail (entity);
            }

            return View (EditView, new EntityEditViewModel ()
            {
                Entity = entity,
                Identification = validIdentification
            });
        }

        public ActionResult Delete (long id)
        {
            TEntity entity = Model.GetEntity (id);
            if (entity == null)
            {
                return RedirectToAction ("Index");
            }

            return View (DeleteView, new EntityViewModel ()
            {
                Entity = entity
            });
        }

        [HttpPost, ActionName ("Delete")]
        public ActionResult DeleteConfirmed (long id)
        {
            Model.DeleteEntity (id);
            return RedirectToAction ("Index");
        }

        public ActionResult Relations (long relationTypeId, long entityId)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity == null)
            {
                return RedirectToAction ("Index");
            }

            TypedRelations relations = RemoteFacade.Instance.GetModel<RelationModel> (db).GetRelations (relationTypeId, entity);
            if (relations == null)
            {
                return RedirectToAction ("Index");
            }

            return View (RelationsView, new EntityRelationsViewModel ()
            {
                Entity = entity,
                Relations = relations
            });
        }

        [HttpPost]
        public JsonResult SearchForRelatableEntities (long relationTypeId, long entityId, string needle)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity != null)
            {
                return new JsonResult ()
                {
                    Data = SerializeEntities (RemoteFacade.Instance.GetModel<RelationModel> (db).GetRelatableEntities (
                        relationTypeId, entity, needle))
                };
            }
            return new JsonResult ();
        }

        [HttpPost]
        public ActionResult AddRelation (long relationTypeId, long entityId, long relatedEntityId)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity != null)
            {
                RemoteFacade.Instance.GetModel<RelationModel> (db).AddRelation (relationTypeId, entity, relatedEntityId);
            }
            return RedirectToAction ("Relations", new { RelationTypeId = relationTypeId, EntityId = entityId });
        }

        [HttpPost]
        public ActionResult CreateAndAddRelation (long relationTypeId, long entityId, string relatedEntityId)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity == null || !entity.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            // TODO better check of relationTypeId validity.
            RelationModel relationModel = RemoteFacade.Instance.GetModel<RelationModel> (db);
            TypedRelations relations = relationModel.GetRelations (relationTypeId, entity);
            if (relations == null)
            {
                return RedirectToAction ("Index");
            }

            return View (CreateAndAddRelationView, new CreateAndAddRelationViewModel ()
            {
                Entity = entity,
                RelationTypeId = relationTypeId,
                NewEntity = relationModel.GetNewObjectiveEntity (relations.RelationType, relatedEntityId)
            });
        }

        [HttpPost]
        public ActionResult PerformCreateAndAddRelation (long relationTypeId, long entityId, FormCollection formCollection)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity == null || !entity.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            // TODO better check of relationTypeId validity.
            RelationModel relationModel = RemoteFacade.Instance.GetModel<RelationModel> (db);
            TypedRelations relations = relationModel.GetRelations (relationTypeId, entity);
            if (relations == null)
            {
                return RedirectToAction ("Index");
            }

            Entity newEntity = relationModel.GetNewObjectiveEntity (relations.RelationType, String.Empty);
            TryUpdateModel (relations.RelationType.ObjectiveEntityType, newEntity, "NewEntity");
            if (ModelState.IsValid)
            {
                newEntity = RemoteFacade.Instance.GetEntityModel (newEntity.GetType (), db).AddNewEntity (newEntity);
                relationModel.AddRelation (relationTypeId, entity, newEntity.Id);
                return RedirectToAction ("Relations", new { RelationTypeId = relationTypeId, EntityId = entityId });
            }

            return View (CreateAndAddRelationView, new CreateAndAddRelationViewModel ()
            {
                Entity = entity,
                RelationTypeId = relationTypeId,
                NewEntity = (Entity) newEntity
            });
        }

        public ActionResult EditRelation (long relationTypeId, long relationId, long entityId)
        {
            Relation relation = RemoteFacade.Instance.GetModel<RelationModel> (db).GetRelation (relationId);
            if (relation == null)
            {
                RedirectToAction ("Index");
            }

            return View (EditRelationView, new RelationEditViewModel ()
            {
                RelationTypeId = relationTypeId,
                Relation = relation,
                EntityId = entityId
            });
        }

        [HttpPost]
        public ActionResult EditRelation (long relationTypeId, long relationId, long entityId, FormCollection formCollection)
        {
            RelationModel relationModel = RemoteFacade.Instance.GetModel<RelationModel> (db);
            Relation relation = relationModel.GetRelation (relationId);
            if (relation == null)
            {
                RedirectToAction ("Index");
            }

            TryUpdateModel (relation, "Relation");
            if (ModelState.IsValid)
            {
                relationModel.SaveRelation (relation);
                return RedirectToAction ("Relations", new
                {
                    RelationTypeId = relationTypeId,
                    EntityId = entityId
                });
            }

            return View (EditRelationView, new RelationEditViewModel ()
            {
                RelationTypeId = relationTypeId,
                Relation = relation,
                EntityId = entityId
            });
        }

        public ActionResult DeleteRelation (long relationTypeId, long entityId, long relationId)
        {
            TEntity entity = Model.GetEntity (entityId);
            if (entity != null && entity.IsEditable)
            {
                RemoteFacade.Instance.GetModel<RelationModel> (db).DeleteRelation (relationTypeId, entity, relationId);
            }
            return RedirectToAction ("Relations", new { RelationTypeId = relationTypeId, EntityId = entityId });
        }

        protected RedirectToRouteResult RedirectToDetail (TEntity entity)
        {
            return RedirectToAction ("Detail", new { Id = entity.Id });
        }

        protected bool TryUpdateModel (Type modelType, object model, string prefix)
        {
            List<MethodInfo> tryUpdateModels = GetType ().GetMethods (BindingFlags.NonPublic | BindingFlags.Instance).Where (
                m => m.Name == "TryUpdateModel" && m.IsGenericMethod).ToList ();
            MethodInfo tryUpdateModel = tryUpdateModels[1].MakeGenericMethod (modelType);
            return (bool) tryUpdateModel.Invoke (this, new object[] { model, prefix });
        }

        private string SerializeEntities (IEnumerable<Entity> entities)
        {
            Type serializationType = typeof (Entity);

            // If all the entities are of the same type, use better fitting type.
            if (entities.Any () && entities.Select<Entity, string> (e => e.GetType ().Name).Distinct ().Count () == 1)
            {
                serializationType = Entity.GetEntityType (entities.First ());
            }

            List<PropertyInfo> propertiesToSerialize = serializationType.GetProperties ().Where (
                p => p.GetCustomAttributes (typeof (DataMemberAttribute), true).Any ()).ToList ();

            List<Dictionary<string, object>> entityProperties = new List<Dictionary<string, object>> ();
            foreach (Entity entity in entities)
            {
                entityProperties.Add (propertiesToSerialize.ToDictionary (p => p.Name, p => p.GetValue (entity, null)));
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer ();
            return serializer.Serialize (entityProperties);
        }

        #endregion

        #region Controller Members

        protected override void Initialize (RequestContext requestContext)
        {
            base.Initialize (requestContext);

            Database.SetInitializer<PayolaContext> (new PayolaContextInitializer ());
            db = new PayolaContext ();

            Model = RemoteFacade.Instance.GetModel<TModel> (db);
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted (ActionExecutedContext filterContext)
        {
            base.OnActionExecuted (filterContext);
            if (filterContext.Result is ViewResult)
            {
                ViewBag.EntityTypeName = typeof (TEntity).Name;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">
        /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected override void Dispose (bool disposing)
        {
            db.Dispose ();
            base.Dispose (disposing);
        }

        #endregion
    }
}