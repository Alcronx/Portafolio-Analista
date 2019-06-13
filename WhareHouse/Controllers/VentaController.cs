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
        private long ticketid;
        public VentaController()
        {
            listPro = cache["productList"] as List<PRODUCT>;
            listPro2 = cache["productList2"] as List<PRODUCT>;
            listTicketDetails = cache["listTicketDetails"] as List<TICKETDETAILS>;
            
            PRODUCT pro = db.PRODUCT.Find(1);
           
            if (pro == null)
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
            if (ticketid==0)
            {
                ViewBag.idTicket = 1;
            }
            else
            {
                ViewBag.idTicket = ticketid;
            }
            ViewBag.listPro= listPro;
            ViewBag.listPro2= listPro2;
            ViewBag.td = listTicketDetails;
            return View();
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
            listTicketDetails.Add(TD);
            SaveCacheTD();
            //////////////////////////////////////////////////////////////////////////////

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
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
        }
      
        public long GetTicketId()
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
    }

    
}