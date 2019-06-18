using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using WhareHouse.Models;
namespace WhareHouse.Controllers
{
    public class OrderController : Controller
    {
        private WhareHouseWebcn db = new WhareHouseWebcn();
        private List<PRODUCT> pro;
        // GET: Order
        public OrderController()
        {
            
          
        }
        public ActionResult CreateOrder()
        {
            if (pro == null)
            {
                pro = new List<PRODUCT>();
                ViewBag.ListProducts = pro;
            }
            else
            {
                ViewBag.ListProducts = pro;
            }
            
            ViewBag.PROVIDERNAME = new SelectList(db.PROVIDER, "IDPROVIDER", "COMPANYNAME");
            return View();
        }

        public ActionResult CreateSearch(string PROVIDERNAME)
        {
            pro = new List<PRODUCT>();
            short idProvider = Convert.ToInt16(PROVIDERNAME);
            var listProducts = (from model in db.PRODUCT where model.IDPROVIDER == idProvider select model).ToList();
            pro = listProducts;
            return RedirectToAction("CreateOrder");
        }
    }
}