using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/Zalo")]
    public class apiZaloController : ApiController
    {
        CConnection _cDAL_ThuTien = new CConnection(CConstantVariable.ThuTien);
        CConnection _cDAL_TrungTam = new CConnection(CConstantVariable.TrungTamKhachHang);
        string access_token = "SVFT5EqUDLzm_zGKw1KzNN7rtXJb0bSiSPxaEQeWH0zgYU4ViIf52764tH-41qCROhNg4FiUKHanz_aByHvDGn2WzLpCCd9d8gxi8CWr2GeXdReSoNO9D1_LgGpsQ2uv6_IX2_Li0XDhWeGhYGa45bNzkW2RL0S_3kgaUEnAB6Xpr8judaywOs3btN-lT3euRPoTAhGlVon8miKUrtHDAZZbysVL3njiBBd_8BDJ7Z9-lA4VdKWq2W6WoHRRAI034RU1D-9UCsHbMQpPq7xk13vJ";

        /// <summary>
        /// nhận webhook từ zalo
        /// </summary>
        /// <param name="fromuid"></param>
        /// <param name="msgid"></param>
        /// <param name="event"></param>
        /// <param name="message"></param>
        /// <param name="oaid"></param>
        /// <param name="mac"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [Route("webhook")]
        [HttpGet]
        public string webhook(string fromuid, string msgid, string @event, string message, string oaid, string mac, string timestamp)
        {
            string strResponse = "success";
            try
            {
                //if (mac == getSHA256(oaid + fromuid + msgid + message + timestamp + "cCBBIsEx7UDj42KA1N5Y"))
                if (@event == "sendmsg")
                {
                    DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + fromuid + "");
                    if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                    {
                        strResponse = sendMessageDangKy(fromuid);
                    }
                    else
                    {
                        if (message == "#get12kyhoadon")
                            foreach (DataRow item in dt_DanhBo.Rows)
                            {
                                string content = item["DanhBo"].ToString() + "\n";
                                DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGet12KyHoaDon(" + item["DanhBo"].ToString() + ")");
                                if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                                    foreach (DataRow itemHD in dt_HoaDon.Rows)
                                    {
                                        content += "Kỳ " + itemHD["KyHD"].ToString() + ": Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3 Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n";
                                    }
                                strResponse = sendMessage(fromuid, content);
                            }
                        else
                        if (message == "#gethoadonton")
                            foreach (DataRow item in dt_DanhBo.Rows)
                            {
                                string content = item["DanhBo"].ToString() + "\n";
                                DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                                if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                                    foreach (DataRow itemHD in dt_HoaDon.Rows)
                                    {
                                        content += "Kỳ " + itemHD["KyHD"].ToString() + ": Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3 Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n";
                                    }
                                else
                                    content += "Hết Nợ";
                                strResponse = sendMessage(fromuid, content);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        /// <summary>
        /// Gửi tin nhắn
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Route("sendMessage")]
        [HttpGet]
        public string sendMessage(string IDZalo, string message)
        {
            string strResponse = "success";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message?access_token=" + access_token;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                var data = new
                {
                    recipient = new { user_id = IDZalo },
                    message = new { text = message }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                request.ContentLength = byteArray.Length;
                //gắn data post
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                //string data = "{"
                //            + "\"recipient\":{"
                //            + "\"user_id\":\"4276209776391262580\""
                //            + "},"
                //            + "\"message\":{"
                //            + "\"text\":\"hello, world!\""
                //            + "}"
                //            + "}";
                //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                //{
                //    streamWriter.Write(data);
                //}

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    strResponse = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                }
                else
                {
                    strResponse = "Error: " + respuesta.StatusCode;
                }
                return strResponse;
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        [Route("sendMessageDangKy")]
        [HttpGet]
        public string sendMessageDangKy(string IDZalo)
        {
            string strResponse = "success";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message?access_token=" + access_token;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                string data = "{"
                            + "\"recipient\":{"
                            + "\"user_id\":\""+IDZalo+"\""
                            + "},"
                            + "\"message\":{"
                            + "\"attachment\":{"
                            +"\"type\":\"template\","
                            +"\"payload\":{"
                            +"\"template_type\":\"list\","
                            +"\"elements\":[{"
                            + "\"title\":\"BẠN CHƯA ĐĂNG KÝ THÔNG TIN\","
                            + "\"subtitle\":\"Để sử dụng dịch vụ, bạn cần phải đăng ký ít nhất một mã khách hàng (Danh Bộ)\","
                            + "\"image_url\":\"http://www.capnuoctanhoa.com.vn/uploads/page/logoctycp.jpg\","
                            + "\"default_action\":{"
                            + "\"type\":\"oa.open.url\","
                            + "\"url\":\"http://service.capnuoctanhoa.com.vn:1010/Zalo\""
                            + "}"
                            +"},"
                            +"{"
                            + "\"title\":\"Click vào đây để đăng ký\","
                            + "\"subtitle\":\"Click vào đây để đăng ký\","
                            + "\"image_url\":\"http://www.capnuoctanhoa.com.vn/uploads/page/logoctycp.jpg\","
                            + "\"default_action\":{"
                            +"\"type\":\"oa.open.url\","
                            + "\"url\":\"http://service.capnuoctanhoa.com.vn:1010/Zalo\""
                            + "}"
                            + "}]"
                            + "}"
                            + "}"
                            + "}"
                            + "}";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    strResponse = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                }
                else
                {
                    strResponse = "Error: " + respuesta.StatusCode;
                }
                return strResponse;
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        private string getSHA256(string strData)
        {
            SHA256Managed crypt = new SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(strData), 0, Encoding.UTF8.GetByteCount(strData));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }
    }
}
