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
        CConnection _cDAL_DHN = new CConnection(CConstantVariable.DHN);
        CConnection _cDAL_DocSo = new CConnection(CConstantVariable.DocSo);
        string access_token = "mmKT2lMZcXN22HGAyAYI5yC37WhzlzHEpraR1VYAb6EdLnjxzwFtOvX2UHZQbfPNxJGUIPhLq5VJAGqVvUk41RaxQ7NCp-yLyWfZLBlyxYwDLWSnxepFKgni5JFbn-8fao8JRTZ8uZVkBMfFeE3YNCmp3aQ1bjCcms0LVRUz_cBpIW8Ub9gD1AKmLqddzDWQfpvAQEcOis-aO6SKwBg92hPlGHNXgPPxZNXvOTIJaWIRD5HqfExOAS0h7cQ3y-Dyj35b3yZfunUM40XIK5W6ahygzhcH6G";
        apiTrungTamKhachHangController apiTTKH = new apiTrungTamKhachHangController();

        /// <summary>
        /// webhook receive zalo
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="event_name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Route("webhook")]
        [HttpGet]
        public string webhook(string IDZalo, string event_name, string message)
        {
            string strResponse = "success";
            try
            {
                //if (mac == getSHA256(oaid + fromuid + msgid + message + timestamp + "cCBBIsEx7UDj42KA1N5Y"))
                //bấm quan tâm
                if (event_name == "follow")
                {
                    string sql = "if not exists(select * from ZaloQuanTam where IDZalo=" + IDZalo + ")"
                        + " insert into ZaloQuanTam(IDZalo, CreateDate)values(" + IDZalo + ", GETDATE())";
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                //bấm bỏ quan tâm
                if (event_name == "unfollow")
                {
                    string sql = "delete ZaloQuanTam where IDZalo=" + IDZalo;
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                if (event_name == "user_send_text")
                {
                    //gửi tin nhắn đăng ký
                    if (message == "$dangkythongtin")
                    {
                        strResponse = sendMessageDangKy(IDZalo);
                    }
                    else
                    {
                        //xét id chưa đăng ký
                        DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                        if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                        {
                            strResponse = sendMessageDangKy(IDZalo);
                        }
                        else
                        //bắt đầu gửi tin nhắn tra cứu
                        {
                            string sql = "";
                            switch (message)
                            {
                                case "$get12kyhoadon"://lấy 12 kỳ hóa đơn gần nhất
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
                                            strResponse = sendMessage(IDZalo, content);
                                        }
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'get12kyhoadon',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "$gethoadonton"://lấy hóa đơn tồn
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                                        if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                                        {
                                            string content = "Danh Bộ: " + dt_HoaDon.Rows[0]["DanhBo"].ToString() + "\n"
                                                        + "Họ tên: " + dt_HoaDon.Rows[0]["HoTen"].ToString() + "\n"
                                                        + "Địa chỉ: " + dt_HoaDon.Rows[0]["DiaChi"].ToString() + "\n"
                                                        + "Giá biểu: " + dt_HoaDon.Rows[0]["GiaBieu"].ToString() + "\n"
                                                        + "Định mức: " + dt_HoaDon.Rows[0]["DinhMuc"].ToString() + "\n\n"
                                                        + "Hiện đang còn nợ\n";
                                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                                            {
                                                content += "Kỳ " + itemHD["KyHD"].ToString() + ": Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3 ; Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n";
                                            }
                                            strResponse = sendMessage(IDZalo, content);
                                        }
                                        else
                                        {
                                            DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                                            string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                        + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                        + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                        + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                        + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n\n"
                                                        + "Hiện đang Hết Nợ";
                                            strResponse = sendMessage(IDZalo, content);
                                        }

                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'gethoadonton',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "$getlichdocso"://lấy lịch đọc số
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");

                                        //string sql_Lich = "select top 1 NoiDung=N'Kỳ '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được ghi chỉ số vào ngày '+CONVERT(varchar(10),b.NgayDoc,103) from Lich_DocSo a,Lich_DocSo_ChiTiet b,Lich_Dot c where a.ID=b.IDDocSo and c.ID=b.IDDot"
                                        //                + " and((c.TB1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TB2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TP1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TP2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + "))"
                                        //                + " order by a.CreateDate desc";
                                        //string result_Lich = _cDAL_TrungTam.ExecuteQuery_ReturnOneValue(sql_Lich).ToString();
                                        string result_Lich = apiTTKH.getLichDocSo_Func(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();

                                        string result_NhanVien = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select NhanVien=N'Nhân viên ghi chỉ số: '+NhanVienID+' : '+DienThoai from MayDS where May=" + dt_ThongTin.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();

                                        string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                    + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                    + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                    + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                    + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n\n"
                                                    + result_NhanVien + "\n"
                                                    + result_Lich;
                                        strResponse = sendMessage(IDZalo, content);
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'getlichdocso',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                case "$getlichthutien"://lấy lịch thu tiền
                                    foreach (DataRow item in dt_DanhBo.Rows)
                                    {
                                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");

                                        //string sql_Lich = "select top 1 NoiDung=N'Kỳ '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được thu tiền từ ngày '+CONVERT(varchar(10),b.NgayThuTien_From,103)+N' đến ngày '+CONVERT(varchar(10),b.NgayThuTien_To,103) from Lich_ThuTien a,Lich_ThuTien_ChiTiet b,Lich_Dot c where a.ID=b.IDThuTien and c.ID=b.IDDot"
                                        //                + " and((c.TB1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TB2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TB2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TP1_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP1_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + ")"
                                        //                + " or (c.TP2_From <= " + dt_ThongTin.Rows[0]["MLT"].ToString() + " and c.TP2_To >= " + dt_ThongTin.Rows[0]["MLT"].ToString() + "))"
                                        //                + " order by a.CreateDate desc";
                                        //string result_Lich = _cDAL_TrungTam.ExecuteQuery_ReturnOneValue(sql_Lich).ToString();
                                        string result_Lich = apiTTKH.getLichThuTien_Func(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();

                                        string result_NhanVien = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select top 1 NhanVien=N'Nhân viên thu tiền: '+HoTen+' : '+DienThoai from HOADON a,TT_NguoiDung b where DANHBA='" + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "' and a.MaNV_HanhThu=b.MaND order by ID_HOADON desc").ToString();

                                        string content = "Danh Bộ: " + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "\n"
                                                    + "Họ tên: " + dt_ThongTin.Rows[0]["HoTen"].ToString() + "\n"
                                                    + "Địa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString() + "\n"
                                                    + "Giá biểu: " + dt_ThongTin.Rows[0]["GiaBieu"].ToString() + "\n"
                                                    + "Định mức: " + dt_ThongTin.Rows[0]["DinhMuc"].ToString() + "\n\n"
                                                    + result_NhanVien + "\n"
                                                    + result_Lich;
                                        strResponse = sendMessage(IDZalo, content);
                                    }
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'getlichthutien',getdate())";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                    break;
                                default:
                                    //insert lịch sử truy vấn
                                    sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",N'" + message + "',getdate())";
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
        /// Gửi tin nhắn cúp nước
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Route("sendMessageCupNuoc")]
        [HttpGet]
        public string sendMessageCupNuoc(string IDZalo, string message)
        {
            string strResponse = "success";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message?access_token=" + access_token;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                string data = "{"
                            + "\"recipient\": {"
                            + "\"user_id\":\"" + IDZalo + "\""
                            + "},"
                            + "\"message\":{"
                            + "\"text\": \"" + message + "\","
                            + "\"attachment\": {"
                            + "\"type\": \"template\","
                            + "\"payload\": {"
                            + "\"template_type\": \"media\","
                            + "\"elements\": [{"
                            + "\"media_type\": \"image\","
                            + "\"url\":\"http://www.capnuoctanhoa.com.vn/zalo/thongbaotamngungcungcapnuoc.jpg\""
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
                    strResponse = sendMessage(item["IDZalo"].ToString(), message);
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
