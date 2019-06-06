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
    public class VentaController : Controller
    {
        List<PRODUCT> productList = new List<PRODUCT>();
        private WhareHouseWebcn db = new WhareHouseWebcn();
        // GET: Venta
        public ActionResult Create(long idbarcode=0)
        {
            
            if (idbarcode == 0)
            {
                ViewBag.productListt = productList;
                return View();
            }
            else
            {
                var SelectTickedDetails = (from model in db.PRODUCT.AsEnumerable()
                                          where model.BARCODE == idbarcode
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
                productList.Add(SelectTickedDetails.First());
                ViewBag.productListt = productList;
                return View();
            }
 
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {


            return View();
        }

        public ActionResult ViegbagTickerDetails(string id)
        {
            long idInt = Convert.ToInt64(id); 
            return RedirectToAction("Create", new { idbarcode = idInt });
        }
    }
}