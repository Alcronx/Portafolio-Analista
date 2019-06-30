using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WhareHouse.Models;
using System.Runtime.Caching;
using Oracle.DataAccess.Client;

namespace WhareHouse.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();
        private List<PRODUCT> listPro;
        private List<PRODUCT> listPro2;
        private ObjectCache cache = MemoryCache.Default;
        private List<TICKETDETAILS> listTicketDetails;
        public VentaController()
        {
            listPro = cache["productList"] as List<PRODUCT>;
            listPro2 = cache["productList2"] as List<PRODUCT>;
            listTicketDetails = cache["listTicketDetails"] as List<TICKETDETAILS>;
            
            TICKET tick = db.TICKET.Find(1);
            TRUSTED tru = db.TRUSTED.Find(1);
            if (tru == null)
            {
                CreateTrustedId();
            }
            if (tick == null)
            {
                CreateTicketId();
            }
            if (listTicketDetails == null)
            {
                listTicketDetails = new List<TICKETDETAILS>();
            }
            if (listPro2 == null)
            {
                listPro2 = new List<PRODUCT>();
            }
            if (listPro == null)
            {
                listPro = new List<PRODUCT>();
            }
        }
        public ActionResult Create(int Error = 0)
        {
            TICKET tick = db.TICKET.Find(1);
            if (tick!=null){
                long idTicket = db.TICKET.Max(x => x.IDTICKET);
                ViewBag.idTicket = (idTicket + 1);
            }
            else
            {
                ViewBag.idTicket = 1;
            }
            ViewBag.listPro= listPro;
            ViewBag.listPro2= listPro2;
            ViewBag.td = listTicketDetails;
            ViewBag.Error = Error;
            ViewBag.total = listTicketDetails.Sum(x => x.TOTAL);
            var ViegbagClient = db.CLIENT.Select(s => new 
                 { 
                   IdClientes = s.IDCLIENT,
                   NombreCompleto = s.NAME1.ToString() +" "+ s.LASTNAME1.ToString()
                 })
                 .ToList();

            ViewBag.cliente = new SelectList(ViegbagClient, "IdClientes", "NombreCompleto");
            var pRODUCT = db.PRODUCT.Include(p => p.PROVIDER);
            return View(pRODUCT.ToList());
        }
        [HttpPost]
        public ActionResult Create(string cliente, string PaidForm, int total,int Error = 0)
        {
            ViewBag.Error = Error;
            int paid = Convert.ToInt16(PaidForm);
            long TicketId = TicketIdAumentate();
            if (cliente.Equals(""))
            {
                TICKET ti = new TICKET
                {
                    IDTICKET = TicketId,
                    TICKETDATE = System.DateTime.Now,
                    TICKETHOUR = DateTime.Now,
                    STATE = "1",
                    TOTALTOTAL = total,
                    IDCLIENT = null,
                    IDTRUSTED = null
                };
                db.TICKET.Add(ti);
                db.TICKETDETAILS.AddRange(listTicketDetails);
                db.SaveChanges();
                
            }
            else
            {
                if (paid == 1)
                {
                    TICKET ti = new TICKET
                    {
                        IDTICKET = TicketId,
                        TICKETDATE = System.DateTime.Now,
                        TICKETHOUR = DateTime.Now,
                        STATE = "1",
                        TOTALTOTAL = total,
                        IDCLIENT = Convert.ToInt16(cliente),
                        IDTRUSTED = null
                    };
                    db.TICKET.Add(ti);
                    db.TICKETDETAILS.AddRange(listTicketDetails);
                    db.SaveChanges();
                }
                else
                {
                    long id = TrustedIdAumentate();
                    TRUSTED tru = new TRUSTED
                    {
                        IDTRUSTED = id,
                        STATE = "1",
                        TRUSTDATE = DateTime.Now,
                        TIMELIMITTRUST = DateTime.Now.AddMonths(1),
                        STATETRUSTED = "1"
                    };
                    db.TRUSTED.Add(tru);
                    TICKET ti = new TICKET
                    {
                        IDTICKET = TicketId,
                        TICKETDATE = System.DateTime.Now,
                        TICKETHOUR = DateTime.Now,
                        STATE = "1",
                        TOTALTOTAL = total,
                        IDCLIENT = Convert.ToInt16(cliente),
                        IDTRUSTED = id
                    };
                    db.TICKET.Add(ti);
                    db.TICKETDETAILS.AddRange(listTicketDetails);
                    db.SaveChanges();
                } 
            }

            var ProductUpdate = (from model in listTicketDetails select new { model.QUANTITY,model.TDIDBARCODE }).ToList();

            for (int i = 0; i < ProductUpdate.Count(); i++)
            {
                short quantity = Convert.ToInt16(ProductUpdate[i].QUANTITY);
                short IdProduct = ProductUpdate[i].TDIDBARCODE;
                PRODUCT pRODUCT = db.PRODUCT.Find(IdProduct);
                Int16 finalstock = Convert.ToInt16(pRODUCT.STOCK - quantity);
                pRODUCT.STOCK = finalstock;
                db.SaveChanges();

            }

                CamcelTicket();
            return RedirectToAction("Index", "Home");
        }
        //Detalle De Ticket
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TICKET tICKET = db.TICKET.Find(id);
            if (tICKET == null)
            {
                return HttpNotFound();
            }
            var TicketDetail = (from model in db.TICKETDETAILS where (model.TDIDTICKET == id) select model).ToList();
            ViewBag.TicketDetails = TicketDetail;
            return View(tICKET);
        }

        public ActionResult TicketDetails(Int64? id)
        {
            
            if (id == null) {
                return RedirectToAction("Create", new { Error = 1 });
            }

            var any = db.PRODUCT.Any(x => x.BARCODE == id);
            if (any == false)
            {
                return RedirectToAction("Create", new { Error = 2 });
            }

            listPro = new List<PRODUCT>();
            long barcodeP = Convert.ToInt64(id);

            var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                  where model.BARCODE == barcodeP
                                  select new PRODUCT()
                                  {
                                      IDBARCODE = model.IDBARCODE,
                                      BARCODE = model.BARCODE,
                                      PURCHASEPRICE = model.PURCHASEPRICE,
                                      SALEPRICE = model.SALEPRICE,
                                      STOCK = model.STOCK,
                                      CRITICALSTOCK = model.CRITICALSTOCK,
                                      PRODUCTNAME = model.PRODUCTNAME,
                                      PRODUCTFAMILY = model.PRODUCTFAMILY,
                                      PRODUCTTYPE = model.PRODUCTTYPE,
                                      PRODUCTDESCRIPTION = model.PRODUCTDESCRIPTION,
                                      STATE = model.STATE,
                                      IDPROVIDER = model.IDPROVIDER
                                  }).ToList();
            listPro.Add(ProductViegbag.First());
            SaveCachePro();
            return RedirectToAction("Create");
           
        }
        public ActionResult ViegbagTicketDetails(int quantity,Int16 IdBarcode, int SalePrice,int IdTicket)
        {
            
                short quantit = Convert.ToInt16(quantity);
                PRODUCT pRODUCT = db.PRODUCT.Find(IdBarcode);
                if ((pRODUCT.STOCK - quantit) <= 0)
                {
                    return RedirectToAction("Create", new { Error = 3 });
                }
            
            TICKETDETAILS TD = new TICKETDETAILS
            {
                TDIDTICKET = IdTicket,
                TDIDBARCODE = IdBarcode,
                QUANTITY = quantity,
                TOTAL = quantity * SalePrice
            };
            if(listTicketDetails.Find(x => x.TDIDBARCODE == IdBarcode)== null)
            {
                listTicketDetails.Add(TD);
                var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                      where model.IDBARCODE == IdBarcode
                                      select new PRODUCT()
                                      {
                                          IDBARCODE = model.IDBARCODE,
                                          BARCODE = model.BARCODE,
                                          PURCHASEPRICE = model.PURCHASEPRICE,
                                          SALEPRICE = model.SALEPRICE,
                                          STOCK = model.STOCK,
                                          CRITICALSTOCK = model.CRITICALSTOCK,
                                          PRODUCTNAME = model.PRODUCTNAME,
                                          PRODUCTFAMILY = model.PRODUCTFAMILY,
                                          PRODUCTTYPE = model.PRODUCTTYPE,
                                          PRODUCTDESCRIPTION = model.PRODUCTDESCRIPTION,
                                          STATE = model.STATE,
                                          IDPROVIDER = model.IDPROVIDER
                                      }).ToList();

                listPro2.Add(ProductViegbag.First());
            }
            else
            {
                var list = listTicketDetails.FirstOrDefault(x => x.TDIDBARCODE == IdBarcode);
                list.QUANTITY = list.QUANTITY + quantity;
                long total = list.QUANTITY;
                list.TOTAL = SalePrice * total;
            };

            SaveCacheTD();
            SaveCachePro2();

            return RedirectToAction("Create");
        }

        public void SaveCacheTD()
        {
            cache["listTicketDetails"] = listTicketDetails;
        }
        public void SaveCachePro2()
        {
            cache["productList2"] = listPro2;
        }

        public void SaveCachePro()
        {
            cache["productList"] = listPro;
        }

        public void CreateTicketId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCETICKET";
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
      
        public long TicketIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCETICKET";
            cmd.Parameters.Add("@IDTICKET",OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            long TicketId = Convert.ToInt64(cmd.Parameters["@IDTICKET"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return TicketId;
        }

        public ActionResult CamcelTicket()
        {
            listPro = new List<PRODUCT>();
            listPro2 = new List<PRODUCT>();
            listTicketDetails = new List<TICKETDETAILS>();
            SaveCachePro();
            SaveCachePro2();
            SaveCacheTD();
            return RedirectToAction("Create");
        }

        public ActionResult Index()
        {

            return View(db.TICKET.ToList());
        }

        public ActionResult CancelTicket(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TICKET tIcket = db.TICKET.Find(id);
            if (tIcket == null)
            {
                return HttpNotFound();
            }
            tIcket.STATE = "0";
            db.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult DontCancelTicket(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TICKET tIcket = db.TICKET.Find(id);
            if (tIcket == null)
            {
                return HttpNotFound();
            }
            tIcket.STATE = "1";
            db.SaveChanges();
            return RedirectToAction("index");
        }


        public void CreateTrustedId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCETRUSTED";
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

        public long TrustedIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCETRUSTED";
            cmd.Parameters.Add("@IDTRUSTED", OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            long TrustedId = Convert.ToInt16(cmd.Parameters["@IDTRUSTED"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return TrustedId;
        }


        public JsonResult ClientCascadeJson(string cliente)
        {
            if(cliente.Equals("")){
                var list = new List<SelectListItem>
                                            {
                                               new SelectListItem{ Text="Efectivo", Value = "1" }
                                            };
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            short client = Convert.ToInt16(cliente);
            var canTrust = db.CLIENT.Where(x => x.IDCLIENT == client).First();
            string result = canTrust.BLACKLIST;
            if (result.Equals("1"))
            {
                var list1 = new List<SelectListItem>
                                            {
                                               new SelectListItem{ Text="Efectivo", Value = "1" },
                                               new SelectListItem{ Text="Fiado", Value = "2" }
                                            };
                
                return Json(list1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list2 = new List<SelectListItem>
                                            {
                                               new SelectListItem{ Text="Efectivo", Value = "1" }
                                            };
                return Json(list2, JsonRequestBehavior.AllowGet);
            }
        }
                                            
    
    }

    
}