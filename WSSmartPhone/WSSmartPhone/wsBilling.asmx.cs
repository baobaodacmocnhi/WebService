using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsBilling
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsBilling : System.Web.Services.WebService
    {
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public bool insertBilling(string DocSoID, string checksum)
        {
            return _cThuTien.insertBilling(DocSoID, checksum);
        }

    }
}
