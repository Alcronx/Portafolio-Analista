using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhareHouse.Models;
using System.Web.Security;

namespace WhareHouse.Controllers
{
    public class AccountController : Controller
    {
        
        private WhareHouseWebcn db = new WhareHouseWebcn();
        private HttpCookie UserIDCookie = new HttpCookie("IsAdmin");
        // GET: Account
        public ActionResult Login()
        {
                return View();
        }
        [HttpPost]
        public ActionResult Login(string UserName, string Pasword)
        {
            bool isValid = db.LOGIN.Any(x => x.USERNAME == UserName && x.PASSWORDUSER == Pasword);
            if (isValid)
            {
                var User = (from model in db.LOGIN where model.USERNAME == UserName && model.PASSWORDUSER == Pasword select new { model.IDUSER }).FirstOrDefault();
                FormsAuthentication.SetAuthCookie(UserName, false);
                UserIDCookie.Value = User.IDUSER.ToString();
                UserIDCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Response.SetCookie(UserIDCookie);
                return RedirectToAction("Index","Home");
                
            }
            return View();
        }
        public bool IsAdmin(string cookie)
        {
            short idUSER = Convert.ToInt16(cookie);
            var isAdmin = (from model in db.LOGIN where model.IDUSER == idUSER select new { model.ROL }).First();
                string rol = isAdmin.ROL.ToUpper();
                if (rol.Equals("ADMIN"))
                {
                    return true;
                }
            
            return false;
        }
        public ActionResult Logout()
        {
            UserIDCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Response.SetCookie(UserIDCookie);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        


    }
}