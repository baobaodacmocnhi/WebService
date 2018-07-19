using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Transactions;

namespace WSSmartPhone
{
    class CThuTien
    {
        CConnection _cDAL = new CConnection("Data Source=192.168.90.9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=P@ssW012d9");

        public string DataTableToJSON(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        public string GetVersion()
        {
            return _cDAL.ExecuteQuery_ReturnOneValue("select Version from TT_DeviceConfig").ToString();
        }

        public string DangNhap(string Username, string Password, string UID)
        {
            try
            {
                string UID_old = _cDAL.ExecuteQuery_ReturnOneValue("select UID from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0").ToString();
                if (String.IsNullOrEmpty(UID_old) != true && UID_old != "NULL")
                {
                    string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0").ToString();
                    SendNotificationToClient("Thông Báo", "Tài khoản của bạn đã được đăng nhập ở máy khác, Bạn bị đăng xuất tại thiết bị này", MaNV, "DangXuat", "", "", "");
                }
                _cDAL.ExecuteQuery_DataTable("update TT_NguoiDung set UID='" + UID + "' where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                return DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select * from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0"));
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public string DangNhaps(string Username, string Password, string UID)
        {
            try
            {
                string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0").ToString();

                int MaNV_UID_Old = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where UID='" + UID + "'");
                if (MaNV_UID_Old > 0)
                    _cDAL.ExecuteQuery_DataTable("delete TT_DeviceSigned where UID='" + UID + "'");

                int MaNV_UID = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                if (MaNV_UID == 0)
                    _cDAL.ExecuteQuery_DataTable("insert TT_DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");

                _cDAL.ExecuteQuery_DataTable("update TT_NguoiDung set UID='" + UID + "' where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");

                return DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Doi,ToTruong,MaTo from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0"));
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public bool DangXuat(string Username)
        {
            try
            {
                return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DangXuats(string Username, string UID)
        {
            try
            {
                string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                _cDAL.ExecuteQuery_DataTable("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateUID(string MaNV, string UID)
        {
            return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "' where MaND=" + MaNV);
        }

        public string GetDSTo()
        {
            string sql = "select MaTo,TenTo,HanhThu from TT_To";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSNhanVien()
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo from TT_NguoiDung where MaND!=0 and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSNhanVien(string MaTo)
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo from TT_NguoiDung where MaND!=0 and MaTo=" + MaTo + " and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDonTon(string MaNV, DateTime NgayDi)
        {
            //string sql = "select ID=ID_HOADON,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
            //            + " NgayGiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
            //            + " DichVuThu=case when exists(select SOHOADON from TT_DichVuThu where SOHOADON=hd.SOHOADON) then 'true' else 'false' end,"
            //            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
            //            + " from TT_GiaoHDDienThoai ghd,HOADON hd where MaNV=" + MaNV + " and NgayDi='" + NgayDi.ToString("yyyy-MM-dd") + "' and ghd.MaHD=hd.ID_HOADON"
            //            + " order by MALOTRINH";
            string sql = "select * from"
                            + " (select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                            + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                            + " from HOADON hd where NgayGiaiTrach is null and MaNV_HanhThu=" + MaNV
                            + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0))t1"
                        + " where t1.ID not in (select MaHD from TT_LenhHuy)"
                        + " order by t1.MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDonTon(string MaNV, string FromDot, string ToDot)
        {
            string sql = "select * from"
                          + " (select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                          + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                          + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                          + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                          + " from HOADON hd where NgayGiaiTrach is null and MaNV_HanhThu=" + MaNV + " and DOT>=" + FromDot + " and DOT<=" + ToDot
                          + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0))t1"
                      + " where t1.ID not in (select MaHD from TT_LenhHuy)"
                      + " order by t1.MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDonTon(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select * from"
                          + " (select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                          + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                          + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                          + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                          + " from HOADON hd where NgayGiaiTrach is null and (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and MaNV_HanhThu=" + MaNV + " and DOT>=" + FromDot + " and DOT<=" + ToDot
                          + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0))t1"
                      + " where t1.ID not in (select MaHD from TT_LenhHuy)"
                      + " order by t1.MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public bool CheckConnection_Firebase()
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            //myRequest.Timeout = 5000;
            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response.Close();
                return true;
            }
            else
            {
                response.Close();
                return false;
            }
        }

        public string SendNotificationToClient_Old(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            //if (CheckConnection_Firebase() == false)
            //{
            //    return "Không có kết nối Internet";
            //}
            string responseMess = "";
            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4

                string serverKey = "AAAAYRLMnTg:APA91bH00qfWWWjIilUlB6gcazcdSUyXnU_SnsSpt8X141z4Kcboqw_qjIpsORxtaOAAGzz-RL-biPz-280wWQhJQu_Pq9JH9hCFfCgF2LNzLakEWA381KWlhoV1zsmG7z3kECf_ePdt"; // Something very long
                string senderId = "416927227192";
                //string deviceId = (string)_DAL.ExecuteQuery_ReturnOneValue("select UID from TT_NguoiDung where MaND=" + MaNV); // Also something very long, 
                string deviceId = UID;
                // got from android
                //string deviceId = "//topics/all";             // Use this to notify all devices, 
                // but App must be subscribed to 
                // topic notification

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                HttpWebResponse response = (HttpWebResponse)tRequest.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return "Không có kết nối Host";
                }
                tRequest.Method = "post";
                tRequest.ContentType = "application/json;charset=UTF-8";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        title = Title,
                        body = Content,
                        Action = Action,
                        NameUpdate = NameUpdate,
                        ValueUpdate = ValueUpdate,
                        ID = ID,
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = (long)byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                responseMess = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                responseMess = ex.Message;
            }
            return responseMess;
        }

        public string SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            string responseMess = "";
            try
            {
                string serverKey = "AAAAYRLMnTg:APA91bH00qfWWWjIilUlB6gcazcdSUyXnU_SnsSpt8X141z4Kcboqw_qjIpsORxtaOAAGzz-RL-biPz-280wWQhJQu_Pq9JH9hCFfCgF2LNzLakEWA381KWlhoV1zsmG7z3kECf_ePdt";
                string senderId = "416927227192";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                request.Method = "POST";
                request.Headers.Add("Authorization", "key=" + serverKey);
                request.Headers.Add("Sender", "id=" + senderId);
                request.ContentType = "application/json";

                var data = new
                {
                    to = UID,
                    data = new
                    {
                        title = Title,
                        body = Content,
                        Action = Action,
                        NameUpdate = NameUpdate,
                        ValueUpdate = ValueUpdate,
                        ID = ID,
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    responseMess = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                }
                else
                {
                    responseMess = "Error: " + respuesta.StatusCode;
                }
            }
            catch (Exception ex)
            {
                responseMess = ex.Message;
            }
            return responseMess;
        }

        public string GetDSDongNuoc_old(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " kqdn.DongNuoc,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.LyDo,kqdn.MoNuoc,kqdn.NgayMN,kqdn.ChiSoMN,GiaiTrach='false',ThuHo='false',TamThu='false',HoaDon=''"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " order by dn.MLT";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(query);
            foreach (DataRow item in dt.Rows)
            {
                DataTable dtCT = _cDAL.ExecuteQuery_DataTable("select ctdn.Ky,hd.TongCong,hd.NgayGiaiTrach from TT_CTDongNuoc ctdn,HOADON hd where MaDN=" + item["ID"].ToString() + " and ctdn.MaHD=hd.ID_HOADON");
                string str = "";
                long TongCong = 0;
                bool GiaiTrach = true;
                foreach (DataRow itemCT in dtCT.Rows)
                {
                    if (String.IsNullOrEmpty(itemCT["NgayGiaiTrach"].ToString()) == true)
                    {
                        GiaiTrach = false;
                        TongCong += int.Parse(itemCT["TongCong"].ToString());
                    }
                    else
                        if (String.IsNullOrEmpty(str) == true)
                            str += itemCT["Ky"].ToString() + " : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", long.Parse(itemCT["TongCong"].ToString()));
                        else
                            str += "\n" + itemCT["Ky"].ToString() + " : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", long.Parse(itemCT["TongCong"].ToString()));
                }
                item["GiaiTrach"] = GiaiTrach.ToString();
                item["HoaDon"] = str;
                item["HoaDon"] += "\nTổng Cộng : " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCong);
            }
            return DataTableToJSON(dt);
        }

