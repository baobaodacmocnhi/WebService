using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSTanHoa.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        public ActionResult ThongTin()
        {
            return View();
        }
    }
}