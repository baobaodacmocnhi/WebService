﻿using System;
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
        CConnection _cDAL_DHN = new CConnection(CConstantVariable.DHN);
        CConnection _cDAL_DocSo = new CConnection(CConstantVariable.DocSo);
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
                    //gửi tin nhắn đăng ký
                    if (message == "#dangkythongtin")
                    {
                        strResponse = sendMessageDangKy(fromuid);
                    }
                    else
                    {
                        //xét id chưa đăng ký
                        DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + fromuid + "");
                        if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                        {
                            strResponse = sendMessageDangKy(fromuid);
                        }
                        else
                        //bắt đầu gửi tin nhắn tra cứu
                        {
                            string sql = "";
                            switch (message)
                            {
                                case "#get12kyhoadon"://lấy 12 kỳ hóa đơn gần nhất
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGet12KyHoaDon(" + item["DanhBo"].ToString() + ")");
                                        if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                                        {
                                            string content = "Danh Bộ: " + dt_HoaDon.Rows[0]["DanhBo"].ToString() + "\n"
                                                        + "Họ tên: " + dt_HoaDon.Rows[0]["HoTen"].ToString() + "\n"
                                                        + "Địa chỉ: " + dt_HoaDon.Rows[0]["DiaChi"].ToString() + "\n"
                                                        + "Giá biểu: " + dt_HoaDon.Rows[0]["GiaBieu"].ToString() + "\n"
                                                        + "Định mức: " + dt_HoaDon.Rows[0]["DinhMuc"].ToString() + "\n\n"
                                                        + "Danh sách 12 kỳ hóa đơn\n";
                                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                                            {
                                                content += "Kỳ " + itemHD["KyHD"].ToString() + ": Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3 ; Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n";
                                            }
                                            strResponse = sendMessage(fromuid, content);
                                        }
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + fromuid + ",'get12kyhoadon',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "#gethoadonton"://lấy hóa đơn tồn
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                                        if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                                        {
                                            string content = "Danh Bộ: " + dt_HoaDon.Rows[0]["DanhBo"].ToString() + "\n"
                                                        + "Họ tên: " + dt_HoaDon.Rows[0]["HoTen"].ToString() + "\n"
                                                        + "Địa chỉ: " + dt_HoaDon.Rows[0]["DiaChi"].ToString() + "\n"
                                                        + "Giá biểu: " + dt_HoaDon.Rows[0]["GiaBieu"].ToString() + "\n"
                                                        + "Định mức: " + dt_HoaDon.Rows[0]["DinhMuc"].ToString() + "\n"
                                                        + "Hiện đang còn nợ\n\n";
                                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                                            {
                                                content += "Kỳ " + itemHD["KyHD"].ToString() + ": Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3 ; Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n";
                                            }
                                            strResponse = sendMessage(fromuid, content);
                                        }
                                        else
                                        {
                                            DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                                            string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                        + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                        + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                        + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                        + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n"
                                                        + "Hiện đang Hết Nợ";
                                            strResponse = sendMessage(fromuid, content);
                                        }

                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + fromuid + ",'gethoadonton',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "#getlichdocso"://lấy lịch đọc số
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                                        string sql_Lich = "select top 1 NoiDung=N'Kỳ hóa đơn '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được ghi chỉ số vào ngày '+CONVERT(varchar(10),b.NgayDoc,103) from Lich_DocSo a,Lich_DocSo_ChiTiet b,Lich_Dot c where a.ID=b.IDDocSo and c.ID=b.IDDot"
                                                        + " and((c.TB1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TB2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TP1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TP2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + "))"
                                                        + " order by a.CreateDate desc";
                                        string result_Lich = _cDAL_TrungTam.ExecuteQuery_ReturnOneValue(sql_Lich).ToString();
                                        string result_NhanVien= _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select NhanVienID+' : '+DienThoai from MayDS where May=" + dt_ThongTin.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();

                                        string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                    + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                    + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                    + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                    + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n"
                                                    + result_Lich + "\n"
                                                    + "Nhân viên ghi chỉ số: "+result_NhanVien;
                                        strResponse = sendMessage(fromuid, content);
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + fromuid + ",'getlichdocso',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "#getlichthutien"://lấy lịch thu tiền
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                                        string sql_Lich = "select top 1 NoiDung=N'Kỳ hóa đơn '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được thu tiền từ ngày '+CONVERT(varchar(10),b.NgayThuTien_From,103)+N' đến ngày '+CONVERT(varchar(10),b.NgayThuTien_To,103) from Lich_ThuTien a,Lich_ThuTien_ChiTiet b,Lich_Dot c where a.ID=b.IDThuTien and c.ID=b.IDDot"
                                                        + " and((c.TB1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TB2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TP1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                                        + " or (c.TP2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + "))"
                                                        + " order by a.CreateDate desc";
                                        string result_Lich = _cDAL_TrungTam.ExecuteQuery_ReturnOneValue(sql_Lich).ToString();
                                        string result_NhanVien = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 HanhThu=HoTen+' : '+DienThoai from HOADON a,TT_NguoiDung b where DANHBA='"+ dt_ThongTin.Rows[0]["DanhBo"].ToString() + "' and a.MaNV_HanhThu=b.MaND order by ID_HOADON desc").ToString();

                                        string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                    + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                    + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                    + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                    + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n"
                                                    + result_Lich + "\n"
                                                    + "Nhân viên thu tiền: " + result_NhanVien;
                                        strResponse = sendMessage(fromuid, content);
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + fromuid + ",'getlichthutien',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                default:
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + fromuid + ",N'" + message + "',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                            }
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

        /// <summary>
        /// Gửi tin nhắn đăng ký
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <returns></returns>
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
                            + "\"user_id\":\"" + IDZalo + "\""
                            + "},"
                            + "\"message\":{"
                            + "\"attachment\":{"
                            + "\"type\":\"template\","
                            + "\"payload\":{"
                            + "\"template_type\":\"list\","
                            + "\"elements\":[{"
                            + "\"title\":\"QUÝ KHÁCH HÀNG CHƯA ĐĂNG KÝ THÔNG TIN\","
                            + "\"subtitle\":\"Để sử dụng dịch vụ, Quý Khách Hàng cần phải đăng ký ít nhất một mã khách hàng (Danh Bộ)\","
                            + "\"image_url\":\"http://www.capnuoctanhoa.com.vn/uploads/zalo/zaloOACover1333x750.png\","
                            + "\"default_action\":{"
                            + "\"type\":\"oa.open.url\","
                            + "\"url\":\"http://service.capnuoctanhoa.com.vn:1010/Zalo?id=" + IDZalo + "\""
                            + "}"
                            + "},"
                            + "{"
                            + "\"title\":\"Click vào đây để đăng ký\","
                            + "\"subtitle\":\"Click vào đây để đăng ký\","
                            + "\"image_url\":\"http://www.capnuoctanhoa.com.vn/uploads/page/logoctycp.jpg\","
                            + "\"default_action\":{"
                            + "\"type\":\"oa.open.url\","
                            + "\"url\":\"http://service.capnuoctanhoa.com.vn:1010/Zalo?id=" + IDZalo + "\""
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

        /// <summary>
        /// Gửi tin nhắn từ CRM cho trưởng phòng
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Route("sendMessage_CRM")]
        [HttpGet]
        public string sendMessage_CRM(string KyHieuPhong, string message)
        {
            string strResponse = "success";
            try
            {
                DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo from Zalo where KyHieuPhong='" + KyHieuPhong + "'");
                foreach (DataRow item in dt.Rows)
                {
                    strResponse= sendMessage(item["IDZalo"].ToString(),message);
                }
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
