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
    public class LoginController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        // GET: Login
        public ActionResult Index()
        {
            return View(db.LOGIN.ToList());
        }

        // GET: Login/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOGIN lOGIN = db.LOGIN.Find(id);
            if (lOGIN == null)
            {
                return HttpNotFound();
            }
            return View(lOGIN);
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            ViewBag.error = 0;
            ViewBag.error2 = 0;
            return View();
        }

        // POST: Login/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDUSER,USERNAME,PASSWORDUSER,ROL")] LOGIN lOGIN)
        {

            var revisarid = db.LOGIN.Any(x => x.IDUSER == lOGIN.IDUSER);
            var revisarNombre = db.LOGIN.Any(x => x.USERNAME == lOGIN.USERNAME);
            if (!revisarid) {
                if (!revisarNombre)
                {
                    if (ModelState.IsValid)
                    {
                        db.LOGIN.Add(lOGIN);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.error2 = 0;
            ViewBag.error = 0;
            if (revisarid)
            {
                ViewBag.error = 1;
                return View(lOGIN);
            }
            if (revisarNombre)
            {
                ViewBag.error2 = 1;
                return View(lOGIN);
            }
            return View(lOGIN);
        }

        // GET: Login/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOGIN lOGIN = db.LOGIN.Find(id);
            if (lOGIN == null)
            {
                return HttpNotFound();
            }
            return View(lOGIN);
            
        }

        // POST: Login/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDUSER,USERNAME,PASSWORDUSER,ROL")] LOGIN lOGIN)
        {
                if (ModelState.IsValid)
                {
                    db.Entry(lOGIN).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            
            return View(lOGIN);
        }

        // GET: Login/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOGIN lOGIN = db.LOGIN.Find(id);
            if (lOGIN == null)
            {
                return HttpNotFound();
            }
            return View(lOGIN);
        }

        // POST: Login/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            LOGIN lOGIN = db.LOGIN.Find(id);
            db.LOGIN.Remove(lOGIN);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
    }
}
