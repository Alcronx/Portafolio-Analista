using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WhareHouse.Models;

namespace WhareHouse.Controllers
{
    [Authorize]
    public class TrustedController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        public ActionResult Index()
        {
            return View(db.TICKET.ToList().Where(x=> x.IDTRUSTED != null));
        }

        public ActionResult NoMoney(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRUSTED tRUSTED = db.TRUSTED.Find(id);
            if (tRUSTED == null)
            {
                return HttpNotFound();
            }
            tRUSTED.STATE = "0";
            db.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult YesMoney(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRUSTED tRUSTED = db.TRUSTED.Find(id);
            if (tRUSTED == null)
            {
                return HttpNotFound();
            }
            tRUSTED.STATE = "1";
            db.SaveChanges();
            return RedirectToAction("index");
        }

    }
}