using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsEContract
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsEContract : System.Web.Services.WebService
    {
        CEContract _cEContract = new CEContract();

        [WebMethod]
        public string getAccess_token(string checksum)
        {
            return _cEContract.getAccess_token(checksum);
        }

        //[WebMethod]
        //public string renderEContract(string checksum)
        //{
        //    return _cEContract.renderEContract(checksum);
        //}

        [WebMethod]
        public string createEContract(string checksum)
        {
            return _cEContract.createEContract(checksum);
        }

    }
}
