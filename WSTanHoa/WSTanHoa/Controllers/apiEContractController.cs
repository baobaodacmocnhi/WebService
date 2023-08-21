using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/EContract")]
    public class apiEContractController : ApiController
    {
        private CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);
        private string urlApi = "https://api-econtract.cskhtanhoa.com.vn:1443/";

        private string getAccess_token()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='econtract'").ToString();
        }

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
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "auth-service/oauth/token");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    var data = new
                    {
                        username = "admintanhoa",
                        password = "Abc@12345",
                        grant_type = "client_credentials",
                        client_id = "tanhoa.client@econtract.vnpt.vn",
                        client_secret = "C29XWd2bDhsz9jB9h8lq5WOPmw3kS2O0"
                    };
                    var json = CGlobalVariable.jsSerializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        bool flag = _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',CreateDate=getdate() where ID='econtract'");
                        strResponse = flag.ToString();
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
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
    }
}
