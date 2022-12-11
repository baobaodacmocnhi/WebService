using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSTanHoa.Controllers
{
    public class QuaySoController : Controller
    {
        // GET: QuaySo
        public ActionResult Index()
        {
            ViewBag.num1 = "3";
            ViewBag.num2 = "1 ";
            ViewBag.num3 = "2";
            ViewBag.donvi = "công ty cpcn tân hòa";
            ViewBag.hoten = "nguyễn văn a";
            return View();
        }
    }
}