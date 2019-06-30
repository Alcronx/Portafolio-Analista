using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Oracle.DataAccess.Client;
using WhareHouse.Models;
namespace WhareHouse.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ObjectCache cache = MemoryCache.Default;
        private WhareHouseWebcn db = new WhareHouseWebcn();
        private List<PRODUCT> pro;
        private List<PRODUCT> pro2;
        private List<ORDERDETAILS> OrderDetails2;
        private List<ORDERDETAILS> OrderDetails;

        // GET: Order
        public OrderController()
        {
            pro = cache["pro"] as List<PRODUCT>;
            pro2 = cache["pro2"] as List<PRODUCT>;
            OrderDetails2 = cache["OrderDetails2"] as List<ORDERDETAILS>;
            OrderDetails = cache["OrderDetails"] as List<ORDERDETAILS>;

            if (pro == null)
            {
                pro = new List<PRODUCT>();
            }
            if (pro2 == null){
                pro2 = new List<PRODUCT>();
            }
            if (OrderDetails2 == null)
            {
                OrderDetails2 = new List<ORDERDETAILS>();
            }
            if (OrderDetails == null)
            {
                OrderDetails = new List<ORDERDETAILS>();
            }

            ORDERPRODUCT orP = db.ORDERPRODUCT.Find(1);
            if (orP == null)
            {
                CreateOrderProductId();
            }
        }
        public ActionResult CreateOrder(int error = 0)
        {
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            ViewBag.pro = pro;
            ViewBag.pro2 = pro2;
            ViewBag.orderDetails = OrderDetails;
            ViewBag.total = OrderDetails.Sum(x => x.TOTAL);
            ViewBag.error = error;

            if (db.ORDERPRODUCT.Find(1) == null)
            {
                ViewBag.idOrderProduct = 1;
            }
            else
            {
                long idOrderProduct = db.ORDERPRODUCT.Max(x => x.ORDERID);
                ViewBag.idOrderProduct = idOrderProduct + 1;
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateOrder(long total)
        {
            
            db.ORDERDETAILS.AddRange(OrderDetails);
            ORDERPRODUCT OR = new ORDERPRODUCT
            {
                ORDERID = OrderProductIdAumentate(),
                ORDERDATE = System.DateTime.Now,
                ORDERHOUR = DateTime.Now,
                STATE = "0",
                TOTALTOTAL = total,
                RECEPTIONDATE = null,
                RECEPTIONHOUR = null
            };
            ViewBag.error = 0;
            db.ORDERPRODUCT.Add(OR);
            db.SaveChanges();
            AllListNull();
            return RedirectToAction("index", "Home");
        }

        public ActionResult Index()
        {
            AllListNull();
            ViewBag.OrderProduct = db.ORDERPRODUCT.ToList();
            ViewBag.OrderDetails = db.ORDERDETAILS.ToList();
            ViewBag.Product = db.PRODUCT.ToList();
            ViewBag.Provider = db.PROVIDER.ToList();
            return View();
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDERPRODUCT oRDERPRODUCT = db.ORDERPRODUCT.Find(id);
            if (oRDERPRODUCT == null)
            {
                return HttpNotFound();
            }
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            ViewBag.pro = pro;
            ViewBag.pro2 = pro2;
            ViewBag.orderid = id;
            if(OrderDetails.Count()==0){
                var order = (from model in db.ORDERDETAILS where (model.ODORDERID == id) select model).ToList();
                ViewBag.OrderDetails = order;
                ViewBag.total = order.Sum(x => x.TOTAL);
            }
            else
            {

                ViewBag.total = OrderDetails.Sum(x => x.TOTAL);
                ViewBag.orderDetails = OrderDetails;
            }
            
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long total,long OrderID)
        {
            if (ModelState.IsValid)
            {
                var orderFor = (from model in db.ORDERDETAILS where model.ODORDERID == OrderID select model).ToList();
                for (int i = 0; i < orderFor.Count(); i++)
                {
                    var OrderDetail = (from model in OrderDetails where model.ODIDBARCODE == orderFor[i].ODIDBARCODE select new { model.QUANTITY }).First();

                        short idbarcode = orderFor[i].ODIDBARCODE;
                        var list = db.ORDERDETAILS.FirstOrDefault(x => x.ODIDBARCODE == idbarcode);
                        list.QUANTITY = OrderDetail.QUANTITY;
                        long totall = list.QUANTITY;
                        list.TOTAL = orderFor[i].PRODUCT.PURCHASEPRICE * totall;
                    db.SaveChanges();
                }
                db.ORDERDETAILS.AddRange(OrderDetails2);
                OrderDetails2 = new List<ORDERDETAILS>();
                SaveCacheOrderDetails2();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            ViewBag.pro = pro;
            ViewBag.pro2 = pro2;
            ViewBag.orderid = OrderID;
            if (OrderDetails.Count() == 0)
            {
                var order = (from model in db.ORDERDETAILS where (model.ODORDERID == OrderID) select model).ToList();
                ViewBag.OrderDetails = order;
                ViewBag.total = order.Sum(x => x.TOTAL);
            }
            else
            {

                ViewBag.total = OrderDetails.Sum(x => x.TOTAL);
                ViewBag.orderDetails = OrderDetails;
            }
            return View();
        }
        //get
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ORDERPRODUCT oRDERPRODUCT = db.ORDERPRODUCT.Find(id);
            if (oRDERPRODUCT == null)
            {
                return HttpNotFound();
            }
            var orderdetails = (from model in db.ORDERDETAILS where (model.ODORDERID == id) select model).ToList();
            ViewBag.OrderDetails = orderdetails;
            return View(oRDERPRODUCT);
        }
        public ActionResult Reception(int idOrder,string CompannyName)
        {
            ViewBag.CompannyName = CompannyName;
            ViewBag.idorder = idOrder;
            var Listorder = db.ORDERDETAILS.Where(x => x.ODORDERID == idOrder);

            ViewBag.orderlist = Listorder.ToList();
            return View();
        }

        public ActionResult ConfirmReception(long idReception)
        {
            var ProductUpdate = (from model in db.ORDERDETAILS where model.ODORDERID==idReception select new { model.QUANTITY, model.ODIDBARCODE }).ToList();

            for (int i = 0; i < ProductUpdate.Count(); i++)
            {
                short quantity = Convert.ToInt16(ProductUpdate[i].QUANTITY);
                short IdProduct = ProductUpdate[i].ODIDBARCODE;
                PRODUCT pRODUCT = db.PRODUCT.Find(IdProduct);
                Int16 finalstock = Convert.ToInt16(pRODUCT.STOCK + quantity);
                pRODUCT.STOCK = finalstock;
                db.SaveChanges();
            }

            var list = db.ORDERPRODUCT.FirstOrDefault(x => x.ORDERID == idReception);
            list.RECEPTIONDATE = System.DateTime.Now;
            list.RECEPTIONHOUR = System.DateTime.Now;
            list.STATE = "1";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateSearch(string PROVIDERNAME,string buttonValue,long? orderid)
        {
            if (PROVIDERNAME != null) {
                pro = new List<PRODUCT>();
                long id = Convert.ToInt16(PROVIDERNAME);

                var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                      where model.IDPROVIDER == id
                                      select model).ToList();
                pro.AddRange(ProductViegbag);
                SaveCachePro();
                if (buttonValue.Equals("Create"))
                {
                    return RedirectToAction("CreateOrder");
                }
                else
                {
                    return RedirectToAction("Edit", new { id = orderid });
                }
            }
            if (buttonValue.Equals("Create"))
            {
                int errorr = 1;
                return RedirectToAction("CreateOrder", new { error = errorr });
            }
            else
            {
                return RedirectToAction("Edit", new { id = orderid });
            }
            

        }

        public ActionResult ListOrderDetails(string IDBARCODE, int quantity, long OrderId)
        {
            short IdProduct = Convert.ToInt16(IDBARCODE);
            int quantit = (quantity);
            PRODUCT pRODUCT = db.PRODUCT.Find(IdProduct);
            if ((pRODUCT.STOCK + quantit)>99999)
            {
                return RedirectToAction("CreateOrder", new { Error = 3 });
            }


           
            var purchasePrice = (from model in db.PRODUCT where model.IDBARCODE == IdProduct select new { model.PURCHASEPRICE }).First();
            long Purchase = Convert.ToInt64(purchasePrice.PURCHASEPRICE);
            
            ORDERDETAILS or = new ORDERDETAILS
            {
                ODIDBARCODE = IdProduct,
                ODORDERID = OrderId,
                QUANTITY = quantity,
                TOTAL = quantity * Purchase
            };
            if (OrderDetails.Find(x => x.ODIDBARCODE == IdProduct) == null)
            {
                OrderDetails.Add(or);
                var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                      where model.IDBARCODE == IdProduct
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
                
                pro2.Add(ProductViegbag.First());
                
            }
            else
            {
                var list = OrderDetails.FirstOrDefault(x => x.ODIDBARCODE == IdProduct);
                list.QUANTITY = list.QUANTITY + quantity;
                long total = list.QUANTITY;
                list.TOTAL = Purchase * total;
            };

            SaveCacheOrderDetails();
            SaveCachePro2();
            
            return RedirectToAction("CreateOrder");
        }
        public ActionResult EditListOrderDetails(long IdOrder, string IDBARCODE, int quantity)
        {

            if(OrderDetails.Count == 0)
            {
                var addOrderDetail = (from model in db.ORDERDETAILS where model.ODORDERID == IdOrder select model).ToList();
                OrderDetails.AddRange(addOrderDetail);
                SaveCacheOrderDetails();
            }
            if (pro2.Count == 0)
            {
                var orderFor = (from model in db.ORDERDETAILS where model.ODORDERID == IdOrder select model).ToList();
                for (int i = 0; i < orderFor.Count(); i++)
                {
                    var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                          where model.IDBARCODE == orderFor[i].ODIDBARCODE
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
                    pro2.Add(ProductViegbag.First());
                }
                SaveCachePro2();
            }
            short IdProduct = Convert.ToInt16(IDBARCODE);
            var purchasePrice = (from model in db.PRODUCT where model.IDBARCODE == IdProduct select new { model.PURCHASEPRICE }).First();
            long Purchase = Convert.ToInt64(purchasePrice.PURCHASEPRICE);

            if (OrderDetails.Find(x => x.ODIDBARCODE == IdProduct) == null)
            {
                ORDERDETAILS or = new ORDERDETAILS
                {
                    ODIDBARCODE = IdProduct,
                    ODORDERID = IdOrder,
                    QUANTITY = quantity,
                    TOTAL = quantity * Purchase
                };
                OrderDetails.Add(or);
                OrderDetails2.Add(or);
                var ProductViegbag = (from model in db.PRODUCT.AsEnumerable()
                                      where model.IDBARCODE == IdProduct
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

                pro2.Add(ProductViegbag.First());
            }
            else
            {
                var list = OrderDetails.FirstOrDefault(x => x.ODIDBARCODE == IdProduct);
                list.QUANTITY = list.QUANTITY + quantity;
                long total = list.QUANTITY;
                list.TOTAL = Purchase * total;
            };

            SaveCacheOrderDetails();
            SaveCachePro2();
            SaveCacheOrderDetails2();
            return RedirectToAction("Edit", new { id = IdOrder });
        }

        public void SaveCachePro()
        {
            cache["pro"] = pro;
        }
        public void SaveCachePro2()
        {
            cache["pro2"] = pro2;
        }
        public void SaveCacheOrderDetails2()
        {
            cache["OrderDetails2"] = OrderDetails2;
        }
        public void SaveCacheOrderDetails()
        {
            cache["OrderDetails"] = OrderDetails;
        }

        public void CreateOrderProductId()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CREATESEQUENCEORDERPRODUCT";
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

        public long OrderProductIdAumentate()
        {
            Connection objectcon = new Connection();
            OracleConnection con = objectcon.GetConection();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "SELECTSEQUENCEORDERPRODUCT";
            cmd.Parameters.Add("@IDORDERPRODUCT", OracleDbType.Int64).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            long OrderProductId = Convert.ToInt16(cmd.Parameters["@IDORDERPRODUCT"].Value.ToString());
            con.Close();
            cmd.Dispose();
            con.Dispose();
            objectcon = null;
            return OrderProductId;
        }

        public void AllListNull()
        {
            pro = new List<PRODUCT>();
            pro2 = new List<PRODUCT>();
            OrderDetails = new List<ORDERDETAILS>();
            SaveCachePro();
            SaveCachePro2();
            SaveCacheOrderDetails();
        }

        public ActionResult CamcelOrder()
        {
            pro = new List<PRODUCT>();
            pro2 = new List<PRODUCT>();
            OrderDetails = new List<ORDERDETAILS>();
            OrderDetails2 = new List<ORDERDETAILS>();
            SaveCachePro();
            SaveCachePro2();
            SaveCacheOrderDetails();
            SaveCacheOrderDetails2();
            return RedirectToAction("CreateOrder");
        }
    }
}