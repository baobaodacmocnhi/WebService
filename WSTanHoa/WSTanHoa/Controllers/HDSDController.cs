using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSTanHoa.Controllers
{
    public class HDSDController : Controller
    {
        // GET: HDSD
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getPDF(string fileName)
        {
            string filePath = "~/Views/HDSD/" + fileName;
            return File(filePath, "application/pdf");
        }

        public ActionResult BaoCao()
        {
            return View();
        }

    }
}