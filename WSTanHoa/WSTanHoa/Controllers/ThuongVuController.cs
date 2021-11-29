using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class ThuongVuController : Controller
    {
        private CConnection cDAL_KinhDoanh = new CConnection(CGlobalVariable.KinhDoanh);

        // GET: ThuongVu
        public ActionResult viewFile(string TableName, string IDFileName, string IDFileContent)
        {
            if (TableName != null && IDFileName != null && IDFileContent != null && TableName != "" && IDFileName != "" && IDFileContent != "")
            {
                byte[] FileContent = getFile(TableName, IDFileName, IDFileContent);
                if (FileContent != null)
                    return new FileStreamResult(new MemoryStream(FileContent), "image/jpeg");
                else
                    return View();
            }
            else
                return null;
        }

        private byte[] getFile(string TableName, string IDFileName, string IDFileContent)
        {
            int count = (int)cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select count(*) from " + TableName + " where " + IDFileName + "=" + IDFileContent);
            if (count > 0)
                return (byte[])cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select Hinh from " + TableName + " where " + IDFileName + "=" + IDFileContent);
            else
                return null;
        }

    }
}