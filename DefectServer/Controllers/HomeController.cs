using DefectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DefectServer.Controllers
{
    public class HomeController : Controller
    {
        private DefectServerContext db;

        public HomeController()
        {
            db = new DefectServerContext();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Defects";

            return View();
        }

        public ActionResult List()
        {
            var Model = db.Defects.ToList();
            return View(Model);
        }

        public ActionResult Details(int id)
        {
            Defect DefectModel = db.Defects
                .Where(d => d.Id == id)
                .FirstOrDefault();

            return View(DefectModel);
        }
    }
}
