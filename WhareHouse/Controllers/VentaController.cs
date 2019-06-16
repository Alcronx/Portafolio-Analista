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
        }
        public ActionResult Create()
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
            ViewBag.total = listTicketDetails.Sum(x => x.TOTAL);
            var ViegbagClient = db.CLIENT.Select(s => new 
                 { 
                   IdClientes = s.IDCLIENT,
                   NombreCompleto = s.NAME1.ToString() +" "+ s.LASTNAME1.ToString()
                 })
                 .ToList();

            ViewBag.cliente = new SelectList(ViegbagClient, "IdClientes", "NombreCompleto");
            return View();
        }
        [HttpPost]
        public ActionResult Create(string cliente, string PaidForm, int total,long TicketId)
        {
            int paid = Convert.ToInt16(PaidForm);
            if (cliente.Equals(""))
            {
                TICKET ti = new TICKET
                {
                    IDTICKET = TicketId,
                    TICKETDATE = System.DateTime.Now,
                    TICKETHOUR = DateTime.Now,
                    STATE = "V",
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
                        STATE = "V",
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
                    TRUSTED tru = new TRUSTED
                    {
                        IDTRUSTED = TicketId,
                        STATE = "d",
                        TRUSTDATE = DateTime.Now,
                        TIMELIMITTRUST = DateTime.Now,
                        STATETRUSTED = "1"
                    };
                    db.TRUSTED.Add(tru);
                    TICKET ti = new TICKET
                    {
                        IDTICKET = TicketId,
                        TICKETDATE = System.DateTime.Now,
                        TICKETHOUR = DateTime.Now,
                        STATE = "V",
                        TOTALTOTAL = total,
                        IDCLIENT = Convert.ToInt16(cliente),
                        IDTRUSTED = TicketId
                    };
                    db.TICKET.Add(ti);
                    db.TICKETDETAILS.AddRange(listTicketDetails);
                    db.SaveChanges();
                } 
            }
            TicketIdAumentate();
            CamcelTicket();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ticketDetails(string id)
        {
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
    }

    
}