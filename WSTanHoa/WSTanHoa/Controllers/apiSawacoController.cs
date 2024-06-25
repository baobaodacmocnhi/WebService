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
    [RoutePrefix("api/SAWACO")]
    public class apiSawacoController : ApiController
    {
        private string urlApi = "https://sawaco-uat.tcrm.vn";
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
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, urlApi + "/apiv1/auth");
                    request.Headers.Add("username", "api_cntanhoa");
                    request.Headers.Add("password", "Tkdonvi@1234");
                    request.Headers.Add("Authorization", "Basic YXBpX2NudGFuaG9hOlRrZG9udmlAMTIzNA==");
                    var response = client.SendAsync(request);
                    var result = response.Result.Content.ReadAsStringAsync();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result.Result.ToString());
                    if (response.Result.IsSuccessStatusCode)
                    {
                        bool a = _cDAL_TrungTam.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',expiration_date='" + obj["timeStamp"] + "',CreateDate=getdate() where ID='sawacocskh'");
                        strResponse = a.ToString();
                    }
                    else
                        strResponse = "Lỗi api - " + obj["errorCode"] + " - " + obj["resultMsg"][0];
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
            return _cDAL_TrungTam.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='sawacocskh'").ToString();
        }
    }
}
