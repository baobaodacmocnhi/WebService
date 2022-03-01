using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSTanHoa.Controllers
{
    public class QLDHNController : Controller
    {
        // GET: QLDHN
        public ActionResult BaoChiSoNuoc()
        {
            return View();
        }

        public ActionResult TaiApp()
        {
            return Redirect("http://113.161.88.180:81/app/docso.apk");
        }
    }
}