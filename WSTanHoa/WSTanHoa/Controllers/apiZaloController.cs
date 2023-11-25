using Newtonsoft.Json.Linq;
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
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        private CConnection _cDAL_sDHN = new CConnection(CGlobalVariable.sDHN);
        private CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection _cDAL_TrungTam = new CConnection(CGlobalVariable.TrungTamKhachHang);
        apiTrungTamKhachHangController _apiTTKH = new apiTrungTamKhachHangController();
        string _IDZalo = "4276209776391262580";
        string _url = "https://service.cskhtanhoa.com.vn";
        string _urlImage = "https://service.cskhtanhoa.com.vn/Images";
        //string _url = "http://service.capnuoctanhoa.com.vn:1010";
        //string _urlImage = "http://service.capnuoctanhoa.com.vn:1010/Image";

        private string getAccess_token()
        {
            return _cDAL_TrungTam.ExecuteQuery_ReturnOneValue("select access_token from Zalo_Configure").ToString();
        }

        private string getRefresh_token()
        {
            return _cDAL_TrungTam.ExecuteQuery_ReturnOneValue("select refresh_token from Zalo_Configure").ToString();
        }

        [Route("getAccess_tokenFromZalo")]
        [HttpGet]
        public string getAccess_tokenFromZalo(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string data = "app_id=3904851815439759378&grant_type=refresh_token&refresh_token=" + getRefresh_token();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Tls11
                           | SecurityProtocolType.Tls12
                           | SecurityProtocolType.Ssl3;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        client.Headers["secret_key"] = "cCBBIsEx7UDj42KA1N5Y";
                        string result = client.UploadString("https://oauth.zaloapp.com/v4/oa/access_token", data);
                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        var obj = jss.Deserialize<dynamic>(result);
                        bool a = _cDAL_TrungTam.ExecuteNonQuery("update Zalo_Configure set access_token='" + obj["access_token"] + "',refresh_token='" + obj["refresh_token"] + "',expires_in=" + obj["expires_in"] + ",CreateDate=getdate()");
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

        /// <summary>
        /// webhook receive zalo, 2020 webhook từ view gọi sang api
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="event_name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("webhook2020")]
        private string webhook2020(string IDZalo, string event_name, string message)
        {
            //log4net.ILog _log = log4net.LogManager.GetLogger("File");
            string strResponse = "";
            try
            {
                //if (mac == getSHA256(oaid + fromuid + msgid + message + timestamp + "cCBBIsEx7UDj42KA1N5Y"))
                //bấm quan tâm
                if (event_name == "follow")
                {
                    sendMessageDangKy(IDZalo);
                    string sql = "if not exists(select * from Zalo_QuanTam where IDZalo=" + IDZalo + ")"
                        + " insert into Zalo_QuanTam(IDZalo)values(" + IDZalo + ")"
                        + " else update Zalo_QuanTam set Follow=1 where IDZalo=" + IDZalo;
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                //bấm bỏ quan tâm
                if (event_name == "unfollow")
                {
                    //string sql = "delete Zalo_QuanTam where IDZalo=" + IDZalo;
                    string sql = "update Zalo_QuanTam set Follow=0 where IDZalo=" + IDZalo;
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
                                //insert chat
                                string sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung)values(" + IDZalo + ",'User',N'" + message + "')";
                                _cDAL_TrungTam.ExecuteNonQuery(sql);
                                //
                                string[] messagesCSN = null;
                                if (message.ToUpper().Contains("CSN_") == true)
                                    messagesCSN = message.Split('_');
                                else
                                    if (message.ToUpper().Contains("CSN-") == true)
                                    messagesCSN = message.Split('-');
                                else
                                    if (message.ToUpper().Contains("CSN ") == true)
                                    messagesCSN = message.Split(' ');

                                string[] messagesKyHD = message.Split('/');

                                if (messagesCSN != null && messagesCSN.Count() > 1)
                                {
                                    if (messagesCSN[0].Trim().ToUpper() == "CSN")
                                    {
                                        //baochisonuoc(IDZalo, messagesCSN, ref strResponse);
                                        strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nTHẤT BẠI\nCú pháp: CSN_danhbo_chisonuoc đã dừng hoạt động\nVui lòng truy cập website để cung cấp chỉ số nước: https://service.cskhtanhoa.com.vn/QLDHN/BaoChiSoNuoc");
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
                if (event_name == "user_send_image")
                {
                    //insert chat
                    string sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung)values(" + IDZalo + ",'User',N'Gửi hình ảnh')";
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                if (event_name == "oa_send_text")
                {
                    //insert chat
                    if (message.Contains("Hệ thống trả lời tự động\n\nDanh Bộ:") == false && message.Contains("Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng") == false)
                    {
                        string sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung)values(" + IDZalo + ",'OA',N'" + message + "')";
                        _cDAL_TrungTam.ExecuteNonQuery(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            //_log.Error("webhook: " + strResponse);
            return strResponse;
        }

        /// <summary>
        /// webhook receive zalo
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("webhook")]
        public string webhook()
        {
            //log4net.ILog _log = log4net.LogManager.GetLogger("File");
            string strResponse = "";
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                var jsonContent = JObject.Parse(jsonResult);
                //_log.Error("link: " + jsonContent);
                //bấm quan tâm
                if (jsonContent["event_name"].ToString() == "follow")
                {
                    sendMessageDangKy(jsonContent["follower"]["id"].ToString());
                    string sql = "if not exists(select * from Zalo_QuanTam where IDZalo=" + jsonContent["follower"]["id"].ToString() + ")"
                        + " insert into Zalo_QuanTam(IDZalo)values(" + jsonContent["follower"]["id"].ToString() + ")"
                        + " else update Zalo_QuanTam set Follow=1,UnFollowDate=NULL where IDZalo=" + jsonContent["follower"]["id"].ToString();
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                //bấm bỏ quan tâm
                if (jsonContent["event_name"].ToString() == "unfollow")
                {
                    //string sql = "delete Zalo_QuanTam where IDZalo=" + jsonContent["follower"]["id"].ToString();
                    string sql = "update Zalo_QuanTam set Follow=0,UnFollowDate=getdate() where IDZalo=" + jsonContent["follower"]["id"].ToString();
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                if (jsonContent["event_name"].ToString() == "user_send_text")
                {
                    string IDZalo = jsonContent["sender"]["id"].ToString();
                    string message = jsonContent["message"]["text"].ToString();
                    //gửi tin nhắn đăng ký
                    if (message == "#dangkythongtin")
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
                            case "#get12kyhoadon"://lấy 12 kỳ hóa đơn gần nhất
                                                  //xét id chưa đăng ký
                                get12kyhoadon(IDZalo, ref strResponse);
                                break;
                            case "#gethoadonton"://lấy hóa đơn tồn
                                                 //xét id chưa đăng ký
                                gethoadonton(IDZalo, ref strResponse);
                                break;
                            case "#getlichdocso"://lấy lịch đọc số
                                                 //xét id chưa đăng ký
                                getlichdocso(IDZalo, ref strResponse);
                                break;
                            case "#getlichthutien"://lấy lịch thu tiền
                                                   //xét id chưa đăng ký
                                getlichthutien(IDZalo, ref strResponse);
                                break;
                            default:
                                //insert chat
                                if (message.Contains("#") == false)
                                {
                                    string sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung)values(" + IDZalo + ",'User',N'" + message + "')";
                                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                                }
                                tinnhankhac(IDZalo, message, ref strResponse);
                                break;
                        }
                    }
                }
                if (jsonContent["event_name"].ToString() == "user_send_image")
                {
                    //insert chat
                    string sql = "";
                    if (jsonContent["message"]["text"] != null)
                        sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung,Image)values(" + jsonContent["sender"]["id"].ToString() + ",'User',N'" + jsonContent["message"]["text"].ToString() + "',N'" + jsonContent["message"]["attachments"][0]["payload"]["url"].ToString() + "')";
                    else
                        sql = "insert into Zalo_Chat(IDZalo,NguoiGui,Image)values(" + jsonContent["sender"]["id"].ToString() + ",'User',N'" + jsonContent["message"]["attachments"][0]["payload"]["url"].ToString() + "')";
                    _cDAL_TrungTam.ExecuteNonQuery(sql);
                }
                if (jsonContent["event_name"].ToString() == "oa_send_text")
                {
                    //insert chat
                    if (jsonContent["message"]["text"].ToString().Contains("Hệ thống trả lời tự động\n\nDanh Bộ:") == false
                        && jsonContent["message"]["text"].ToString().Contains("Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng") == false
                        && jsonContent["message"]["text"].ToString().Contains("Chào mừng Quý Khách Hàng đến với Zalo Official Account") == false)
                    {
                        string sql = "insert into Zalo_Chat(IDZalo,NguoiGui,NoiDung)values(" + jsonContent["recipient"]["id"].ToString() + ",'OA',N'" + jsonContent["message"]["text"].ToString() + "')";
                        _cDAL_TrungTam.ExecuteNonQuery(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            //_log.Error("webhook: " + strResponse);
            return strResponse;
        }

        private void get12kyhoadon(string IDZalo, ref string strResponse)
        {
            try
            {
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo_DangKy where IDZalo=" + IDZalo + "");
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
                        if (content == "")
                        {
                            strResponse = sendMessage(IDZalo, "Danh Bộ " + item["DanhBo"].ToString() + " không tồn tại");
                        }
                        else
                        {
                            content += "Danh sách 6 kỳ hóa đơn\n";
                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                            {
                                content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                    + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                    + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m³    Định Mức: " + itemHD["DinhMuc"].ToString() + "\n";
                                if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                    content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                                content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + " đồng\n"
                                        + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + " đồng\n"
                                        + "    TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + " đồng\n"
                                        + "    Thuế TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT_Thue"].ToString())) + " đồng\n"
                                        + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + " đồng\n\n";
                            }
                            strResponse = sendMessage(IDZalo, content);
                        }
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'get12kyhoadon',getdate(),N'" + strResponse + "')";
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
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo_DangKy where IDZalo=" + IDZalo + "");
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
                        if (content == "")
                        {
                            strResponse = sendMessage(IDZalo, "Danh Bộ " + item["DanhBo"].ToString() + " không tồn tại");
                        }
                        else
                        {
                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                            {
                                content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                    + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                    + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m³    Định Mức: " + itemHD["DinhMuc"].ToString() + "\n";
                                if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                    content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                                content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + " đồng\n"
                                        + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + " đồng\n"
                                        + "    TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + " đồng\n"
                                        + "    Thuế TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT_Thue"].ToString())) + " đồng\n"
                                        + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + " đồng\n\n";
                            }
                            strResponse = sendMessage(IDZalo, content);
                        }
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'get12kyhoadon',getdate(),N'" + strResponse + "')";
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
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo_DangKy where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    string content = getTTKH(item["DanhBo"].ToString());
                    if (content == "")
                    {
                        strResponse = sendMessage(IDZalo, "Danh Bộ " + item["DanhBo"].ToString() + " không tồn tại");
                    }
                    else
                    {
                        DataTable dt_HoaDon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                        if (dt_HoaDon != null && dt_HoaDon.Rows.Count > 0)
                        {
                            content += "Hiện đang còn nợ\n";
                            foreach (DataRow itemHD in dt_HoaDon.Rows)
                            {
                                content += "Kỳ " + itemHD["KyHD"].ToString() + ":\n"
                                    + "    " + getCSC_CSM(itemHD["DanhBo"].ToString(), int.Parse(itemHD["Nam"].ToString()), int.Parse(itemHD["Ky"].ToString())) + "\n"
                                    + "    Tiêu Thụ: " + itemHD["TieuThu"].ToString() + "m³    Định Mức: " + itemHD["DinhMuc"].ToString() + "\n";
                                if (string.IsNullOrEmpty(itemHD["ChiTietTienNuoc"].ToString()) == false)
                                    content += "       " + itemHD["ChiTietTienNuoc"].ToString();
                                content += "    Giá Bán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["GiaBan"].ToString())) + " đồng\n"
                                        + "    Thuế GTGT: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["ThueGTGT"].ToString())) + " đồng\n"
                                        + "    TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT"].ToString())) + " đồng\n"
                                        + "    Thuế TDVTN: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["PhiBVMT_Thue"].ToString())) + " đồng\n"
                                        + "    Tổng Cộng: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(itemHD["TongCong"].ToString())) + " đồng\n\n";
                            }
                            strResponse = sendMessage(IDZalo, content);
                        }
                        else
                        {
                            content += "Hiện đang Hết Nợ";
                            strResponse = sendMessage(IDZalo, content);
                        }
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'gethoadonton',getdate(),N'" + strResponse + "')";
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
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo_DangKy where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    string content = getTTKH(item["DanhBo"].ToString());
                    if (content == "")
                    {
                        strResponse = sendMessage(IDZalo, "Danh Bộ " + item["DanhBo"].ToString() + " không tồn tại");
                    }
                    else
                    {
                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                        string result_Lich = _apiTTKH.getLichDocSo_Func_String(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();
                        string result_NhanVien = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select NhanVien=N'Nhân viên ghi chỉ số: '+HoTen+' : '+DienThoai from NguoiDung where ActiveMobile=1 and May=" + dt_ThongTin.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();
                        content += result_NhanVien + "\n"
                                    + result_Lich;
                        strResponse = sendMessage(IDZalo, content);
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'getlichdocso',getdate(),N'" + strResponse + "')";
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
                DataTable dt_DanhBo = _cDAL_TrungTam.ExecuteQuery_DataTable("select DanhBo from Zalo_DangKy where IDZalo=" + IDZalo + "");
                if (dt_DanhBo == null || dt_DanhBo.Rows.Count == 0)
                {
                    strResponse = sendMessageDangKy(IDZalo);
                }
                foreach (DataRow item in dt_DanhBo.Rows)
                {
                    string content = getTTKH(item["DanhBo"].ToString());
                    if (content == "")
                    {
                        strResponse = sendMessage(IDZalo, "Danh Bộ " + item["DanhBo"].ToString() + " không tồn tại");
                    }
                    else
                    {
                        DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"].ToString() + "' order by ID_HOADON desc");
                        string result_Lich = _apiTTKH.getLichThuTien_Func_String(item["DanhBo"].ToString(), dt_ThongTin.Rows[0]["MLT"].ToString()).ToString();
                        string result_NhanVien = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select top 1 NhanVien=N'Nhân viên nhắc nợ: '+HoTen+' : '+DienThoai from HOADON a,TT_NguoiDung b where DANHBA='" + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "' and a.MaNV_HanhThu=b.MaND order by ID_HOADON desc").ToString();
                        content += result_NhanVien + "\n"
                                     + result_Lich;
                        strResponse = sendMessage(IDZalo, content);
                    }
                }
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'getlichthutien',getdate(),N'" + strResponse + "')";
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
                        if (messages[1].Trim().Length != 11)
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai Danh Bộ, Vui lòng thử lại");
                if (messages[2].Trim() == "" || messages[2].Trim().All(char.IsNumber) == false)
                    strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai Chỉ Số Nước, Vui lòng thử lại");
                else
                {
                    DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select DanhBo,MLT=LOTRINH from TB_DULIEUKHACHHANG where DanhBo='" + messages[1].Trim() + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow drLich = _apiTTKH.getLichDocSo_Func_DataRow(dt.Rows[0]["DanhBo"].ToString(), dt.Rows[0]["MLT"].ToString());
                        //nếu trước 1 ngày
                        if (DateTime.Now.Date < DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-1))
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
                            if (DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date >= DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-1)
                                || DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date == DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date)
                            {
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nDanh Bộ này đã gửi chỉ số nước rồi");
                                return;
                            }
                        //kiểm tra chỉ số nước
                        if (IsNumber(messages[2].Trim()) == true)
                        {
                            sql = "insert into DocSo_Zalo(DanhBo,ChiSo,CreateDate)values(N'" + messages[1].Trim() + "',N'" + messages[2].Trim() + "',getdate())";
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

        private void getdangkydanhbocuphap(string IDZalo, ref string strResponse)
        {
            try
            {
                string content = "Cú pháp đăng ký thông tin: DK DB <danh bộ>"
                    + "\nCú pháp hủy đăng ký thông tin: HUY DB <danh bộ>";
                strResponse = sendMessage(IDZalo, content);
                //insert lịch sử truy vấn
                string sql = "insert into Zalo_LichSuTruyVan(IDZalo,TruyVan,CreateDate,Result)values(" + IDZalo + ",'getdangkycuphapdanhbo',getdate(),N'" + strResponse + "')";
                _cDAL_TrungTam.ExecuteNonQuery(sql);
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
                string str = message.Trim().ToUpper();
                string[] messagesKyHD = message.Split('/');
                if (messagesKyHD != null && messagesKyHD.Count() > 1)
                {
                    getkyhoadon(IDZalo, messagesKyHD[1], messagesKyHD[0], ref strResponse);
                }
                else
                    if (str.Contains("DK ECONTRACT"))
                {
                    string[] strs = str.Split(' ');
                    if (strs[2].Trim().Length != 10)
                    {
                        sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai số điện thoại");
                    }
                    else
                    {
                        DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select * from Zalo_EContract where DienThoai='" + strs[2].Trim() + "' and IDZalo='" + IDZalo + "'");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSố điện thoại và tài khoản Zalo đã tồn tại");
                        }
                        else
                        {
                            if (_cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_EContract(DienThoai,IDZalo)values('" + strs[2].Trim() + "','" + IDZalo + "')"))
                                sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thành công eContract");
                            else
                                sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thất bại eContract");
                        }
                    }
                }
                else
                    if (str.Contains("CSN"))
                {
                    string[] strs = str.Split(' ');
                    if (strs[1].Trim().Length != 11)
                    {
                        sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai số Danh Bộ");
                    }
                    else
                    {
                        DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select ttkh.* from [sDHN].[dbo].[sDHN_TCT] sdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                                + " where sdhn.DanhBo = ttkh.DANHBO and sdhn.Valid = 1 and sdhn.DanhBo = '" + strs[1].Trim() + "'");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DataTable dtCS = null;
                            if (strs.Count() == 3)
                            {
                                string[] dates = strs[2].Trim().Split('/');
                                dtCS = _cDAL_DHN.ExecuteQuery_DataTable("declare @date date"
                               + " set @date = '" + dates[2] + "-" + dates[1] + "-" + dates[0] + "';"
                               + " select t1.*,TieuThu = t1.CSM - t1.CSC from"
                               + " (select TuNgay = convert(varchar(10), DATEADD(DAY, -1, @date), 103), CSC = (select ChiSo from[sDHN].[dbo].[sDHN_LichSu_TCT] where CAST(ThoiGianCapNhat as date) = DATEADD(DAY, -1, @date) and DATEPART(HOUR, ThoiGianCapNhat) = 01 and DATEPART(MINUTE, ThoiGianCapNhat) = 0 and DanhBo = '" + strs[1].Trim() + "')"
                               + " , DenNgay = convert(varchar(10), @date, 103),CSM = (select ChiSo from[sDHN].[dbo].[sDHN_LichSu_TCT] where CAST(ThoiGianCapNhat as date)=@date and DATEPART(HOUR, ThoiGianCapNhat)=01 and DATEPART(MINUTE, ThoiGianCapNhat)=0  and DanhBo = '" + strs[1].Trim() + "'))t1");
                            }
                            else
                                dtCS = _cDAL_DHN.ExecuteQuery_DataTable("declare @date date"
                                   + " set @date = DATEADD(DAY, -1, getdate());"
                                   + " select t1.*,TieuThu = t1.CSM - t1.CSC from"
                                   + " (select TuNgay = convert(varchar(10), DATEADD(DAY, -1, @date), 103), CSC = (select ChiSo from[sDHN].[dbo].[sDHN_LichSu_TCT] where CAST(ThoiGianCapNhat as date) = DATEADD(DAY, -1, @date) and DATEPART(HOUR, ThoiGianCapNhat) = 01 and DATEPART(MINUTE, ThoiGianCapNhat) = 0 and DanhBo = '" + strs[1].Trim() + "')"
                                   + " , DenNgay = convert(varchar(10), @date, 103),CSM = (select ChiSo from[sDHN].[dbo].[sDHN_LichSu_TCT] where CAST(ThoiGianCapNhat as date)=@date and DATEPART(HOUR, ThoiGianCapNhat)=01 and DATEPART(MINUTE, ThoiGianCapNhat)=0  and DanhBo = '" + strs[1].Trim() + "'))t1");
                            if (dtCS != null && dtCS.Rows.Count > 0)
                            {
                                string NoiDung = getTTKH(strs[1].Trim())
                                    + "Chỉ số đồng hồ nước thông minh tính từ 01g00 ngày " + dtCS.Rows[0]["TuNgay"].ToString() + " đến 01g00 ngày " + dtCS.Rows[0]["DenNgay"].ToString() + "\n"
                                    + "CSC: " + float.Parse(dtCS.Rows[0]["CSC"].ToString()).ToString("0.00") + "    CSM: " + float.Parse(dtCS.Rows[0]["CSM"].ToString()).ToString("0.00") + "    TT: " + float.Parse(dtCS.Rows[0]["TieuThu"].ToString()).ToString("0.00") + " m3";
                                //DataTable dtZalo = _cDAL_TrungTam.ExecuteQuery_DataTable("select a.IDZalo from Zalo_QuanTam a,Zalo_DangKy b where a.IDZalo=b.IDZalo and Follow=1 and DanhBo='" + item["DanhBo"].ToString() + "'");
                                //foreach (DataRow itemZalo in dtZalo.Rows)
                                {
                                    strResponse = sendMessage(IDZalo, NoiDung);
                                    _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,Loai,NoiDung,Result)values(" + IDZalo + ",N'CSNsDHN',N'" + NoiDung + "',N'" + strResponse + "')");
                                }
                            }
                            else
                                sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nKhông có dữ liệu");
                        }
                        else
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nSai số Danh Bộ");
                    }
                }
                else
                    switch (str)
                    {
                        //cảnh báo
                        case "DK CLN":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set CLN=1 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thành công group Chất Lượng Nước");
                            break;
                        case "DK DMA":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set DMA=1 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thành công group DMA");
                            break;
                        case "DK SDHN":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set sDHN=1 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thành công group ĐHN Thông Minh");
                            break;
                        case "HUY CLN":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set CLN=0 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã HỦY thành công group Chất Lượng Nước");
                            break;
                        case "HUY DMA":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set DMA=0 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã HỦY thành công group DMA");
                            break;
                        case "HUY SDHN":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set sDHN=0 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã HỦY thành công group ĐHN Thông Minh");
                            break;
                        case "DK DHNDT":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set DHNDT=1 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã ĐĂNG KÝ thành công group ĐHN điện từ");
                            break;
                        case "HUY DHNDT":
                            _cDAL_TrungTam.ExecuteNonQuery("update Zalo_QuanTam set DHNDT=0 where IDZalo=" + IDZalo);
                            sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nBạn đã HỦY thành công group ĐHN điện từ");
                            break;
                        default:
                            DateTime date = DateTime.Now;
                            if (date.Date.DayOfWeek == DayOfWeek.Saturday || date.Date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nXin cám ơn Quý khách đã liên hệ Công ty Cổ phần Cấp nước Tân Hòa. Hiện đã hết giờ làm việc xin Quý khách liên hệ lại vào giờ hành chính (từ thứ hai đến thứ sáu). Trường hợp Quý khách báo sự cố cung cấp nước vui lòng liên hệ Tổng đài 19006489. Trân trọng cảm ơn!");
                            }
                            else
                            if ((date.Hour == 17 && date.Minute > 0)
                            || date.Hour > 17
                            || date.Hour < 7
                            || (date.Hour == 7 && date.Minute < 30))
                            {
                                strResponse = sendMessage(IDZalo, "Hệ thống trả lời tự động\n\nXin cám ơn Quý khách đã liên hệ Công ty Cổ phần Cấp nước Tân Hòa. Hiện đã hết giờ làm việc xin Quý khách liên hệ lại vào giờ hành chính (từ thứ hai đến thứ sáu). Trường hợp Quý khách báo sự cố cung cấp nước vui lòng liên hệ Tổng đài 19006489. Trân trọng cảm ơn!");
                            }
                            break;
                    }
            }
            catch (Exception ex)
            {
                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,Loai,NoiDung,Result)values(" + IDZalo + ",N'CSNsDHN',N'',N'" + ex.Message + "')");
                throw ex;
            }
        }

        /// <summary>
        /// Gửi tin nhắn
        /// </summary>
        /// <param name="IDZalo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string sendMessage(string IDZalo, string message)
        {
            string strResponse = "";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["access_token"] = getAccess_token();

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
                //            + "\"user_id\":\""+ IDZalo + "\""
                //            + "},"
                //            + "\"message\":{"
                //            + "\"text\":\""+ message + "\""
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
                    jsonReult deserializedResult = CGlobalVariable.jsSerializer.Deserialize<jsonReult>(read.ReadToEnd());
                    strResponse = deserializedResult.error + " : " + deserializedResult.message;
                    read.Close();
                    respuesta.Close();
                }
                else
                {
                    strResponse = "Error: " + respuesta.StatusCode;
                }
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
        private string sendMessageDangKy(string IDZalo)
        {
            string strResponse = "";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["access_token"] = getAccess_token();

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
                            + "\"subtitle\":\"Để sử dụng dịch vụ tra cứu tự động, Quý Khách Hàng cần phải đăng ký ít nhất một mã khách hàng (Danh Bộ)\","
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
                    jsonReult deserializedResult = CGlobalVariable.jsSerializer.Deserialize<jsonReult>(read.ReadToEnd());
                    strResponse = deserializedResult.message;
                    read.Close();
                    respuesta.Close();
                }
                else
                {
                    strResponse = "Error: " + respuesta.StatusCode;
                }
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
        private string sendMessageCupNuoc(string IDZalo, string message)
        {
            string strResponse = "";
            try
            {
                string url = "https://openapi.zalo.me/v2.0/oa/message";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["access_token"] = getAccess_token();

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
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end,GiaBieu=GB,DinhMuc=DM,DinhMucHN,MLT=MALOTRINH from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
            if (dt.Rows.Count == 0)
                dt = _cDAL_DHN.ExecuteQuery_DataTable("select DanhBo=DANHBO,HoTen=HOTEN,DiaChi=(SONHA+' '+TENDUONG),MLT=LOTRINH,GiaBieu,DinhMuc,DinhMucHN from TB_DULIEUKHACHHANG where DANHBO='" + DanhBo + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                return "Hệ thống trả lời tự động\n\n"
                        + "Danh Bộ: " + dt.Rows[0]["DanhBo"].ToString() + "\n"
                        + "Họ tên: " + dt.Rows[0]["HoTen"].ToString() + "\n"
                        + "Địa chỉ: " + dt.Rows[0]["DiaChi"].ToString() + "\n"
                        + "Giá biểu: " + dt.Rows[0]["GiaBieu"].ToString() + "\n"
                        + "Định mức: " + dt.Rows[0]["DinhMuc"].ToString() + "    Định mức HN: " + dt.Rows[0]["DinhMucHN"].ToString() + "\n"
                        + getCSC_CSM_MoiNhat(dt.Rows[0]["DanhBo"].ToString()) + "\n\n";
            }
            else
                return "";
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

        /// <summary>
        /// Gửi tin nhắn thông báo đến ngày ghi chỉ số nước
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("ThongBaoNgayGhiChiSoNuoc")]
        [HttpGet]
        public string ThongBaoNgayGhiChiSoNuoc(string checksum)
        {
            string DanhBo = "", strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select a.Nam, a.Ky, NgayDoc = CONVERT(varchar(10), NgayDoc, 103), z.IDZalo, ttkh.DanhBo, ttkh.HoTen, DiaChi = SONHA + ' ' + TENDUONG,DienThoai = (select DienThoai from [DocSoTH].[dbo].[NguoiDung]"
                                + " where ActiveMobile=1 and May = SUBSTRING(ttkh.LOTRINH, 3, 2))"
                                + " from Lich_DocSo a, Lich_DocSo_ChiTiet b, Lich_Dot c, [TRUNGTAMKHACHHANG].[dbo].Zalo_DangKy z, [TRUNGTAMKHACHHANG].[dbo].Zalo_QuanTam zq, [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
                                + " where a.ID = b.IDDocSo and c.ID = b.IDDot and z.DanhBo=ttkh.DanhBo"
                                + " and CAST(DATEADD(DAY, -1, NgayDoc) as date)=CAST(GETDATE() as date)"
                                + " and((TB1_From <= ttkh.LOTRINH and ttkh.LOTRINH <= TB1_To)or(TB2_From <= ttkh.LOTRINH and ttkh.LOTRINH <= TB2_To)or(TP1_From <= ttkh.LOTRINH and ttkh.LOTRINH <= TP1_To)or(TP2_From <= ttkh.LOTRINH and ttkh.LOTRINH <= TP2_To))"
                                + " and z.IDZalo=zq.IDZalo and zq.Follow= 1 and ttkh.ViTriDHN_Ngoai=0 order by ttkh.DANHBO asc";
                    DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
                    string message = "";
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            DanhBo = item["DanhBo"].ToString();
                            DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end,GiaBieu=GB,DinhMuc=DM,DinhMucHN,MLT=MALOTRINH from HOADON where DANHBA='" + item["DanhBo"] + "' order by ID_HOADON desc");
                            message = "Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng thông báo đến Quý khách hàng: " + item["HoTen"]
                                        + "\nĐịa chỉ: " + dt_ThongTin.Rows[0]["DiaChi"].ToString()
                                        + "\nDanh bộ: " + item["DanhBo"].ToString()
                                        + "\n\nKỳ " + item["Ky"] + "/" + item["Nam"] + " sẽ được ghi chỉ số vào ngày " + item["NgayDoc"]
                                        + "\nTrường hợp Quý khách đi vắng (đồng hồ nước nằm trong bất động sản), Quý khách có thể cung cấp chỉ số nước qua tin nhắn Zalo OA hoặc Tổng đài: 1900.6489"
                                        + "\nTrường hợp Quý khách không thể cung cấp chỉ số thì Công ty sẽ tạm tính tiêu thụ bằng trung bình 03 kỳ hóa đơn gần nhất của Quý khách. Trân trọng!"
                                        + "\n\n***Truy cập website để cung cấp chỉ số nước: https://service.cskhtanhoa.com.vn/QLDHN/BaoChiSoNuoc?function=KiemTra&DanhBo=" + item["DanhBo"]
                                        + "\n***Chỉ số nước là dãy số MÀU ĐEN NỀN TRẮNG trên đồng hồ nước"
                                        + "\nHoặc Quý khách có thể gửi chụp hình đồng hồ nước kèm theo Danh bộ và Địa chỉ cho Zalo: " + item["DienThoai"];
                            strResponse = sendMessage(item["IDZalo"].ToString(), message);
                        }
                        catch (Exception ex)
                        {
                            strResponse = ex.Message;
                        }
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"] + ",N'" + item["DanhBo"] + "',N'ghichisonuoc',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(0,'" + DanhBo + "',N'ghichisonuoc',N'',N'" + strResponse + "')");
            }
            return strResponse;
        }

        /// <summary>
        /// Gửi tin nhắn thông báo phát hành hóa đơn
        /// </summary>
        /// <returns></returns>
        [Route("ThongBaoPhatHanhHoaDon")]
        [HttpGet]
        public string ThongBaoPhatHanhHoaDon(string checksum)
        {
            string DanhBo = "", strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    //string sql = "select DanhBo='13011030051',HoTen='VO DAI PHAT',DiaChi='18 (763) LY THUONG KIET'"
                    //    + " ,Nam=2021,Ky=8,CSC=0,CSM=9,TuNgay='18/06/2021',DenNgay='19/07/2021',TieuThu=9,TongCong=65205,IDZalo='4276209776391262580'";
                    string sql = "select DanhBo = DANHBA,HoTen = TENKH,DiaChi =case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end, NAM, KY"
                            + " , CSC = CSCU, CSM = CSMOI, TUNGAY = CONVERT(varchar(10), TUNGAY, 103), DENNGAY = CONVERT(varchar(10), DENNGAY, 103), TIEUTHU, TONGCONG, CODE,z.IDZalo"
                            + " from HOADON hd, [TRUNGTAMKHACHHANG].[dbo].[Zalo_DangKy] z, [TRUNGTAMKHACHHANG].[dbo].[Zalo_QuanTam] zq"
                            + " where CAST(hd.CreateDate as date)=CAST(getdate() as date) and hd.DANHBA=z.DanhBo"
                            + " and zq.Follow=1 and z.IDZalo=zq.IDZalo and not exists(select * from TT_ChanThuHo where Nam=hd.NAM and Ky=hd.KY and Dot=hd.DOT)  order by hd.DANHBA asc";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    string message = "", hdTon;
                    DataTable dtTon;
                    decimal TongCongNo;
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            DanhBo = item["DanhBo"].ToString();
                            message = "Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng thông báo đến Quý khách hàng: " + item["HoTen"]
                                    + "\nĐịa chỉ: " + item["DiaChi"]
                                    + "\nDanh bộ: " + item["DanhBo"]
                                    + "\n\nThông báo tiền nước kỳ " + item["Ky"] + "/" + item["Nam"] + " từ ngày " + item["TuNgay"] + " đến ngày " + item["DenNgay"];
                            if (item["Code"].ToString() != "F" && item["Code"].ToString() != "6")
                            {
                                message += ""
                                //+ "\nChỉ số cũ: " + item["CSC"]
                                //+ "\nChỉ số mới: " + item["CSM"]
                                + "\n  + Lượng nước tiêu thụ: " + item["TieuThu"] + " m3";
                            }
                            else
                                message += "\n  + Lượng nước tiêu thụ tạm tính: " + item["TieuThu"];

                            dtTon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                            TongCongNo = 0;
                            hdTon = "";
                            foreach (DataRow itemTon in dtTon.Rows)
                                if (int.Parse(itemTon["Nam"].ToString()) < int.Parse(item["Nam"].ToString()) || (int.Parse(itemTon["Nam"].ToString()) == int.Parse(item["Nam"].ToString()) && int.Parse(itemTon["Ky"].ToString()) < int.Parse(item["Ky"].ToString())))
                                {
                                    hdTon += "\n  - Kỳ " + itemTon["Ky"] + "/" + itemTon["Nam"] + " : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", decimal.Parse(itemTon["TongCong"].ToString())) + " đồng";
                                    TongCongNo += decimal.Parse(itemTon["TongCong"].ToString());
                                }
                            message += "\n  + Số tiền nước: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["TongCong"].ToString())) + " đồng";
                            if (TongCongNo > 0)
                            {
                                message += "\nTổng số tiền nước còn nợ: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCongNo) + " đồng"
                                          + hdTon;
                                message += "\nTổng số tiền phải thanh toán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCongNo + int.Parse(item["TongCong"].ToString())) + " đồng"
                                    + "\n\nGhi chú: Quý khách thanh toán gấp tiền nước, do nợ quá hạn. Nếu đã thanh toán vui lòng bỏ qua thông báo này.";
                            }
                            else
                            {
                                message += "\nTổng số tiền phải thanh toán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCongNo + int.Parse(item["TongCong"].ToString())) + " đồng"
                                    + "\n\nGhi chú: Quý khách thanh toán trước ngày " + DateTime.Now.AddDays(3).ToString("dd/MM/yyyy") + ". Nếu đã thanh toán vui lòng bỏ qua thông báo này.";
                            }
                            message += getThongTinThanhToan();
                            strResponse = sendMessage(item["IDZalo"].ToString(), message);
                        }
                        catch (Exception ex)
                        {
                            strResponse = ex.Message;
                        }
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"].ToString() + ",N'" + item["DanhBo"] + "',N'phathanhhoadon',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(0,'" + DanhBo + "',N'phathanhhoadon',N'',N'" + strResponse + "')");
            }
            return strResponse;
        }

        /// <summary>
        /// Gửi tin nhắn thông báo nhắc nợ
        /// </summary>
        /// <returns></returns>
        [Route("ThongBaoNhacNo")]
        [HttpGet]
        public string ThongBaoNhacNo(string checksum)
        {
            string DanhBo = "", strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select DanhBo = DANHBA,HoTen = TENKH,DiaChi =case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end, NAM, KY"
                            + " , CSC = CSCU, CSM = CSMOI, TUNGAY = CONVERT(varchar(10), TUNGAY, 103), DENNGAY = CONVERT(varchar(10), DENNGAY, 103), TIEUTHU, TONGCONG,z.IDZalo"
                            + " from HOADON hd, [TRUNGTAMKHACHHANG].[dbo].[Zalo_DangKy] z, [TRUNGTAMKHACHHANG].[dbo].[Zalo_QuanTam] zq"
                            + " where hd.NGAYGIAITRACH is null and CAST(DATEADD(DAY, +7, hd.CreateDate) as date)=CAST(GETDATE() as date) and hd.DANHBA=z.DanhBo"
                            + " and zq.Follow=1 and z.IDZalo=zq.IDZalo order by hd.DANHBA asc";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    string message = "", hdTon;
                    DataTable dtTon;
                    decimal TongCong;
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            dtTon = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + item["DanhBo"].ToString() + ")");
                            TongCong = 0;
                            hdTon = "";
                            foreach (DataRow itemTon in dtTon.Rows)
                            {
                                hdTon += "\n  - Kỳ " + itemTon["Ky"] + "/" + itemTon["Nam"] + " : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", decimal.Parse(itemTon["TongCong"].ToString())) + " đồng";
                                TongCong += decimal.Parse(itemTon["TongCong"].ToString());
                            }
                            if (TongCong > 0)
                            {
                                DanhBo = item["DanhBo"].ToString();
                                message = "Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng thông báo đến Quý khách hàng: " + item["HoTen"]
                                            + "\nĐịa chỉ: " + item["DiaChi"]
                                            + "\nDanh bộ: " + item["DanhBo"]
                                            + "\n\nTổng số hóa đơn phải thanh toán: " + dtTon.Rows.Count + " hóa đơn"
                                            + hdTon
                                            + "\nTổng số tiền phải thanh toán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCong) + " đồng"
                                            + "\n\nGhi chú: Quý khách thanh toán trước ngày " + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy") + ". Nếu đã thanh toán vui lòng bỏ qua thông báo này."
                                            + getThongTinThanhToan();
                                strResponse = sendMessage(item["IDZalo"].ToString(), message);
                            }
                        }
                        catch (Exception ex)
                        {
                            strResponse = ex.Message;
                        }
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"] + ",N'" + item["DanhBo"] + "',N'nhacno',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values('','" + DanhBo + "',N'nhacno',N'',N'" + strResponse + "')");
            }
            return strResponse;
        }

        /// <summary>
        /// Gửi tin nhắn thông báo đóng nước khi nhân viên lập lệnh
        /// </summary>
        /// <param name="MaDN"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("ThongBaoDongNuoc")]
        [HttpGet]
        public string ThongBaoDongNuoc(string MaDN, string checksum)
        {
            string DanhBo = "", strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string message = "", hdTon = "";
                    decimal TongCong = 0;
                    string sql = "select distinct DanhBo = DANHBA,z.IDZalo"
                            + " from HOADON hd,TT_CTDongNuoc ctdn, [TRUNGTAMKHACHHANG].[dbo].[Zalo_DangKy] z, [TRUNGTAMKHACHHANG].[dbo].[Zalo_QuanTam] zq"
                            + " where ctdn.MaDN=" + MaDN + " and hd.ID_HOADON=ctdn.MaHD and hd.DANHBA=z.DanhBo"
                            + " and zq.Follow=1 and z.IDZalo=zq.IDZalo order by hd.DANHBA asc";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    DataTable dtTon = _cDAL_ThuTien.ExecuteQuery_DataTable("select DanhBo = DANHBA, HoTen = TENKH, DiaChi =case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end, NAM, hd.KY"
                            + " , CSC = CSCU, CSM = CSMOI, TUNGAY = CONVERT(varchar(10), TUNGAY, 103), DENNGAY = CONVERT(varchar(10), DENNGAY, 103), hd.TIEUTHU, hd.TONGCONG"
                            + " from HOADON hd,TT_CTDongNuoc ctdn where ctdn.MaDN=" + MaDN + " and hd.ID_HOADON=ctdn.MaHD order by hd.ID_HOADON asc");
                    foreach (DataRow itemTon in dtTon.Rows)
                    {
                        hdTon += "\n  - Kỳ " + itemTon["Ky"] + "/" + itemTon["Nam"] + " : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", decimal.Parse(itemTon["TongCong"].ToString())) + " đồng";
                        TongCong += decimal.Parse(itemTon["TongCong"].ToString());
                    }
                    if (TongCong > 0)
                    {
                        DanhBo = dtTon.Rows[0]["DanhBo"].ToString();
                        message = "Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng thông báo đến Quý khách hàng: " + dtTon.Rows[0]["HoTen"]
                                            + "\nĐịa chỉ: " + dtTon.Rows[0]["DiaChi"]
                                            + "\nDanh bộ: " + dtTon.Rows[0]["DanhBo"]
                                            + "\n\nThông báo tạm ngưng cung cấp nước"
                                            + "\nCông ty sẽ tạm ngưng cung cấp nước tại địa chỉ trên kể từ ngày " + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy")
                                            + "\nLý do: Quý khách chưa thanh toán tiền nước"
                                            + hdTon
                                            + "\nTổng số tiền phải thanh toán: " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCong) + " đồng"
                                            + "\n\nGhi chú: Quý khách thanh toán trước ngày " + DateTime.Now.AddDays(1).ToString("dd/MM/yyyy") + ". Nếu đã thanh toán vui lòng bỏ qua thông báo này."
                                            + getThongTinThanhToan();
                    }
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            if (TongCong > 0)
                            {
                                strResponse = sendMessage(item["IDZalo"].ToString(), message);
                            }
                        }
                        catch (Exception ex)
                        {
                            strResponse = ex.Message;
                        }
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"] + ",N'" + dtTon.Rows[0]["DanhBo"] + "',N'dongnuoc',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(0,'" + DanhBo + "',N'dongnuoc',N'',N'" + strResponse + "')");
            }
            return strResponse;
        }

        private string getThongTinThanhToan_Old()
        {
            string str = "\n  - Ví điện tử: Payoo, Momo, VNpay, Shopeepay, Zalopay, Viettelpay,..."
                    + "\n  - Các ngân hàng: Agribank, MB bank, Vietcombank, ACB, Đông Á,..."
                    + "\n  - Hoặc chuyển khoản cho Công ty Cổ phần Cấp nước Tân Hòa theo số tài khoản 6220 4311 01100092 tại Ngân hàng Nông nghiệp và Phát triển nông thôn Việt Nam-Chi nhánh Chợ Lớn(AGR), số tài khoản 046 100 057 3975 tại Ngân hàng TMCP Ngoại thương Việt Nam-Chi nhánh Tân Bình Dương(VCB), số tài khoản 201 110 067 9999 tại Ngân hàng TMCP Quân đội-Chi nhánh Bắc Sài Gòn(MB),..."
                    + "\nKhi thanh toán tiền nước, Quý khách cần ghi rõ số danh bộ, kỳ hóa đơn, địa chỉ và số điện thoại."
                    + "\nNếu đã thanh toán vui lòng bỏ qua thông báo này."
                    + "\n\nĐể được phục vụ và hỗ trợ thêm thông tin, vui lòng liên hệ tổng đài 1900.6489 hoặc website: https://cskhtanhoa.com.vn"
                    + "\nTrân trọng!";
            return str;
        }

        private string getThongTinThanhToan()
        {
            string str = "\nCác kênh thanh toán gồm: ví điện tử (Payoo,Momo,Viettelpay,...), các ngân hàng (Agribank, Vietcombank, MB Bank,...)"
                    + "\n- Hoặc chuyển khoản cho Công ty Cổ phần Cấp nước Tân Hòa theo số tài khoản 6220 4311 01100092 tại Ngân hàng Nông nghiệp và Phát triển nông thôn Việt Nam-Chi nhánh Chợ Lớn(AGR), số tài khoản 046 100 057 3975 tại Ngân hàng TMCP Ngoại thương Việt Nam-Chi nhánh Tân Bình Dương(VCB), số tài khoản 201 110 067 9999 tại Ngân hàng TMCP Quân đội-Chi nhánh Bắc Sài Gòn(MB),... Khi thanh toán tiền nước, Quý khách ghi rõ số danh bộ, kỳ hóa đơn, địa chỉ và số điện thoại."
                    + "\nTrân trọng!";
            return str;
        }

        /// <summary>
        /// Gửi tin nhắn thông báo đã thanh toán hóa đơn
        /// </summary>
        /// <param name="MaHD"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("ThongBaoDaThanhToanHoaDon")]
        [HttpGet]
        public string ThongBaoDaThanhToanHoaDon(int MaHD, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select DanhBo = DANHBA,HoTen = TENKH,DiaChi =case when SO is null then DUONG else case when DUONG is null then SO else SO + ' ' + DUONG end end, NAM, KY"
                            + " , CSC = CSCU, CSM = CSMOI, TUNGAY = CONVERT(varchar(10), TUNGAY, 103), DENNGAY = CONVERT(varchar(10), DENNGAY, 103), TIEUTHU, TONGCONG, CODE,z.IDZalo"
                            + " from HOADON hd, [TRUNGTAMKHACHHANG].[dbo].[Zalo_DangKy] z, [TRUNGTAMKHACHHANG].[dbo].[Zalo_QuanTam] zq"
                            + " where hd.ID_HOADON=" + MaHD + " and hd.DANHBA=z.DanhBo"
                            + " and zq.Follow=1 and z.IDZalo=zq.IDZalo";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    string message;
                    foreach (DataRow item in dt.Rows)
                    {
                        message = "Công ty Cổ phần Cấp nước Tân Hòa xin trân trọng thông báo đến Quý khách hàng: " + item["HoTen"]
                                    + "\nĐịa chỉ: " + item["DiaChi"]
                                    + "\nDanh bộ: " + item["DanhBo"]
                                    + "\n\nCảm ơn Quý khách đã thanh toán hóa đơn tiền nước kỳ " + item["Ky"] + "/" + item["Nam"]
                                    + ", với số tiền " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["TongCong"].ToString())) + " đồng"
                                    + "\n\nĐể được phục vụ và hỗ trợ thêm thông tin, vui lòng liên hệ tổng đài 1900.6489 hoặc website: https://cskhtanhoa.com.vn"
                                    + "\nTrân trọng!";
                        strResponse = sendMessage(item["IDZalo"].ToString(), message);
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"].ToString() + ",N'" + item["DanhBo"] + "',N'thanhtoanhoadon',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values('',N'',N'thanhtoanhoadon',N'',N'" + strResponse + "')");
            return strResponse;
        }

        [Route("resendMessageDangKy")]
        [HttpGet]
        public string resendMessageDangKy(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select * from Zalo_QuanTam where IDZalo not in (select IDZalo from Zalo_DangKy) and Follow=1";
                    DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable(sql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string result = sendMessageDangKy(item["IDZalo"].ToString());
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"] + ",N'',N'resendMessageDangKy',N'',N'" + result + "')");
                    }
                    strResponse = "Đã xử lý";
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

        [Route("ThongBaoDangKyCCCD")]
        [HttpGet]
        public string ThongBaoDangKyCCCD(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select b.DanhBo,b.IDZalo from TRUNGTAMKHACHHANG.dbo.Zalo_QuanTam a, TRUNGTAMKHACHHANG.dbo.Zalo_DangKy b, HOADON_TA.dbo.HOADON hd where a.IDZalo = b.IDZalo and a.Follow = 1"
                                 + " and DanhBo not in (select t1.DanhBo from (select distinct DanhBo from KTKS_DonKH.dbo.ChungTu_ChiTiet where MaLCT = 15 and cat = 0) t1,HOADON_TA.dbo.HOADON hd"
                                 + " where hd.NAM = 2023 and hd.ky = 10 and t1.DanhBo = hd.DANHBA and hd.DM >= 40)"
                                 + " and hd.NAM = 2023 and hd.ky = 10 and b.DanhBo = hd.DANHBA and hd.DM >= 40"
                                 + " and DANHBA not in (select distinct DanhBo from TRUNGTAMKHACHHANG.dbo.Zalo_Send where Loai like '%cccd%' and DanhBo in (select distinct ct.danhbo from KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet ct"
                                 + " where dt.MaDon = ct.MaDon  and CAST(ct.CreateDate as date) >= '20231012' and Name_NhomDon like N'%định mức%'))";
                    //string sql = "select IDZalo='" + IDZalo + "'";
                    DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable(sql);
                    string message;
                    foreach (DataRow item in dt.Rows)
                    {
                        message = "    Công ty Cổ phần Cấp nước Tân Hòa (Công ty) trân trọng thông báo đến Quý khách hàng việc cấp định mức nước theo số định danh cá nhân."
                                    + "\n    Kính đề nghị Quý khách hàng khi nhận được thông báo này, khẩn trương liên hệ Công ty qua số điện thoại: 1900 6489 để được hướng dẫn."
                                    + "\n    Trường hợp hết ngày 11/11/2023, khách hàng vẫn không liên hệ, Công ty buộc lòng điều chỉnh định mức nước = 0m3/tháng."
                                    + "\n    Trân trọng kính báo./.";
                        strResponse = sendMessage(item["IDZalo"].ToString(), message);
                        _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"].ToString() + ",N'" + item["DanhBo"].ToString() + "',N'thongbaocccd',N'" + message + "',N'" + strResponse + "')");
                    }
                    strResponse = "Đã xử lý";
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values('0',N'',N'thongbaocccd',N'',N'" + strResponse + "')");
            return strResponse;
        }

        public class jsonReult
        {
            public string error { get; set; }
            public string message { get; set; }
        }

        /// <summary>
        /// update danh sách quan tâm
        /// </summary>
        /// <param name="checksum"></param>
        /// <param name="number">số lượng quan tâm</param>
        /// <returns></returns>
        [Route("updateDSQuanTam")]
        [HttpGet]
        public string updateDSQuanTam(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    int index = 0, number = 0;
                    string url = "https://openapi.zalo.me/v2.0/oa/getfollowers?data={\"offset\":0,\"count\":1}";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers["access_token"] = getAccess_token();

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        var jsonContent = JObject.Parse(read.ReadToEnd());
                        number = int.Parse(jsonContent["data"]["total"].ToString());

                        read.Close();
                        respuesta.Close();
                    }

                    while (index < number)
                    {
                        url = "https://openapi.zalo.me/v2.0/oa/getfollowers?data={\"offset\":" + index + ",\"count\":50}";
                        request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "GET";
                        request.ContentType = "application/json";
                        request.Headers["access_token"] = getAccess_token();

                        respuesta = (HttpWebResponse)request.GetResponse();
                        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader read = new StreamReader(respuesta.GetResponseStream());
                            var jsonContent = JObject.Parse(read.ReadToEnd());
                            for (int i = 0; i < jsonContent["data"]["followers"].Count(); i++)
                            {
                                string sql = "if not exists(select * from Zalo_QuanTam where IDZalo=" + jsonContent["data"]["followers"][i]["user_id"] + ")"
                                + " insert into Zalo_QuanTam(IDZalo)values(" + jsonContent["data"]["followers"][i]["user_id"] + ")"
                                + " else update Zalo_QuanTam set Follow=1 where IDZalo=" + jsonContent["data"]["followers"][i]["user_id"];
                                _cDAL_TrungTam.ExecuteNonQuery(sql);
                            }
                            read.Close();
                            respuesta.Close();
                            strResponse = "Đã xử lý";
                        }
                        else
                        {
                            strResponse = "Error: " + respuesta.StatusCode;
                        }
                        index += 50;
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

        /// <summary>
        /// update thông tin tên+avatar
        /// </summary>
        /// <param name="checksum"></param>
        /// <param name="number">1 lần chạy</param>
        /// <returns></returns>
        [Route("updateThongTinQuanTam")]
        [HttpGet]
        public string updateThongTinQuanTam(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    string sql = "select * from Zalo_QuanTam where [Name] is null or Avatar is null order by CreateDate desc";
                    DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable(sql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string url = "https://openapi.zalo.me/v2.0/oa/getprofile?data={\"user_id\":\"" + item["IDZalo"] + "\"}";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "GET";
                        request.ContentType = "application/json";
                        request.Headers["access_token"] = getAccess_token();

                        HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                        if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                        {
                            StreamReader read = new StreamReader(respuesta.GetResponseStream());
                            var jsonContent = JObject.Parse(read.ReadToEnd());
                            if (jsonContent["error"].ToString() == "0")
                            {
                                string sql2 = "update Zalo_QuanTam set Name=N'" + jsonContent["data"]["display_name"].ToString().Replace("'", "''") + "'"
                                    + ",Avatar=N'" + jsonContent["data"]["avatar"].ToString() + "' where IDZalo=" + item["IDZalo"];
                                _cDAL_TrungTam.ExecuteNonQuery(sql2);
                            }
                            else
                                if (jsonContent["error"].ToString() == "-213")//unfollow
                            {
                                string sql2 = "update Zalo_QuanTam set Follow=0,UnFollowDate=getdate() where IDZalo=" + item["IDZalo"];
                                _cDAL_TrungTam.ExecuteNonQuery(sql2);
                            }
                            read.Close();
                            respuesta.Close();
                        }
                        else
                        {
                            strResponse = "Error: " + respuesta.StatusCode;
                        }
                    }
                    strResponse = "Đã xử lý";
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

        [Route("sendCanhBaoQuanLy")]
        [HttpPost]
        public string sendCanhBaoQuanLy()
        {
            string strResponse = "";
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string Loai = jsonContent["Loai"].ToString(), NoiDung = jsonContent["NoiDung"].ToString(), checksum = jsonContent["checksum"].ToString();
                    if (checksum == CGlobalVariable.checksum)
                    {
                        DataTable dt = new DataTable();
                        string str = Loai.ToUpper();
                        if (str == "CLN")
                            dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo from Zalo_QuanTam where CLN=1");
                        else
                            if (str == "DMA")
                            dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo from Zalo_QuanTam where DMA=1");
                        else
                                if (str == "SDHN")
                            dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo from Zalo_QuanTam where sDHN=1");
                        else
                                if (str == "DHNDT")
                        {
                            dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo from Zalo_QuanTam where DHNDT=1");
                            DataTable dtDHN = _cDAL_sDHN.ExecuteQuery_DataTable("select * from sDHN_PMAC");
                            string[] ngayxems = jsonContent["NgayXem"].ToString().Split('/');
                            MView enBT = new MView();
                            enBT.TieuDe = "Bình Thường";
                            enBT.SoLuong = "0";
                            MView enTT0 = new MView();
                            enTT0.TieuDe = "Tiêu Thụ 0m3";
                            enTT0.SoLuong = "0";
                            MView enTHY = new MView();
                            enTHY.TieuDe = "Tín Hiệu Yếu";
                            enTHY.SoLuong = "0";
                            MView enKTH = new MView();
                            enKTH.TieuDe = "Không Tín Hiệu";
                            enKTH.SoLuong = "0";
                            foreach (DataRow item in dtDHN.Rows)
                            {
                                DataTable dtCS = _cDAL_sDHN.ExecuteQuery_DataTable("  declare @date date='" + ngayxems[2] + "-" + ngayxems[1] + "-" + ngayxems[0] + "'"
                                        + " select  ThoiGian = TimeStamp, ChiSo = (select cast(t0.value - (select t2.Value FROM[SERVER14].[viwater].[dbo].[" + item["TableNameNguoc"].ToString() + "] t2 where t2.TimeStamp = t0.TimeStamp) as decimal(10, 0))),TieuThu=0"
                                        + " FROM[SERVER14].[viwater].[dbo].[" + item["TableName"].ToString() + "] t0 where CAST(TimeStamp as date) = CAST(DATEADD(DAY, -1, @date) as date) and DATEPART(HOUR, TimeStamp) = 23 and DATEPART(MINUTE, TimeStamp) = 0"
                                        + " union"
                                        + " select  ThoiGian = TimeStamp, ChiSo = (select cast(t0.value - (select t2.Value FROM[SERVER14].[viwater].[dbo].[" + item["TableNameNguoc"].ToString() + "] t2 where t2.TimeStamp = t0.TimeStamp) as decimal(10, 0))),TieuThu=0"
                                        + " FROM[SERVER14].[viwater].[dbo].[" + item["TableName"].ToString() + "] t0 where CAST(TimeStamp as date) = @date and DATEPART(MINUTE, TimeStamp) = 0");
                                for (int i = 1; i < dtCS.Rows.Count; i++)
                                {
                                    dtCS.Rows[i]["TieuThu"] = int.Parse(dtCS.Rows[i]["ChiSo"].ToString()) - int.Parse(dtCS.Rows[i - 1]["ChiSo"].ToString());
                                }

                                if (dtCS.Rows.Count == 0)
                                {
                                    enKTH.SoLuong = (int.Parse(enKTH.SoLuong) + 1).ToString();
                                    MView en = new MView();
                                    en.DanhBo = item["DanhBo"].ToString();
                                    enKTH.lst.Add(en);
                                }
                                else
                                if (dtCS.Rows.Count != 25)
                                {
                                    enTHY.SoLuong = (int.Parse(enTHY.SoLuong) + 1).ToString();
                                    MView en = new MView();
                                    en.DanhBo = item["DanhBo"].ToString();
                                    enTHY.lst.Add(en);
                                }
                                else
                                {
                                    int countTT0 = 0;
                                    for (int i = 1; i < dtCS.Rows.Count; i++)
                                    {
                                        if (int.Parse(dtCS.Rows[i]["TieuThu"].ToString()) == 0)
                                            countTT0++;
                                    }
                                    if (countTT0 == 24)
                                    {
                                        enTT0.SoLuong = (int.Parse(enTT0.SoLuong) + 1).ToString();
                                        MView en = new MView();
                                        en.DanhBo = item["DanhBo"].ToString();
                                        enTT0.lst.Add(en);
                                    }
                                    else
                                    {
                                        enBT.SoLuong = (int.Parse(enBT.SoLuong) + 1).ToString();
                                        MView en = new MView();
                                        en.DanhBo = item["DanhBo"].ToString();
                                        enBT.lst.Add(en);
                                    }
                                }
                            }
                            NoiDung = "ĐHN điện từ\r\n" + jsonContent["NgayXem"].ToString();
                            if (int.Parse(enKTH.SoLuong) != 0)
                            {
                                @NoiDung += "\n   + Có " + enKTH.SoLuong + " ĐHN không tín hiệu";
                            }
                            else
                                 if (int.Parse(enTHY.SoLuong) != 0)
                            {
                                @NoiDung += "\n   + Có " + enTHY.SoLuong + " ĐHN tín hiệu yếu";
                            }
                            else
                                 if (int.Parse(enTT0.SoLuong) != 0)
                            {
                                @NoiDung += "\n   + Có " + enTT0.SoLuong + " ĐHN tiêu thụ 0m3";
                            }
                            else
                                 if (int.Parse(enBT.SoLuong) != 0)
                            {
                                @NoiDung += "\n   + Có " + enBT.SoLuong + " ĐHN hoạt động bình thường";
                            }
                            NoiDung += "\r\nTruy cập https://service.cskhtanhoa.com.vn/QLDHN/DHNDT để biết thêm chi tiết";
                        }
                        foreach (DataRow item in dt.Rows)
                        {
                            strResponse = sendMessage(item["IDZalo"].ToString(), NoiDung);
                            _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,Loai,NoiDung,Result)values(" + item["IDZalo"].ToString() + ",N'canhbao',N'" + NoiDung + "',N'" + strResponse + "')");
                        }
                        strResponse = "Đã xử lý";
                    }
                    else
                        strResponse = "Sai checksum";
                }
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }

        [Route("sendEContract")]
        [HttpPost]
        public MResult sendEContract()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string DienThoai = jsonContent["sdt"].ToString(), NoiDung = jsonContent["message"].ToString(), checksum = jsonContent["checksum"].ToString();
                    if (checksum == CGlobalVariable.checksum)
                    {
                        DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable("select * from Zalo_EContract where DienThoai='" + DienThoai + "'");
                        foreach (DataRow item in dt.Rows)
                        {
                            string strResponse = sendMessage(item["IDZalo"].ToString(), NoiDung);
                            _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,DanhBo,Loai,NoiDung,Result)values(" + item["IDZalo"].ToString() + ",N'" + DienThoai + "',N'econtract',N'" + NoiDung + "',N'" + strResponse + "')");
                        }
                        result.success = true;
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        //gửi dự liệu chỉ số đhn >= 100ly
        [Route("sendsDHN100")]
        [HttpGet]
        public string sendsDHN100(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    DataTable dtsDHN = _cDAL_ThuTien.ExecuteQuery_DataTable("SELECT [DanhBo],[TableName],[TableNameNguoc] FROM[sDHN].[dbo].[sDHN_PMAC]");
                    foreach (DataRow item in dtsDHN.Rows)
                    {
                        DataTable dtCS = _cDAL_TrungTam.ExecuteQuery_DataTable("declare @date date"
                                        + " set @date = getdate();"
                                        + " select t1.*,TieuThu = t1.CSM - t1.CSC from"
                                        + " (select TuNgay = convert(varchar(10),DATEADD(DAY, -1, @date),103), CSC = (select cast(t0.value-(select t2.Value FROM [SERVER14].[viwater].[dbo].[" + item["TableNameNguoc"].ToString() + "] t2 where t2.TimeStamp=t0.TimeStamp) as decimal(10,0)) FROM [SERVER14].[viwater].[dbo].[" + item["TableName"].ToString() + "] t0"
                                        + " where CAST(TimeStamp as date) = DATEADD(DAY, -1, @date) and DATEPART(HOUR, TimeStamp) = 01 and DATEPART(MINUTE, TimeStamp) = 0)"
                                        + " ,DenNgay = convert(varchar(10),@date,103),CSM = (select cast(t0.value-(select t2.Value FROM [SERVER14].[viwater].[dbo].[" + item["TableNameNguoc"].ToString() + "] t2 where t2.TimeStamp=t0.TimeStamp) as decimal(10,0)) FROM [SERVER14].[viwater].[dbo].[" + item["TableName"].ToString() + "] t0"
                                        + " where CAST(TimeStamp as date)=@date and DATEPART(HOUR, TimeStamp)=01 and DATEPART(MINUTE, TimeStamp)=0))t1");

                        if (dtCS != null || dtCS.Rows.Count > 0)
                        {
                            string NoiDung = getTTKH(item["DanhBo"].ToString())
                                + "Chỉ số đồng hồ nước điện từ tính từ 01g00 ngày " + dtCS.Rows[0]["TuNgay"].ToString() + " đến 01g00 ngày " + dtCS.Rows[0]["DenNgay"].ToString() + "\n"
                                + "CSC: " + float.Parse(dtCS.Rows[0]["CSC"].ToString()).ToString("0.00") + "    CSM: " + float.Parse(dtCS.Rows[0]["CSM"].ToString()).ToString("0.00") + "    TT: " + float.Parse(dtCS.Rows[0]["TieuThu"].ToString()).ToString("0.00") + " m3";
                            DataTable dtZalo = _cDAL_TrungTam.ExecuteQuery_DataTable("select a.IDZalo from Zalo_QuanTam a,Zalo_DangKy b where a.IDZalo=b.IDZalo and Follow=1 and DanhBo='" + item["DanhBo"].ToString() + "'");
                            foreach (DataRow itemZalo in dtZalo.Rows)
                            {
                                strResponse = sendMessage(itemZalo["IDZalo"].ToString(), NoiDung);
                                _cDAL_TrungTam.ExecuteNonQuery("insert into Zalo_Send(IDZalo,Loai,NoiDung,Result)values(" + itemZalo["IDZalo"].ToString() + ",N'sendsDHN100',N'" + NoiDung + "',N'" + strResponse + "')");
                            }
                        }
                    }
                    strResponse = "Đã xử lý";
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
