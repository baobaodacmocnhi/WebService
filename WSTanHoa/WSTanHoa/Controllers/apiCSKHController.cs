using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    //[Authorize(Roles = "SAWACO")]
    [RoutePrefix("api/CSKH")]
    public class apiCSKHController : ApiController
    {
        private CConnection _cDAL_TrungTam = new CConnection(CGlobalVariable.TrungTamKhachHang);

        [Route("getAccess_token")]
        [HttpGet]
        public string getAccess_token(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string data = "app_id=3904851815439759378&grant_type=refresh_token&refresh_token=" + getRefresh_token();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        client.Headers["secret_key"] = "cCBBIsEx7UDj42KA1N5Y";
                        string result = client.UploadString("https://oauth.zaloapp.com/v4/oa/access_token", data);
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        bool a = _cDAL_TrungTam.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',refresh_token='" + obj["refresh_token"] + "',expires_in='" + obj["expires_in"] + "',CreateDate=getdate() where ID='zalo'");
                        strResponse = a.ToString();
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        private string getAccess_token()
        {
            return _cDAL_TrungTam.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='zalo'").ToString();
        }
    }
}
