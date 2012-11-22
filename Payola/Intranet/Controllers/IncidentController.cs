using System;
using System.Web.Mvc;
using Payola.DomainModel;
using Payola.Model;

namespace Payola.Intranet.Controllers
{
    public class IncidentController : InformativeEntityController<Incident, IncidentModel>
    {
        #region Methods

        public ActionResult Open (long id)
        {
            Incident incidentToOpen = Model.GetEntity (id);
            if (incidentToOpen == null)
            {
                return RedirectToAction ("Index");
            }

            Model.OpenIncident (incidentToOpen);
            return RedirectToDetail (incidentToOpen);
        }

        public ActionResult Close (long id)
        {
            Incident incidentToClose = Model.GetEntity (id);
            if (incidentToClose == null || !incidentToClose.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            return View (incidentToClose);
        }

        [HttpPost]
        public ActionResult Close (long id, FormCollection formCollection)
        {
            Incident incidentToClose = Model.GetEntity (id);
            if (incidentToClose == null || !incidentToClose.IsEditable)
            {
                return RedirectToAction ("Index");
            }

            TryUpdateModel (incidentToClose);
            Model.CloseIncident (incidentToClose);
            return RedirectToDetail (incidentToClose);
        }

        #endregion

        #region EntityController<Incident, IncidentModel> Members

        protected override string DetailView
        {
            get
            {
                return String.Empty;
            }
        }

        #endregion
    }
}