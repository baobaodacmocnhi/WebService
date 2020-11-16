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
        //string _url = "https://service.cskhtanhoa.com.vn";
        //string _urlImage = "https://service.cskhtanhoa.com.vn/Image";
        string _url = "http://service.capnuoctanhoa.com.vn:1010";
        string _urlImage = "http://service.capnuoctanhoa.com.vn:1010/Image";

        /// <summary>
        /// webhook receive zalo
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="event_name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("webhook")]
        public string webhook(string IDZalo, string event_name, string message)
        {
            log4net.ILog _log = log4net.LogManager.GetLogger("File");
            string strResponse = "success";
            try
            {
                //if (mac == getSHA256(oaid + fromuid + msgid + message + timestamp + "cCBBIsEx7UDj42KA1N5Y"))
                //bấm quan tâm
                if (event_name == "follow")
                {
                    sendMessageDangKy(IDZalo);
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
                        //bắt đầu gửi tin nhắn tra cứu
                        //DataTable dt_DanhBo;
                        //string sql = "";
                        switch (message)
                        {
                            case "$get12kyhoadon"://lấy 12 kỳ hóa đơn gần nhất
                                                  //xét id chưa đăng ký
                                get12kyhoadon(IDZalo, ref strResponse);
                                break;
                            case "$gethoadonton"://lấy hóa đơn tồn
                                                 //xét id chưa đăng ký
                                gethoadonton(IDZalo, ref strResponse);
                                break;
                            case "$getlichdocso"://lấy lịch đọc số
                                                 //xét id chưa đăng ký
                                getlichdocso(IDZalo, ref strResponse);
                                break;
                            case "$getlichthutien"://lấy lịch thu tiền
                                                   //xét id chưa đăng ký
                                getlichthutien(IDZalo, ref strResponse);
                                break;
                            default:
                                //insert lịch sử truy vấn
                                //sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",N'" + message + "',getdate())";
                                //_cDAL_TrungTam.ExecuteNonQuery(sql);
                                string[] messagesCSN = message.Split('_');
                                string[] messagesKyHD = message.Split('/');
                                if (messagesCSN.Count() > 1)
                                {
                                    if (messagesCSN[0].ToUpper() == "CSN")
                                    {
                                        baochisonuoc(IDZalo, messagesCSN, ref strResponse);
                                    }
                                }
                                else
                                    if (messagesKyHD.Count() > 1)
                                {
                                    getkyhoadon(IDZalo, messagesKyHD[1], messagesKyHD[0], ref strResponse);
                                }
                                else
                                {
                                    tinnhankhac(IDZalo, message, ref strResponse);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            _log.Error("webhook: " + strResponse);
            return strResponse;
        }

        private void get12kyhoadon(string IDZalo, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 6 * from fnGet12KyHoaDon(" + item["DanhBo"].ToString() + ")");
                    if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                    {
                        string content = getTTKH(item["DanhBo"].ToString());
                        content += "Danh sách 6 kỳ hóa đơn\n";
                        foreach (DataRow itemHD in dt_HoaDon.Rows)
                        {
                            content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3\n";
                            if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                            content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + "đ\n"
                                    + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + "đ\n"
                                    + "    Phí BVMT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + "đ\n"
                                    + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n\n";
                        }
                        strResponse = sendMessage(IDZalo, content);
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'get12kyhoadon',getdate())";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void getkyhoadon(string IDZalo, string Nam, string Ky, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetKyHoaDon(" + item["DanhBo"].ToString() + "," + Nam + "," + Ky + ")");
                    if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                    {
                        string content = getTTKH(item["DanhBo"].ToString());
                        foreach (DataRow itemHD in dt_HoaDon.Rows)
                        {
                            content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3\n";
                            if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                            content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + "đ\n"
                                    + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + "đ\n"
                                    + "    Phí BVMT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + "đ\n"
                                    + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n\n";
                        }
                        strResponse = sendMessage(IDZalo, content);
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'get12kyhoadon',getdate())";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void gethoadonton(string IDZalo, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    string content = getTTKH(item["DanhBo"].ToString());
                    DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                    if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                    {
                        content += "Hiện đang còn nợ\n";
                        foreach (DataRow itemHD in dt_HoaDon.Rows)
                        {
                            content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m3\n";
                            if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                            content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + "đ\n"
                                    + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + "đ\n"
                                    + "    Phí BVMT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + "đ\n"
                                    + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + "đ\n\n";
                        }
                        strResponse = sendMessage(IDZalo, content);
                    }
                    else
                    {
                        content += "Hiện đang Hết Nợ";
                        strResponse = sendMessage(IDZalo, content);
                    }

                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'gethoadonton',getdate())";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void getlichdocso(string IDZalo, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");

                    string result_Lich = apiTTKH.getLichDocSo_Func_String(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();

                    string result_NhanVien = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select NhanVien=N'Nhân viên ghi chỉ số: '+NhanVienID+' : '+DienThoai from MayDS where May=" + dt_ThongTin.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();

                    string content = getTTKH(item["DanhBo"].ToString());
                    content += result_NhanVien + "\n"
                                + result_Lich;
                    strResponse = sendMessage(IDZalo, content);
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'getlichdocso',getdate())";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void getlichthutien(string IDZalo, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");

                    string result_Lich = apiTTKH.getLichThuTien_Func_String(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();

                    string result_NhanVien = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select top 1 NhanVien=N'Nhân viên thu tiền: '+HoTen+' : '+DienThoai from HOADON a,TT_NguoiDung b where DANHBA='" + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "' and a.MaNV_HanhThu=b.MaND order by ID_HOADON desc").ToString();

                    string content = getTTKH(item["DanhBo"].ToString());
                    content += result_NhanVien + "\n"
                                 + result_Lich;
                    strResponse = sendMessage(IDZalo, content);
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate)values(" + IDZalo + ",'getlichthutien',getdate())";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void baochisonuoc(string IDZalo, string[] messages, ref string strResponse)
        {
            try
            {
                if (messages.Count() != 3)
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai Cú Pháp, Vui lòng thử lại");
                else
                        if (messages[1].Length != 11)
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai Danh Bộ, Vui lòng thử lại");
                else
                {
                    DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select DanhBo,MLT=LOTRINH from TB_DULIEUKHACHHANG where DanhBo='" + messages[1] + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow drLich = apiTTKH.getLichDocSo_Func_DataRow(dt.Rows[0]["DanhBo"].ToString(), dt.Rows[0]["MLT"].ToString());
                        //nếu trước 2 ngày
                        if (DateTime.Now.Date < DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-2))
                        {
                            strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nChưa đến kỳ đọc số tiếp theo, Vui lòng tra cứu lịch đọc số");
                            return;
                        }
                        else
                        //nếu sau 12h ngày chuyển listing
                        if (DateTime.Now.Date == DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date && DateTime.Now.Hour > 11)
                        {
                            strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nĐã quá thời gian ghi chỉ số");
                            return;
                        }
                        else
                        //nếu sau ngày chuyển listing
                        if (DateTime.Now.Date > DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date)
                        {
                            strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nChưa đến kỳ đọc số tiếp theo, Vui lòng tra cứu lịch đọc số");
                            return;
                        }
                        //kiểm tra đã gửi chỉ số nước rồi
                        string sql = "select top 1 * from DocSo_Zalo where DanhBo='" + dt.Rows[0]["DanhBo"].ToString() + "' order by CreateDate desc";
                        DataTable dtResult = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
                        if (dtResult != null && dtResult.Rows.Count > 0)
                            if (DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date >= DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-2)
                                || DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date == DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date)
                            {
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nDanh Bộ này đã gửi chỉ số nước rồi");
                                return;
                            }
                        //kiểm tra chỉ số nước
                        if (IsNumber(messages[2]) == true)
                        {
                            sql = "insert into DocSo_Zalo(DanhBo,ChiSo,CreateDate)values(N'" + messages[1] + "',N'" + messages[2] + "',getdate())";
                            if (_cDAL_DocSo.ExecuteNonQuery(sql) == true)
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nThành Công, Cám ơn Quý Khách Hàng đã cung cấp chỉ số nước");
                            else
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nThất Bại, Vui lòng thử lại");
                        }
                        else
                            strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nChỉ Số không đúng, Vui lòng thử lại");
                    }
                    else
                        strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nDanh Bộ này không tồn tại, Vui lòng thử lại");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void tinnhankhac(string IDZalo, string message, ref string strResponse)
        {
            try
            {
                DateTime date = DateTime.Now;
                if (date.Date.DayOfWeek == DayOfWeek.Saturday || date.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nXin cám ơn Quý khách đã liên hện Công ty Cổ phần Cấp nước Tân Hòa. Hiện đã hết giờ làm việc xin Quý khách liên hệ lại vào giờ hành chính (từ thứ hai đến thứ sáu). Hoặc liên hệ tổng đài 19006489 để được giải đáp nhanh hơn. Xin cám ơn!");
                }
                else
                    if ((date.Hour == 17 && date.Minute > 0)
                    || date.Hour > 17
                    || date.Hour < 7
                    || (date.Hour == 7 && date.Minute < 30))
                {
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nXin cám ơn Quý khách đã liên hện Công ty Cổ phần Cấp nước Tân Hòa. Hiện đã hết giờ làm việc xin Quý khách liên hệ lại vào giờ hành chính (từ thứ hai đến thứ sáu). Hoặc liên hệ tổng đài 19006489 để được giải đáp nhanh hơn. Xin cám ơn!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                            + "\"image_url\":\"" + _urlImage + "/zaloOACover1333x750.png\","
                            + "\"default_action\":{"
                            + "\"type\":\"oa.open.url\","
                            + "\"url\":\"" + _url + "/Zalo?id=" + IDZalo + "\""
                            + "}"
                            + "},"
                            + "{"
                            + "\"title\":\"Click vào đây để đăng ký\","
                            + "\"subtitle\":\"Click vào đây để đăng ký\","
                            + "\"image_url\":\"" + _urlImage + "/logoctycp.png\","
                            + "\"default_action\":{"
                            + "\"type\":\"oa.open.url\","
                            + "\"url\":\"" + _url + "/Zalo?id=" + IDZalo + "\""
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
                            + "\"url\":\"" + _urlImage + "/zalothongbaotamngungcungcapnuoc.jpg\""
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

        private bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private string getTTKH(string DanhBo)
        {
            string sql = "select DanhBo"
                             + ",HoTen"
                             + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                             + ",DinhMuc"
                             + ",DinhMucHN"
                             + ",GiaBieu"
                             + " from TB_DULIEUKHACHHANG where DanhBo=" + DanhBo;
            DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return "Hệ thống trả lời tự động\n\n"
                        + "Danh Bộ: " + dt.Rows[0]["DanhBo"].ToString() + "\n"
                        + "Họ tên: " + dt.Rows[0]["HoTen"].ToString() + "\n"
                        + "Địa chỉ: " + dt.Rows[0]["DiaChi"].ToString() + "\n"
                        + "Giá biểu: " + dt.Rows[0]["GiaBieu"].ToString() + "\n"
                        + "Định mức: " + dt.Rows[0]["DinhMuc"].ToString() + "    Định mức HN: " + dt.Rows[0]["DinhMuc"].ToString() + "\n"
                        + getCSC_CSM_MoiNhat(dt.Rows[0]["DanhBo"].ToString()) + "\n\n";
            }
            else
                return null;
        }

        private string getCSC_CSM_MoiNhat(string DanhBo)
        {
            return _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select top 1 'CSC: '+ CONVERT(varchar(10),CSCU) + '    ' + 'CSM: ' + CONVERT(varchar(10),CSMOI) from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc").ToString();
        }

        private string getCSC_CSM(string DanhBo, int Nam, int Ky)
        {
            string sql = "select top 1 'CSC: '+ CONVERT(varchar(10),CSCU) + '    ' + 'CSM: ' + CONVERT(varchar(10),CSMOI) from HOADON where DANHBA='" + DanhBo + "' and NAM=" + Nam + " and KY=" + Ky
                        + " union all"
                        + " select top 1 'CSC: ' + CONVERT(varchar(10), CSCU) + '    ' + 'CSM: ' + CONVERT(varchar(10), CSMOI) from TT_HoaDonCu where DANHBA='" + DanhBo + "' and NAM=" + Nam + " and KY=" + Ky;
            return _cDAL_ThuTien.ExecuteQuery_ReturnOneValue(sql).ToString();
        }

    }
}
