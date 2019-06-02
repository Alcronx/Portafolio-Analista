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
    public class ExpirationDateController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        // GET: ExpirationDate
        public ActionResult Index()
        {
            var FirsId = from m in db.PRODUCT.ToList()
                         select m.IDPROVIDER;
            int id = FirsId.First();
            ViewBag.IdProviderOne = id;
            var eXPIRATIONDATE = db.EXPIRATIONDATE.Include(e => e.PRODUCT);
            return View(eXPIRATIONDATE.ToList());
        }

        // GET: ExpirationDate/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXPIRATIONDATE eXPIRATIONDATE = db.EXPIRATIONDATE.Find(id);
            if (eXPIRATIONDATE == null)
            {
                return HttpNotFound();
            }
            return View(eXPIRATIONDATE);
        }

        // GET: ExpirationDate/Create
        public ActionResult Create(int id) 
        {
            var FirsId = from m in db.PRODUCT.ToList()
                         select m;
            
            List<PRODUCT> pro = BuscarProductos(id);
            ViewBag.PROVIDERNAME = new SelectList(FirsId, "IDPROVIDER", "COMPANYNAME");
            ViewBag.Provider = new PROVIDER();
            var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER);
            ViewBag.ProductList = pro;
            return View();
        }

        // POST: ExpirationDate/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOTNUMBER,EXPIREDATE,PRODUCTQUANTITY,BARCODE")] EXPIRATIONDATE eXPIRATIONDATE)
        {
                    db.EXPIRATIONDATE.Add(eXPIRATIONDATE);
                    db.SaveChanges();
                    return RedirectToAction("Index");
        }

        // GET: ExpirationDate/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXPIRATIONDATE eXPIRATIONDATE = db.EXPIRATIONDATE.Find(id);
            if (eXPIRATIONDATE == null)
            {
                return HttpNotFound();
            }
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER);
            ViewBag.ProductList = ProductCreate.ToList();
            return View(eXPIRATIONDATE);
        }

        // POST: ExpirationDate/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOTNUMBER,EXPIREDATE,PRODUCTQUANTITY,BARCODE")] EXPIRATIONDATE eXPIRATIONDATE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eXPIRATIONDATE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BARCODE = new SelectList(db.PRODUCT, "IDBARCODE", "PRODUCTNAME", eXPIRATIONDATE.BARCODE);
            return View(eXPIRATIONDATE);
        }

        // GET: ExpirationDate/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXPIRATIONDATE eXPIRATIONDATE = db.EXPIRATIONDATE.Find(id);
            if (eXPIRATIONDATE == null)
            {
                return HttpNotFound();
            }
            return View(eXPIRATIONDATE);
        }

        // POST: ExpirationDate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            EXPIRATIONDATE eXPIRATIONDATE = db.EXPIRATIONDATE.Find(id);
            db.EXPIRATIONDATE.Remove(eXPIRATIONDATE);
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

        public List<PRODUCT> BuscarProductos(int IdProvedor)
        {
            var listProduct = db.PRODUCT.ToList();
            var consulta = from model in listProduct
                           where model.IDPROVIDER == IdProvedor
                           select model;

            return consulta.ToList();
        }
    }
}
