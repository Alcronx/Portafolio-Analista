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
    public class ExpirationDateController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();
        public ExpirationDateController()
        {
            EXPIRATIONDATE exp = db.EXPIRATIONDATE.Find(1);
            if (exp == null)
            {
                CreateExpirationDateId();
            }
        }
        // GET: ExpirationDate
        public ActionResult Index()
        {
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
        public ActionResult Create(string PROVIDERNAME="0", int Error = 0) 
        {
            if (db.EXPIRATIONDATE.Find(1) == null)
            {
                ViewBag.idExpirationDate = 1;
            }
            else
            {
                long idExpirationDate = db.EXPIRATIONDATE.Max(x => x.LOTNUMBER);
                ViewBag.idExpirationDate = idExpirationDate + 1;
            }
            int convert = Convert.ToInt16(PROVIDERNAME);
            ViewBag.Error = Error;
            var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDPROVIDER == convert); 
            ViewBag.ProductList = ProductCreate.ToList();
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            return View();
        }

        // POST: ExpirationDate/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOTNUMBER,EXPIREDATE,PRODUCTQUANTITY,BARCODE")] EXPIRATIONDATE eXPIRATIONDATE,string PROVIDERNAME= "0", int Error = 0)
        {
            if (eXPIRATIONDATE.BARCODE != 0)
            {
                if (ModelState.IsValid)
                {
                    eXPIRATIONDATE.LOTNUMBER = ExpirationDateIdAumentate();
                    db.EXPIRATIONDATE.Add(eXPIRATIONDATE);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            if (db.EXPIRATIONDATE.Find(1) == null)
            {
                ViewBag.idExpirationDate = 1;
            }
            else
            {
                long idExpirationDate = db.EXPIRATIONDATE.Max(x => x.LOTNUMBER);
                ViewBag.idExpirationDate = idExpirationDate + 1;
            }
            if (eXPIRATIONDATE.BARCODE == 0)
            {
                ViewBag.ErrorRadioButton = "Seleccione Un Producto";
            }
            int convert = Convert.ToInt16(PROVIDERNAME);
            ViewBag.Error = Error;
            var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDPROVIDER == convert);
            ViewBag.ProductList = ProductCreate.ToList();
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            return View(eXPIRATIONDATE);
        }
        [ActionName("CreateSearch")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? PROVIDERNAME)
        {
            if(PROVIDERNAME == null)
            {
                return RedirectToAction("Create", new { Error = 1 });
            }
            string convert = PROVIDERNAME.ToString();
            return RedirectToAction("Create",new { PROVIDERNAME = convert });
        }

        // GET: ExpirationDate/Edit/5
        public ActionResult Edit(long? id, string PROVIDERNAME = "0")
        {
            int convert = Convert.ToInt16(PROVIDERNAME);
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXPIRATIONDATE eXPIRATIONDATE = db.EXPIRATIONDATE.Find(id);
            if (eXPIRATIONDATE == null)
            {
                return HttpNotFound();
            }
            
            if (convert == 0)
            {
                var lotnumber = (from model in db.EXPIRATIONDATE where model.LOTNUMBER == id select new { model.BARCODE }).Single();
                int converLotNumber = lotnumber.BARCODE;
                var idbarcode = (from model in db.PRODUCT where model.IDBARCODE == converLotNumber select new { model.IDBARCODE }).Single();
                int convertidbarcode = idbarcode.IDBARCODE;
                var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDBARCODE== convertidbarcode);
                ViewBag.ProductList = ProductCreate.ToList();
                return View(eXPIRATIONDATE);
            }

            var Product = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDPROVIDER==convert);
            ViewBag.ProductList = Product.ToList();
            return View(eXPIRATIONDATE);
        }

        // POST: ExpirationDate/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOTNUMBER,EXPIREDATE,PRODUCTQUANTITY,BARCODE")] EXPIRATIONDATE eXPIRATIONDATE,string PROVIDERNAME = "0")
        {
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            if (ModelState.IsValid)
            {
                db.Entry(eXPIRATIONDATE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            long id = eXPIRATIONDATE.LOTNUMBER;
            int convert = Convert.ToInt16(PROVIDERNAME);
            EXPIRATIONDATE Expire = db.EXPIRATIONDATE.Find(id);
            if (eXPIRATIONDATE == null)
            {
                return HttpNotFound();
            }

            if (convert == 0)
            {
                var lotnumber = (from model in db.EXPIRATIONDATE where model.LOTNUMBER == id select new { model.BARCODE }).Single();
                int converLotNumber = lotnumber.BARCODE;
                var idbarcode = (from model in db.PRODUCT where model.IDBARCODE == converLotNumber select new { model.IDBARCODE }).Single();
                int convertidbarcode = idbarcode.IDBARCODE;
                var ProductCreate = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDBARCODE == convertidbarcode);
                ViewBag.ProductList = ProductCreate.ToList();
                return View(eXPIRATIONDATE);
            }

            var Product = db.PRODUCT.Include(x => x.PROVIDER).Where(x => x.IDPROVIDER == convert);
            ViewBag.ProductList = Product.ToList();

            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            ViewBag.BARCODE = new SelectList(db.PRODUCT, "IDBARCODE", "PRODUCTNAME", eXPIRATIONDATE.BARCODE);
            return View(eXPIRATIONDATE);
        }
        [ActionName("SearchExpiredate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int PROVIDERNAME,int LOTNUMBER)
        {

            string convert = PROVIDERNAME.ToString();
            return RedirectToAction("edit", new { id = LOTNUMBER, PROVIDERNAME = convert });
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

        public void CreateExpirationDateId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCEEXPIRATIONDATE";
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

        public long ExpirationDateIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCEEXPIRATIONDATE";
            cmd.Parameters.Add("@IDEXPIRATIONDATE", OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            long ExpirationDateId = Convert.ToInt64(cmd.Parameters["@IDEXPIRATIONDATE"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return ExpirationDateId;
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
