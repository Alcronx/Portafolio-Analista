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
    public class ClientsController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        // GET: Clients
        public ActionResult Index()
        {
            return View(db.CLIENT.ToList());
        }

        // GET: Clients/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLIENT cLIENT = db.CLIENT.Find(id);
            if (cLIENT == null)
            {
                return HttpNotFound();
            }
            return View(cLIENT);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCLIENT,CLIENTRUT,NAME1,NAME2,LASTNAME1,LASTNAME2,CELLPHONE,BLACKLIST,BIRTHDATE,STATE")] CLIENT cLIENT)
        {
            if (ModelState.IsValid)
            {
                db.CLIENT.Add(cLIENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cLIENT);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLIENT cLIENT = db.CLIENT.Find(id);
            if (cLIENT == null)
            {
                return HttpNotFound();
            }
            return View(cLIENT);
        }

        // POST: Clients/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCLIENT,CLIENTRUT,NAME1,NAME2,LASTNAME1,LASTNAME2,CELLPHONE,BLACKLIST,BIRTHDATE,STATE")] CLIENT cLIENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cLIENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cLIENT);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLIENT cLIENT = db.CLIENT.Find(id);
            if (cLIENT == null)
            {
                return HttpNotFound();
            }
            return View(cLIENT);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            CLIENT cLIENT = db.CLIENT.Find(id);
            db.CLIENT.Remove(cLIENT);
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
