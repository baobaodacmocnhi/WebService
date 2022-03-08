using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSTanHoa.Controllers
{
    public class ThuTienController : Controller
    {
        // GET: ThuTien
        public ActionResult Taiapp()
        {
            return Redirect("http://113.161.88.180:81/app/thutien.apk");
        }
    }
}