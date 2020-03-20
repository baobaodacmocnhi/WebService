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
        CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        CKinhDoanh _cKinhDoanh = new CKinhDoanh();

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

                return DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Doi,ToTruong,MaTo,DienThoai,TestApp from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0"));
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
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai from TT_NguoiDung where MaND!=0 and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSNhanVien(string MaTo)
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo from TT_NguoiDung where MaND!=0 and MaTo=" + MaTo + " and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        //send notification
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
                //gắn data post
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

        //hành thu
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
                            + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0 and dn.MaNV_DongNuoc is not null))t1"
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
                          + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0 and dn.MaNV_DongNuoc is not null))t1"
                      + " where t1.ID not in (select MaHD from TT_LenhHuy)"
                      + " order by t1.MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDonTon(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select * from"
                            + " (select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                            + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong,"
                            + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                            + " ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end"
                            + " from HOADON hd where (NAM<" + Nam + " or (Ky<=" + Ky + " and NAM=" + Nam + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=" + MaNV + " and NgayGiaiTrach is null"
                            + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.MaDN=ctdn.MaDN and dn.Huy=0 and dn.MaNV_DongNuoc is not null))t1"
                            + " where t1.ID not in (select MaHD from TT_LenhHuy)"
                            + " order by t1.MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSHoaDonTon_HoaDonDienTu(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                            + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong,"
                            + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                            + " ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end,"
                            + " DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen,"
                //+ " TBDongNuoc_Ngay=(select a.CreateDate from TT_DongNuoc a,TT_CTDongNuoc b where a.MaDN=b.MaDN and Huy=0 and b.MaHD=hd.ID_HOADON),"
                            + " PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA)),"
                            + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                            + " from HOADON hd where (NAM<" + Nam + " or (Ky<=" + Ky + " and NAM=" + Nam + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=" + MaNV
                            + " and (NgayGiaiTrach is null or DangNgan_DienThoai=1) and DangNgan_Ton=0"
                            + " order by MLT";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string XuLy_HoaDonDienTu(string LoaiXuLy, string MaNV, string MaHDs, DateTime Ngay, DateTime NgayHen)
        {
            try
            {
                string sql = "";
                //string[] MaHD = MaHDs.Split(',');
                //for (int i = 0; i < MaHD.Length; i++)
                //{
                //    switch (LoaiXuLy)
                //    {
                //        case "DangNgan":
                //            sql += " update HOADON set DangNgan_DienThoai=1,DangNgan_HanhThu=1,MaNV_DangNgan=" + MaNV + ",NGAYGIAITRACH='" + Ngay + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                //            break;
                //        case "PhieuBao":
                //            sql += " update HOADON set InPhieuBao_Ngay='" + Ngay + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                //            break;
                //        case "PhieuBao2":
                //            sql += " update HOADON set InPhieuBao2_Ngay='" + Ngay + "',InPhieuBao2_NgayHen='" + NgayHen + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                //            break;
                //        case "TBDongNuoc":
                //            sql += " update HOADON set TBDongNuoc_Ngay='" + Ngay + "',TBDongNuoc_NgayHen='" + NgayHen + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                //            break;
                //        case "XoaDangNgan":
                //            sql += " update HOADON set XoaDangNgan_Ngay_DienThoai='" + Ngay + "',DangNgan_DienThoai=0,DangNgan_HanhThu=0,MaNV_DangNgan=NULL,NGAYGIAITRACH=NULL,ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is not null ";
                //            break;
                //        default:
                //            break;
                //    }
                //}
                string sqlCheck = "select MaHD=ID_HOADON,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),"
                                    + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                                    + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                                    + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                                    + " from HOADON hd where ID_HOADON in (" + MaHDs + ")";
                DataTable dt = _cDAL.ExecuteQuery_DataTable(sqlCheck);
                switch (LoaiXuLy)
                {
                    case "XoaDangNgan":
                        foreach (DataRow item in dt.Rows)
                        {
                            if (bool.Parse(item["GiaiTrach"].ToString()) == false)
                                return "false,GiaiTrach,false," + item["MaHD"].ToString() + ",Kỳ " + item["Ky"].ToString() + " chưa Giải Trách";
                        }
                        break;
                    default:
                        foreach (DataRow item in dt.Rows)
                        {
                            if (bool.Parse(item["ThuHo"].ToString()) == true)
                                return "false,ThuHo,true," + item["MaHD"].ToString() + ",Kỳ " + item["Ky"].ToString() + " đã Thu Hộ";
                            else
                                if (bool.Parse(item["TamThu"].ToString()) == true)
                                    return "false,TamThu,true," + item["MaHD"].ToString() + ",Kỳ " + item["Ky"].ToString() + " đã Tạm Thu";
                                else
                                    if (bool.Parse(item["GiaiTrach"].ToString()) == true)
                                        return "false,GiaiTrach,true," + item["MaHD"].ToString() + ",Kỳ " + item["Ky"].ToString() + " đã Giải Trách";
                        }
                        break;
                }

                switch (LoaiXuLy)
                {
                    case "DangNgan":
                        sql += " update HOADON set DangNgan_DienThoai=1,XoaDangNgan_Ngay_DienThoai=NULL,DangNgan_HanhThu=1,MaNV_DangNgan=" + MaNV + ",NGAYGIAITRACH='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "PhieuBao":
                        sql += " update HOADON set InPhieuBao_MaNV=" + MaNV + ",InPhieuBao_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "PhieuBao2":
                        sql += " update HOADON set InPhieuBao2_MaNV=" + MaNV + ",InPhieuBao2_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao2_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "TBDongNuoc":
                        sql += " update HOADON set TBDongNuoc_MaNV=" + MaNV + ",TBDongNuoc_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',TBDongNuoc_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "XoaDangNgan":
                        sql += " update HOADON set XoaDangNgan_MaNV_DienThoai=" + MaNV + ",XoaDangNgan_Ngay_DienThoai='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',DangNgan_DienThoai=0,DangNgan_HanhThu=0,MaNV_DangNgan=NULL,NGAYGIAITRACH=NULL,ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is not null ";
                        break;
                    default:
                        break;
                }
                //using (var scope = new TransactionScope())
                //{
                //    if (_cDAL.ExecuteNonQuery(sql) == true)
                //    {
                //        scope.Complete();
                //        scope.Dispose();
                //        return true;
                //    }
                //}
                if (_cDAL.ExecuteNonQuery(sql) == true)
                {
                    return "true,";
                }
                else
                    return "false,error query";
            }
            catch (Exception ex)
            {
                return "false," + ex.Message;
            }
        }

        public string get_GhiChu(string DanhBo)
        {
            string sql = "select DanhBo,DienThoai,GiaBieu,NiemChi,DiemBe from TT_GhiChu where DanhBo='" + DanhBo + "'";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public bool update_GhiChu(string MaNV, string DanhBo, string DienThoai, string GiaBieu, string NiemChi, string DiemBe)
        {
            try
            {
                string sql = "if not exists (select 1 from TT_GhiChu where DanhBo='" + DanhBo + "')"
                            + " 	insert into TT_GhiChu(DanhBo,DienThoai,GiaBieu,NiemChi,DiemBe,CreateBy,CreateDate)values('" + DanhBo + "','" + DienThoai + "',N'" + GiaBieu + "',N'" + NiemChi + "',N'" + DiemBe + "'," + MaNV + ",GETDATE());"
                            + " else"
                            + " 	update TT_GhiChu set DienThoai='" + DienThoai + "',GiaBieu=N'" + GiaBieu + "',NiemChi=N'" + NiemChi + "',DiemBe=N'" + DiemBe + "',ModifyBy=" + MaNV + ",ModifyDate=GETDATE() where DanhBo='" + DanhBo + "';";
                return _cDAL.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return false;
            }
        }

        //tạm thu
        public string GetDSTamThu(bool RutSot, string MaNV, DateTime FromCreateDate, DateTime ToCreateDate)
        {
            string sql = "";
            if (RutSot == true)
                sql = "if((select DongNuoc from TT_NguoiDung where MaND=" + MaNV + ")=0)"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt where MaNV_HanhThu=" + MaNV+" and NGAYGIAITRACH is null"
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON"
                        + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.Huy=0 and dn.MaDN=ctdn.MaDN)"
                    + " else"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt,TT_DongNuoc dn,TT_CTDongNuoc ctdn where MaNV_DongNuoc=" + MaNV+" and NGAYGIAITRACH is null"
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON and dn.Huy=0 and dn.MaDN=ctdn.MaDN and hd.ID_HOADON=ctdn.MaHD";
            else
                sql = "if((select DongNuoc from TT_NguoiDung where MaND="+MaNV+")=0)"
	                    + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
	                    + " from HOADON hd,TAMTHU tt where MaNV_HanhThu="+MaNV
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
	                    + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON"
	                    + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.Huy=0 and dn.MaDN=ctdn.MaDN)"
                    + " else"
	                    + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
	                    + " from HOADON hd,TAMTHU tt,TT_DongNuoc dn,TT_CTDongNuoc ctdn where MaNV_DongNuoc="+MaNV
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
	                    + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON and dn.Huy=0 and dn.MaDN=ctdn.MaDN and hd.ID_HOADON=ctdn.MaHD";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        //đóng nước
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
            string query = "select ID=dn.MaDN,dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " DongNuoc=case when kqdn.DongNuoc is null then 'false' else case when kqdn.DongNuoc=1 then 'true' else 'false' end end,"
                            + " DongNuoc2=case when kqdn.DongNuoc2 is null then 'false' else case when kqdn.DongNuoc2=1 then 'true' else 'false' end end,"
                            + " MoNuoc=case when kqdn.MoNuoc is null then 'false' else case when kqdn.MoNuoc=1 then 'true' else 'false' end end,"
                            + " DongPhi=case when kqdn.DongPhi is null then 'false' else case when kqdn.DongPhi=1 then 'true' else 'false' end end,"
                            + " ButChi=case when kqdn.ButChi is null then 'false' else case when kqdn.ButChi=1 then 'true' else 'false' end end,"
                            + " KhoaTu=case when kqdn.KhoaTu is null then 'false' else case when kqdn.KhoaTu=1 then 'true' else 'false' end end,"
                            + " KhoaKhac=case when kqdn.KhoaKhac is null then 'false' else case when kqdn.KhoaKhac=1 then 'true' else 'false' end end,"
                            + " kqdn.NgayDN,kqdn.ChiSoDN,kqdn.NiemChi,kqdn.KhoaKhac_GhiChu,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.ViTri,kqdn.LyDo,kqdn.NgayDN1,kqdn.ChiSoDN1,kqdn.NiemChi1,kqdn.NgayMN,kqdn.ChiSoMN"
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
            string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,Ky,TongCong,"
                            + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
                            + " PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong where MaHDs like '%'+CONVERT(varchar(8),ctdn.MaHD)+'%'),"
                            + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                            + " from TT_DongNuoc dn left join TT_CTDongNuoc ctdn on dn.MaDN=ctdn.MaDN left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0)"
                            + " order by dn.MLT,ctdn.MaHD";
            //string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,MLT=MALOTRINH,ctdn.Ky,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
            //                + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,hd.TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),hd.GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,hd.TongCong,"
            //                + " DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen,"
            //                + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
            //                + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
            //                + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
            //                + " PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA)),"
            //                + " PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong where MaHDs like '%'+CONVERT(varchar(8),ctdn.MaHD)+'%'),"
            //                + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
            //                + " from TT_DongNuoc dn"
            //                + " left join TT_CTDongNuoc ctdn on dn.MaDN=ctdn.MaDN"
            //                + " left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
            //                + " left join HOADON hd on hd.ID_HOADON=ctdn.MaHD"
            //                + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(dn.NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(dn.NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
            //                + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
            //                + " or kqdn.MoNuoc=0)"
            //                + " order by dn.MLT,ctdn.MaHD";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(sql);

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

        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, DateTime NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string KhoaKhac, string KhoaKhac_GhiChu, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string ViTri, string LyDo, string CreateBy)
        {
            int flagButChi = 0;
            int flagKhoaTu = 0;
            int flagKhoaKhac = 0;
            if (bool.Parse(KhoaTu) == false && bool.Parse(KhoaKhac) == false)
            {
                if (NiemChi == "")
                    return false;
                if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID=" + NiemChi).ToString()) == 0)
                    return false;
                if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID=" + NiemChi + " and SuDung=1").ToString()) == 1)
                    return false;
            }
            else
            {
                NiemChi = "NULL";
                if (bool.Parse(KhoaTu) == true)
                {
                    flagKhoaTu = 1;
                }
                if (bool.Parse(KhoaKhac) == true)
                {
                    flagKhoaKhac = 1;
                }
                else
                    KhoaKhac_GhiChu = "NULL";
            }

            if (bool.Parse(ButChi) == true)
                flagButChi = 1;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int MaKQDN = (int)_cDAL.ExecuteQuery_ReturnOneValue("select case when (select COUNT(MaKQDN) from TT_KQDongNuoc)=0 then 1 else (select MAX(MaKQDN) from TT_KQDongNuoc)+1 end");
                    //insert đóng nước
                    //string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,HinhDN,NgayDN,NgayDN_ThucTe,ChiSoDN,ButChi,KhoaTu,NiemChi,KhoaKhac,KhoaKhac_GhiChu,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,ViTri,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                    //         + "" + MaKQDN + "," + MaDN + ",'" + DanhBo.Replace(" ", "") + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1,@HinhDN"
                    //         + ",'" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',getDate()," + ChiSoDN + "," + flagButChi + "," + flagKhoaTu + "," + NiemChi + "," + flagKhoaKhac + ",N'" + KhoaKhac_GhiChu + "','" + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + ViTri + "',N'" + LyDo + "',(select PhiMoNuoc from TT_CacLoaiPhi where CoDHN like '%" + Co + "%')," + CreateBy + ",getDate())";
                    string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,NgayDN,NgayDN_ThucTe,ChiSoDN,ButChi,KhoaTu,NiemChi,KhoaKhac,KhoaKhac_GhiChu,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,ViTri,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                             + "" + MaKQDN + "," + MaDN + ",'" + DanhBo.Replace(" ", "") + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1"
                             + ",'" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',getDate()," + ChiSoDN + "," + flagButChi + "," + flagKhoaTu + "," + NiemChi + "," + flagKhoaKhac + ",N'" + KhoaKhac_GhiChu + "','" + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + ViTri + "',N'" + LyDo + "',(select PhiMoNuoc from TT_CacLoaiPhi where CoDHN like '%" + Co + "%')," + CreateBy + ",getDate())";

                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhDN == "NULL")
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

                    if (_cDAL.ExecuteNonQuery(command) == true)
                    {
                        //insert table hình
                        if (HinhDN == "")
                        {
                            //string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end)," + MaKQDN + ",1,@Hinh," + CreateBy + ",getDate())";
                            //command = new SqlCommand(sql_Hinh);
                            //command.Parameters.Add("@Hinh", SqlDbType.Image).Value = DBNull.Value;
                            //_cDAL.ExecuteNonQuery(command);
                        }
                        else
                        {
                            string[] HinhDNs = HinhDN.Split(';');
                            for (int i = 0; i < HinhDNs.Count(); i++)
                            {
                                string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end)," + MaKQDN + ",1,@Hinh," + CreateBy + ",getDate())";
                                command = new SqlCommand(sql_Hinh);
                                command.Parameters.Add("@Hinh", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDNs[i]);
                                _cDAL.ExecuteNonQuery(command);
                            }
                        }
                        //insert niêm chì
                        if (NiemChi != "NULL")
                        {
                            string sqlNiemChi = "update TT_NiemChi set SuDung=1,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where ID=" + NiemChi + " and SuDung=0";

                            if (_cDAL.ExecuteNonQuery(sqlNiemChi) == true)
                            {
                                scope.Complete();
                                return true;
                            }
                        }
                        else
                        {
                            scope.Complete();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
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

        public bool ThemDongNuoc2(string MaDN, string HinhDN, DateTime NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string KhoaKhac, string KhoaKhac_GhiChu, string CreateBy)
        {
            int flagButChi = 0;
            int flagKhoaTu = 0;
            int flagKhoaKhac = 0;
            if (bool.Parse(KhoaTu) == false && bool.Parse(KhoaKhac) == false)
            {
                if (NiemChi == "")
                    return false;
                if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID=" + NiemChi).ToString()) == 0)
                    return false;
                if (int.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID=" + NiemChi + " and SuDung=1").ToString()) == 1)
                    return false;
            }
            else
            {
                NiemChi = "NULL";
                if (bool.Parse(KhoaTu) == true)
                {
                    flagKhoaTu = 1;
                }
                if (bool.Parse(KhoaKhac) == true)
                {
                    flagKhoaKhac = 1;
                }
                else
                    KhoaKhac_GhiChu = "NULL";
            }

            if (bool.Parse(ButChi) == true)
                flagButChi = 1;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert đóng nước
                    //string sql = "update TT_KQDongNuoc set DongNuoc2=1,PhiMoNuoc=(select top 1 PhiMoNuoc from TT_CacLoaiPhi)*2,HinhDN1=HinhDN,NgayDN1=NgayDN,NgayDN1_ThucTe=NgayDN_ThucTe,ChiSoDN1=ChiSoDN,NiemChi1=NiemChi,"
                    //            + "HinhDN=@HinhDN,NgayDN='" + NgayDN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ",NiemChi=" + NiemChi + ",KhoaKhac_GhiChu=N'" + KhoaKhac_GhiChu + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate(),"
                    //            + "SoPhieuDN1=SoPhieuDN,NgaySoPhieuDN1=NgaySoPhieuDN,ChuyenDN1=ChuyenDN,NgayChuyenDN1=NgayChuyenDN,SoPhieuDN=NULL,NgaySoPhieuDN=NULL,ChuyenDN=0,NgayChuyenDN=NULL"
                    //            + " where DongNuoc2=0 and MaDN=" + MaDN;
                    //string sql = "update TT_KQDongNuoc set DongNuoc2=1,PhiMoNuoc=PhiMoNuoc*2,HinhDN1=HinhDN,NgayDN1=NgayDN,NgayDN1_ThucTe=NgayDN_ThucTe,ChiSoDN1=ChiSoDN,NiemChi1=NiemChi,"
                    //            + "HinhDN=@HinhDN,NgayDN='" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ",NiemChi=" + NiemChi + ",KhoaKhac_GhiChu=N'" + KhoaKhac_GhiChu + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate(),"
                    //            + "SoPhieuDN1=SoPhieuDN,NgaySoPhieuDN1=NgaySoPhieuDN,ChuyenDN1=ChuyenDN,NgayChuyenDN1=NgayChuyenDN,SoPhieuDN=NULL,NgaySoPhieuDN=NULL,ChuyenDN=0,NgayChuyenDN=NULL"
                    //            + " where DongNuoc2=0 and MaDN=" + MaDN;
                    string sql = "update TT_KQDongNuoc set DongNuoc2=1,PhiMoNuoc=PhiMoNuoc*2,NgayDN1=NgayDN,NgayDN1_ThucTe=NgayDN_ThucTe,ChiSoDN1=ChiSoDN,NiemChi1=NiemChi,"
                               + "NgayDN='" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ",ButChi=" + flagButChi + ",KhoaTu=" + flagKhoaTu + ",NiemChi=" + NiemChi + ",KhoaKhac=" + flagKhoaKhac + ",KhoaKhac_GhiChu=N'" + KhoaKhac_GhiChu + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate(),"
                               + "SoPhieuDN1=SoPhieuDN,NgaySoPhieuDN1=NgaySoPhieuDN,ChuyenDN1=ChuyenDN,NgayChuyenDN1=NgayChuyenDN,SoPhieuDN=NULL,NgaySoPhieuDN=NULL,ChuyenDN=0,NgayChuyenDN=NULL"
                               + " where DongNuoc2=0 and MaDN=" + MaDN;

                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhDN == "NULL")
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

                    if (_cDAL.ExecuteNonQuery(command) == true)
                    {
                        //insert table hình
                        //string sql_Hinh = "declare @MaKQDN int;"
                        //                + " set @MaKQDN=(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + ")"
                        //                + " if not exists (select MaKQDN from TT_KQDongNuoc_Hinh where MaKQDN=@MaKQDN)"
                        //                + " insert into TT_KQDongNuoc_Hinh(MaKQDN,HinhDN,CreateBy,CreateDate)values(@MaKQDN,@HinhDN," + CreateBy + ",getDate())"
                        //                + " else"
                        //                + " update TT_KQDongNuoc_Hinh set HinhDN1=HinhDN,HinhDN=@HinhDN,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaKQDN=@MaKQDN";

                        //insert table hình
                        if (HinhDN == "")
                        {
                            //string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc2,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc)+1 end),(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + "),1,@Hinh," + CreateBy + ",getDate())";
                            //command = new SqlCommand(sql_Hinh);
                            //command.Parameters.Add("@Hinh", SqlDbType.Image).Value = DBNull.Value;
                            //_cDAL.ExecuteNonQuery(command);
                        }
                        else
                        {
                            string[] HinhDNs = HinhDN.Split(';');
                            for (int i = 0; i < HinhDNs.Count(); i++)
                            {
                                string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc2,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end),(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + "),1,@Hinh," + CreateBy + ",getDate())";
                                command = new SqlCommand(sql_Hinh);
                                command.Parameters.Add("@Hinh", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDNs[i]);
                                _cDAL.ExecuteNonQuery(command);
                            }
                        }

                        //insert niêm chì
                        if (NiemChi != "NULL")
                        {
                            string sqlNiemChi = "update TT_NiemChi set SuDung=1,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where ID=" + NiemChi + " and SuDung=0";

                            if (_cDAL.ExecuteNonQuery(sqlNiemChi) == true)
                            {
                                scope.Complete();
                                return true;
                            }
                        }
                        else
                        {
                            scope.Complete();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
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
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //string sql = "update TT_KQDongNuoc set MoNuoc=1,HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    //string sql = "update TT_KQDongNuoc set MoNuoc=1,HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd HH:mm:ss") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    string sql = "update TT_KQDongNuoc set MoNuoc=1,NgayMN='" + NgayMN.ToString("yyyyMMdd HH:mm:ss") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhMN == "NULL")
                    //    command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMN);

                    if (_cDAL.ExecuteNonQuery(command) == true)
                    {
                        //insert table hình
                        //string sql_Hinh = "declare @MaKQDN int;"
                        //                    + " set @MaKQDN=(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + ")"
                        //                    + " if not exists (select MaKQDN from TT_KQDongNuoc_Hinh where MaKQDN=@MaKQDN)"
                        //                    + " insert into TT_KQDongNuoc_Hinh(MaKQDN,HinhMN,CreateBy,CreateDate)values(@MaKQDN,@HinhMN," + CreateBy + ",getDate())"
                        //                    + " else"
                        //                    + " update TT_KQDongNuoc_Hinh set HinhMN=@HinhMN,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaKQDN=@MaKQDN";

                        if (HinhMN == "")
                        {
                            //string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,MoNuoc,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc)+1 end),(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + "),1,@Hinh," + CreateBy + ",getDate())";
                            //command = new SqlCommand(sql_Hinh);
                            //command.Parameters.Add("@Hinh", SqlDbType.Image).Value = DBNull.Value;
                            //_cDAL.ExecuteNonQuery(command);
                        }
                        else
                        {
                            string[] HinhMNs = HinhMN.Split(';');
                            for (int i = 0; i < HinhMNs.Count(); i++)
                            {
                                string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,MoNuoc,Hinh,CreateBy,CreateDate)values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end),(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + "),1,@Hinh," + CreateBy + ",getDate())";
                                command = new SqlCommand(sql_Hinh);
                                command.Parameters.Add("@Hinh", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMNs[i]);
                                _cDAL.ExecuteNonQuery(command);
                            }
                        }

                        scope.Complete();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
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
            string sql = "select MaHD=ID_HOADON,Ky=CAST(KY as varchar)+'/'+CAST(NAM as varchar),MLT=MALOTRINH,DanhBo=DANHBA,HoTen=TENKH,DiaChi=SO+' '+DUONG"
                + " ,GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong"
                + " ,DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen"
                + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
                + " PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong where MaHDs like '%'+CONVERT(varchar(8),ctdn.MaHD)+'%'),"
                + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                + " from HOADON where DANHBA='" + DanhBo + "' and NGAYGIAITRACH is null";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        //tìm kiếm
        public string GetDSTimKiem(string DanhBo)
        {
            //string sql = "select top 12 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,"
            //             + " Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),TieuThu,TongCong,NgayGiaiTrach=CONVERT(varchar(10),NgayGiaiTrach,103)"
            //             + " from HOADON where DANHBA='" + DanhBo+"'"
            //             + " order by ID_HOADON desc";

            string sql = "select * from fnTimKiem('" + DanhBo + "','') order by MaHD desc";

            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetDSTimKiemTTKH(string HoTen, string SoNha, string TenDuong)
        {
            string sql = "select * from fnTimKiemTTKH('" + HoTen + "','" + SoNha + "','" + TenDuong + "')";

            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        //quản lý
        public string GetTongGiaoHoaDon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG),TyLeThuDuoc=ROUND(CAST(SUM(case when NGAYGIAITRACH is not null then 1 else 0 end) as float)/COUNT(ID_HOADON)*100,2) from HOADON"
                        + " where NAM=" + Nam + " and KY=" + Ky + " and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongDangNgan(string MaTo, DateTime FromNgayGiaiTrach, DateTime ToNgayGiaiTrach)
        {
            //string sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_DangNgan,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
            //            + " where CAST(NGAYGIAITRACH as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NGAYGIAITRACH as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd")+"'"
            //            + " and (select MaTo from TT_NguoiDung where MaND=MaNV_DangNgan)="+MaTo
            //            + " group by MaNV_DangNgan) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_DangNgan=t2.MaND"
            //            + " order by t2.STT asc";
            string sql = "declare @HanhThu bit;"
                        + " select @HanhThu=HanhThu from TT_To where MaTo=" + MaTo
                        + " if(@HanhThu=1)"
                        + " begin"
                        + " select MaNV_DangNgan=t1.MaND,t1.HoTen,TongHD=case when t2.TongHD is null then 0 else t2.TongHD end,TongCong=case when t2.TongCong is null then 0 else t2.TongCong end from"
                        + " (select MaND,STT,HoTen from TT_NguoiDung where HanhThu=1 and MaTo=" + MaTo + ")t1"
                        + " left join"
                        + " (select MaNV_DangNgan,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where CAST(NGAYGIAITRACH as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NGAYGIAITRACH as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd") + "'"
                        + " group by MaNV_DangNgan) t2 on t1.MaND=t2.MaNV_DangNgan"
                        + " order by t1.STT asc"
                        + " end"
                        + " else"
                        + " begin"
                        + " select MaNV_DangNgan=t1.MaND,t1.HoTen,TongHD=case when t2.TongHD is null then 0 else t2.TongHD end,TongCong=case when t2.TongCong is null then 0 else t2.TongCong end from"
                        + " (select MaND,STT,HoTen from TT_NguoiDung where HanhThuVanPhong=1 and MaTo=" + MaTo + ")t1"
                        + " left join"
                        + " (select MaNV_DangNgan,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where CAST(NGAYGIAITRACH as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NGAYGIAITRACH as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd") + "'"
                        + " group by MaNV_DangNgan) t2 on t1.MaND=t2.MaNV_DangNgan"
                        + " order by t1.STT asc"
                        + " end";
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

        public string GetTongDongMoNuoc_Tong(bool DongNuoc, string MaTo, DateTime FromNgayDN, DateTime ToNgayDN)
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

        public string GetTongDongMoNuoc_ChiTiet(bool DongNuoc, string MaTo, DateTime FromNgayDN, DateTime ToNgayDN)
        {
            string sql = "";
            if (DongNuoc == true)
                sql = "select t1.*,t2.HoTen from"
                            + " (select dn.MaNV_DongNuoc,kqdn.DanhBo,kqdn.DiaChi from TT_KQDongNuoc kqdn,TT_DongNuoc dn"
                            + " where CAST(kqdn.NgayDN as date)>='" + FromNgayDN.ToString("yyyyMMdd") + "' and CAST(kqdn.NgayDN as date)<='" + ToNgayDN.ToString("yyyyMMdd") + "' and kqdn.MaDN=dn.MaDN"
                            + " and (select MaTo from TT_NguoiDung where MaND=dn.MaNV_DongNuoc)=" + MaTo
                            + " ) t1,TT_NguoiDung t2"
                            + " where t1.MaNV_DongNuoc=t2.MaND"
                            + " order by t2.STT asc";
            else
                sql = "select t1.*,t2.HoTen from"
                            + " (select dn.MaNV_DongNuoc,kqdn.DanhBo,kqdn.DiaChi from TT_KQDongNuoc kqdn,TT_DongNuoc dn"
                            + " where CAST(kqdn.NgayMN as date)>='" + FromNgayDN.ToString("yyyyMMdd") + "' and CAST(kqdn.NgayMN as date)<='" + ToNgayDN.ToString("yyyyMMdd") + "' and kqdn.MaDN=dn.MaDN"
                            + " and (select MaTo from TT_NguoiDung where MaND=dn.MaNV_DongNuoc)=" + MaTo
                            + " ) t1,TT_NguoiDung t2"
                            + " where t1.MaNV_DongNuoc=t2.MaND"
                            + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongThuHo_Tong(string MaTo, DateTime FromCreateDate, DateTime ToCreateDate)
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

        public string GetTongThuHo_Tong(string MaTo, DateTime FromCreateDate, DateTime ToCreateDate, string Loai)
        {
            string sql = "";
            //switch (Loai)
            //{
            //    case "Chưa Giải Trách":
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and hd.NGAYGIAITRACH is null and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //    case "Giải Trách":
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and hd.NGAYGIAITRACH is not null and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //    default:
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //}
            sql = "select t1.MaNV_HanhThu,t2.HoTen,t2.STT,TongHD=COUNT(t1.MaHD),TongCong=SUM(t1.TongCong) from"
                + " (select MaNV_HanhThu=case when exists(select a.MaDN from TT_DongNuoc a, TT_CTDongNuoc b where a.MaDN=b.MaDN and a.Huy=0 and b.MaHD=dvt.MaHD)"
                + " then (select a.MaNV_DongNuoc from TT_DongNuoc a, TT_CTDongNuoc b where a.MaDN=b.MaDN and a.Huy=0 and b.MaHD=dvt.MaHD) else hd.MaNV_HanhThu end"
                + " ,MaHD=ID_HOADON,TongCong=TONGCONG from TT_DichVuThu dvt,HOADON hd"
                + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON";
            switch (Loai)
            {
                case "Chưa Giải Trách":
                    sql += " and hd.NGAYGIAITRACH is null";
                    break;
                case "Giải Trách":
                    sql += " and hd.NGAYGIAITRACH is not null";
                    break;
                default:
                    break;
            }
            sql += " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo + ") t1,TT_NguoiDung t2"
                + " where t1.MaNV_HanhThu=t2.MaND"
                + " group by MaNV_HanhThu,HoTen,STT"
                + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongThuHo_ChiTiet(string MaTo, DateTime FromCreateDate, DateTime ToCreateDate)
        {
            string sql = "select t1.*,t2.HoTen from"
                        + " (select MaNV_HanhThu,MLT=MALOTRINH,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG from TT_DichVuThu dvt,HOADON hd"
                        + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
                        + " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
                        + " ) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string GetTongThuHo_ChiTiet(string MaTo, DateTime FromCreateDate, DateTime ToCreateDate, string Loai)
        {
            string sql = "";
            //switch (Loai)
            //{
            //    case "Chưa Giải Trách":
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,MLT=MALOTRINH,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and hd.NGAYGIAITRACH is null and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " ) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //    case "Giải Trách":
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,MLT=MALOTRINH,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and hd.NGAYGIAITRACH is not null and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " ) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //    default:
            //        sql = "select t1.*,t2.HoTen from"
            //            + " (select MaNV_HanhThu,MLT=MALOTRINH,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG from TT_DichVuThu dvt,HOADON hd"
            //            + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON"
            //            + " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo
            //            + " ) t1,TT_NguoiDung t2"
            //            + " where t1.MaNV_HanhThu=t2.MaND"
            //            + " order by t2.STT asc";
            //        break;
            //}

            sql = "select t1.*,t2.HoTen from"
                 + " (select MaNV_HanhThu=case when exists(select a.MaDN from TT_DongNuoc a, TT_CTDongNuoc b where a.MaDN=b.MaDN and a.Huy=0 and b.MaHD=dvt.MaHD)"
                 + " then (select a.MaNV_DongNuoc from TT_DongNuoc a, TT_CTDongNuoc b where a.MaDN=b.MaDN and a.Huy=0 and b.MaHD=dvt.MaHD) else hd.MaNV_HanhThu end"
                 + " ,MLT=MALOTRINH,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG"
                 + " ,PhiMoNuoc=(select PhiMoNuoc from TT_DichVuThuTong where ID=dvt.IDDichVu)"
                 + " from TT_DichVuThu dvt,HOADON hd"
                 + " where CAST(dvt.CreateDate as date)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(dvt.CreateDate as date)<='" + ToCreateDate.ToString("yyyyMMdd") + "' and dvt.MaHD=hd.ID_HOADON";
            switch (Loai)
            {
                case "Chưa Giải Trách":
                    sql += " and hd.NGAYGIAITRACH is null";
                    break;
                case "Giải Trách":
                    sql += " and hd.NGAYGIAITRACH is not null";
                    break;
                default:
                    break;
            }
            sql += " and (select MaTo from TT_NguoiDung where MaND=MaNV_HanhThu)=" + MaTo + ") t1,TT_NguoiDung t2"
                + " where t1.MaNV_HanhThu=t2.MaND"
                + " order by t1.MLT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _cDAL.ExecuteQuery_DataTable("select top 1 * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

        //lệnh hủy
        public string GetDSHoaDon_LenhHuy(string LoaiCat, string ID)
        {
            string DanhBo = "";
            switch (LoaiCat)
            {
                case "Cắt Tạm":
                    DanhBo = _cKinhDoanh.getDanhBo_CatTam(ID);
                    break;
                case "Cắt Hủy":
                    DanhBo = _cKinhDoanh.getDanhBo_CatHuy(ID);
                    break;
                case "Danh Bộ":
                    DanhBo = ID;
                    break;
                default:
                    return DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select DanhBo=hd.DANHBA,DiaChi=SO+' '+DUONG,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),lh.MaHD,lh.TinhTrang,Cat=case when lh.Cat=1 then 'true' else 'false' end from TT_LenhHuy lh,HOADON hd where lh.MaHD=hd.ID_HOADON and hd.NGAYGIAITRACH is null"));
            }
            if (DanhBo == "")
                return "";
            else
                return DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select DanhBo=hd.DANHBA,DiaChi=SO+' '+DUONG,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),lh.MaHD,lh.TinhTrang,Cat=case when lh.Cat=1 then 'true' else 'false' end from TT_LenhHuy lh,HOADON hd where lh.MaHD=hd.ID_HOADON and DanhBo='" + DanhBo + "' and hd.NGAYGIAITRACH is null"));
        }

        public bool Sua_LenhHuy(string MaHDs, string Cat, string TinhTrang, string CreateBy)
        {
            //try
            //{
            //    string[] MaHD = MaHDs.Split(',');
            //    using (var scope = new TransactionScope())
            //    {
            //        for (int i = 0; i < MaHD.Length; i++)
            //        {
            //            string sql = "update TT_LenhHuy set Cat=,TinhTrang=N'',ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaHD="+MaHD[i];
            //            if (_cDAL.ExecuteNonQuery(sql) == false)
            //                return false;
            //        }
            //        scope.Complete();
            //        scope.Dispose();
            //    }
            //    return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            try
            {
                int flagCat = 0;
                if (bool.Parse(Cat) == true)
                    flagCat = 1;
                using (var scope = new TransactionScope())
                {
                    string sql = "update TT_LenhHuy set Cat=" + flagCat + ",TinhTrang=N'" + TinhTrang + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaHD in (" + MaHDs + ")";
                    if (_cDAL.ExecuteNonQuery(sql) == true)
                    {
                        scope.Complete();
                        scope.Dispose();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