        public string GetDSDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " kqdn.DongNuoc,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.LyDo,kqdn.MoNuoc,kqdn.NgayMN,kqdn.ChiSoMN,GiaiTrach='false',TamThu='false',ThuHo='false',LenhHuy='false',kqdn.DongPhi"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0)"
                            + " order by dn.MLT";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public string GetDSCTDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,MaHD,Ky,TongCong,"
                            + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
                            + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                            + " from TT_DongNuoc dn left join TT_CTDongNuoc ctdn on dn.MaDN=ctdn.MaDN left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0)"
                            + " order by dn.MLT,ctdn.MaHD";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public bool CheckExist_DongNuoc(string MaDN)
        {
            if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where DongNuoc=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            return true;
        }

        public bool CheckExist_DongNuoc2(string MaDN)
        {
            if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where DongNuoc2=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            return true;
        }

        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, DateTime NgayDN, string ChiSoDN, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,HinhDN,NgayDN,NgayDN_ThucTe,ChiSoDN,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                        + "(select case when (select COUNT(MaKQDN) from TT_KQDongNuoc)=0 then 1 else (select MAX(MaKQDN) from TT_KQDongNuoc)+1 end)," + MaDN + ",'" + DanhBo + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1,@HinhDN"
                        + ",'" + NgayDN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',getDate()," + ChiSoDN + ",'" + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + LyDo + "',(select top 1 PhiMoNuoc from TT_CacLoaiPhi)," + CreateBy + ",getDate())";

            SqlCommand command = new SqlCommand(sql);
            if (HinhDN == "NULL")
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

            return _cDAL.ExecuteNonQuery(command);
        }

        public bool SuaDongNuoc(string MaDN, string HinhDN, DateTime NgayDN, string ChiSoDN, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set HinhDN=@HinhDN,NgayDN='" + NgayDN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ","
                + "ChiMatSo=N'" + ChiMatSo + "',ChiKhoaGoc=N'" + ChiKhoaGoc + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate() where CAST(NgayDN_ThucTe as date)=CAST(getDate() as date) and MaDN=" + MaDN;

            SqlCommand command = new SqlCommand(sql);
            if (HinhDN == "NULL")
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

            return _cDAL.ExecuteNonQuery(command);
        }

        public bool ThemDongNuoc2(string MaDN, string HinhDN, DateTime NgayDN, string ChiSoDN, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set DongNuoc2=1,PhiMoNuoc=(select top 1 PhiMoNuoc from TT_CacLoaiPhi)*2,HinhDN1=HinhDN,NgayDN1=NgayDN,NgayDN1_ThucTe=NgayDN_ThucTe,ChiSoDN1=ChiSoDN,"
                        + "HinhDN=@HinhDN,NgayDN='" + NgayDN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate(),"
                        + "SoPhieuDN1=SoPhieuDN,NgaySoPhieuDN1=NgaySoPhieuDN,ChuyenDN1=ChuyenDN,NgayChuyenDN1=NgayChuyenDN,SoPhieuDN=NULL,NgaySoPhieuDN=NULL,ChuyenDN=NULL,NgayChuyenDN=NULL"
                        + " where DongNuoc2=0 and MaDN=" + MaDN;

            SqlCommand command = new SqlCommand(sql);
            if (HinhDN == "NULL")
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

            return _cDAL.ExecuteNonQuery(command);
        }

        public bool CheckExist_MoNuoc(string MaDN)
        {
            if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where MoNuoc=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            else
                return true;
        }

        public bool ThemMoNuoc(string MaDN, string HinhMN, DateTime NgayMN, string ChiSoMN, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set MoNuoc=1,HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;

            SqlCommand command = new SqlCommand(sql);
            if (HinhMN == "NULL")
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMN);

            return _cDAL.ExecuteNonQuery(command);
        }

        public bool SuaMoNuoc(string MaDN, string HinhMN, DateTime NgayMN, string ChiSoMN, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where CAST(NgayMN_ThucTe as date)=CAST(getDate() as date) and MaDN=" + MaDN;

            SqlCommand command = new SqlCommand(sql);
            if (HinhMN == "NULL")
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMN);

            return _cDAL.ExecuteNonQuery(command);
        }

        public bool DangNganDongNuoc(string MaNV_DangNgan, string MaHDs)
        {
            try
            {
                string[] MaHD = MaHDs.Split(',');
                using (var scope = new TransactionScope())
                {
                    for (int i = 0; i < MaHD.Length; i++)
                    {
                        string sql = "update HOADON set DangNgan_DienThoai=1,DangNgan_Ton=1,MaNV_DangNgan=" + MaNV_DangNgan + ",NGAYGIAITRACH=getDate(),ModifyBy=" + MaNV_DangNgan + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                        if (_cDAL.ExecuteNonQuery(sql) == false)
                            return false;
                    }
                    scope.Complete();
                    scope.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetDSHoaDonTon_DongNuoc(string DanhBo, string MaHDs)
        {
            string sql = "select MaHD=ID_HOADON,Ky=CAST(KY as varchar)+'/'+CAST(NAM as varchar),TongCong from HOADON where DANHBA='" + DanhBo + "' and NGAYGIAITRACH is null and ID_HOADON not in (" + MaHDs + ")";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDon(string DanhBo)
        {
            //string sql = "select top 12 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,"
            //             + " Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),TieuThu,TongCong,NgayGiaiTrach=CONVERT(varchar(10),NgayGiaiTrach,103)"
            //             + " from HOADON where DANHBA='" + DanhBo+"'"
            //             + " order by ID_HOADON desc";

            string sql = "select * from fnGetDSHoaDon(" + DanhBo + ")";

            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDon(string HoTen, string SoNha, string TenDuong)
        {
            string sql = "select * from TimKiemTTKH('" + HoTen + "','" + SoNha + "','" + TenDuong + "')";

            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongGiaoHoaDon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where NAM=" + Nam + " and KY=" + Ky + " and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongDangNgan(string MaTo, DateTime FromNgayGiaiTrach,DateTime ToNgayGiaiTrach)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_DangNgan,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where CAST(NGAYGIAITRACH as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NGAYGIAITRACH as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd")+"'"
                        + " and (select MaTo from TT_NguoiDung where MaND=MaNV_DangNgan)="+MaTo
                        + " group by MaNV_DangNgan) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_DangNgan=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongTon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where NgayGiaiTrach is null and (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongDongMoNuoc(bool DongNuoc, string MaTo, DateTime FromNgayDN, DateTime ToNgayDN)
        {
            string sql = "";
            if (DongNuoc == true)
                sql = "select t1.*,t2.HoTen from"
                            + " (select dn.MaNV_DongNuoc,TongHD=COUNT(kqdn.MaDN) from TT_KQDongNuoc kqdn,TT_DongNuoc dn"
                            + " where CAST(kqdn.NgayDN as date)>='" + FromNgayDN.ToString("yyyyMMdd") + "' and CAST(kqdn.NgayDN as date)<='" + ToNgayDN.ToString("yyyyMMdd") + "' and kqdn.MaDN=dn.MaDN"
                            + " and (select MaTo from TT_NguoiDung where MaND=dn.MaNV_DongNuoc)=" + MaTo
                            + " group by dn.MaNV_DongNuoc) t1,TT_NguoiDung t2"
                            + " where t1.MaNV_DongNuoc=t2.MaND"
                            + " order by t2.STT asc";
            else
                sql = "select t1.*,t2.HoTen from"
                            + " (select dn.MaNV_DongNuoc,TongHD=COUNT(kqdn.MaDN) from TT_KQDongNuoc kqdn,TT_DongNuoc dn"
                            + " where CAST(kqdn.NgayMN as date)>='" + FromNgayDN.ToString("yyyyMMdd") + "' and CAST(kqdn.NgayMN as date)<='" + ToNgayDN.ToString("yyyyMMdd") + "' and kqdn.MaDN=dn.MaDN"
                            + " and (select MaTo from TT_NguoiDung where MaND=dn.MaNV_DongNuoc)=" + MaTo
                            + " group by dn.MaNV_DongNuoc) t1,TT_NguoiDung t2"
                            + " where t1.MaNV_DongNuoc=t2.MaND"
                            + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongThuHo(string MaTo, DateTime FromCreateDate, DateTime ToCreateDate)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from TT_DichVuThu dvt,HOADON hd"
                        + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
                        + " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _cDAL.ExecuteQuery_DataTable("select top 1 * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

        #region Thu Hộ

        public DataTable getHoaDonTon(string DanhBo)
        {
            return _cDAL.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + DanhBo + ")");
        }

        public int getPhiMoNuoc(string DanhBo)
        {
            return (int)_cDAL.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + DanhBo + ")");
        }

        public int getTienDu(string DanhBo)
        {
            return (int)_cDAL.ExecuteQuery_ReturnOneValue("select TienDu=dbo.fnGetTienDu(" + DanhBo + ")");
        }

        public bool insertThuHo(string DanhBo, string MaHDs, int SoTien, int PhiMoNuoc, int TienDu, string TenDichVu, string IDGiaoDich)
        {
            try
            {
                _cDAL.BeginTransaction();
                int ID = (int)_cDAL.ExecuteQuery_ReturnOneValue_Transaction("select MAX(ID)+1 from TT_DichVuThu_Tong");
                string[] arrayMaHD = MaHDs.Split(',');
                string SoHoaDons = "", sql = "";
                for (int i = 0; i < arrayMaHD.Length; i++)
                {
                    DataTable dt = _cDAL.ExecuteQuery_DataTable_Transaction("select MaHD=ID_HOADON,SOHOADON,DanhBo=DANHBA,NAM,KY,GIABAN,ThueGTGT=THUE,PhiBVMT=PHI,TONGCONG from HOADON where ID_HOADON=" + arrayMaHD[i]);
                    sql = "insert into TT_DichVuThu(MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,IDDichVu,IDGiaoDich,CreateDate)"
                        +" values("+dt.Rows[0]["MaHD"]+",'"+dt.Rows[0]["SoHoaDon"]+"','"+dt.Rows[0]["DanhBo"]+"',"+dt.Rows[0]["Nam"]+","+dt.Rows[0]["Ky"]+","+dt.Rows[0]["TongCong"]+",N'"+TenDichVu+"',"+ID+",'"+IDGiaoDich+"',getdate())";
                    _cDAL.ExecuteNonQuery_Transaction(sql);
                    if (string.IsNullOrEmpty(SoHoaDons) == true)
                        SoHoaDons = dt.Rows[0]["SoHoaDon"].ToString();
                    else
                        SoHoaDons += ","+dt.Rows[0]["SoHoaDon"];
                }
                sql = "insert into TT_DichVuThu_Tong(ID,DanhBo,MaHDs,SoHoaDons,SoTien,PhiMoNuoc,TienDu,TenDichVu,IDGiaoDich,CreateDate)"
                            + " values(" + ID + ",'" + DanhBo + "','" + MaHDs + "','" + SoHoaDons + "'," + SoTien + "," + PhiMoNuoc + "," + TienDu + ",N'" + TenDichVu + "','" + IDGiaoDich + "',getdate())";
                _cDAL.ExecuteNonQuery_Transaction(sql);
                _cDAL.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _cDAL.RollbackTransaction();
                throw ex;
            }
        }

        public bool deleteThuHo(string TenDichVu, string IDGiaoDich)
        {
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select MaHD from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int count = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID_HOADON) from HOADON where ID_HOADON=" + dt.Rows[i]["MaHD"] + " and NGAYGIAITRACH is not null");
                    if (count > 0)
                    {
                        throw new Exception("Hóa đơn đã giải trách. Không xóa được");
                    }
                }
                _cDAL.BeginTransaction();
                _cDAL.ExecuteNonQuery_Transaction("delete TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.ExecuteNonQuery_Transaction("delete TT_DichVuThu_Tong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _cDAL.RollbackTransaction();
                throw ex;
            }
        }

        public DataTable getThuHo(string TenDichVu, string IDGiaoDich)
        {
            return _cDAL.ExecuteQuery_DataTable("select MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,CreateDate from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
        }

        public DataTable getThuHoTong(string TenDichVu, string IDGiaoDich)
        {
            return _cDAL.ExecuteQuery_DataTable("select DanhBo,MaHDs,SoHoaDons,SoTien,PhiMoNuoc,TienDu,CreateDate from TT_DichVuThu_Tong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
        }

        #endregion

    }
}
