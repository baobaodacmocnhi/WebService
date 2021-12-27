using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsDHN
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsDHN : System.Web.Services.WebService
    {
        CDHN _cDHN = new CDHN();

        [WebMethod]
        public bool insertBilling(string DocSoID, string checksum)
        {
            return _cDHN.insertBilling(DocSoID, checksum);
        }
    }
}
