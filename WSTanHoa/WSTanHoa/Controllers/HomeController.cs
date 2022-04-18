using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class HomeController : Controller
    {
        private CConnection cDAL_KinhDoanh = new CConnection(CGlobalVariable.KinhDoanh);
        private CConnection cDAL_BauCu = new CConnection(CGlobalVariable.BauCu);

        public ActionResult Index()
        {
            ViewBag.Title = "Tân Hòa Service";

            return View();
        }

        public ActionResult Login(string action, string username, string password)
        {
            if (action == "login")
            {
                //DataTable dt = cDAL_KinhDoanh.ExecuteQuery_DataTable("select MaU,HoTen from users where an=0 and taikhoan='" + username + "' and matkhau='" + password + "'");
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    Session["ID"] = dt.Rows[0]["MaU"].ToString();
                //    Session["HoTen"] = dt.Rows[0]["HoTen"].ToString();
                //    return Redirect(Session["Url"].ToString());
                //}
                DataTable dt = cDAL_BauCu.ExecuteQuery_DataTable("select ID,[Name] from Users where [Name]='" + username + "' and Password='" + password + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    Session["ID"] = dt.Rows[0]["ID"].ToString();
                    Session["HoTen"] = dt.Rows[0]["Name"].ToString();
                    return Redirect(Session["Url"].ToString());
                }
            }
            return View();
        }


    }
}
