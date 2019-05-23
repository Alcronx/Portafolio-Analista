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
using Oracle.DataAccess.Types;

namespace WhareHouse.Controllers
{
    public class ProvidersController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();

        // GET: Providers
        public ActionResult Index()
        {

            Connection objconexion = new Connection();
            OracleConnection cn = objconexion.GetConection();
            cn.Open();
            OracleCommand cmd = cn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "LISTPROEVIDER";
            OracleParameter par1 = new OracleParameter
            {
                OracleDbType = OracleDbType.RefCursor,
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(par1);
            cmd.ExecuteNonQuery();
            OracleRefCursor cursor = (OracleRefCursor)par1.Value;
            OracleDataReader dr = cursor.GetDataReader();
            List<PROVIDER> listprovider = new List<PROVIDER>();
            while (dr.Read())
            {
                PROVIDER pro = new PROVIDER
                {
                    IDPROVIDER = dr.GetByte(0),
                    RUT = dr.GetString(1),
                    COMPANYNAME = dr.GetString(2),
                    NAME1 = dr.GetString(3),
                    NAME2 = dr.GetString(4),
                    LASTNAME1 = dr.GetString(5),
                    LASTNAME2 = dr.GetString(6),
                    REGION = dr.GetString(7),
                    COMMUNE = dr.GetString(8),
                    DIRECTION = dr.GetString(9),
                    COMPANYITEM = dr.GetString(10),
                    CELLPHONE = dr.GetInt64(11),
                    MAIL = dr.GetString(12)
                };
                listprovider.Add(pro);
            }
            return View(listprovider);

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
            return View();
        }

        // POST: Providers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPROVIDER,RUT,COMPANYNAME,NAME1,NAME2,LASTNAME1,LASTNAME2,REGION,COMMUNE,DIRECTION,COMPANYITEM,CELLPHONE,MAIL")] PROVIDER pROVIDER)
        {
            if (ModelState.IsValid)
            {
                db.PROVIDER.Add(pROVIDER);
                db.SaveChanges();
                return RedirectToAction("Index");
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
        public ActionResult Edit([Bind(Include = "IDPROVIDER,RUT,COMPANYNAME,NAME1,NAME2,LASTNAME1,LASTNAME2,REGION,COMMUNE,DIRECTION,COMPANYITEM,CELLPHONE,MAIL")] PROVIDER pROVIDER)
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
    }
}
