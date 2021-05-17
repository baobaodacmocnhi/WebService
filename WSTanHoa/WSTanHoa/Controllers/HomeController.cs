using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class HomeController : Controller
    {
        CConnection _cDAL_KinhDoanh = new CConnection(CConstantVariable.KinhDoanh);
        public ActionResult Index()
        {
            ViewBag.Title = "Tân Hòa Service";

            return View();
        }

        public ActionResult viewFile(string TableName, string IDFileContent)
        {
            if (TableName != "" && IDFileContent != "")
            {
                byte[] FileContent = getFile("KTXM_ChiTiet_Hinh", "1");
                if (FileContent != null)
                    return new FileStreamResult(new MemoryStream(FileContent), "image/jpeg");
                else
                    return null;
            }
            else
                return null;
        }

        public byte[] getFile(string TableName, string IDFileContent)
        {
            int count = (int)_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select count(*) from " + TableName + " where ID=" + IDFileContent);
            if (count > 0)
                return (byte[])_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select Hinh from " + TableName + " where ID=" + IDFileContent);
            else
                return null;
        }
    }
}
