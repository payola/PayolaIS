using System;
using System.Web.Mvc;
using Payola.DomainModel;
using Payola.Model;

namespace Payola.Intranet.Controllers
{
    public class ReportController : InformativeEntityController<Report, ReportModel>
    {
        #region Methods

        public ActionResult Open (long id)
        {
            Report report = Model.GetEntity (id);
            if (report == null)
            {
                return RedirectToAction ("Index");
            }

            Model.OpenReport (report);
            return RedirectToDetail (report);
        }

        public ActionResult Close (long id)
        {
            Report report = Model.GetEntity (id);
            if (report == null)
            {
                return RedirectToAction ("Index");
            }

            Model.CloseReport (report);
            return RedirectToDetail (report);
        }

        #endregion

        #region EntityController<Report, ReportModel> Members

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