using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WhareHouse.Models;
using Oracle.DataAccess.Client;

namespace WhareHouse.Controllers
{
    [Authorize]
    public class ProvidersController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        public ProvidersController()
        {
            PROVIDER PRO = db.PROVIDER.Find(1);
            if (PRO == null)
            {
                CreateProviderId();
            }
        }
        // GET: Providers
        public ActionResult Index()
        {    
            return View(db.PROVIDER.ToList());
        }

        // GET: Providers/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROVIDER pROVIDER = db.PROVIDER.Find(id);
            if (pROVIDER == null)
            {
                return HttpNotFound();
            }
            return View(pROVIDER);
        }

        // GET: Providers/Create
        public ActionResult Create()
        {
            if (db.PROVIDER.Find(1) == null)
            {
                ViewBag.idProvider = 1;
            }
            else
            {
                short idProvider = db.PROVIDER.Max(x => x.IDPROVIDER);
                ViewBag.idProvider = idProvider + 1;
            }
            return View();
        }

        // POST: Providers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RUT,COMPANYNAME,NAME1,NAME2,LASTNAME1,LASTNAME2,REGION,COMMUNE,DIRECTION,COMPANYITEM,CELLPHONE,MAIL")] PROVIDER pROVIDER)
        {
            if (ModelState.IsValid)
            {
                pROVIDER.IDPROVIDER = ProviderIdAumentate();
                pROVIDER.STATE = "1";
                db.PROVIDER.Add(pROVIDER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (db.PROVIDER.Find(1) == null)
            {
                ViewBag.idProvider = 1;
            }
            else
            {
                short idProvider = db.PROVIDER.Max(x => x.IDPROVIDER);
                ViewBag.idProvider = idProvider + 1;
            }

            return View(pROVIDER);
        }

        // GET: Providers/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROVIDER pROVIDER = db.PROVIDER.Find(id);
            if (pROVIDER == null)
            {
                return HttpNotFound();
            }
            return View(pROVIDER);
        }

        // POST: Providers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPROVIDER,RUT,COMPANYNAME,NAME1,NAME2,LASTNAME1,LASTNAME2,REGION,COMMUNE,DIRECTION,COMPANYITEM,CELLPHONE,MAIL,STATE")] PROVIDER pROVIDER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pROVIDER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pROVIDER);
        }

        // GET: Providers/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROVIDER pROVIDER = db.PROVIDER.Find(id);
            if (pROVIDER == null)
            {
                return HttpNotFound();
            }
            return View(pROVIDER);
        }

        // POST: Providers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            PROVIDER pROVIDER = db.PROVIDER.Find(id);
            db.PROVIDER.Remove(pROVIDER);
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

        public void CreateProviderId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCEPROVIDER";
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
                cmd.Dispose();
                con.Dispose();
                objectcon = null;
            }

        }

        public short ProviderIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCEPROVIDER";
            cmd.Parameters.Add("@IDPROVIDER", OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            short ClientId = Convert.ToInt16(cmd.Parameters["@IDPROVIDER"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return ClientId;
        }
    }
}
