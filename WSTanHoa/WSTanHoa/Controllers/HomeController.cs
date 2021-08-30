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
        public ActionResult Index()
        {
            ViewBag.Title = "Tân Hòa Service";

            return View();
        }

        public ActionResult Login(string action, string username, string password)
        {
            if (action == "login")
            {
                DataTable dt = cDAL_KinhDoanh.ExecuteQuery_DataTable("select MaU,HoTen from users where an=0 and taikhoan='" + username + "' and matkhau='" + password + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    Session["ID"] = dt.Rows[0]["MaU"].ToString();
                    Session["HoTen"] = dt.Rows[0]["HoTen"].ToString();
                    return Redirect(Session["Url"].ToString());
                }
            }
            return View();
        }


    }
}
