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

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/Zalo")]
    public class apiZaloController : ApiController
    {
        CConnection _cDAL_ThuTien = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        CConnection _cDAL_TrungTam = new CConnection("Data Source=serverg8-01;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");

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
                    if (message == "#get12kyhoadon")
                    {
                        DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + fromuid + "");
                        if (dt_DanhBo != null && dt_DanhBo.Rows.Count > 0)
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
                    }
                    else
                        if (message == "#gethoadonton")
                    {
                        DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + fromuid + "");
                        if (dt_DanhBo != null && dt_DanhBo.Rows.Count > 0)
                            foreach (DataRow item in dt_DanhBo.Rows)
                            {
                                string content = item["DanhBo"].ToString() + "\n";
                                DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                                if (dt_HoaDon!=null&&dt_HoaDon.Rows.Count > 0)
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
        public string sendMessage(string IDZalo,string message)
        {
            string strResponse = "success";
            try
            {
                string access_token = "SVFT5EqUDLzm_zGKw1KzNN7rtXJb0bSiSPxaEQeWH0zgYU4ViIf52764tH-41qCROhNg4FiUKHanz_aByHvDGn2WzLpCCd9d8gxi8CWr2GeXdReSoNO9D1_LgGpsQ2uv6_IX2_Li0XDhWeGhYGa45bNzkW2RL0S_3kgaUEnAB6Xpr8judaywOs3btN-lT3euRPoTAhGlVon8miKUrtHDAZZbysVL3njiBBd_8BDJ7Z9-lA4VdKWq2W6WoHRRAI034RU1D-9UCsHbMQpPq7xk13vJ";
                string url = "https://openapi.zalo.me/v2.0/oa/message?access_token="+access_token;
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
            //DataTable dt = new DataTable();
            //int count = 0;
            ////check exist
            //try
            //{
            //    count = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID_HOADON) from HOADON where DANHBA='" + DanhBo + "'");
            //}
            //catch (Exception ex)
            //{
            //    ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            //}
            //if (count == 0)
            //{
            //    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorKhongDung, ErrorResponse.ErrorCodeKhongDung);
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            //}
            ////get 12 ky hoa don
            //try
            //{
            //    dt = _cDAL.ExecuteQuery_DataTable("select * from fnGet12KyHoaDon(" + DanhBo + ")");
            //}
            //catch (Exception ex)
            //{
            //    ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            //}
            ////
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    List<HoaDon> hoadons = new List<HoaDon>();
            //    foreach (DataRow item in dt.Rows)
            //    {
            //        HoaDon entity = new HoaDon();
            //        entity.HoTen = item["HoTen"].ToString();
            //        entity.DiaChi = item["DiaChi"].ToString();
            //        entity.DanhBo = (string)item["DanhBo"];
            //        entity.KyHD = item["KyHD"].ToString();
            //        entity.GiaBan = int.Parse(item["GiaBan"].ToString());
            //        entity.ThueGTGT = int.Parse(item["ThueGTGT"].ToString());
            //        entity.PhiBVMT = int.Parse(item["PhiBVMT"].ToString());
            //        entity.TongCong = int.Parse(item["TongCong"].ToString());
            //        hoadons.Add(entity);
            //    }
            //    return hoadons;
            //}
            //else
            //{
            //    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorHetNo, ErrorResponse.ErrorCodeHetNo);
            //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            //}
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
