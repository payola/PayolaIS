using System.Web.Mvc;
using Payola.DomainModel;
using Payola.Intranet.Models.ViewModels;
using Payola.Model;

namespace Payola.Intranet.Controllers
{
    public abstract class InformativeEntityController<TEntity, TModel> : EntityController<TEntity, TModel>
        where TEntity : InformativeEntity, new ()
        where TModel : InformativeEntityModel<TEntity>
    {
        #region Methods

        public ActionResult Publish (long id)
        {
            TEntity informativeEntity = Model.GetEntity (id);
            if (informativeEntity == null || !informativeEntity.IsPublishable)
            {
                return RedirectToAction ("Index");
            }

            return View ("Entity/InformativeEntity/Publish", new InformativeEntityPublicationViewModel ()
            {
                Entity = informativeEntity,
                Publication = new Publication ()
            });
        }

        [HttpPost]
        public ActionResult Publish (long id, FormCollection formCollection)
        {
            TEntity informativeEntity = Model.GetEntity (id);
            if (informativeEntity == null || !informativeEntity.IsPublishable)
            {
                return RedirectToAction ("Index");
            }

            Publication publication = new Publication ();
            TryUpdateModel (publication, "Publication");
            if (ModelState.IsValid)
            {
                Model.Publish (informativeEntity, publication);
                return RedirectToAction ("Publications", new { Id = informativeEntity.Id });
            }

            return View ("Entity/InformativeEntity/Publish", new InformativeEntityPublicationViewModel ()
            {
                Entity = informativeEntity,
                Publication = publication
            });
        }

        public ActionResult Publications (long id)
        {
            TEntity informativeEntity = Model.GetEntity (id);
            if (informativeEntity == null)
            {
                return RedirectToAction ("Index");
            }

            return View ("Entity/InformativeEntity/Publications", informativeEntity);
        }

        #endregion

        #region EntityController<TEntity, TModel> Members

        protected override string DetailView
        {
            get
            {
                return "Entity/InformativeEntity/Detail";
            }
        }

        #endregion
    }
}