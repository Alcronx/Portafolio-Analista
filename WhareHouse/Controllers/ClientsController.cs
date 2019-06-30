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
using System.Data.Entity.Migrations;

namespace WhareHouse.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();
        
        public ClientsController()
        {
            CLIENT cli = db.CLIENT.Find(1);
            if (cli == null)
            {
                CreateClientId();
            }
        }
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
            if (db.CLIENT.Find(1)==null)
            {
                ViewBag.idClient = 1;
            }
            else
            {
                short idClient = db.CLIENT.Max(x => x.IDCLIENT);
                ViewBag.idClient = idClient+1;
            }
           
            return View();
        }

        // POST: Clients/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CLIENTRUT,NAME1,NAME2,LASTNAME1,LASTNAME2,CELLPHONE,BLACKLIST,BIRTHDATE")] CLIENT cLIENT)
        {
            if (ModelState.IsValid)
            {
                cLIENT.IDCLIENT = ClientIdAumentate();
                cLIENT.STATE = "1";
                db.CLIENT.Add(cLIENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idClient = 1;
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
        public ActionResult Edit([Bind(Include = "IDCLIENT,CLIENTRUT,NAME1,NAME2,LASTNAME1,LASTNAME2,CELLPHONE,BLACKLIST,BIRTHDATE")] CLIENT cLIENT)
        {
            if (ModelState.IsValid)
            {
                cLIENT.STATE = "1";
                db.Set<CLIENT>().AddOrUpdate(cLIENT);
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

        public void CreateClientId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCECLIENT";
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

        public short ClientIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCECLIENT";
            cmd.Parameters.Add("@IDCLIENT", OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            short ClientId = Convert.ToInt16(cmd.Parameters["@IDCLIENT"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return ClientId;
        }
    }
}
