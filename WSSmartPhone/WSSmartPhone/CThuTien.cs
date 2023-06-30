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
using WSSmartPhone.LinQ;
using System.Web;
using System.Data.Odbc;
using System.Drawing;
using System.Globalization;

namespace WSSmartPhone
{
    class CThuTien
    {
        CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        dbThuTienDataContext _dbThuTien = new dbThuTienDataContext();
        CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        CConnection _cDAL_DocSo12 = new CConnection(CGlobalVariable.DocSo12);
        CConnection _cDAL_KinhDoanh = new CConnection(CGlobalVariable.KinhDoanh);
        CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TTKH);
        JavaScriptSerializer jss = new JavaScriptSerializer();

        public string DataTableToJSON(DataTable table)
        {
            jss.MaxJsonLength = Int32.MaxValue;
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
            return jss.Serialize(parentRow);
        }

        #region Thu Tiền

        public string GetVersion()
        {
            return _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select Version from TT_DeviceConfig").ToString();
        }

        private bool checkActiveMobile(string MaNV)
        {
            return (bool)_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select ActiveMobile from TT_NguoiDung where MaND=" + MaNV);
        }

        private bool checkHanhThu(string MaNV)
        {
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from TT_NguoiDung where DongNuoc=1 and MaND=" + MaNV);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private bool checkDongNuoc(string MaNV)
        {
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from TT_NguoiDung where DongNuoc=1 and MaND=" + MaNV);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private bool checkThuTien_HanhThu()
        {
            object result = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select ThuTien_HanhThu from TT_DeviceConfig");
            if (result != null)
                return (bool)result;
            else
                return false;
        }

        private bool checkThuTien_DongNuoc()
        {
            object result = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select ThuTien_DongNuoc from TT_DeviceConfig");
            if (result != null)
                return (bool)result;
            else
                return false;
        }

        private bool checkChotDangNgan(string NgayGiaiTrach)
        {
            if ((int)_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(*) from TT_ChotDangNgan where CAST(NgayChot as date)='" + NgayGiaiTrach + "' and Chot=1") > 0)
                return true;
            else
                return false;
        }

        public string DangNhaps(string Username, string Password, string IDMobile, string UID)
        {
            try
            {
                object MaNV = null;
                MaNV = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    MaNV = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

                if (MaNV == null || MaNV.ToString() == "")
                    return "false;Sai mật khẩu hoặc IDMobile";

                //xóa máy đăng nhập MaNV khác
                object MaNV_UID_Old = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
                if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
                    _cDAL_ThuTien.ExecuteNonQuery("delete TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

                //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                //{
                //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                //    }
                //}

                object MaNV_UID = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                if (MaNV_UID != null)
                    if ((int)MaNV_UID == 0)
                        _cDAL_ThuTien.ExecuteNonQuery("insert TT_DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
                    else
                        _cDAL_ThuTien.ExecuteNonQuery("update TT_DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

                _cDAL_ThuTien.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);

                return "true;" + DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Admin,HanhThu,DongNuoc,Doi,ToTruong,MaTo,DienThoai,Zalo,InPhieuBao,TestApp,SyncNopTien from TT_NguoiDung where MaND=" + MaNV));
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangNhaps_Admin(string Username, string Password, string IDMobile, string UID)
        {
            try
            {
                object MaNV = "";
                MaNV = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                //if (MaNV != "0" && MaNV != "1")
                //    MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0").ToString();

                if (MaNV == null || MaNV.ToString() == "")
                    return "false;Sai mật khẩu hoặc IDMobile";

                ////xóa máy đăng nhập MaNV khác
                //int MaNV_UID_Old = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where UID='" + UID + "'");
                //if (MaNV_UID_Old > 0)
                //    _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + UID + "'");

                //if (MaNV != "0" && MaNV != "1")
                //{
                //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                //    }
                //}

                //int MaNV_UID = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                //if (MaNV_UID == 0)
                //    _cDAL.ExecuteNonQuery("insert TT_DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");

                //_cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "' where MaND=" + MaNV);

                return "true;" + DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Admin,HanhThu,DongNuoc,Doi,ToTruong,MaTo,DienThoai,Zalo,InPhieuBao,TestApp,SyncNopTien from TT_NguoiDung where MaND=" + MaNV));
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangXuats(string Username, string UID)
        {
            try
            {
                //string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                //_cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL_ThuTien.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangXuats_Person(string Username, string UID)
        {
            try
            {
                object MaNV = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                if (MaNV != null)
                    _cDAL_ThuTien.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL_ThuTien.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangXuats_Admin(string Username, string UID)
        {
            try
            {
                //string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                //_cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                //return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
                return "true; ";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public bool UpdateUID(string MaNV, string UID)
        {
            return _cDAL_ThuTien.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);
        }

        public bool updateLogin(string MaNV, string UID)
        {
            return _cDAL_ThuTien.ExecuteNonQuery("update TT_DeviceSigned set ModifyDate=getdate() where UID='" + UID + "' and MaNV=" + MaNV);
        }

        public string getDS_To()
        {
            string sql = "select MaTo,TenTo,HanhThu,DongNuoc from TT_To where An=0";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien_HanhThu()
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and HanhThu=1 and DongNuoc=0 and An=0 and ActiveMobile=1 order by STT asc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien()
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and An=0 and ActiveMobile=1 order by STT asc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien(string MaTo)
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and MaTo=" + MaTo + " and An=0 and ActiveMobile=1 order by STT asc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string getDS_Nam()
        {
            string sql = "select * from ViewGetNamHD order by Nam desc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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


                var json = jss.Serialize(data);
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
                string serverKey = "AAAAYRLMnTg:APA91bH4MPTCqY4WntyOtKOo-DARDX3fIFXDihCdNyxRVzZilsP_pWE9kMYrWDyUjo-XGgX7IZuSzwz-zZYHgMyMLtTx9S3YdrSAwyqNgHjNaWehxtu5Usnd36q1lEo6e2zQRx7R6Wf3";
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
                        Title = Title,
                        Body = Content,
                        Action = Action,
                        NameUpdate = NameUpdate,
                        ValueUpdate = ValueUpdate,
                        ID = ID,
                    }
                };
                var json = jss.Serialize(data);
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
                responseMess = "Error: " + ex.Message;
            }
            return responseMess;
        }

        //hành thu
        public string getDSHoaDonTon_NhanVien(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select * from"
                            + " (select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end,CoDH"
                            + " ,GiaBieu=GB,DinhMuc=DM,DinhMucHN,CSC=CSCU,CSM=CSMOI,Code,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,PhiBVMT_Thue=case when ThueGTGT_TDVTN is null then 0 else ThueGTGT_TDVTN end,TongCong"
                            + " ,GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                //+ " ,ChoDCHD=case when exists(select ID_DIEUCHINH_HD from DIEUCHINH_HD where FK_HOADON=hd.ID_HOADON and TONGCONG_END is null) then 'true' else 'false' end"
                            + " ,ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end"
                            + " ,DangNgan_DienThoai"
                            + " ,MaNV_DangNgan,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_NgayHen,DCHD,TienDuTruoc_DCHD"
                            + " ,TBDongNuoc_Ngay=(select a.CreateDate from TT_DongNuoc a,TT_CTDongNuoc b where a.MaDN=b.MaDN and Huy=0 and b.MaHD=hd.ID_HOADON)"
                            + " ,PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA))"
                            + " ,PhiMoNuocThuHo=(select a.PhiMoNuoc from TT_DichVuThuTong a,TT_DichVuThu b where b.MaHD=hd.ID_HOADON and a.ID=b.IDDichVu)"
                            + " ,MaKQDN=(select MaKQDN from TT_DongNuoc a,TT_KQDongNuoc b where a.Huy=0 and b.DanhBo=hd.DANHBA and b.MoNuoc=0 and b.TroNgaiMN=0 and a.MaDN=b.MaDN)"
                            + " ,DongPhi =(select DongPhi from TT_DongNuoc a,TT_KQDongNuoc b where a.Huy=0 and b.DanhBo=hd.DANHBA and b.MoNuoc=0 and b.TroNgaiMN=0 and a.MaDN=b.MaDN)"
                            + " ,LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,LenhHuyCat=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON and Cat=1) then 'true' else 'false' end"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=hd.DANHBA)"
                            + " ,DongA=case when exists(select DanhBo from TT_DuLieuKhachHang_DanhBo where DanhBo=hd.DANHBA) then 'true' else 'false' end"
                            + " ,CuaHangThuHo1,CuaHangThuHo2,ChiTietTienNuoc"
                            + " from HOADON hd"
                            + " where (NAM<" + Nam + " or (NAM=" + Nam + " and Ky<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=" + MaNV
                            + " and (NGAYGIAITRACH is null or CAST(NGAYGIAITRACH as date)=CAST(GETDATE() as date))"
                //+ " and (GB=10 and (NAM>2021 or (NAM=2021 and Ky<6)))"
                            + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                //+ " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and UpdatedHDDT=0 and ID_HOADON=FK_HOADON)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and SoPhieu is null)"
                //+ " and hd.ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and (SoPhieu is null or CAST(Ngay_DC as date)<'20220701' or (NAM<2022 or (NAM=2022 and KY<5))))"
                            + " )t1"
                //+ "  where DanhBo not in (select DanhBo from KTKS_DonKH.dbo.DonTu_ChiTiet dtct where ChanHoaDon=1 and not exists(select ID from KTKS_DonKH.dbo.DonTu_LichSu where MaDon=dtct.MaDon and (ID_NoiNhan=20 or (ID_NoiChuyen=6 and IDCT is not null))))"
                            + " order by MLT asc,MaHD desc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string getDSHoaDonTon_May(string MaNV, string Nam, string Ky, string FromDot, string ToDot, string TuMay, string DenMay)
        {
            string sql = "select * from"
                            + " (select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end,CoDH"
                            + " ,GiaBieu=GB,DinhMuc=DM,DinhMucHN,CSC=CSCU,CSM=CSMOI,Code,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,PhiBVMT_Thue=case when ThueGTGT_TDVTN is null then 0 else ThueGTGT_TDVTN end,TongCong"
                            + " ,GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                //+ " ,ChoDCHD=case when exists(select ID_DIEUCHINH_HD from DIEUCHINH_HD where FK_HOADON=hd.ID_HOADON and TONGCONG_END is null) then 'true' else 'false' end"
                            + " ,ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end"
                            + " ,DangNgan_DienThoai"
                            + " ,MaNV_DangNgan,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_NgayHen,DCHD,TienDuTruoc_DCHD"
                            + " ,TBDongNuoc_Ngay=(select a.CreateDate from TT_DongNuoc a,TT_CTDongNuoc b where a.MaDN=b.MaDN and Huy=0 and b.MaHD=hd.ID_HOADON)"
                            + " ,PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA))"
                            + " ,PhiMoNuocThuHo=(select a.PhiMoNuoc from TT_DichVuThuTong a,TT_DichVuThu b where b.MaHD=hd.ID_HOADON and a.ID=b.IDDichVu)"
                            + " ,MaKQDN=(select MaKQDN from TT_DongNuoc a,TT_KQDongNuoc b where a.Huy=0 and b.DanhBo=hd.DANHBA and b.MoNuoc=0 and b.TroNgaiMN=0 and a.MaDN=b.MaDN)"
                            + " ,DongPhi =(select DongPhi from TT_DongNuoc a,TT_KQDongNuoc b where a.Huy=0 and b.DanhBo=hd.DANHBA and b.MoNuoc=0 and b.TroNgaiMN=0 and a.MaDN=b.MaDN)"
                            + " ,LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,LenhHuyCat=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON and Cat=1) then 'true' else 'false' end"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=hd.DANHBA)"
                            + " ,DongA=case when exists(select DanhBo from TT_DuLieuKhachHang_DanhBo where DanhBo=hd.DANHBA) then 'true' else 'false' end"
                            + " ,CuaHangThuHo1,CuaHangThuHo2,ChiTietTienNuoc"
                            + " from HOADON hd"
                            + " where (NAM<" + Nam + " or (NAM=" + Nam + " and Ky<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=" + MaNV + " and MAY>=" + TuMay + " and MAY<=" + DenMay
                            + " and (NGAYGIAITRACH is null or CAST(NGAYGIAITRACH as date)=CAST(GETDATE() as date))"
                //+ " and (GB=10 and (NAM>2021 or (NAM=2021 and Ky<6)))"
                            + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                //+ " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and UpdatedHDDT=0 and ID_HOADON=FK_HOADON)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and SoPhieu is null)"
                //+ " and hd.ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and (SoPhieu is null or CAST(Ngay_DC as date)<'20220701' or (NAM<2022 or (NAM=2022 and KY<5))))"
                            + " )t1"
                //+ "  where DanhBo not in (select DanhBo from KTKS_DonKH.dbo.DonTu_ChiTiet dtct where ChanHoaDon=1 and not exists(select ID from KTKS_DonKH.dbo.DonTu_LichSu where MaDon=dtct.MaDon and (ID_NoiNhan=20 or (ID_NoiChuyen=6 and IDCT is not null))))"
                            + " order by MLT asc,MaHD desc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string XuLy_HoaDonDienTu(string LoaiXuLy, string MaNV, string MaHDs, DateTime Ngay, DateTime NgayHen, string MaKQDN, bool XoaDCHD, string Location)
        {
            try
            {
                string sql = "";
                if (LoaiXuLy != "DongPhi" || LoaiXuLy != "XoaDongPhi")
                {
                    string sqlCheck = "select MaHD=ID_HOADON,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),"
                                        + " GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end,"
                                        + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                                        + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                                        + " from HOADON hd where ID_HOADON in (" + MaHDs + ")";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sqlCheck);
                    switch (LoaiXuLy)
                    {
                        case "XoaDangNgan":
                            if (checkQuyenXoa(MaNV) == false)
                                return "false;Tính năng này tạm thời ẩn";
                            foreach (DataRow item in dt.Rows)
                            {
                                if (bool.Parse(item["GiaiTrach"].ToString()) == false)
                                    return "false;Kỳ " + item["Ky"].ToString() + " chưa Giải Trách;GiaiTrach;false;" + item["MaHD"].ToString();
                            }
                            break;
                        default:
                            foreach (DataRow item in dt.Rows)
                            {
                                if (bool.Parse(item["ThuHo"].ToString()) == true)
                                    return "false;Kỳ " + item["Ky"].ToString() + " đã Thu Hộ;ThuHo;true;" + item["MaHD"].ToString();
                                else
                                    if (bool.Parse(item["TamThu"].ToString()) == true)
                                        return "false;Kỳ " + item["Ky"].ToString() + " đã Tạm Thu;TamThu;true;" + item["MaHD"].ToString();
                                    else
                                        if (bool.Parse(item["GiaiTrach"].ToString()) == true)
                                            return "false;Kỳ " + item["Ky"].ToString() + " đã Giải Trách;GiaiTrach;true;" + item["MaHD"].ToString();
                            }
                            break;
                    }
                }

                switch (LoaiXuLy)
                {
                    case "DangNgan":
                        if (checkThuTien_HanhThu() == false)
                        {
                            if (checkThuTien_DongNuoc() == false)
                                return "false;Tính năng đã bị Khóa";
                            else
                                if (checkDongNuoc(MaNV) == false)
                                    return "false;Tính năng đã bị Khóa";
                        }
                        if (checkActiveMobile(MaNV) == false)
                            return "false;Chưa Active Mobile";
                        if (checkChotDangNgan(Ngay.ToString("yyyyMMdd")) == true)
                        {
                            Ngay = Ngay.AddDays(1);
                            TimeSpan ts = new TimeSpan(1, 0, 0);
                            Ngay = Ngay.Date + ts;
                            //return "false;Đã Chốt Ngày Giải Trách";
                        }
                        if (bool.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("if not exists(select * from TT_DeviceConfig where checkUpdatedHDDT=1)"
                                                                    + " 	select 'true'"
                                                                    + " else"
                                                                    + " if exists(select ID_HOADON from HOADON where ID_HOADON=" + MaHDs + " and (NAM<2020 or (NAM=2020 and KY<=6)))"
                                                                    + "	select 'true'"
                                                                    + " else"
                                                                    + "	if not exists(select FK_HOADON from DIEUCHINH_HD where FK_HOADON=" + MaHDs + ")"
                                                                    + "		select 'true'"
                                                                    + "	else"
                                                                    + "		if exists(select FK_HOADON from DIEUCHINH_HD where FK_HOADON=" + MaHDs + " and UpdatedHDDT=1)"
                                                                    + "			select 'true'"
                                                                    + "		else"
                                                                    + "			select 'false'").ToString()) == false)
                            return "false;Hóa Đơn có Điều Chỉnh nhưng chưa update HĐĐT";

                        sql += " update HOADON set DangNgan_DienThoai=1,DangNgan_DienThoai_Location='" + Location + "',XoaDangNgan_MaNV_DienThoai=NULL,XoaDangNgan_Ngay_DienThoai=NULL,DangNgan_Ton=1,MaNV_DangNgan=" + MaNV + ",NGAYGIAITRACH='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate()";
                        if (XoaDCHD == true)
                        {
                            sql += ",DCHD=0,TONGCONG=TongCongTruoc_DCHD,TongCongTruoc_DCHD=NULL,TienDuTruoc_DCHD=NULL";
                        }
                        sql += " where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "DongPhi":
                        //return "false;Tính năng đã bị Khóa";
                        //if (checkActiveMobile(MaNV) == false)
                        //    return "false,Chưa Active Mobile";
                        //if (checkChotDangNgan(Ngay) == true)
                        //    return "false,Đã Chốt Ngày Giải Trách";
                        sql += " update TT_KQDongNuoc set DongPhi=1,MaNV_DongPhi=" + MaNV + ",NgayDongPhi='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where MaKQDN=" + MaKQDN + " and NgayDongPhi is null ";
                        break;
                    case "PhieuBao":
                        sql += " update HOADON set InPhieuBao_MaNV=" + MaNV + ",InPhieuBao_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and MaNV_DangNgan is null and InPhieuBao_Ngay is null ";
                        break;
                    case "PhieuBao2":
                        sql += " update HOADON set InPhieuBao2_MaNV=" + MaNV + ",InPhieuBao2_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao2_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao2_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and MaNV_DangNgan is null and InPhieuBao2_Ngay is null ";
                        break;
                    case "TBDongNuoc":
                        //insert table TBDongNuoc
                        if ((int)_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(*) from TT_DongNuoc a,TT_CTDongNuoc b where a.Huy=0 and b.MaHD in (" + MaHDs + ") and a.MaDN=b.MaDN") > 0)
                            return "false,Hóa Đơn đã Lập TB Đóng Nước";
                        else
                        {
                            List<HOADON> lstHDTemp = new List<HOADON>();
                            string[] strMaHDs = MaHDs.Split(',');
                            foreach (string item in strMaHDs)
                            {
                                lstHDTemp.Add(getHoaDon(int.Parse(item)));
                            }
                            TT_DongNuoc dongnuoc = new TT_DongNuoc();
                            dongnuoc.DanhBo = lstHDTemp[0].DANHBA;
                            dongnuoc.HoTen = lstHDTemp[0].TENKH;
                            dongnuoc.DiaChi = lstHDTemp[0].SO + " " + lstHDTemp[0].DUONG + getPhuongQuan(lstHDTemp[0].DANHBA);
                            dongnuoc.MLT = lstHDTemp[0].MALOTRINH;
                            foreach (HOADON item in lstHDTemp)
                            {
                                TT_CTDongNuoc ctdongnuoc = new TT_CTDongNuoc();
                                ctdongnuoc.MaDN = dongnuoc.MaDN;
                                ctdongnuoc.MaHD = item.ID_HOADON;
                                ctdongnuoc.SoHoaDon = item.SOHOADON;
                                ctdongnuoc.Ky = item.KY + "/" + item.NAM;
                                ctdongnuoc.TieuThu = (int)item.TIEUTHU;
                                ctdongnuoc.GiaBan = (int)item.GIABAN;
                                ctdongnuoc.ThueGTGT = (int)item.THUE;
                                ctdongnuoc.PhiBVMT = (int)item.PHI;
                                ctdongnuoc.TongCong = (int)item.TONGCONG;
                                ctdongnuoc.CreateBy = int.Parse(MaNV);
                                ctdongnuoc.CreateDate = DateTime.Now;

                                dongnuoc.TT_CTDongNuocs.Add(ctdongnuoc);
                            }
                            if (ThemDN(dongnuoc, int.Parse(MaNV)) == false)
                                return "false,Lỗi Lập TB Đóng Nước";
                        }
                        sql += " update HOADON set TBDongNuoc_MaNV=" + MaNV + ",TBDongNuoc_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',TBDongNuoc_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',TBDongNuoc_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and MaNV_DangNgan is null and TBDongNuoc_Ngay is null ";
                        break;
                    case "XoaDangNgan":
                        //return "false;Tính năng đã bị Khóa";
                        if (checkQuyenXoa(MaNV) == false)
                            return "false;Tính năng này tạm thời ẩn";
                        if (checkActiveMobile(MaNV) == false)
                            return "false;Chưa Active Mobile";
                        if (checkChotDangNgan(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select convert(varchar, NGAYGIAITRACH, 112) from HOADON where ID_HOADON=" + MaHDs).ToString()) == true)
                            return "false;Đã Chốt Ngày Giải Trách";
                        sql += " update HOADON set XoaDangNgan_MaNV_DienThoai=" + MaNV + ",XoaDangNgan_Ngay_DienThoai='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',XoaDangNgan_Location_DienThoai='" + Location + "',DangNgan_DienThoai=0,DangNgan_Ton=0,MaNV_DangNgan=NULL,NGAYGIAITRACH=NULL,ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is not null ";
                        break;
                    case "XoaDongPhi":
                        //return "false;Tính năng đã bị Khóa";
                        if (checkQuyenXoa(MaNV) == false)
                            return "false;Tính năng này tạm thời ẩn";
                        //if (checkActiveMobile(MaNV) == false)
                        //    return "false,Chưa Active Mobile";
                        //if (checkChotDangNgan(Ngay) == true)
                        //    return "false,Đã Chốt Ngày Giải Trách";
                        sql += " update TT_KQDongNuoc set DongPhi=0,MaNV_DongPhi=NULL,NgayDongPhi=NULL,ModifyBy=" + MaNV + ",ModifyDate=getDate() where MaKQDN=" + MaKQDN + " and NgayDongPhi is not null ";
                        break;
                    default:
                        break;
                }

                //if (_cDAL.ExecuteNonQuery(sql) == true)
                //{
                //    if (XoaDCHD == true)
                //    {
                //        if (_cDAL.ExecuteNonQuery("insert into TT_DieuChinhTienDuXoa(MaHD,CreateBy,CreateDate)values(" + MaHDs + "," + MaNV + ",getdate())") == true)
                //            return "true; ";
                //        else
                //            return "false; ";
                //    }
                //    else
                //        return "true; ";
                //}
                //else
                //    return "false;error query";
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                    if (_cDAL_ThuTien.ExecuteNonQuery(sql) == true)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("insert into TT_Location_NhanVien(MaNV,Location,Action)values(" + MaNV + ",'" + Location + "','" + LoaiXuLy + "')");
                        if (XoaDCHD == true)
                        {
                            if (_cDAL_ThuTien.ExecuteNonQuery("insert into TT_DieuChinhTienDuXoa(MaHD,CreateBy,CreateDate)values(" + MaHDs + "," + MaNV + ",getdate())") == true)
                            {
                                scope.Complete();
                                scope.Dispose();
                                return "true; ";
                            }
                            else
                                return "false; ";
                        }
                        else
                        {
                            scope.Complete();
                            scope.Dispose();
                            return "true; ";
                        }
                    }
                    else
                        return "false;error query";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string get_GhiChu(string DanhBo)
        {
            string sql = "select DanhBo,DienThoai,GiaBieu,NiemChi,DiemBe from TT_GhiChu where DanhBo='" + DanhBo + "'";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string update_GhiChu(string MaNV, string DanhBo, string DienThoai, string GiaBieu, string NiemChi, string DiemBe)
        {
            try
            {
                if (checkActiveMobile(MaNV) == false)
                    return "false;Chưa Active Mobile";
                TT_GhiChu gc = _dbThuTien.TT_GhiChus.SingleOrDefault(item => item.DanhBo == DanhBo);
                if (gc == null)
                {
                    TT_GhiChu en = new TT_GhiChu();
                    en.DanhBo = DanhBo;
                    en.DienThoai = DienThoai;
                    en.CreateBy = int.Parse(MaNV);
                    en.CreateDate = DateTime.Now;

                    if (GiaBieu != "")
                    {
                        en.GiaBieu = GiaBieu;
                        en.GiaBieu_CreateBy = int.Parse(MaNV);
                        en.GiaBieu_Ngay = DateTime.Now;
                    }
                    if (NiemChi != "")
                    {
                        en.NiemChi = NiemChi;
                        en.NiemChi_CreateBy = int.Parse(MaNV);
                        en.NiemChi_Ngay = DateTime.Now;
                    }
                    if (DiemBe != "")
                    {
                        en.DiemBe = DiemBe;
                        en.DiemBe_CreateBy = int.Parse(MaNV);
                        en.DiemBe_Ngay = DateTime.Now;
                    }

                    _dbThuTien.TT_GhiChus.InsertOnSubmit(en);
                    _dbThuTien.SubmitChanges();
                }
                else
                {
                    gc.DienThoai = DienThoai;
                    gc.ModifyBy = int.Parse(MaNV);
                    gc.ModifyDate = DateTime.Now;

                    if (GiaBieu != "" && GiaBieu != gc.GiaBieu)
                    {
                        gc.GiaBieu = GiaBieu;
                        gc.GiaBieu_CreateBy = int.Parse(MaNV);
                        gc.GiaBieu_Ngay = DateTime.Now;
                    }
                    if (NiemChi != "" && NiemChi != gc.NiemChi)
                    {
                        gc.NiemChi = NiemChi;
                        gc.NiemChi_CreateBy = int.Parse(MaNV);
                        gc.NiemChi_Ngay = DateTime.Now;
                    }
                    if (DiemBe != "" && DiemBe != gc.DiemBe)
                    {
                        gc.DiemBe = DiemBe;
                        gc.DiemBe_CreateBy = int.Parse(MaNV);
                        gc.DiemBe_Ngay = DateTime.Now;
                    }

                    _dbThuTien.SubmitChanges();
                }
                return "true;";
                //string sql = "if not exists (select 1 from TT_GhiChu where DanhBo='" + DanhBo + "')"
                //            + " 	insert into TT_GhiChu(DanhBo,DienThoai,GiaBieu,NiemChi,DiemBe,CreateBy,CreateDate)values('" + DanhBo + "','" + DienThoai + "',N'" + GiaBieu + "',N'" + NiemChi + "',N'" + DiemBe + "'," + MaNV + ",GETDATE());"
                //            + " else"
                //            + " 	update TT_GhiChu set DienThoai='" + DienThoai + "',GiaBieu=N'" + GiaBieu + "',NiemChi=N'" + NiemChi + "',DiemBe=N'" + DiemBe + "',ModifyBy=" + MaNV + ",ModifyDate=GETDATE() where DanhBo='" + DanhBo + "';";
                //return _cDAL.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string update_DiaChiDHN(string MaNV, string DanhBo, string DiaChiDHN)
        {
            try
            {
                if (checkActiveMobile(MaNV) == false)
                    return "false;Chưa Active Mobile";
                string sql = "if exists (select DanhBo from TT_DiaChiDHN where DanhBo='" + DanhBo + "')"
                                + " 	update TT_DiaChiDHN set DiaChi=N'" + DiaChiDHN + "',ModifyDate=GETDATE() where DanhBo='" + DanhBo + "'"
                                + " else"
                                + " 	insert into TT_DiaChiDHN(DanhBo,DiaChi,ModifyDate)values('" + DanhBo + "',N'" + DiaChiDHN + "',GETDATE())";
                if (_cDAL_ThuTien.ExecuteNonQuery(sql) == true)
                {
                    return "true; ";
                }
                else
                    return "false;error query";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public bool checkQuyenXoa(string MaNV)
        {
            string sql = "select case when (select ToTruong from TT_NguoiDung where MaND=" + MaNV + ")=1 then 'true' else"
                        + " case when (select Doi from TT_NguoiDung where MaND=" + MaNV + ")=1 then 'true' else 'false' end end";
            return bool.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue(sql).ToString());
        }

        //tạm thu
        public string GetDSTamThu(bool RutSot, string MaNV, DateTime FromCreateDate, DateTime ToCreateDate)
        {
            string sql = "";
            if (RutSot == true)
                sql = "if((select DongNuoc from TT_NguoiDung where MaND=" + MaNV + ")=0)"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt where MaNV_HanhThu=" + MaNV + " and NGAYGIAITRACH is null"
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and (hd.NAM<2020 or (hd.NAM=2020 and hd.KY<7))"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON"
                        + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.Huy=0 and dn.MaDN=ctdn.MaDN)"
                    + " else"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt,TT_DongNuoc dn,TT_CTDongNuoc ctdn where MaNV_DongNuoc=" + MaNV + " and NGAYGIAITRACH is null"
                        + " and (hd.NAM<2020 or (hd.NAM=2020 and hd.KY<7))"
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON and dn.Huy=0 and dn.MaDN=ctdn.MaDN and hd.ID_HOADON=ctdn.MaHD";
            else
                sql = "if((select DongNuoc from TT_NguoiDung where MaND=" + MaNV + ")=0)"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt where MaNV_HanhThu=" + MaNV
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and (hd.NAM<2020 or (hd.NAM=2020 and hd.KY<7))"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON"
                        + " and ID_HOADON not in (select ctdn.MaHD from TT_DongNuoc dn,TT_CTDongNuoc ctdn where dn.Huy=0 and dn.MaDN=ctdn.MaDN)"
                    + " else"
                        + " select ID=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),hd.TongCong,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,GiaBieu=GB"
                        + " from HOADON hd,TAMTHU tt,TT_DongNuoc dn,TT_CTDongNuoc ctdn where MaNV_DongNuoc=" + MaNV
                        + " and CAST(tt.CreateDate as DATE)>='" + FromCreateDate.ToString("yyyyMMdd") + "' and CAST(tt.CreateDate as DATE)<='" + ToCreateDate.ToString("yyyyMMdd") + "'"
                        + " and (hd.NAM<2020 or (hd.NAM=2020 and hd.KY<7))"
                        + " and tt.ChuyenKhoan=1 and hd.ID_HOADON=tt.FK_HOADON and dn.Huy=0 and dn.MaDN=ctdn.MaDN and hd.ID_HOADON=ctdn.MaHD";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        //đóng nước
        public string GetDSDongNuoc_old(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " kqdn.DongNuoc,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.LyDo,kqdn.MoNuoc,kqdn.NgayMN,kqdn.ChiSoMN,GiaiTrach='false',ThuHo='false',TamThu='false',HoaDon=''"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " order by dn.MLT";
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(query);
            foreach (DataRow item in dt.Rows)
            {
                DataTable dtCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select ctdn.Ky,hd.TongCong,hd.NgayGiaiTrach from TT_CTDongNuoc ctdn,HOADON hd where MaDN=" + item["ID"].ToString() + " and ctdn.MaHD=hd.ID_HOADON");
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

        public string GetDSDongNuoc(string MaNV_DongNuoc)
        {
            string query = "select ID=dn.MaDN,dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,dn.CreateDate"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=dn.DanhBo)"
                            + " ,Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,DongNuoc=case when kqdn.DongNuoc is null then 'false' else case when kqdn.DongNuoc=1 then 'true' else 'false' end end"
                            + " ,DongNuoc2=case when kqdn.DongNuoc2 is null then 'false' else case when kqdn.DongNuoc2=1 then 'true' else 'false' end end"
                            + " ,MoNuoc=case when kqdn.MoNuoc is null then 'false' else case when kqdn.MoNuoc=1 then 'true' else 'false' end end"
                            + " ,DongPhi=case when kqdn.DongPhi is null then 'false' else case when kqdn.DongPhi=1 then 'true' else 'false' end end"
                            + " ,ButChi=case when kqdn.ButChi is null then 'false' else case when kqdn.ButChi=1 then 'true' else 'false' end end"
                            + " ,KhoaTu=case when kqdn.KhoaTu is null then 'false' else case when kqdn.KhoaTu=1 then 'true' else 'false' end end"
                            + " ,KhoaKhac=case when kqdn.KhoaKhac is null then 'false' else case when kqdn.KhoaKhac=1 then 'true' else 'false' end end"
                            + " ,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.NiemChi,kqdn.MauSac,kqdn.KhoaKhac_GhiChu,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.ViTri,kqdn.LyDo,kqdn.NgayDN1,kqdn.ChiSoDN1,kqdn.NiemChi1,kqdn.MauSac1,kqdn.NgayMN,kqdn.ChiSoMN,kqdn.NiemChiMN,kqdn.MauSacMN,kqdn.MaKQDN"
                            + " ,CuaHangThuHo1=(select top 1 CuaHangThuHo1 from HOADON where DANHBA=dn.DanhBo order by ID_HOADON desc)"
                            + " ,CuaHangThuHo2=(select top 1 CuaHangThuHo2 from HOADON where DANHBA=dn.DanhBo order by ID_HOADON desc)"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc
                            + " and exists(select * from HOADON a,TT_CTDongNuoc b where a.ID_HOADON=b.MaHD and b.MaDN=dn.MaDN)"
                            + " and (select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and ChuyenNoKhoDoi=0 and (NGAYGIAITRACH is null or CAST(NGAYGIAITRACH as DATE)=CAST(getdate() as DATE)))>0"
                            + " and (kqdn.MaDN is null or ((kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and TroNgaiMN=0) or (CAST(kqdn.NgayMN as date)=CAST(GETDATE() as date))))"
                            + " order by dn.MLT";
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public string GetDSDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,dn.CreateDate"
                            + " DiaChiDHN=(select [SONHA]+' '+[TENDUONG] FROM [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] where DanhBo=dn.DanhBo),"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " DongNuoc=case when kqdn.DongNuoc is null then 'false' else case when kqdn.DongNuoc=1 then 'true' else 'false' end end,"
                            + " DongNuoc2=case when kqdn.DongNuoc2 is null then 'false' else case when kqdn.DongNuoc2=1 then 'true' else 'false' end end,"
                            + " MoNuoc=case when kqdn.MoNuoc is null then 'false' else case when kqdn.MoNuoc=1 then 'true' else 'false' end end,"
                            + " DongPhi=case when kqdn.DongPhi is null then 'false' else case when kqdn.DongPhi=1 then 'true' else 'false' end end,"
                            + " ButChi=case when kqdn.ButChi is null then 'false' else case when kqdn.ButChi=1 then 'true' else 'false' end end,"
                            + " KhoaTu=case when kqdn.KhoaTu is null then 'false' else case when kqdn.KhoaTu=1 then 'true' else 'false' end end,"
                            + " KhoaKhac=case when kqdn.KhoaKhac is null then 'false' else case when kqdn.KhoaKhac=1 then 'true' else 'false' end end,"
                            + " ,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.NiemChi,kqdn.MauSac,kqdn.KhoaKhac_GhiChu,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.ViTri,kqdn.LyDo,kqdn.NgayDN1,kqdn.ChiSoDN1,kqdn.NiemChi1,kqdn.MauSac1,kqdn.NgayMN,kqdn.ChiSoMN,kqdn.NiemChiMN,kqdn.MauSacMN,kqdn.MaKQDN"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=dn.DanhBo)"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0 or CAST(kqdn.NgayMN as DATE)=CAST(getdate() as DATE))"
                            + " order by dn.MLT";
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public string GetDSCTDongNuoc(string MaNV_DongNuoc)
        {
            string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,MLT=MALOTRINH,ctdn.Ky,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end"
                            + " ,GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,hd.Code,hd.TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),hd.GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,PhiBVMT_Thue=case when ThueGTGT_TDVTN is null then 0 else ThueGTGT_TDVTN end,hd.TongCong,hd.DCHD,hd.TienDuTruoc_DCHD,hd.ChiTietTienNuoc"
                            + " ,DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen"
                            + " ,GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                //+ " ,ChoDCHD=case when exists(select ID_DIEUCHINH_HD from DIEUCHINH_HD where FK_HOADON=ctdn.MaHD and TONGCONG_END is null) then 'true' else 'false' end"
                            + " ,PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA))"
                            + " ,PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong a,TT_DichVuThu b where b.MaHD=ctdn.MaHD and a.ID=b.IDDichVu)"
                            + " ,LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                            + " from TT_DongNuoc dn"
                            + " left join TT_CTDongNuoc ctdn on dn.MaDN=ctdn.MaDN"
                            + " left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " left join HOADON hd on hd.ID_HOADON=ctdn.MaHD"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc
                            + " and exists(select * from HOADON a,TT_CTDongNuoc b where a.ID_HOADON=b.MaHD and b.MaDN=dn.MaDN)"
                            + " and (select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and ChuyenNoKhoDoi=0 and (NGAYGIAITRACH is null or CAST(NGAYGIAITRACH as DATE)=CAST(getdate() as DATE)))>0"
                            + " and (kqdn.MaDN is null or ((kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and TroNgaiMN=0) or (CAST(kqdn.NgayMN as date)=CAST(GETDATE() as date))))"
                            + " order by dn.MLT asc,ctdn.MaHD desc";
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);

            return DataTableToJSON(dt);
        }

        public string GetDSCTDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,MLT=MALOTRINH,ctdn.Ky,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                            + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,hd.Code,hd.TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),hd.GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,PhiBVMT_Thue=case when ThueGTGT_TDVTN is null then 0 else ThueGTGT_TDVTN end,hd.TongCong,hd.DCHD,hd.TienDuTruoc_DCHD,hd.ChiTietTienNuoc"
                            + " DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen,"
                            + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
                //+ " ,ChoDCHD=case when exists(select ID_DIEUCHINH_HD from DIEUCHINH_HD where FK_HOADON=ctdn.MaHD and TONGCONG_END is null) then 'true' else 'false' end"
                            + " PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA)),"
                            + " PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong a,TT_DichVuThu b where b.MaHD=ctdn.MaHD and a.ID=b.IDDichVu),"
                            + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=ctdn.MaHD) then 'true' else 'false' end"
                            + " from TT_DongNuoc dn"
                            + " left join TT_CTDongNuoc ctdn on dn.MaDN=ctdn.MaDN"
                            + " left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " left join HOADON hd on hd.ID_HOADON=ctdn.MaHD"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(dn.NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(dn.NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0 or CAST(kqdn.NgayMN as DATE)=CAST(getdate() as DATE))"
                            + " order by dn.MLT asc,ctdn.MaHD desc";
            DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);

            return DataTableToJSON(dt);
        }

        public bool CheckExist_DongNuoc(string MaDN)
        {
            if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where DongNuoc=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            return true;
        }

        public bool CheckExist_DongNuoc2(string MaDN)
        {
            if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where DongNuoc2=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            return true;
        }

        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, DateTime NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string MauSac, string KhoaKhac, string KhoaKhac_GhiChu, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string ViTri, string LyDo, string CreateBy)
        {
            int flagButChi = 0;
            int flagKhoaTu = 0;
            int flagKhoaKhac = 0;
            if (bool.Parse(KhoaTu) == false && bool.Parse(KhoaKhac) == false)
            {
                if (NiemChi == "")
                    return false;
                if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "'").ToString()) == 0)
                    return false;
                if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "' and SuDung=1").ToString()) == 1)
                    return false;
            }
            else
            {
                NiemChi = "NULL";
                MauSac = "NULL";
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
            if (NiemChi != "NULL")
                NiemChi = "N'" + NiemChi + "'";
            if (MauSac != "NULL")
                MauSac = "N'" + MauSac + "'";
            if (KhoaKhac_GhiChu != "NULL")
                KhoaKhac_GhiChu = "N'" + KhoaKhac_GhiChu + "'";
            try
            {
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    int MaKQDN = (int)_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select case when (select COUNT(MaKQDN) from TT_KQDongNuoc)=0 then 1 else (select MAX(MaKQDN) from TT_KQDongNuoc)+1 end");
                    //insert đóng nước
                    //string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,HinhDN,NgayDN,NgayDN_ThucTe,ChiSoDN,ButChi,KhoaTu,NiemChi,KhoaKhac,KhoaKhac_GhiChu,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,ViTri,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                    //         + "" + MaKQDN + "," + MaDN + ",'" + DanhBo.Replace(" ", "") + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1,@HinhDN"
                    //         + ",'" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',getDate()," + ChiSoDN + "," + flagButChi + "," + flagKhoaTu + "," + NiemChi + "," + flagKhoaKhac + ",N'" + KhoaKhac_GhiChu + "','" + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + ViTri + "',N'" + LyDo + "',(select PhiMoNuoc from TT_CacLoaiPhi where CoDHN like '%" + Co + "%')," + CreateBy + ",getDate())";
                    string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,NgayDN,NgayDN_ThucTe,ChiSoDN,ButChi,KhoaTu,NiemChi,MauSac,KhoaKhac,KhoaKhac_GhiChu,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,ViTri,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                             + "" + MaKQDN + "," + MaDN + ",'" + DanhBo.Replace(" ", "") + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1,'" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',getDate()"
                             + "," + ChiSoDN + "," + flagButChi + "," + flagKhoaTu + "," + NiemChi + "," + MauSac + "," + flagKhoaKhac + "," + KhoaKhac_GhiChu + ",'"
                             + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + ViTri + "',N'" + LyDo + "',(select PhiMoNuoc from TT_CacLoaiPhi where CoDHN like '%" + Co + "%')," + CreateBy + ",getDate())";

                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhDN == "NULL")
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

                    if (_cDAL_ThuTien.ExecuteNonQuery(command) == true)
                    {
                        //string NoiDung = "Đóng Nước, ngày " + NgayDN.ToString("dd/MM/yyyy") + ", CS: " + ChiSoDN + ", " + LyDo;
                        //sql = "insert into TB_GHICHU(DANHBO,DONVI,NOIDUNG,CREATEDATE,CREATEBY)values('" + DanhBo.Replace(" ", "") + "',N'DTT',N'" + NoiDung + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "',N'" + _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select HoTen from TT_NguoiDung where MaND=" + CreateBy).ToString() + "')";
                        //_cDAL_DHN.ExecuteNonQuery(sql);
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
                                string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc,Hinh,CreateBy,CreateDate)"
                                    + " values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end)," + MaKQDN + ",1,@Hinh," + CreateBy + ",getDate())";
                                command = new SqlCommand(sql_Hinh);
                                command.Parameters.Add("@Hinh", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDNs[i]);
                                _cDAL_ThuTien.ExecuteNonQuery(command);
                            }
                        }
                        //insert niêm chì
                        if (NiemChi != "NULL")
                        {
                            string sqlNiemChi = "update TT_NiemChi set SuDung=1,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where ID=" + NiemChi + " and SuDung=0";

                            if (_cDAL_ThuTien.ExecuteNonQuery(sqlNiemChi) == true)
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

            return _cDAL_ThuTien.ExecuteNonQuery(command);
        }

        public bool ThemDongNuoc2(string MaDN, string HinhDN, DateTime NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string MauSac, string KhoaKhac, string KhoaKhac_GhiChu, string CreateBy)
        {
            int flagButChi = 0;
            int flagKhoaTu = 0;
            int flagKhoaKhac = 0;
            if (bool.Parse(KhoaTu) == false && bool.Parse(KhoaKhac) == false)
            {
                if (NiemChi == "")
                    return false;
                if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "'").ToString()) == 0)
                    return false;
                if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "' and SuDung=1").ToString()) == 1)
                    return false;
            }
            else
            {
                NiemChi = "NULL";
                MauSac = "NULL";
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
            if (NiemChi != "NULL")
                NiemChi = "N'" + NiemChi + "'";
            if (MauSac != "NULL")
                MauSac = "N'" + MauSac + "'";
            if (KhoaKhac_GhiChu != "NULL")
                KhoaKhac_GhiChu = "N'" + KhoaKhac_GhiChu + "'";
            if (bool.Parse(ButChi) == true)
                flagButChi = 1;

            try
            {
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
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
                    string sql = "update TT_KQDongNuoc set DongNuoc2=1,PhiMoNuoc=PhiMoNuoc*2,NgayDN1=NgayDN,NgayDN1_ThucTe=NgayDN_ThucTe,ChiSoDN1=ChiSoDN,NiemChi1=NiemChi,MauSac1=MauSac"
                               + ",NgayDN='" + NgayDN.ToString("yyyyMMdd HH:mm:ss") + "',NgayDN_ThucTe=getDate(),ChiSoDN=" + ChiSoDN + ",ButChi=" + flagButChi + ",KhoaTu=" + flagKhoaTu
                               + ",NiemChi=" + NiemChi + ",MauSac=" + MauSac + ",KhoaKhac=" + flagKhoaKhac + ",KhoaKhac_GhiChu=" + KhoaKhac_GhiChu + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate()"
                               + ",SoPhieuDN1=SoPhieuDN,NgaySoPhieuDN1=NgaySoPhieuDN,ChuyenDN1=ChuyenDN,NgayChuyenDN1=NgayChuyenDN,SoPhieuDN=NULL,NgaySoPhieuDN=NULL,ChuyenDN=0,NgayChuyenDN=NULL"
                               + " where DongNuoc2=0 and MaDN=" + MaDN;

                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhDN == "NULL")
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhDN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDN);

                    if (_cDAL_ThuTien.ExecuteNonQuery(command) == true)
                    {
                        //string NoiDung = "Đóng Nước, ngày " + NgayDN.ToString("dd/MM/yyyy") + ", CS: " + ChiSoDN;
                        //sql = "insert into TB_GHICHU(DANHBO,DONVI,NOIDUNG,CREATEDATE,CREATEBY)values('" + _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select DanhBo from TT_KQDongNuoc where MaDN=" + MaDN).ToString() + "',N'DTT',N'" + NoiDung + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "',N'" + _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select HoTen from TT_NguoiDung where MaND=" + CreateBy).ToString() + "')";
                        //_cDAL_DHN.ExecuteNonQuery(sql);
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
                                string sql_Hinh = "insert into TT_KQDongNuoc_Hinh(ID,MaKQDN,DongNuoc2,Hinh,CreateBy,CreateDate)"
                                    + " values((select case when (select COUNT(ID) from TT_KQDongNuoc_Hinh)=0 then 1 else (select MAX(ID) from TT_KQDongNuoc_Hinh)+1 end),(select MaKQDN from TT_KQDongNuoc where MaDN=" + MaDN + "),1,@Hinh," + CreateBy + ",getDate())";
                                command = new SqlCommand(sql_Hinh);
                                command.Parameters.Add("@Hinh", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhDNs[i]);
                                _cDAL_ThuTien.ExecuteNonQuery(command);
                            }
                        }
                        //insert niêm chì
                        if (NiemChi != "NULL")
                        {
                            string sqlNiemChi = "update TT_NiemChi set SuDung=1,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where ID='" + NiemChi + "' and SuDung=0";

                            if (_cDAL_ThuTien.ExecuteNonQuery(sqlNiemChi) == true)
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
            if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where MoNuoc=1 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            else
                return true;
        }

        public bool ThemMoNuoc(string MaDN, string HinhMN, DateTime NgayMN, string ChiSoMN, string NiemChi, string MauSac, string CreateBy)
        {
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select KhoaTu,KhoaKhac from TT_KQDongNuoc where MaDN=" + MaDN);
                if (bool.Parse(dt.Rows[0]["KhoaTu"].ToString()) == false && bool.Parse(dt.Rows[0]["KhoaKhac"].ToString()) == false)
                {
                    if (NiemChi == "")
                        return false;
                    if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "'").ToString()) == 0)
                        return false;
                    if (int.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(ID) from TT_NiemChi where ID='" + NiemChi + "' and SuDung=1").ToString()) == 1)
                        return false;
                }
                else
                {
                    NiemChi = "NULL";
                    MauSac = "NULL";
                }
                if (NiemChi != "NULL")
                    NiemChi = "N'" + NiemChi + "'";
                if (MauSac != "NULL")
                    MauSac = "N'" + MauSac + "'";
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    //string sql = "update TT_KQDongNuoc set MoNuoc=1,HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    //string sql = "update TT_KQDongNuoc set MoNuoc=1,HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd HH:mm:ss") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    string sql = "update TT_KQDongNuoc set MoNuoc=1,NgayMN='" + NgayMN.ToString("yyyyMMdd HH:mm:ss") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN
                        + ",NiemChiMN=" + NiemChi + ",MauSacMN=" + MauSac + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaDN=" + MaDN;
                    SqlCommand command = new SqlCommand(sql);
                    //if (HinhMN == "NULL")
                    //    command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = DBNull.Value;
                    //else
                    //    command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMN);

                    if (_cDAL_ThuTien.ExecuteNonQuery(command) == true)
                    {
                        //string NoiDung = "Mở Nước, ngày " + NgayMN.ToString("dd/MM/yyyy") + ", CS: " + ChiSoMN;
                        //sql = "insert into TB_GHICHU(DANHBO,DONVI,NOIDUNG,CREATEDATE,CREATEBY)values('" + _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select DanhBo from TT_KQDongNuoc where MaDN=" + MaDN).ToString() + "',N'DTT',N'" + NoiDung + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture) + "',N'" + _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select HoTen from TT_NguoiDung where MaND=" + CreateBy).ToString() + "')";
                        //_cDAL_DHN.ExecuteNonQuery(sql);
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
                                _cDAL_ThuTien.ExecuteNonQuery(command);
                            }
                        }
                        //insert niêm chì
                        if (NiemChi != "NULL")
                        {
                            string sqlNiemChi = "update TT_NiemChi set SuDung=1,ModifyBy=" + CreateBy + ",ModifyDate=getDate() where ID=" + NiemChi + " and SuDung=0";

                            if (_cDAL_ThuTien.ExecuteNonQuery(sqlNiemChi) == true)
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

        public bool SuaMoNuoc(string MaDN, string HinhMN, DateTime NgayMN, string ChiSoMN, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set HinhMN=@HinhMN,NgayMN='" + NgayMN.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy=" + CreateBy + ",ModifyDate=getDate() where CAST(NgayMN_ThucTe as date)=CAST(getDate() as date) and MaDN=" + MaDN;

            SqlCommand command = new SqlCommand(sql);
            if (HinhMN == "NULL")
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = DBNull.Value;
            else
                command.Parameters.Add("@HinhMN", SqlDbType.Image).Value = System.Convert.FromBase64String(HinhMN);

            return _cDAL_ThuTien.ExecuteNonQuery(command);
        }

        public bool DangNganDongNuoc(string MaNV_DangNgan, string MaHDs)
        {
            try
            {
                string[] MaHD = MaHDs.Split(',');
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    for (int i = 0; i < MaHD.Length; i++)
                    {
                        string sql = "update HOADON set DangNgan_DienThoai=1,DangNgan_Ton=1,MaNV_DangNgan=" + MaNV_DangNgan + ",NGAYGIAITRACH=getDate(),ModifyBy=" + MaNV_DangNgan + ",ModifyDate=getDate() where ID_HOADON=" + MaHD[i] + " and NGAYGIAITRACH is null ";
                        if (_cDAL_ThuTien.ExecuteNonQuery(sql) == false)
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
            string sql = "select MaHD=ID_HOADON,Ky=CAST(KY as varchar)+'/'+CAST(NAM as varchar),MLT=MALOTRINH,DanhBo=DANHBA,HoTen=TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end"
                 + " ,GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,PhiBVMT_Thue=case when ThueGTGT_TDVTN is null then 0 else ThueGTGT_TDVTN end,TongCong,hd.DCHD,hd.TienDuTruoc_DCHD"
                 + " ,DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen"
                 + " ,GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                 + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                 + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                //+ " ,ChoDCHD=case when exists(select ID_DIEUCHINH_HD from DIEUCHINH_HD where FK_HOADON=hd.ID_HOADON and TONGCONG_END is null) then 'true' else 'false' end"
                 + " ,PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA))"
                 + " ,PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong where MaHDs like '%'+CONVERT(varchar(8),hd.ID_HOADON)+'%')"
                 + " ,LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                 + " from HOADON hd where DANHBA='" + DanhBo + "' and NGAYGIAITRACH is null and hd.ID_HOADON not in (" + MaHDs + ")"
                //+ " and (GB=10 and (NAM>2021 or (NAM=2021 and Ky<6)))"
                 + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                 + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                //+ " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and UpdatedHDDT=0 and ID_HOADON=FK_HOADON)"
                + " and hd.ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and SoPhieu is null)"
                //+ " and hd.ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and (SoPhieu is null or CAST(Ngay_DC as date)<'20220701' or (NAM<2022 or (NAM=2022 and KY<5))))"
                //+ " and hd.DANHBA not in (select DanhBo from KTKS_DonKH.dbo.DonTu_ChiTiet dtct where ChanHoaDon=1 and not exists(select ID from KTKS_DonKH.dbo.DonTu_LichSu where MaDon=dtct.MaDon and (ID_NoiNhan=20 or (ID_NoiChuyen=6 and IDCT is not null))))"
                 + "";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        //lập thông báo đóng nước
        public bool ThemDN(TT_DongNuoc dongnuoc, int MaNV)
        {
            try
            {
                if (_dbThuTien.TT_DongNuocs.Count() > 0)
                {
                    string ID = "MaDN";
                    string Table = "TT_DongNuoc";
                    decimal MaDN = _dbThuTien.ExecuteQuery<decimal>("declare @Ma int " +
                        "select @Ma=MAX(SUBSTRING(CONVERT(nvarchar(50)," + ID + "),LEN(CONVERT(nvarchar(50)," + ID + "))-1,2)) from " + Table + " " +
                        "select MAX(" + ID + ") from " + Table + " where SUBSTRING(CONVERT(nvarchar(50)," + ID + "),LEN(CONVERT(nvarchar(50)," + ID + "))-1,2)=@Ma").Single();
                    //decimal MaCHDB = db.CHDBs.Max(itemCHDB => itemCHDB.MaCHDB);
                    dongnuoc.MaDN = getMaxNextIDTable(MaDN);
                }
                else
                    dongnuoc.MaDN = decimal.Parse("1" + DateTime.Now.ToString("yy"));
                dongnuoc.CreateDate = DateTime.Now;
                dongnuoc.CreateBy = MaNV;
                _dbThuTien.TT_DongNuocs.InsertOnSubmit(dongnuoc);
                _dbThuTien.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                _dbThuTien = new dbThuTienDataContext();
                throw ex;
            }
        }

        public decimal getMaxNextIDTable(decimal id)
        {
            string nam = id.ToString().Substring(id.ToString().Length - 2, 2);
            string stt = id.ToString().Substring(0, id.ToString().Length - 2);
            if (decimal.Parse(nam) == decimal.Parse(DateTime.Now.ToString("yy")))
            {
                stt = (decimal.Parse(stt) + 1).ToString();
            }
            else
            {
                stt = "1";
                nam = DateTime.Now.ToString("yy");
            }
            return decimal.Parse(stt + nam);
        }

        public HOADON getHoaDon(int MaHD)
        {
            return _dbThuTien.HOADONs.SingleOrDefault(item => item.ID_HOADON == MaHD);
        }

        //tìm kiếm
        public string GetDSTimKiem(string DanhBo)
        {
            //string sql = "select top 12 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,"
            //             + " Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),TieuThu,TongCong,NgayGiaiTrach=CONVERT(varchar(10),NgayGiaiTrach,103)"
            //             + " from HOADON where DANHBA='" + DanhBo+"'"
            //             + " order by ID_HOADON desc";

            string sql = "select * from fnTimKiem('" + DanhBo + "','') order by MaHD desc";

            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string GetDSTimKiemTTKH(string HoTen, string SoNha, string TenDuong)
        {
            string sql = "select * from fnTimKiemTTKH('" + HoTen + "','" + SoNha + "','" + TenDuong + "')";

            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string GetTongTon_DenKy(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen,TyLe=CAST(ROUND(CONVERT(float,t1.TongHD)/(select COUNT(ID_HOADON) from HOADON a where (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=t1.MaNV_HanhThu)*100,2)as varchar(5)) from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where NgayGiaiTrach is null and (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string GetTongTon_TrongKy(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen,TyLe=CAST(ROUND(CONVERT(float,t1.TongHD)/(select COUNT(ID_HOADON) from HOADON a where NAM=" + Nam + " and KY=" + Ky + " and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=t1.MaNV_HanhThu)*100,2)as varchar(5)) from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where NgayGiaiTrach is null and NAM=" + Nam + " and KY=" + Ky + " and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
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
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

        //admin
        public string truyvan(string sql)
        {
            try
            {
                return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string capnhat(string sql)
        {
            try
            {
                return _cDAL_ThuTien.ExecuteNonQuery(sql).ToString();
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }

        }

        //lệnh hủy
        public string GetDSHoaDon_LenhHuy(string LoaiCat, string ID)
        {
            string DanhBo = "";
            switch (LoaiCat)
            {
                case "Cắt Tạm":
                    DanhBo = getDanhBo_CatTam(ID);
                    break;
                case "Cắt Hủy":
                    DanhBo = getDanhBo_CatHuy(ID);
                    break;
                case "Danh Bộ":
                    DanhBo = ID;
                    break;
                default:
                    return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable("select DanhBo=hd.DANHBA,DiaChi=SO+' '+DUONG,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),lh.MaHD,lh.TinhTrang,Cat=case when lh.Cat=1 then 'true' else 'false' end from TT_LenhHuy lh,HOADON hd where lh.MaHD=hd.ID_HOADON and hd.NGAYGIAITRACH is null"));
            }
            if (DanhBo == "")
                return "";
            else
                return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable("select DanhBo=hd.DANHBA,DiaChi=SO+' '+DUONG,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),lh.MaHD,lh.TinhTrang,Cat=case when lh.Cat=1 then 'true' else 'false' end from TT_LenhHuy lh,HOADON hd where lh.MaHD=hd.ID_HOADON and DanhBo='" + DanhBo + "' and hd.NGAYGIAITRACH is null"));
        }

        public bool Sua_LenhHuy(string MaHDs, string Cat, string TinhTrang, string CreateBy)
        {
            try
            {
                int flagCat = 0;
                if (bool.Parse(Cat) == true)
                    flagCat = 1;
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    string sql = "update TT_LenhHuy set Cat=" + flagCat + ",TinhTrang=N'" + TinhTrang + "',ModifyBy=" + CreateBy + ",ModifyDate=getDate() where MaHD in (" + MaHDs + ")";
                    if (_cDAL_ThuTien.ExecuteNonQuery(sql) == true)
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

        //nộp tiền
        public string getDS_ChotDangNgan(DateTime FromNgayGiaiTrach, DateTime ToNgayGiaiTrach)
        {
            string sql = "select ID,NgayChot=CONVERT(varchar(10),NgayChot,103),Chot,"
                        + " SLDangNgan=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and MaNV_DangNgan is not null),"
                        + " TCDangNgan=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and MaNV_DangNgan is not null),"
                        + " SLCNKD=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and ChuyenNoKhoDoi=1),"
                        + " TCCNKD=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and ChuyenNoKhoDoi=1),"
                        + " SLGiay=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM<2020 or (NAM=2020 and KY<7)) and MaNV_DangNgan is not null),"
                        + " TCGiay=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM<2020 or (NAM=2020 and KY<7)) and MaNV_DangNgan is not null),"
                        + " SLHDDT=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null),"
                        + " TCHDDT=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null),"
                        + " SLHDDTDC=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is not null),"
                        + " TCHDDTDC=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is not null),"
                        + " SLHDDTDCBCT=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and BaoCaoThue=1),"
                        + " TCHDDTDCBCT=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and BaoCaoThue=1),"
                        + " SLHDDTSach=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is null and BaoCaoThue=0),"
                        + " TCHDDTSach=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is null and BaoCaoThue=0),"
                        + " SLNopTien=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and SyncNopTien=1),"
                        + " TCNopTien=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and SyncNopTien=1)"
                        + " from TT_ChotDangNgan where CAST(NgayChot as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NgayChot as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd") + "'"
                        + " group by ID,NgayChot,Chot order by ID desc";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string them_ChotDangNgan(DateTime NgayGiaiTrach, string CreateBy)
        {
            try
            {
                if (bool.Parse(_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select case when exists(select ID from TT_ChotDangNgan where CAST(NgayChot as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "') then 'true' else 'false' end").ToString()) == true)
                    return "false;Ngày Chốt đã tồn tại";
                if (_cDAL_ThuTien.ExecuteNonQuery("insert into TT_ChotDangNgan(ID,NgayChot,Chot,CreateBy,CreateDate)values((select case when exists(select ID from TT_ChotDangNgan) then (select MAX(ID)+1 from TT_ChotDangNgan) else 1 end),'" + NgayGiaiTrach.ToString("yyyyMMdd HH:mm:ss") + "',0," + CreateBy + ",getdate())") == true)
                    return "true; ";
                else
                    return "false; ";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string chotDangNgan(string ID, bool Chot, string CreateBy)
        {
            try
            {
                int value = 0;
                if (Chot == true)
                    value = 1;
                if (_cDAL_ThuTien.ExecuteNonQuery("update TT_ChotDangNgan set Chot=" + value + ",ModifyBy=" + CreateBy + ",ModifyDate=getdate() where ID=" + ID) == true)
                    return "true; ";
                else
                    return "false; ";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string showError_NopTien(DateTime NgayGiaiTrach)
        {
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select DanhBo=DANHBA,Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),Result=(select top 1 Result from Temp_SyncHoaDon where MaHD=HOADON.ID_HOADON or SoHoaDon=HOADON.SOHOADON) from HOADON where CAST(NGAYGIAITRACH as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and SyncNopTien=0 and MaNV_DangNgan is not null and (NAM>2020 or (NAM=2020 and KY>=7)) and BaoCaoThue=0");
                if (dt != null && dt.Rows.Count > 0)
                    return "true;" + DataTableToJSON(dt);
                else
                    return "false;Không có hóa đơn";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string showHDDCBaoCaoThue(DateTime NgayGiaiTrach)
        {
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select DanhBo=DANHBA,Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),Result=(select Result from Temp_SyncHoaDon where SoHoaDon=HOADON.SOHOADON) from HOADON where CAST(NGAYGIAITRACH as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and (NAM>2020 or (NAM=2020 and KY>=7)) and BaoCaoThue=1");
                if (dt != null && dt.Rows.Count > 0)
                    return "true;" + DataTableToJSON(dt);
                else
                    return "false;Không có hóa đơn";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        //sync tổng
        #region Class Tổng

        string urlTong = "https://hoadon.sawaco.com.vn";
        string taxCode = "0301129367";
        string userName = "tanhoaapi";
        string passWord = "tanhoaapi@sawaco.com.vn#2020";
        string branchcode = "TH";
        //string pattern21E = "01GTKT0/002";
        //string pattern22E = "01GTKT0/003";
        //string serial = "CT/20E";

        public class HoaDonThanhToan
        {
            public string branchcode { get; set; }
            public string pattern { get; set; }
            public string serial { get; set; }
            public string SoHD { get; set; }
            public string NgayThanhToan { get; set; }
            public string TongSoTien { get; set; }
            public string LoaiThuTien { get; set; }
            public string TenThuTien { get; set; }
            public string ThanhToan { get; set; }
        }

        public class HoaDonNopTien
        {
            public string branchcode { get; set; }
            public string pattern { get; set; }
            public string serial { get; set; }
            public string SoHD { get; set; }
            public string NgayNopTien { get; set; }
            public string TongSoTien { get; set; }
            public string HinhThucThanhToan { get; set; }
        }

        public class HoaDonNopTienLo
        {
            public string SoHD { get; set; }
            public string NgayNopTien { get; set; }
            public string TongSoTien { get; set; }
            public string HinhThucThanhToan { get; set; }
        }

        public class HoaDonNopTienResult
        {
            public string branchcode { get; set; }
            public string pattern { get; set; }
            public string serial { get; set; }
            public decimal SoHD { get; set; }
            public string NgayNopTien { get; set; }
            public string TongSoTien { get; set; }
            public string HinhThucThanhToan { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
        }

        public class HoaDonNopTienLoResult
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public List<HoaDonNopTienResult> result { get; set; }
        }

        #endregion

        public string getBieuMau(string KyHieu)
        {
            return _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select BieuMau from TT_BieuMauHoaDon where KyHieu='" + KyHieu + "'").ToString();
        }

        public string syncThanhToan_01072022(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where DCHD=0 and BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                        request.Method = "POST";
                        request.Headers.Add("taxcode", taxCode);
                        request.Headers.Add("username", userName);
                        request.Headers.Add("password", passWord);
                        request.ContentType = "application/json; charset=utf-8";

                        string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                        if (item["NgayGiaiTrach"].ToString() != "")
                            NgayThanhToan = item["NgayGiaiTrach"].ToString();
                        else
                            NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                        if (bool.Parse(item["DangNgan_Ton"].ToString()) == true)
                            LoaiThuTien = "0";
                        else
                            if (bool.Parse(item["DangNgan_ChuyenKhoan"].ToString()) == true)
                                LoaiThuTien = "2";
                            else
                                if (bool.Parse(item["DangNgan_Quay"].ToString()) == true)
                                    LoaiThuTien = "1";

                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";

                        if (item["DangNgan"].ToString() != "")
                            TenThuTien = item["DangNgan"].ToString();
                        else
                            TenThuTien = "NULL";

                        //var data = new
                        //{
                        //    branchcode = branchcode,
                        //    pattern = pattern,
                        //    serial = serial,
                        //    SoHD = item["SoHoaDon"].ToString().Substring(6),
                        //    NgayThanhToan = NgayThanhToan,
                        //    TongSoTien = item["TongCong"].ToString(),
                        //    LoaiThuTien = LoaiThuTien,
                        //    TenThuTien = TenThuTien,
                        //    ThanhToan = ThanhToan,
                        //};
                        HoaDonThanhToan en = new HoaDonThanhToan();
                        en.branchcode = branchcode;
                        if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
                        {
                            en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 7));
                            en.serial = item["SoHoaDon"].ToString().Substring(0, 7);
                            en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                        }
                        else
                            if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
                            {
                                en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 6));
                                en.serial = item["SoHoaDon"].ToString().Substring(0, 6);
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                            }
                        en.NgayThanhToan = NgayThanhToan;
                        en.TongSoTien = item["TongCong"].ToString();
                        en.LoaiThuTien = LoaiThuTien;
                        en.TenThuTien = TenThuTien;
                        en.ThanhToan = ThanhToan;

                        var json = jss.Serialize(en);
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
                            result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();

                            var obj = jss.Deserialize<dynamic>(result);
                            if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                                _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                            }
                            else
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                            }
                            result = "true;" + obj["status"] + " = " + obj["message"];
                        }
                        else
                            result = "false;" + respuesta.StatusCode;
                    }
                }
                else
                {
                    result = "false;TH: Hóa Đơn không có";
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where DCHD=0 and BaoCaoThue=1 and ID_HOADON=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string ThanhToan = "-1";
                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                        _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncThanhToan(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                string sql = "select SoHoaDon=hd.SoHoaDonCu,TongCong=TONGCONG_BD,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112))"
                            + " ,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan)"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD
                            + " union all"
                            + " select hd.SoHoaDon,TongCong=TONGCONG_DC,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112))"
                            + " ,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan)"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD;
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                if (dt != null && dt.Rows.Count == 0)
                    dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where DCHD=0 and BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                        request.Method = "POST";
                        request.Headers.Add("taxcode", taxCode);
                        request.Headers.Add("username", userName);
                        request.Headers.Add("password", passWord);
                        request.ContentType = "application/json; charset=utf-8";

                        string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                        if (item["NgayGiaiTrach"].ToString() != "")
                            NgayThanhToan = item["NgayGiaiTrach"].ToString();
                        else
                            NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                        if (bool.Parse(item["DangNgan_Ton"].ToString()) == true)
                            LoaiThuTien = "0";
                        else
                            if (bool.Parse(item["DangNgan_ChuyenKhoan"].ToString()) == true)
                                LoaiThuTien = "2";
                            else
                                if (bool.Parse(item["DangNgan_Quay"].ToString()) == true)
                                    LoaiThuTien = "1";

                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";

                        if (item["DangNgan"].ToString() != "")
                            TenThuTien = item["DangNgan"].ToString();
                        else
                            TenThuTien = "NULL";

                        //var data = new
                        //{
                        //    branchcode = branchcode,
                        //    pattern = pattern,
                        //    serial = serial,
                        //    SoHD = item["SoHoaDon"].ToString().Substring(6),
                        //    NgayThanhToan = NgayThanhToan,
                        //    TongSoTien = item["TongCong"].ToString(),
                        //    LoaiThuTien = LoaiThuTien,
                        //    TenThuTien = TenThuTien,
                        //    ThanhToan = ThanhToan,
                        //};
                        HoaDonThanhToan en = new HoaDonThanhToan();
                        en.branchcode = branchcode;
                        if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
                        {
                            en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 7));
                            en.serial = item["SoHoaDon"].ToString().Substring(0, 7);
                            en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                        }
                        else
                            if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
                            {
                                en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 6));
                                en.serial = item["SoHoaDon"].ToString().Substring(0, 6);
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                            }
                        en.NgayThanhToan = NgayThanhToan;
                        en.TongSoTien = item["TongCong"].ToString();
                        en.LoaiThuTien = LoaiThuTien;
                        en.TenThuTien = TenThuTien;
                        en.ThanhToan = ThanhToan;

                        var json = jss.Serialize(en);
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
                            result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();

                            var obj = jss.Deserialize<dynamic>(result);
                            if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                                _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                            }
                            else
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                            }
                            result = "true;" + obj["status"] + " = " + obj["message"];
                        }
                        else
                            result = "false;" + respuesta.StatusCode;
                    }
                }
                else
                {
                    result = "false;TH: Hóa Đơn không có";
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where DCHD=0 and BaoCaoThue=1 and ID_HOADON=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string ThanhToan = "-1";
                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                        _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        //public string syncThanhToan(string SoHoaDon, bool GiaiTrach, int IDTemp_SyncHoaDon)
        //{
        //    string result = "";
        //    try
        //    {
        //        DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
        //            request.Method = "POST";
        //            request.Headers.Add("taxcode", taxCode);
        //            request.Headers.Add("username", userName);
        //            request.Headers.Add("password", passWord);
        //            request.ContentType = "application/json; charset=utf-8";

        //            string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
        //            if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
        //                NgayThanhToan = dt.Rows[0]["NgayGiaiTrach"].ToString();
        //            else
        //                NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

        //            if (bool.Parse(dt.Rows[0]["DangNgan_Ton"].ToString()) == true)
        //                LoaiThuTien = "0";
        //            else
        //                if (bool.Parse(dt.Rows[0]["DangNgan_ChuyenKhoan"].ToString()) == true)
        //                    LoaiThuTien = "2";
        //                else
        //                    if (bool.Parse(dt.Rows[0]["DangNgan_Quay"].ToString()) == true)
        //                        LoaiThuTien = "1";

        //            if (GiaiTrach == true)
        //                ThanhToan = "1";
        //            else
        //                ThanhToan = "0";

        //            if (dt.Rows[0]["DangNgan"].ToString() != "")
        //                TenThuTien = dt.Rows[0]["DangNgan"].ToString();
        //            else
        //                TenThuTien = "NULL";

        //            //var data = new
        //            //{
        //            //    branchcode = branchcode,
        //            //    pattern = pattern,
        //            //    serial = serial,
        //            //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
        //            //    NgayThanhToan = NgayThanhToan,
        //            //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
        //            //    LoaiThuTien = LoaiThuTien,
        //            //    TenThuTien = TenThuTien,
        //            //    ThanhToan = ThanhToan,
        //            //};
        //            HoaDonThanhToan en = new HoaDonThanhToan();
        //            en.branchcode = branchcode;
        //            if (dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
        //            {
        //                en.pattern = getBieuMau(dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 7));
        //                en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 7);
        //                en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(7);
        //            }
        //            else
        //                if (dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
        //                {
        //                    en.pattern = getBieuMau(dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6));
        //                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
        //                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
        //                }
        //            en.NgayThanhToan = NgayThanhToan;
        //            en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
        //            en.LoaiThuTien = LoaiThuTien;
        //            en.TenThuTien = TenThuTien;
        //            en.ThanhToan = ThanhToan;

        //            var json = jss.Serialize(en);
        //            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //            request.ContentLength = byteArray.Length;
        //            //gắn data post
        //            Stream dataStream = request.GetRequestStream();
        //            dataStream.Write(byteArray, 0, byteArray.Length);
        //            dataStream.Close();

        //            HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
        //            if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
        //            {
        //                StreamReader read = new StreamReader(respuesta.GetResponseStream());
        //                result = read.ReadToEnd();
        //                read.Close();
        //                respuesta.Close();

        //                var obj = jss.Deserialize<dynamic>(result);
        //                if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
        //                {
        //                    _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and SOHOADON='" + SoHoaDon + "'");
        //                    _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
        //                }
        //                else
        //                {
        //                    _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
        //                }
        //                result = "true;" + obj["status"] + " = " + obj["message"];
        //            }
        //            else
        //                result = "false;" + respuesta.StatusCode;
        //        }
        //        else
        //        {
        //            result = "false;TH: Hóa Đơn không có";
        //            DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
        //            if (dtBCT != null && dtBCT.Rows.Count > 0)
        //            {
        //                string ThanhToan = "-1";
        //                if (GiaiTrach == true)
        //                    ThanhToan = "1";
        //                else
        //                    ThanhToan = "0";
        //                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and SOHOADON='" + SoHoaDon + "'");
        //                _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "false;" + ex.Message;
        //    }
        //    return result;
        //}

        public string syncThanhToan_ThuHo(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt;
                if (GiaiTrach == true)
                {
                    string sql = "select SoHoaDon=hd.SoHoaDonCu,TongCong=TONGCONG_BD,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112))"
                            + " ,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu"
                            + " from TT_DichVuThu dvt,HOADON hd,DIEUCHINH_HD dc where dvt.MaHD=hd.ID_HOADON and hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD
                            + " union all"
                            + " select hd.SoHoaDon,TongCong=TONGCONG_DC,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112))"
                            + " ,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu"
                            + " from TT_DichVuThu dvt,HOADON hd,DIEUCHINH_HD dc where dvt.MaHD=hd.ID_HOADON and hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD;
                    dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count == 0)
                        dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu dvt,HOADON hd where dvt.MaHD=hd.ID_HOADON and DCHD=0 and BaoCaoThue=0 and MaHD=" + MaHD);
                }
                else
                {
                    string sql = "select SoHoaDon=hd.SoHoaDonCu,TongCong=TONGCONG_BD,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112))"
                            + " ,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu"
                            + " from TT_DichVuThu_Huy dvt,HOADON hd,DIEUCHINH_HD dc where dvt.MaHD=hd.ID_HOADON and hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD
                            + " union all"
                            + " select hd.SoHoaDon,TongCong=TONGCONG_DC,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112))"
                            + " ,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu"
                            + " from TT_DichVuThu_Huy dvt,HOADON hd,DIEUCHINH_HD dc where dvt.MaHD=hd.ID_HOADON and hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD;
                    dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count == 0)
                        dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu_Huy dvt,HOADON hd where dvt.MaHD=hd.ID_HOADON and DCHD=0 and BaoCaoThue=0 and MaHD=" + MaHD);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                        request.Method = "POST";
                        request.Headers.Add("taxcode", taxCode);
                        request.Headers.Add("username", userName);
                        request.Headers.Add("password", passWord);
                        request.ContentType = "application/json; charset=utf-8";

                        string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                        if (item["NgayGiaiTrach"].ToString() != "")
                            NgayThanhToan = item["NgayGiaiTrach"].ToString();
                        else
                            NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                        if (bool.Parse(item["DangNgan_Ton"].ToString()) == true)
                            LoaiThuTien = "0";
                        else
                            if (bool.Parse(item["DangNgan_ChuyenKhoan"].ToString()) == true)
                                LoaiThuTien = "2";
                            else
                                if (bool.Parse(item["DangNgan_Quay"].ToString()) == true)
                                    LoaiThuTien = "1";

                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";

                        if (item["DangNgan"].ToString() != "")
                            TenThuTien = item["DangNgan"].ToString();
                        else
                            TenThuTien = "NULL";

                        //var data = new
                        //{
                        //    branchcode = branchcode,
                        //    pattern = pattern,
                        //    serial = serial,
                        //    SoHD = item["SoHoaDon"].ToString().Substring(6),
                        //    NgayThanhToan = NgayThanhToan,
                        //    TongSoTien = item["TongCong"].ToString(),
                        //    LoaiThuTien = LoaiThuTien,
                        //    TenThuTien = TenThuTien,
                        //    ThanhToan = ThanhToan,
                        //};
                        HoaDonThanhToan en = new HoaDonThanhToan();
                        en.branchcode = branchcode;
                        if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
                        {
                            en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 7));
                            en.serial = item["SoHoaDon"].ToString().Substring(0, 7);
                            en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                        }
                        else
                            if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
                            {
                                en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 6));
                                en.serial = item["SoHoaDon"].ToString().Substring(0, 6);
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                            }
                        en.NgayThanhToan = NgayThanhToan;
                        en.TongSoTien = item["TongCong"].ToString();
                        en.LoaiThuTien = LoaiThuTien;
                        en.TenThuTien = TenThuTien;
                        en.ThanhToan = ThanhToan;

                        var json = jss.Serialize(en);
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
                            result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();

                            var obj = jss.Deserialize<dynamic>(result);
                            if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                                _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                            }
                            else
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                            }
                            result = "true;" + obj["status"] + " = " + obj["message"];
                        }
                        else
                        {
                            result = "false;" + respuesta.StatusCode;
                            _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + result + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                        }
                    }
                }
                else
                {
                    result = "false;TH: Hóa Đơn không có";
                    DataTable dtBCT = new DataTable();
                    if (GiaiTrach == true)
                        dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu dvt,HOADON hd where dvt.MaHD=hd.ID_HOADON and DCHD=0 and BaoCaoThue=1 and MaHD=" + MaHD);
                    else
                        dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, dvt.CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu_Huy dvt,HOADON hd where dvt.MaHD=hd.ID_HOADON and DCHD=0 and BaoCaoThue=1 and MaHD=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string ThanhToan = "-1";
                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                        _cDAL_ThuTien.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false; " + ex.Message;
                _cDAL_ThuTien.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + result + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
            }
            return result;
        }

        public string syncNopTien_01072022(int MaHD)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where DCHD=0 and BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptien");
                        request.Method = "POST";
                        request.Headers.Add("taxcode", taxCode);
                        request.Headers.Add("username", userName);
                        request.Headers.Add("password", passWord);
                        request.ContentType = "application/json; charset=utf-8";

                        string NgayNopTien = "", HinhThucThanhToan = "";
                        if (item["NgayGiaiTrach"].ToString() != "")
                            NgayNopTien = item["NgayGiaiTrach"].ToString();
                        else
                            NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                        if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                            HinhThucThanhToan = "2";
                        else
                            HinhThucThanhToan = "1";

                        //var data = new
                        //{
                        //    branchcode = branchcode,
                        //    pattern = pattern,
                        //    serial = serial,
                        //    SoHD = item["SoHoaDon"].ToString().Substring(6),
                        //    NgayNopTien = NgayNopTien,
                        //    TongSoTien = item["TongCong"].ToString(),
                        //    HinhThucThanhToan = HinhThucThanhToan,
                        //};
                        HoaDonNopTien en = new HoaDonNopTien();
                        en.branchcode = branchcode;
                        if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
                        {
                            en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 7));
                            en.serial = item["SoHoaDon"].ToString().Substring(0, 7);
                            en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                        }
                        else
                            if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
                            {
                                en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 6));
                                en.serial = item["SoHoaDon"].ToString().Substring(0, 6);
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                            }
                        en.NgayNopTien = NgayNopTien;
                        en.TongSoTien = item["TongCong"].ToString();
                        en.HinhThucThanhToan = HinhThucThanhToan;

                        var json = jss.Serialize(en);
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
                            result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();

                            var obj = jss.Deserialize<dynamic>(result);
                            if (obj["status"] == "OK" || obj["status"] == "ERR:7" || obj["status"] == "ERR:8")
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD
                                                    + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                            }
                            else
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "')"
                                               + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + item["SoHoaDon"].ToString() + "'),'" + item["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')"
                                               + " else update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where SoHoaDon='" + item["SoHoaDon"].ToString() + "'");
                            }
                            result = "true;" + obj["status"] + " = " + obj["message"];
                        }
                    }
                }
                else
                {
                    //nộp tiền báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where DCHD=0 and BaoCaoThue=1 and ID_HOADON=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + dtBCT.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncNopTien(int MaHD)
        {
            string result = "";
            try
            {
                string sql = "select SoHoaDon=hd.SoHoaDonCu,TongCong=TONGCONG_BD,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),ChuyenNoKhoDoi"
                            + " ,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan)"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD
                            + " union all"
                            + " select hd.SoHoaDon,TongCong=TONGCONG_DC,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),ChuyenNoKhoDoi"
                            + " ,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan)"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.ID_HOADON=dc.FK_HOADON and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and hd.ID_HOADON=" + MaHD;
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                if (dt != null && dt.Rows.Count == 0)
                    dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where DCHD=0 and BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptien");
                        request.Method = "POST";
                        request.Headers.Add("taxcode", taxCode);
                        request.Headers.Add("username", userName);
                        request.Headers.Add("password", passWord);
                        request.ContentType = "application/json; charset=utf-8";

                        string NgayNopTien = "", HinhThucThanhToan = "";
                        if (item["NgayGiaiTrach"].ToString() != "")
                            NgayNopTien = item["NgayGiaiTrach"].ToString();
                        else
                            NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                        if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                            HinhThucThanhToan = "2";
                        else
                            HinhThucThanhToan = "1";

                        //var data = new
                        //{
                        //    branchcode = branchcode,
                        //    pattern = pattern,
                        //    serial = serial,
                        //    SoHD = item["SoHoaDon"].ToString().Substring(6),
                        //    NgayNopTien = NgayNopTien,
                        //    TongSoTien = item["TongCong"].ToString(),
                        //    HinhThucThanhToan = HinhThucThanhToan,
                        //};
                        HoaDonNopTien en = new HoaDonNopTien();
                        en.branchcode = branchcode;
                        if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
                        {
                            en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 7));
                            en.serial = item["SoHoaDon"].ToString().Substring(0, 7);
                            en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                        }
                        else
                            if (item["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
                            {
                                en.pattern = getBieuMau(item["SoHoaDon"].ToString().Substring(0, 6));
                                en.serial = item["SoHoaDon"].ToString().Substring(0, 6);
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                            }
                        en.NgayNopTien = NgayNopTien;
                        en.TongSoTien = item["TongCong"].ToString();
                        en.HinhThucThanhToan = HinhThucThanhToan;

                        var json = jss.Serialize(en);
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
                            result = read.ReadToEnd();
                            read.Close();
                            respuesta.Close();

                            var obj = jss.Deserialize<dynamic>(result);
                            if (obj["status"] == "OK" || obj["status"] == "ERR:7" || obj["status"] == "ERR:8")
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD
                                                    + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                            }
                            else
                            {
                                _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "')"
                                               + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + item["SoHoaDon"].ToString() + "'),'" + item["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')"
                                               + " else update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where SoHoaDon='" + item["SoHoaDon"].ToString() + "'");
                            }
                            result = "true;" + obj["status"] + " = " + obj["message"];
                        }
                    }
                }
                else
                {
                    //nộp tiền báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where DCHD=0 and BaoCaoThue=1 and ID_HOADON=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + dtBCT.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        //public string syncNopTien(string SoHoaDon)
        //{
        //    string result = "";
        //    try
        //    {
        //        DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptien");
        //            request.Method = "POST";
        //            request.Headers.Add("taxcode", taxCode);
        //            request.Headers.Add("username", userName);
        //            request.Headers.Add("password", passWord);
        //            request.ContentType = "application/json; charset=utf-8";

        //            string NgayNopTien = "", HinhThucThanhToan = "";
        //            if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
        //                NgayNopTien = dt.Rows[0]["NgayGiaiTrach"].ToString();
        //            else
        //                NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

        //            if (bool.Parse(dt.Rows[0]["ChuyenNoKhoDoi"].ToString()) == true)
        //                HinhThucThanhToan = "2";
        //            else
        //                HinhThucThanhToan = "1";

        //            //var data = new
        //            //{
        //            //    branchcode = branchcode,
        //            //    pattern = pattern,
        //            //    serial = serial,
        //            //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
        //            //    NgayNopTien = NgayNopTien,
        //            //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
        //            //    HinhThucThanhToan = HinhThucThanhToan,
        //            //};
        //            HoaDonNopTien en = new HoaDonNopTien();
        //            en.branchcode = branchcode;
        //            if (dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 2).Contains("1K"))
        //            {
        //                en.pattern = getBieuMau(dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 7));
        //                en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 7);
        //                en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(7);
        //            }
        //            else
        //                if (dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 2).Contains("CT"))
        //                {
        //                    en.pattern = getBieuMau(dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6));
        //                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
        //                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
        //                }
        //            en.NgayNopTien = NgayNopTien;
        //            en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
        //            en.HinhThucThanhToan = HinhThucThanhToan;

        //            var json = jss.Serialize(en);
        //            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //            request.ContentLength = byteArray.Length;
        //            //gắn data post
        //            Stream dataStream = request.GetRequestStream();
        //            dataStream.Write(byteArray, 0, byteArray.Length);
        //            dataStream.Close();

        //            HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
        //            if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
        //            {
        //                StreamReader read = new StreamReader(respuesta.GetResponseStream());
        //                result = read.ReadToEnd();
        //                read.Close();
        //                respuesta.Close();

        //                var obj = jss.Deserialize<dynamic>(result);
        //                if (obj["status"] == "OK" || obj["status"] == "ERR:7" || obj["status"] == "ERR:8")
        //                {
        //                    _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SOHOADON='" + SoHoaDon + "'"
        //                                            + " delete Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
        //                }
        //                else
        //                {
        //                    _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "')"
        //                                   + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "'),'" + dt.Rows[0]["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')"
        //                                   + " else update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "'");
        //                }
        //                result = "true;" + obj["status"] + " = " + obj["message"];
        //            }
        //            else
        //                result = "false;" + respuesta.StatusCode;
        //        }
        //        else
        //        {
        //            DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=1 and SOHOADON='" + SoHoaDon + "'");
        //            if (dtBCT != null && dtBCT.Rows.Count > 0)
        //            {
        //                _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SOHOADON='" + SoHoaDon + "'"
        //                                + " delete Temp_SyncHoaDon where SoHoaDon='" + dtBCT.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = "false;" + ex.Message;
        //    }
        //    return result;
        //}

        public string syncNopTienLo(DateTime NgayGiaiTrach)
        {
            string result = "";
            try
            {
                DataTable dtSerial = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and LEN(SOHOADON)=13 group by SUBSTRING(SOHOADON,0,7)");
                DataTable dtSerial2022 = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,8) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and LEN(SOHOADON)=14 group by SUBSTRING(SOHOADON,0,8)");
                if ((dtSerial == null || dtSerial.Rows.Count == 0) && (dtSerial2022 == null || dtSerial2022.Rows.Count == 0))
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();
                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                                                    + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
                foreach (DataRow itemSerial in dtSerial2022.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    string sql = "select SoHoaDon=hd.SoHoaDonCu,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_BD,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_DC,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where (SoHoaDonCu is null or exists(select hd.ID_HOADON from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and CAST(dc.Ngay_DC as date)<'20220701' and hd.ID_HOADON=HOADON.ID_HOADON)) and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'";
                    //string sql = " select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where SoHoaDonCu is null and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 * from (" + sql + ")t1 order by NgayGiaiTrach asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();
                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                                                         + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncNopTienLo_Except12(DateTime NgayGiaiTrach)
        {
            string result = "";
            try
            {
                DataTable dtSerial = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and LEN(SOHOADON)=13 group by SUBSTRING(SOHOADON,0,7)");
                DataTable dtSerial2022 = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,8) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and LEN(SOHOADON)=14 group by SUBSTRING(SOHOADON,0,8)");
                if ((dtSerial == null || dtSerial.Rows.Count == 0) && (dtSerial2022 == null || dtSerial2022.Rows.Count == 0))
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();

                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                             + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
                foreach (DataRow itemSerial in dtSerial2022.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    string sql = "select SoHoaDon=hd.SoHoaDonCu,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_BD,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_DC,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where (SoHoaDonCu is null or exists(select hd.ID_HOADON from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and CAST(dc.Ngay_DC as date)<'20220701' and hd.ID_HOADON=HOADON.ID_HOADON)) and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM<" + NgayGiaiTrach.Year + " or (NAM=" + NgayGiaiTrach.Year + " and KY<12)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 * from (" + sql + ")t1 order by NgayGiaiTrach asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();

                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                             + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncNopTienLo_12(DateTime NgayGiaiTrach)
        {
            string result = "";
            try
            {
                DataTable dtSerial = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=0 and LEN(SOHOADON)=13 group by SUBSTRING(SOHOADON,0,7)");
                DataTable dtSerial2022 = _cDAL_ThuTien.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,8) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=0 and LEN(SOHOADON)=14 group by SUBSTRING(SOHOADON,0,8)");
                if ((dtSerial == null || dtSerial.Rows.Count == 0) && (dtSerial2022 == null || dtSerial2022.Rows.Count == 0))
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(6);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();
                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                             + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
                foreach (DataRow itemSerial in dtSerial2022.Rows)
                {
                    //nộp báo cáo thuế trước
                    DataTable dtBCT = _cDAL_ThuTien.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    foreach (DataRow item in dtBCT.Rows)
                    {
                        _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'"
                                        + " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'");
                    }
                    string sql = "select SoHoaDon=hd.SoHoaDonCu,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_BD,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select hd.SoHoaDon,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TongCong=TONGCONG_DC,ChuyenNoKhoDoi"
                            + " from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and hd.DCHD=0 and hd.BaoCaoThue=0 and CAST(dc.Ngay_DC as date)>='20220701' and hd.SoHoaDonCu is not null and SUBSTRING(hd.SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'"
                            + " union all"
                            + " select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where (SoHoaDonCu is null or exists(select hd.ID_HOADON from HOADON hd,DIEUCHINH_HD dc where hd.SoHoaDonCu is not null and hd.ID_HOADON=dc.FK_HOADON and CAST(dc.Ngay_DC as date)<'20220701' and hd.ID_HOADON=HOADON.ID_HOADON)) and Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and (NAM=" + NgayGiaiTrach.Year + " and KY=12) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,8)='" + itemSerial["serial"].ToString() + "'";
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(getBieuMau(itemSerial["serial"].ToString())) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1000 * from (" + sql + ")t1 order by NgayGiaiTrach asc");
                            foreach (DataRow item in dt.Rows)
                            {
                                string NgayNopTien = "", HinhThucThanhToan = "";
                                if (item["NgayGiaiTrach"].ToString() != "")
                                    NgayNopTien = item["NgayGiaiTrach"].ToString();
                                else
                                    NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                                if (bool.Parse(item["ChuyenNoKhoDoi"].ToString()) == true)
                                    HinhThucThanhToan = "2";
                                else
                                    HinhThucThanhToan = "1";
                                HoaDonNopTienLo en = new HoaDonNopTienLo();
                                en.SoHD = item["SoHoaDon"].ToString().Substring(7);
                                en.NgayNopTien = NgayNopTien;
                                en.TongSoTien = item["TongCong"].ToString();
                                en.HinhThucThanhToan = HinhThucThanhToan;
                                lstHD.Add(en);
                            }
                            var json = jss.Serialize(lstHD);
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
                                result = read.ReadToEnd();
                                read.Close();
                                respuesta.Close();
                                HoaDonNopTienLoResult deserializedResult = jss.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'"
                                             + " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'");
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],MaHD,SoHoaDon,Result)values('NopTien',(select ID_HOADON from HOADON where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'),'" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
                                            + " else update Temp_SyncHoaDon set Result=N'" + item.Status + " = " + item.Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'");
                                        }
                                    }
                                    result = "true;" + deserializedResult.Status + " = " + deserializedResult.Message;
                                }
                                else
                                    result = "false;" + deserializedResult.Status + " = " + deserializedResult.Message;
                            }
                            else
                                result = "false;" + respuesta.StatusCode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public void updateResultSyncNopTien(HoaDonNopTienLoResult itemResult, int index, DataRow itemSerial)
        {
            for (int i = index; i < itemResult.result.Count; i++)
            {
                if (itemResult.result[i].Status == "OK" || itemResult.result[i].Status == "ERR:7")
                {
                    string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "'";
                    sql += " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "' and [Action]='NopTien'";
                    _cDAL_ThuTien.ExecuteNonQuery(sql);
                }
                else
                {
                    _cDAL_ThuTien.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "')"
                    + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "',N'" + itemResult.result[i].Status + " = " + itemResult.result[i].Message + "')"
                    + " else update Temp_SyncHoaDon set Result=N'" + itemResult.result[i].Status + " = " + itemResult.result[i].Message + "',ModifyDate=getdate() where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "'");
                }
            }
        }

        public string getErrorCode_syncThanhToan(string status)
        {
            switch (status)
            {
                case "ERR:1":
                    return "Tham số không đúng";
                case "ERR:2":
                    return "Lỗi xử lý dữ liệu đầu vào";
                case "ERR:3":
                    return "Lệch tổng tiền";
                case "ERR:4":
                    return "Đã thanh toán";
                case "ERR:5":
                    return "Ngày thanh toán sai định dạng";
                case "ERR:6":
                    return "Đã nộp tiền";
                case "ERR:7":
                    return "Đã gạch nợ";
                case "ERR:9":
                    return "Trạng thái hóa đơn không phù hợp";
                case "ERR:10":
                    return "Lỗi exception, kiểm tra ";
            }
            return null;
        }

        public string getErrorCode_syncNopTien(string status)
        {
            switch (status)
            {
                case "ERR:1":
                    return "Tham số không đúng";
                case "ERR:2":
                    return "Hình thức thanh toán không đúng";
                case "ERR:4":
                    return "Lệch tổng tiền";
                case "ERR:5":
                    return "Ngày thanh toán sai định dạng";
                case "ERR:6":
                    return "Hóa đơn chưa thanh toán";
                case "ERR:7":
                    return "Hóa đơn đã nộp tiền";
                case "ERR:8":
                    return "Hóa đơn đã gạch nợ";
                case "ERR:9":
                    return "Trạng thái thanh toán không hợp lệ";
                case "ERR:10":
                    return "Lỗi không xác định, kiểm tra log hệ thống";
                case "ERR:11":
                    return "Lô hóa đơn quá giới hạn";
            }
            return null;
        }

        public string updateChiTietTienNuoc(int Nam, int Ky, int Dot)
        {
            try
            {
                string ChiTietNamCu = "", ChiTietNamMoi = "", ChiTietPhiBVMTNamCu = "", ChiTietPhiBVMTNamMoi = "";
                int TyleSH = 0, TyLeSX = 0, TyLeDV = 0, TyLeHCSN = 0, TongTienNamCu = 0, TongTienNamMoi = 0, PhiBVMTNamCu = 0, PhiBVMTNamMoi = 0, TieuThu_DieuChinhGia = 0, DinhMucHN = 0, TienNuoc = 0, ThueGTGT = 0, TDVTN = 0, ThueTDVTN = 0;
                List<HOADON> lst = _dbThuTien.HOADONs.Where(item => item.NAM == Nam && item.KY == Ky && item.DOT == Dot && item.ChiTietTienNuoc == null).ToList();
                foreach (HOADON item in lst)
                {
                    ChiTietNamCu = ChiTietNamMoi = "";
                    TyleSH = TyLeSX = TyLeDV = TyLeHCSN = DinhMucHN = 0;
                    if (item.TILESH != null && item.TILESH.Value != 0)
                        TyleSH = item.TILESH.Value;
                    if (item.TILESX != null && item.TILESX.Value != 0)
                        TyLeSX = item.TILESX.Value;
                    if (item.TILEDV != null && item.TILEDV.Value != 0)
                        TyLeDV = item.TILEDV.Value;
                    if (item.TILEHCSN != null && item.TILEHCSN.Value != 0)
                        TyLeHCSN = item.TILEHCSN.Value;
                    if (item.DinhMucHN != null)
                        DinhMucHN = item.DinhMucHN.Value;
                    if (item.TUNGAY != null)
                        TinhTienNuoc(true, false, false, 0, item.DANHBA, Ky, Nam, item.TUNGAY.Value, item.DENNGAY.Value, item.GB, TyleSH, TyLeSX, TyLeDV, TyLeHCSN, (int)item.DM.Value, DinhMucHN, (int)item.TIEUTHU.Value, ref TongTienNamCu, ref ChiTietNamCu, ref TongTienNamMoi, ref ChiTietNamMoi, ref TieuThu_DieuChinhGia, ref  PhiBVMTNamCu, ref  ChiTietPhiBVMTNamCu, ref  PhiBVMTNamMoi, ref ChiTietPhiBVMTNamMoi, ref TienNuoc, ref ThueGTGT, ref TDVTN, ref ThueTDVTN);
                    else
                    {
                        DataTable dt = getDocSo(item.DANHBA, item.NAM.ToString(), item.KY.ToString());
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DateTime TuNgay = DateTime.Parse(dt.Rows[0]["TuNgay"].ToString()), DenNgay = DateTime.Parse(dt.Rows[0]["DenNgay"].ToString());
                            TinhTienNuoc(true, false, false, 0, item.DANHBA, Ky, Nam, TuNgay, DenNgay, item.GB, TyleSH, TyLeSX, TyLeDV, TyLeHCSN, (int)item.DM.Value, DinhMucHN, (int)item.TIEUTHU.Value, ref TongTienNamCu, ref ChiTietNamCu, ref TongTienNamMoi, ref ChiTietNamMoi, ref TieuThu_DieuChinhGia, ref  PhiBVMTNamCu, ref  ChiTietPhiBVMTNamCu, ref  PhiBVMTNamMoi, ref ChiTietPhiBVMTNamMoi, ref TienNuoc, ref ThueGTGT, ref TDVTN, ref ThueTDVTN);
                        }
                    }
                    if (ChiTietNamCu != "" || ChiTietNamMoi != "")
                    {
                        item.ChiTietTienNuoc = ChiTietNamCu + "\r\n" + ChiTietNamMoi;
                        _dbThuTien.SubmitChanges();
                    }
                    else
                        if (item.TIEUTHU == 0)
                        {
                            item.ChiTietTienNuoc = "";
                            _dbThuTien.SubmitChanges();
                        }
                }

                return "true; ";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        #endregion

        #region QLĐHN

        public string GetVersion_DHN()
        {
            CResult result = new CResult();
            try
            {
                result.message = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select Version from DeviceConfig").ToString();
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public bool UpdateUID_DHN(string MaNV, string UID)
        {
            return _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);
        }

        public string DangNhaps_DHN(string Username, string Password, string IDMobile, string UID)
        {
            try
            {
                object MaNV = null;
                MaNV = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    MaNV = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

                if (MaNV == null || MaNV.ToString() == "")
                    return "false;Sai mật khẩu hoặc IDMobile";

                //xóa máy đăng nhập MaNV khác
                object MaNV_UID_Old = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
                if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
                    _cDAL_DocSo.ExecuteNonQuery("delete DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

                //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                //{
                //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                //    }
                //}

                object MaNV_UID = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                if (MaNV_UID != null)
                    if ((int)MaNV_UID == 0)
                        _cDAL_DocSo.ExecuteNonQuery("insert DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
                    else
                        _cDAL_DocSo.ExecuteNonQuery("update DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

                _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);

                return "true;" + DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,May,Admin,Doi,ToTruong,MaTo,DienThoai from NguoiDung where MaND=" + MaNV));
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangXuats_DHN(string Username, string UID)
        {
            try
            {
                //string MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                //_cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string DangXuats_Person_DHN(string Username, string UID)
        {
            try
            {
                object MaNV = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                if (MaNV != null)
                    _cDAL_DocSo.ExecuteNonQuery("delete DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL_DocSo.ExecuteNonQuery("update NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public string getDS_Nam_DHN()
        {
            string sql = "select Nam=CAST(SUBSTRING(BillID,0,5)as int)"
                          + " from BillState"
                          + " group by SUBSTRING(BillID,0,5)"
                          + " order by Nam desc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_To_DHN()
        {
            string sql = "select MaTo,TenTo,HanhThu from [To] where HanhThu=1";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien_HanhThu_DHN()
        {
            string sql = "select MaND,HoTen,May,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from NguoiDung where MaND!=0 and May is not null and An=0 and ActiveMobile=1 order by STT asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_May_DHN()
        {
            string sql = "select May,MaTo from MayDS order by May asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_May_DHN(string MaTo)
        {
            string sql = "select May,MaTo from MayDS where MaTo=" + MaTo + " order by May asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_ViTriDHN()
        {
            return DataTableToJSON(_cDAL_DHN.ExecuteQuery_DataTable("select KyHieu from ViTriDHN"));
        }

        public string getDS_NoiDung_KinhDoanh()
        {
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select Name from MaHoa_NoiDung_Device where KinhDoanh=1"));
        }

        public string getDS_Code_DHN()
        {
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select Code,MoTa from TTDHN order by stt asc"));
        }

        public bool checkChot_BillState_DHN(string Nam, string Ky, string Dot)
        {
            return bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select BillID from BillState where BillID='" + Nam + Ky + Dot + "' and izDS=1) then 'true' else 'false' end").ToString());
        }

        public bool checkXuLy_DHN(string ID)
        {
            return bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select DocSoID from DocSo where DocSoID='" + ID + "' and StaCapNhat='1') then 'true' else 'false' end").ToString());
        }

        public bool checkChuBao_DHN(string ID)
        {
            return bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select DocSoID from DocSo where DocSoID='" + ID + "' and ChuBao=1) then 'true' else 'false' end").ToString());
        }

        public bool checkNgayDoc_DHN(string Nam, string Ky, string Dot, string May)
        {
            if (bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select NgayDoc from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where CheckNgayDoc=0 and ds.Nam=" + Nam + " and ds.Ky=" + Ky + " and dsct.IDDot=" + Dot + " and ds.ID=dsct.IDDocSo) then 'true' else 'false' end").ToString()) == true)
                return true;
            else
                if (bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select Nam from DocSoTruoc where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and May='" + May + "') then 'true' else 'false' end").ToString()) == true)
                    return true;
                else
                    return bool.Parse(_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select case when exists(select NgayDoc from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.Nam=" + Nam + " and ds.Ky=" + Ky + " and dsct.IDDot=" + Dot + " and ((dsct.NgayDoc=CAST(DATEADD(day,1,GETDATE()) as date) and CONVERT(varchar(10),GETDATE(),108)>='16:00:00') or dsct.NgayDoc<=CAST(GETDATE() as date)) and ds.ID=dsct.IDDocSo) then 'true' else 'false' end").ToString());
        }

        //ghi chỉ số
        public string getDS_GiaNuoc_DHN()
        {
            return DataTableToJSON(_cDAL_KinhDoanh.ExecuteQuery_DataTable("SELECT ID,Name,SHN,SHTM,SHVM1,SHVM2,SX,HCSN,KDDV,NgayTangGia=CONVERT(char(10),NgayTangGia,103),PhiBVMT,VAT,VAT2_Ky,VAT2 FROM GiaNuoc2"));
        }

        public string getDS_KhongTinhPBVMT_DHN()
        {
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select DanhBo from DanhBoKPBVMT"));
        }

        //        select
        //    g1.DanhBo
        //    , stuff((
        //        select ' | ' + g.DienThoai+' '+g.HoTen
        //        from SDT_DHN g        
        //        where g.DanhBo = g1.DanhBo        
        //        order by g.DienThoai
        //        for xml path('')
        //    ),1,2,'') as DienThoai
        //from SDT_DHN g1
        //group by g1.DanhBo

        public string getDS_DocSo_DHN(string Nam, string Ky, string Dot, string May)
        {
            string sql = "DECLARE @LastNamKy INT;"
                        + " declare @Nam int"
                        + " declare @Ky char(2)"
                        + " declare @Dot char(2)"
                        + " declare @May char(2)"
                        + " set @Nam=" + Nam
                        + " set @Ky='" + Ky + "'"
                        + " set @Dot='" + Dot + "'"
                        + " set @May='" + May + "'"
                        + " SET @LastNamKy = @Nam * 12  + @Ky;"
                        + " IF (OBJECT_ID('tempdb.dbo.#ChiSo', 'U') IS NOT NULL) DROP TABLE #ChiSo;"
                        + " SELECT DanhBa, MAX([ChiSo0]) AS [ChiSo0], MAX([ChiSo1]) AS [ChiSo1], MAX([ChiSo2]) AS [ChiSo2], MAX([Code0]) AS [Code0],"
                        + "     MAX([Code1]) AS [Code1], MAX([Code2]) AS [Code2], MAX([TieuThu0]) AS [TieuThu0], MAX([TieuThu1]) AS [TieuThu1],"
                        + "     MAX([TieuThu2]) AS [TieuThu2]"
                        + "     INTO #ChiSo"
                        + "     FROM ("
                        + "         SELECT DanhBa, 'ChiSo'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS ChiSoKy, 'Code'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS CodeKy,"
                        + "             'TieuThu'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS TieuThuKy, [CSCu], [CodeCu], [TieuThuCu]"
                        + "             FROM [DocSoTH].[dbo].[DocSo]"
                        + "             WHERE @LastNamKy-Nam*12-Ky between 0 and 2 and (PhanMay=@May or May=@May)) src"
                        + "     PIVOT (MAX([CSCu]) FOR ChiSoKy IN ([ChiSo0],[ChiSo1],[ChiSo2])) piv_cs"
                        + "     PIVOT (MAX([CodeCu]) FOR CodeKy IN ([Code0],[Code1],[Code2])) piv_code"
                        + "     PIVOT (MAX([TieuThuCu]) FOR TieuThuKy IN ([TieuThu0],[TieuThu1],[TieuThu2])) piv_tt"
                        + "     GROUP BY DanhBa;"
                        + "     with sdt as("
                        + "        select g1.DanhBo"
                        + "        , stuff(("
                        + "            select ' | ' + g.DienThoai+' '+g.HoTen"
                        + "            from CAPNUOCTANHOA.dbo.SDT_DHN g"
                        + "            where g.DanhBo = g1.DanhBo and SoChinh=1"
                        + "            order by CreateDate desc"
                        + "            for xml path('')"
                        + "        ),1,2,'') as DienThoai"
                        + "        from CAPNUOCTANHOA.dbo.SDT_DHN g1"
                        + "        group by g1.DanhBo)"
                        + " select ds.DocSoID,MLT=kh.LOTRINH,DanhBo=ds.DanhBa,HoTen=kh.HOTEN,SoNha=kh.SONHA,TenDuong=kh.TENDUONG,ds.Nam,ds.Ky,ds.Dot,ds.PhanMay"
                        + "                          ,Hieu=kh.HIEUDH,Co=kh.CODH,SoThan=kh.SOTHANDH,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,bd.SH,bd.SX,bd.DV,HCSN=bd.HC,ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT,PhiBVMT_Thue=ds.BVMT_Thue,TongCong=ds.TongTien"
                        + "                          ,DiaChi=(select top 1 DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO+' '+DUONG end end from HOADON_TA.dbo.HOADON where DanhBa=ds.DanhBa order by ID_HOADON desc)"
                        + "                          ,GiaBieu=ds.GB,DinhMuc=ds.DM,DinhMucHN=ds.DMHN,CSMoi,CodeMoi,TieuThuMoi,ds.TBTT,TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),cs.*"
                        + "                          ,kh.Gieng,kh.KhoaTu,kh.AmSau,kh.XayDung,kh.DutChi_Goc,kh.DutChi_Than,kh.NgapNuoc,kh.KetTuong,kh.LapKhoaGoc,kh.BeHBV,kh.BeNapMatNapHBV,kh.GayTayVan"
                        + "                          ,kh.TroNgaiThay,kh.DauChungMayBom,kh.MauSacChiGoc,ds.ChuBao,DienThoai=sdt.DienThoai,kh.GhiChu,kh.KinhDoanh"
                        + "                          ,NgayThuTien=(select CONVERT(varchar(10),NgayThuTien,103) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.ID=dsct.IDDocSo and ds.Nam=@Nam and ds.Ky=@Ky and dsct.IDDot=@Dot)"
                        + "                          ,TinhTrang=(select"
                        + "                             case when exists (select top 1 MaKQDN from HOADON_TA.dbo.TT_KQDongNuoc  kqdn,HOADON_TA.dbo.TT_DongNuoc dn where dn.MaDN=kqdn.MaDN and Huy=0 and MoNuoc=0 and TroNgaiMN=0 and kqdn.DanhBo=ds.DanhBa order by NgayDN desc)"
                        + "                             then (select top 1 N'Thu Tiền đóng nước: '+CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108) from HOADON_TA.dbo.TT_KQDongNuoc kqdn,HOADON_TA.dbo.TT_DongNuoc dn where dn.MaDN=kqdn.MaDN and Huy=0 and MoNuoc=0 and TroNgaiMN=0 and kqdn.DanhBo=ds.DanhBa order by NgayDN desc)"
                        + "                             else ''"
                        + "                             end)"
                        + "                          ,CuaHangThuHo=(select CuaHangThuHo1+CHAR(10)+case when CuaHangThuHo2 is null or CuaHangThuHo2=CuaHangThuHo1 then '' else CuaHangThuHo2 end from HOADON_TA.dbo.TT_DichVuThu_DanhBo_CuaHang where DanhBo=ds.DanhBa)"
                        + "                          from DocSo ds left join CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG kh on ds.DanhBa=kh.DANHBO"
                        + "                          left join BienDong bd on ds.DocSoID=bd.BienDongID"
                        + "                          left join #ChiSo cs on ds.DanhBa=cs.DanhBa"
                        + "                          left join sdt on sdt.DanhBo=ds.DanhBa"
                        + "                          where ds.Nam=@Nam and ds.Ky=@Ky and ds.Dot=@Dot and ds.PhanMay=@May order by ds.MLT1 asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_DocSo_Ton_DHN(string Nam, string Ky, string Dot, string May)
        {
            string sql = "select DocSoID,CodeMoi,CSMoi from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' order by MLT1 asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_Hinh_Ton_DHN(string Nam, string Ky, string Dot, string May)
        {
            string sql = "select DocSoID,GhiHinh=CAST(0 as bit) from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' order by MLT1 asc";
            DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
                if (checkExists_Hinh_DHN(dt.Rows[0]["DocSoID"].ToString()) == true)
                {
                    dt.Rows[0]["GhiHinh"] = 1;
                }
            return DataTableToJSON(dt);
        }

        public string getDS_HoaDonTon_DHN(string Nam, string Ky, string Dot, string May)
        {
            string sql = "select MaHD=hd.ID_HOADON,DanhBo=hd.DANHBA,KyHD=(RIGHT('0' + CAST(hd.Ky AS VARCHAR(2)), 2)+'/'+convert(varchar(4),hd.NAM))"
                    + " 	,hd.GiaBan,ThueGTGT=hd.THUE,PhiBVMT=hd.PHI,PhiBVMT_Thue=case when hd.ThueGTGT_TDVTN is null then 0 else hd.ThueGTGT_TDVTN end,hd.TongCong"
                    + " 	from HOADON hd,DocSoTH.dbo.DocSo ds"
                    + " 	where ds.Nam=" + Nam + " and ds.Ky='" + Ky + "' and ds.Dot='" + Dot + "' and ds.PhanMay='" + May + "'"
                    + " 	and hd.DANHBA=ds.DanhBa and NGAYGIAITRACH is null"
                    + " 	and ID_HOADON not in (select MaHD from TT_DichVuThu)"
                    + " 	and ID_HOADON not in (select MaHD from TT_TraGop)"
                    + " 	and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                    + " 	and not exists(select * from TT_ChanThuHo where Nam=hd.NAM and Ky=hd.KY and Dot=hd.DOT)--chặn thu hộ"
                //+ " 	and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and UpdatedHDDT=0)"
                 + "     and ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and SoPhieu is null)"
                //+ "     and ID_HOADON not in (select distinct FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and SoHoaDonMoi is null and (SoPhieu is null or CAST(Ngay_DC as date)<'20220701' or (NAM<2022 or (NAM=2022 and KY<5))))"
                    + " 	order by ds.MLT1,ds.DanhBa,hd.ID_HOADON";
            //string sql = "EXEC [dbo].[spGetDSHoaDonTon_DocSo]	@Nam = " + Nam + ",@Ky = N'" + Ky + "',@Dot = N'" + Dot + "',@May = N'" + May + "'";
            return DataTableToJSON(_cDAL_ThuTien.ExecuteQuery_DataTable(sql));
        }

        public string ghi_ChiSo_DHN(string ID, string Code, string ChiSo, string HinhDHN, string Dot, string MaNV, string TBTT, string Location)
        {
            CResult result = new CResult();
            try
            {
                if (ChiSo == "")
                    ChiSo = "0";
                if (checkChot_BillState_DHN(ID.Substring(0, 4), ID.Substring(4, 2), Dot) == true)
                {
                    result.success = false;
                    result.error = "Đã chốt dữ liệu";
                }
                else
                    if (checkChuBao_DHN(ID) == true)
                    {
                        result.success = false;
                        result.error = "Chủ Báo";
                    }
                    else
                        if (checkXuLy_DHN(ID) == true)
                        {
                            result.success = false;
                            result.error = "Tổ đã xử lý";
                        }
                        else
                        {
                            CHoaDon hd = new CHoaDon();
                            bool success = tinhCodeTieuThu(ID, Code, int.Parse(ChiSo), out hd.TieuThu, out hd.TienNuoc, out hd.ThueGTGT, out hd.PhiBVMT, out hd.PhiBVMT_Thue);
                            if (success == true)
                            {
                                //if (hd.TieuThu < 0)
                                //{
                                //    result.success = false;
                                //    result.error = "Tiêu Thụ âm = " + hd.TieuThu;
                                //}
                                //else
                                {
                                    string Latitude = "", Longitude = "";
                                    if (Location != "" && Location.Contains(","))
                                    {
                                        string[] Locations = Location.Split(',');
                                        Latitude = Locations[0];
                                        Longitude = Locations[1];
                                    }
                                    DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select CodeCu,CSCu from DocSo where DocSoID=" + ID);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        if (Code.Substring(0, 1) == "4" && (dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "F" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "6" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "K" || dt.Rows[0]["CodeCu"].ToString().Substring(0, 1) == "N"))
                                        {
                                            Code = "5" + dt.Rows[0]["CodeCu"].ToString().Substring(0, 1);
                                        }
                                        if (Code.Substring(0, 1) == "F" || Code == "61" || Code == "66")
                                            ChiSo = (int.Parse(dt.Rows[0]["CSCu"].ToString()) + int.Parse(TBTT)).ToString();
                                        else
                                            if (Code.Substring(0, 1) == "K")
                                                ChiSo = dt.Rows[0]["CSCu"].ToString();
                                            else
                                                if (Code.Substring(0, 1) == "N")
                                                {
                                                    //ChiSo = "0"; 
                                                }
                                                else
                                                    if (Code == "5N" || Code == "5F" || Code == "5K")
                                                        hd.CSC = (int.Parse(ChiSo) - hd.TieuThu).ToString();
                                    }
                                    hd.TongCong = hd.TienNuoc + hd.ThueGTGT + hd.PhiBVMT + hd.PhiBVMT_Thue;
                                    string sql = "update DocSo set CodeMoi=N'" + Code + "',TTDHNMoi=(select TTDHN from TTDHN where Code='" + Code + "'),CSMoi=" + ChiSo + ",TieuThuMoi=" + hd.TieuThu
                                        + ",TienNuoc=" + hd.TienNuoc + ",Thue=" + hd.ThueGTGT + ",BVMT=" + hd.PhiBVMT + ",BVMT_Thue=" + hd.PhiBVMT_Thue + ",TongTien=" + hd.TongCong
                                        + ",NVCapNhat=N'" + MaNV + "',NgayCapNhat=getdate(),GioGhi=getdate(),Latitude='" + Latitude + "',Longitude='" + Longitude + "' where DocSoID='" + ID + "'";
                                    success = _cDAL_DocSo.ExecuteNonQuery(sql);
                                    if (HinhDHN != "")
                                        success = ghi_Hinh_DHN(ID, HinhDHN);
                                    result.success = success;
                                    if (hd.TieuThu < 0)
                                    {
                                        result.error = "Tiêu Thụ âm = " + hd.TieuThu;
                                    }
                                    else
                                        if (hd.TieuThu == 0)
                                        {
                                            result.alert = "Tiêu Thụ = " + hd.TieuThu;
                                        }
                                        else
                                            if (hd.TieuThu > 0 && (hd.TieuThu < int.Parse(TBTT) - int.Parse(TBTT) * 1.4 || hd.TieuThu >= int.Parse(TBTT) * 1.4))
                                            {
                                                result.alert = "Tiêu Thụ bất thường = " + hd.TieuThu;
                                            }
                                }
                                hd.CodeMoi = Code;
                                hd.ChiSoMoi = ChiSo;
                                result.message = jss.Serialize(hd);
                                //DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select KyHD,TongCong from fnGetHoaDonTon(" + ID.Substring(6, 11) + ")");
                                //if (dt.Rows.Count > 0)
                                //    result.hoadonton = DataTableToJSON(dt);
                            }
                            else
                            {
                                result.success = false;
                                if (hd.TieuThu < 0)
                                {
                                    result.error = "Tiêu Thụ âm = " + hd.TieuThu;
                                }
                            }
                        }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string ghi_ChiSo_DHN(string ID, string Code, string ChiSo, string TieuThu, string TienNuoc, string ThueGTGT, string PhiBVMT, string PhiBVMT_Thue, string TongCong, string HinhDHN, string Dot, string MaNV, string NgayDS, string Location)
        {
            CResult result = new CResult();
            try
            {
                if (ChiSo == "")
                    ChiSo = "0";
                if (checkChot_BillState_DHN(ID.Substring(0, 4), ID.Substring(4, 2), Dot) == true)
                {
                    result.success = false;
                    result.error = "Đã chốt dữ liệu";
                }
                else
                    if (checkChuBao_DHN(ID) == true)
                    {
                        result.success = false;
                        result.error = "Chủ Báo";
                    }
                    else
                        if (checkXuLy_DHN(ID) == true)
                        {
                            result.success = false;
                            result.error = "Tổ đã xử lý";
                        }
                        else
                        {
                            string Latitude = "", Longitude = "";
                            if (Location != "" && Location.Contains(","))
                            {
                                string[] Locations = Location.Split(',');
                                Latitude = Locations[0];
                                Longitude = Locations[1];
                            }
                            IFormatProvider culture = new CultureInfo("en-US", true);
                            DateTime date = DateTime.ParseExact(NgayDS, "dd/MM/yyyy HH:mm:ss", culture);
                            string sql = "update DocSo set CodeMoi=N'" + Code + "',TTDHNMoi=(select TTDHN from TTDHN where Code='" + Code + "'),CSMoi=" + ChiSo + ",TieuThuMoi=" + TieuThu
                                + ",TienNuoc=" + TienNuoc + ",Thue=" + ThueGTGT + ",BVMT=" + PhiBVMT + ",BVMT_Thue=" + PhiBVMT_Thue + ",TongTien=" + TongCong
                                + ",NVCapNhat=N'" + MaNV + "',NgayCapNhat=getdate(),GioGhi='" + date.ToString("yyyyMMdd HH:mm:ss") + "',Latitude='" + Latitude + "',Longitude='" + Longitude + "' where DocSoID='" + ID + "'";
                            result.success = _cDAL_DocSo.ExecuteNonQuery(sql);
                            if (HinhDHN != "")
                                result.success = ghi_Hinh_DHN(ID, HinhDHN);
                        }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public byte[] get_Hinh_DHN(string ID)
        {
            //try
            //{
            //    byte[] hinh = null;
            //    string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
            //    string filename = ID.Substring(6, 11) + ".jpg";
            //    bool fileExists = File.Exists(folder + @"\" + filename);
            //    if (fileExists == true)
            //    {
            //        using (MemoryStream ms = new MemoryStream())
            //        {
            //            Image img = Image.FromFile(folder + @"\" + filename);
            //            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //            hinh = ms.ToArray();
            //        }
            //    }
            //    else
            //    {
            //        string sql = "SELECT top 1 Image " +
            //           "FROM DocSoTH_Hinh.dbo.HinhDHN " +
            //           "WHERE HinhDHNID =" + ID;
            //        hinh = (byte[])_cDAL_DocSo12.ExecuteQuery_ReturnOneValue(sql);
            //    }
            //    return hinh;
            //}
            //catch
            //{
            //    return null;
            //}
            try
            {
                byte[] hinh = null;
                string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
                string filename = ID.Substring(6, 11) + ".jpg";
                bool fileExists = File.Exists(folder + @"\" + filename);
                if (fileExists == true)
                    hinh = File.ReadAllBytes(folder + @"\" + filename);
                if (hinh.Length == 0)
                    return null;
                else
                    return hinh;
            }
            catch
            {
                return null;
            }
        }

        public bool ghi_Hinh_DHN(string ID, string HinhDHN)
        {
            try
            {
                string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
                string filename = ID.Substring(6, 11) + ".jpg";
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);
                if (File.Exists(folder + @"\" + filename) == true)
                    File.Delete(folder + @"\" + filename);
                byte[] hinh = System.Convert.FromBase64String(HinhDHN);
                File.WriteAllBytes(folder + @"\" + filename, hinh);
                return true;

                //string sql = " if exists(select ID from Temp_HinhDHN where ID=N'" + ID + "')"
                //            + " update Temp_HinhDHN set Hinh=N'" + HinhDHN + "' where ID=N'" + ID + "'"
                //            + " else"
                //            + " insert into Temp_HinhDHN(ID,Hinh)values(N'" + ID + "',N'" + HinhDHN + "')";
                //return _cDAL_DocSo.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool xoa_Hinh_DHN(string ID)
        {
            try
            {
                string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
                string filename = ID.Substring(6, 11) + ".jpg";
                if (File.Exists(folder + @"\" + filename) == true)
                    File.Delete(folder + @"\" + filename);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool checkExists_Hinh_DHN(string ID)
        {
            try
            {
                string folder = CGlobalVariable.pathHinhDHN + @"\" + ID.Substring(0, 6);
                string filename = ID.Substring(6, 11) + ".jpg";
                return File.Exists(folder + @"\" + filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string get_ThongTin_DHN(string DanhBo, string Nam, string Ky)
        {
            CResult result = new CResult();
            try
            {
                DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select MLT=ds.MLT1,DanhBo=ds.DanhBa,ttkh.HoTen,DiaChi=ttkh.SoNha+' '+ttkh.TenDuong,ds.Nam,ds.Ky,GiaBieu=bd.GB,DinhMuc=bd.DM"
                    + " ,TuNgay=CONVERT(varchar(10),ds.TuNgay,103),DenNgay=CONVERT(varchar(10),ds.DenNgay,103),ds.CSCu,ds.CodeMoi,ds.CSMoi,ds.TieuThuMoi"
                    + " ,ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT,PhiBVMT_Thue=ds.BVMT_Thue,TongCong=ds.TongTien"
                    + " from DocSo ds,BienDong bd,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh where ds.DocSoID=bd.BienDongID and ds.DanhBa='" + DanhBo + "' and ds.Nam=" + Nam + " and ds.Ky=" + Ky + " and ds.DanhBa=ttkh.DANHBO");
                CHoaDon hd = new CHoaDon();
                hd.Ky = dt.Rows[0]["Ky"].ToString();
                hd.Nam = dt.Rows[0]["Nam"].ToString();
                hd.TuNgay = dt.Rows[0]["TuNgay"].ToString();
                hd.DenNgay = dt.Rows[0]["DenNgay"].ToString();
                hd.MLT = dt.Rows[0]["MLT"].ToString();
                hd.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                hd.HoTen = dt.Rows[0]["HoTen"].ToString();
                hd.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                hd.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                hd.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                hd.CodeMoi = dt.Rows[0]["CodeMoi"].ToString();
                hd.CSC = dt.Rows[0]["CSCu"].ToString();
                hd.ChiSoMoi = dt.Rows[0]["CSMoi"].ToString();
                hd.TieuThuMoi = dt.Rows[0]["TieuThuMoi"].ToString();
                hd.TienNuoc = int.Parse(dt.Rows[0]["TienNuoc"].ToString());
                hd.ThueGTGT = int.Parse(dt.Rows[0]["ThueGTGT"].ToString());
                hd.PhiBVMT = int.Parse(dt.Rows[0]["PhiBVMT"].ToString());
                hd.PhiBVMT_Thue = int.Parse(dt.Rows[0]["PhiBVMT_Thue"].ToString());
                hd.TongCong = int.Parse(dt.Rows[0]["TongCong"].ToString());
                result.message = jss.Serialize(hd);
                byte[] hinh = get_Hinh_DHN(Nam + Ky + DanhBo);
                if (hinh != null)
                    result.alert = Convert.ToBase64String(hinh);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public void getTonFromDienThoai_DHN()
        {
            string sql = "select Nam,Ky=RIGHT('0' + CAST(Ky AS VARCHAR(2)), 2),Dot=RIGHT('0' + CAST(IDDot AS VARCHAR(2)), 2) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct,Lich_Dot dot"
                         + " where ds.ID=dsct.IDDocSo and dot.ID=dsct.IDDot and CAST(dsct.NgayDoc as date)=CAST(GETDATE() as date)";
            DataTable dtKy = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
            DataTable dtDocSo = _cDAL_DocSo.ExecuteQuery_DataTable("select DocSoID,CodeMoi,Dot from DocSo where Nam=" + dtKy.Rows[0]["Nam"].ToString() + " and Ky='" + dtKy.Rows[0]["Ky"].ToString() + "' and Dot='" + dtKy.Rows[0]["Dot"].ToString() + "'");
            foreach (DataRow item in dtDocSo.Rows)
                if (item["CodeMoi"] == null || item["CodeMoi"].ToString() == "")
                {
                    _cDAL_DocSo.ExecuteNonQuery("exec [dbo].[spSendNotificationToClient] N'CodeTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'");
                }
                else
                    if (checkExists_Hinh_DHN(item["DocSoID"].ToString()) == false)
                    {
                        _cDAL_DocSo.ExecuteNonQuery("exec [dbo].[spSendNotificationToClient] N'HinhTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'");
                    }
            int count = 5;
            int Nam = int.Parse(dtKy.Rows[0]["Nam"].ToString());
            int Ky = int.Parse(dtKy.Rows[0]["Ky"].ToString());
            int Dot = int.Parse(dtKy.Rows[0]["Dot"].ToString());
            while (count > 0)
            {
                Dot--;
                if (Dot == 0)
                {
                    Dot = 20;
                    Ky--;
                    if (Ky == 0)
                    {
                        Ky = 12;
                        Nam--;
                    }
                }
                DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select DocSoID,CodeMoi,Dot from DocSo where Nam=" + Nam.ToString() + " and Ky='" + Ky.ToString("00") + "' and Dot='" + Dot.ToString("00") + "'");
                foreach (DataRow item in dt.Rows)
                    if (item["CodeMoi"].ToString() != "" && checkExists_Hinh_DHN(item["DocSoID"].ToString()) == false)
                    {
                        sql = "exec [dbo].[spSendNotificationToClient] N'HinhTon',N'" + item["Dot"].ToString() + "',N'" + item["DocSoID"].ToString() + "'";
                        _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                count--;
            }
        }

        public string getTonCongTy_DHN(string Nam, string Ky, string Dot, string May)
        {
            CResult result = new CResult();
            try
            {
                string sql = "select ChuaDoc=(select COUNT(DocSoID) from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' and CodeMoi like '')"
                    + ",F=(select COUNT(DocSoID) from DocSo where Nam=" + Nam + " and Ky='" + Ky + "' and Dot='" + Dot + "' and PhanMay='" + May + "' and CodeMoi like 'F%')";
                result.message = DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string getDS_LichSu_DocSo_DHN(string DanhBo)
        {
            CResult result = new CResult();
            try
            {
                string sql = "select top 12 Ky=ds.Ky+'/'+CONVERT(char(4),ds.Nam),CodeMoi,CSMoi,TieuThuMoi,GiaBieu=bd.GB,DinhMuc=bd.DM,DinhMucHN=bd.DMHN"
                    + ",TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT"
                    + ",PhiBVMT_Thue=case when ds.BVMT_Thue is null then 0 else ds.BVMT_Thue end,TongCong=ds.TongTien"
                    + " from DocSo ds,BienDong bd where ds.DanhBa='" + DanhBo.Replace(" ", "") + "' and ds.DocSoID=bd.BienDongID order by ds.DocSoID desc";
                result.message = DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        //ghi chú
        public string get_GhiChu_DHN(string DanhBo)
        {
            string sql = "select SoNha,TenDuong,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,Gieng,KhoaTu,AmSau,XayDung,NgapNuoc,KetTuong,LapKhoaGoc,BeHBV,BeNapMatNapHBV from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo.Replace(" ", "") + "'";
            return DataTableToJSON(_cDAL_DHN.ExecuteQuery_DataTable(sql));
        }

        public string update_GhiChu_DHN(string DanhBo, string SoNha, string TenDuong, string ViTri, string ViTriNgoai, string ViTriHop, string Gieng, string KhoaTu, string AmSau, string XayDung, string DutChiGoc, string DutChiThan
            , string NgapNuoc, string KetTuong, string LapKhoaGoc, string BeHBV, string BeNapMatNapHBV, string GayTayVan, string TroNgaiThay, string DauChungMayBom, string MauSacChiGoc, string GhiChu, string KinhDoanh, string MaNV)
        {
            CResult result = new CResult();
            try
            {
                string flagGieng = bool.Parse(Gieng) == true ? "1" : "0";
                string flagKhoaTu = bool.Parse(KhoaTu) == true ? "1" : "0";
                string flagAmSau = bool.Parse(AmSau) == true ? "1" : "0";
                string flagXayDung = bool.Parse(XayDung) == true ? "1" : "0";
                string flagDutChiGoc = bool.Parse(XayDung) == true ? "1" : "0";
                string flagDutChiThan = bool.Parse(XayDung) == true ? "1" : "0";
                string flagNgapNuoc = bool.Parse(NgapNuoc) == true ? "1" : "0";
                string flagKetTuong = bool.Parse(KetTuong) == true ? "1" : "0";
                string flagLapKhoaGoc = bool.Parse(LapKhoaGoc) == true ? "1" : "0";
                string flagBeHBV = bool.Parse(BeHBV) == true ? "1" : "0";
                string flagBeNapMatNapHBV = bool.Parse(BeNapMatNapHBV) == true ? "1" : "0";
                string flagGayTayVan = bool.Parse(GayTayVan) == true ? "1" : "0";
                string flagTroNgaiThay = bool.Parse(TroNgaiThay) == true ? "1" : "0";
                string flagDauChungMayBom = bool.Parse(DauChungMayBom) == true ? "1" : "0";
                string flagViTriNgoai = bool.Parse(ViTriNgoai) == true ? "1" : "0";
                string flagViTriHop = bool.Parse(ViTriHop) == true ? "1" : "0";
                //string sql = "";
                //sql += "update TB_DULIEUKHACHHANG set AmSau=" + flagAmSau + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and AmSau=1";
                //sql += " update TB_DULIEUKHACHHANG set XayDung=" + flagXayDung + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and XayDung=1";
                //sql += " update TB_DULIEUKHACHHANG set DutChi_Goc=" + flagDutChiGoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and DutChi_Goc=1";
                //sql += " update TB_DULIEUKHACHHANG set DutChi_Than=" + flagDutChiThan + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and DutChi_Than=1";
                //sql += " update TB_DULIEUKHACHHANG set NgapNuoc=" + flagNgapNuoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and NgapNuoc=1";
                //sql += " update TB_DULIEUKHACHHANG set KetTuong=" + flagKetTuong + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and KetTuong=1";
                //sql += " update TB_DULIEUKHACHHANG set LapKhoaGoc=" + flagLapKhoaGoc + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and LapKhoaGoc=1";
                //sql += " update TB_DULIEUKHACHHANG set BeHBV=" + flagBeHBV + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and BeHBV=1";
                //sql += " update TB_DULIEUKHACHHANG set BeNapMatNapHBV=" + flagBeNapMatNapHBV + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and BeNapMatNapHBV=1";
                //sql += " update TB_DULIEUKHACHHANG set GayTayVan=" + flagGayTayVan + " where DanhBo='" + DanhBo.Replace(" ", "") + "' and GayTayVan=1";
                //sql += " update TB_DULIEUKHACHHANG set SoNha=N'" + SoNha + "',TenDuong=N'" + TenDuong + "',VITRIDHN=N'" + ViTri + "',ViTriDHN_Ngoai=" + flagViTriNgoai + ",ViTriDHN_Hop=" + flagViTriHop
                //    + ",Gieng=" + flagGieng + ",KhoaTu=" + flagKhoaTu
                //    + ",MauSacChiGoc=N'" + MauSacChiGoc + "',GhiChu=N'" + GhiChu + "',MODIFYBY=" + MaNV + ",MODIFYDATE=getdate() where DanhBo='" + DanhBo.Replace(" ", "") + "'";
                string sql = "update TB_DULIEUKHACHHANG set SoNha=N'" + SoNha + "',TenDuong=N'" + TenDuong + "',VITRIDHN=N'" + ViTri + "',ViTriDHN_Ngoai=" + flagViTriNgoai + ",ViTriDHN_Hop=" + flagViTriHop
                    + ",Gieng=" + flagGieng + ",KhoaTu=" + flagKhoaTu
                    //+ ",AmSau=" + flagAmSau
                    //+ ",XayDung=" + flagXayDung
                    //+ ",DutChi_Goc=" + flagDutChiGoc
                    //+ ",DutChi_Than=" + flagDutChiThan
                    //+ ",NgapNuoc=" + flagNgapNuoc
                    //+ ",KetTuong=" + flagKetTuong
                    //+ ",LapKhoaGoc=" + flagLapKhoaGoc
                    //+ ",BeHBV=" + flagBeHBV
                    //+ ",BeNapMatNapHBV=" + flagBeNapMatNapHBV
                    //+ ",GayTayVan=" + flagGayTayVan
                    //+ ",TroNgaiThay=" + flagTroNgaiThay
                    //+ ",DauChungMayBom=" + flagDauChungMayBom
                    + ",MauSacChiGoc=N'" + MauSacChiGoc + "',GhiChu=N'" + GhiChu + "',KinhDoanh=N'" + KinhDoanh + "',MODIFYBY=" + MaNV + ",MODIFYDATE=getdate() where DanhBo='" + DanhBo.Replace(" ", "") + "'";
                result.success = _cDAL_DHN.ExecuteNonQuery(sql);
                if (result.success)
                {
                    //string sql2 = "";
                    //if (flagAmSau == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Âm Sâu',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagXayDung == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Xây Dựng',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagDutChiGoc == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Đứt Chì Góc',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagDutChiThan == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Đứt Chì Thân',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagNgapNuoc == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Ngập Nước',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagKetTuong == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Kẹt Tường',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagLapKhoaGoc == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Lấp Khóa Góc',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagBeHBV == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Bể HBV',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagBeNapMatNapHBV == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Bể Nấp, Mất Nấp HBV',N'Xóa'," + MaNV + ",getdate())";
                    //if (flagGayTayVan == "0")
                    //    sql2 += " insert into MaHoa_PhieuChuyen_LichSu(DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values('" + DanhBo + "',N'Gãy Tay Van',N'Xóa'," + MaNV + ",getdate())";
                    //if (sql2 != "")
                    //    _cDAL_DocSo.ExecuteNonQuery(sql2);
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string getDS_DienThoai_DHN(string DanhBo)
        {
            CResult result = new CResult();
            try
            {
                result.message = DataTableToJSON(_cDAL_DHN.ExecuteQuery_DataTable("select DanhBo,DienThoai,HoTen,SoChinh,GhiChu from SDT_DHN where DanhBo='" + DanhBo.Replace(" ", "") + "' order by CreateDate desc"));
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string update_DienThoai_DHN(string DanhBo, string DienThoai, string HoTen, string SoChinh, string MaNV)
        {
            CResult result = new CResult();
            try
            {
                if (DienThoai.Length != 10 || DienThoai.All(char.IsNumber) == false)
                {
                    result.success = false;
                    result.error = "Không đủ 10 số";
                }
                else
                {
                    string flagSoChinh = bool.Parse(SoChinh) == true ? "1" : "0";
                    string sql = "declare @DanhBo char(11)"
                            + " declare @DienThoai varchar(15)"
                            + " declare @HoTen nvarchar(50)"
                            + " declare @SoChinh bit"
                            + " declare @GhiChu nvarchar(50)"
                            + " set @DanhBo='" + DanhBo.Replace(" ", "") + "'"
                            + " set @DienThoai='" + DienThoai + "'"
                            + " set @HoTen=N'" + HoTen + "'"
                            + " set @SoChinh=" + flagSoChinh
                            + " set @GhiChu=N'Đ. QLĐHN'"
                            + " if exists(select DanhBo from SDT_DHN where DanhBo=@DanhBo and DienThoai=@DienThoai)"
                            + " update SDT_DHN set HoTen=@HoTen,SoChinh=@SoChinh,GhiChu=@GhiChu,ModifyBy=" + MaNV + ",ModifyDate=GETDATE() where DanhBo=@DanhBo and DienThoai=@DienThoai"
                            + " else"
                            + " insert into SDT_DHN(DanhBo,DienThoai,HoTen,SoChinh,GhiChu,CreateBy,CreateDate)values(@DanhBo,@DienThoai,@HoTen,@SoChinh,@GhiChu," + MaNV + ",GETDATE())";
                    result.success = _cDAL_DHN.ExecuteNonQuery(sql);
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string delete_DienThoai_DHN(string DanhBo, string DienThoai)
        {
            CResult result = new CResult();
            try
            {
                result.success = _cDAL_DHN.ExecuteNonQuery("delete SDT_DHN where DanhBo='" + DanhBo.Replace(" ", "") + "' and DienThoai='" + DienThoai + "'");
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        //phiếu chuyển
        public string getDS_PhieuChuyen_DHN()
        {
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select [Name],KhongLapDon from MaHoa_PhieuChuyen where App=1 order by [Name] asc"));
        }

        public string ghi_DonTu_DHN(string DanhBo, string NoiDung, string GhiChu, string Hinh, string MaNV)
        {
            CResult result = new CResult();
            try
            {
                DataTable dtPC = _cDAL_DocSo.ExecuteQuery_DataTable("select Folder from MaHoa_PhieuChuyen where App=1 and KhongLapDon=1 and Name=N'" + NoiDung + "'");
                if (dtPC != null && dtPC.Rows.Count > 0)
                {
                    object checkExists = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_PhieuChuyen_LichSu where DanhBo='" + DanhBo + "' and NoiDung=N'" + NoiDung + "' and TinhTrang=N'Tồn'");
                    if (checkExists != null)
                    {
                        result.success = false;
                        result.error = "Danh Bộ đã báo nội dung này rồi";
                    }
                    else
                    {
                        CHoaDon hd = new CHoaDon();
                        //switch (NoiDung)
                        //{
                        //    case "Đứt Chì Góc":
                        //        result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set DutChi_Goc=1,DutChi_Goc_Ngay=getdate() where DanhBo='" + DanhBo + "'");
                        //        break;
                        //    case "Đứt Chì Thân":
                        //        result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set DutChi_Than=1,DutChi_Than_Ngay=getdate() where DanhBo='" + DanhBo + "'");
                        //        break;
                        //    //case "Ngập Nước":
                        //    //    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set NgapNuoc=1,NgapNuoc_Ngay=getdate() where DanhBo='" + DanhBo + "' and NgapNuoc=0");
                        //    //    break;
                        //    //case "Kẹt Tường":
                        //    //    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set KetTuong=1,KetTuong_Ngay=getdate() where DanhBo='" + DanhBo + "' and KetTuong=0");
                        //    //    break;
                        //    //case "Lấp Khóa Góc":
                        //    //    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set LapKhoaGoc=1,LapKhoaGoc_Ngay=getdate() where DanhBo='" + DanhBo + "' and LapKhoaGoc=0");
                        //    //    break;
                        //    //case "Bể HBV":
                        //    //    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set BeHBV=1,BeHBV_Ngay=getdate() where DanhBo='" + DanhBo + "' and BeHBV=0");
                        //    //    break;
                        //    //case "Bể Nấp, Mất Nấp HBV":
                        //    //    result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set BeNapMatNapHBV=1,BeNapMatNapHBV_Ngay=getdate() where DanhBo='" + DanhBo + "' and BeNapMatNapHBV=0");
                        //    //    break;
                        //    default:
                        //        result.success = _cDAL_DHN.ExecuteNonQuery("update TB_DULIEUKHACHHANG set " + dtPC.Rows[0]["Folder"].ToString() + "=1," + dtPC.Rows[0]["Folder"].ToString() + "_Ngay=getdate() where DanhBo='" + DanhBo + "'");
                        //        break;
                        //}
                        switch (NoiDung)
                        {
                            case "Âm Sâu":
                            case "Ngập Nước":
                            case "Kẹt Tường":
                            case "Lấp Khóa Góc":
                                hd.TieuThu = 1;
                                break;
                        }
                        result.message = jss.Serialize(hd);
                        string ID = "";
                        checkExists = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_PhieuChuyen_LichSu where ID like '" + DateTime.Now.ToString("yy") + "%'");
                        if (checkExists != null)
                        {
                            object stt = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MAX(SUBSTRING(CAST(ID as varchar(8)),3,5))+1 from MaHoa_PhieuChuyen_LichSu where ID like '" + DateTime.Now.ToString("yy") + "%'");
                            if (stt != null)
                                ID = DateTime.Now.ToString("yy") + ((int)stt).ToString("00000");
                        }
                        else
                        {
                            ID = DateTime.Now.ToString("yy") + 1.ToString("00000");
                        }
                        result.success = _cDAL_DocSo.ExecuteNonQuery("insert into MaHoa_PhieuChuyen_LichSu(ID,DanhBo,NoiDung,GhiChu,CreateBy,CreateDate)values(" + ID + ",'" + DanhBo + "',N'" + NoiDung + "',N'" + GhiChu + "'," + MaNV + ",getdate())");
                        if (Hinh != "")
                            result.success = ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, dtPC.Rows[0]["Folder"].ToString(), "", DanhBo + ".jpg", Hinh);
                    }
                }
                else
                {
                    DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select top 1 MLT=MLT1,HoTen=TENKH,DiaChi=SO+' '+DUONG,GiaBieu=GB,DinhMuc=DM,DinhMucHN=DMHN,Dot,Ky,Nam,Phuong,Quan,HopDong from BienDong where DanhBa='" + DanhBo + "' order by BienDongID desc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataTable checkExists_DanhBoBoQua = _cDAL_DocSo.ExecuteQuery_DataTable("select DanhBo,NoiDung from MaHoa_DanhBo_Except where DanhBo='" + DanhBo + "'");
                        if (NoiDung.Contains("Giá Biểu") && checkExists_DanhBoBoQua != null && checkExists_DanhBoBoQua.Rows.Count > 0)
                        {
                            result.success = false;
                            if (string.IsNullOrEmpty(checkExists_DanhBoBoQua.Rows[0]["NoiDung"].ToString()))
                                result.error = "Danh Bộ VIP";
                            else
                                result.error = checkExists_DanhBoBoQua.Rows[0]["NoiDung"].ToString();
                        }
                        else
                        {
                            object checkExists = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_DonTu where DanhBo='" + DanhBo + "' and NoiDung=N'" + NoiDung + "' and cast(getdate() as date)=cast(createdate as date)");
                            if (checkExists == null)
                            {
                                if (NoiDung.Contains("Giá Biểu") && checkExists_DonTu(DanhBo, NoiDung, "30"))
                                {
                                    result.success = false;
                                    result.error = "Danh Bộ có đơn Thương Vụ cùng nội dung trong 30 ngày";
                                }
                                else
                                {
                                    string ID = "";
                                    checkExists = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 ID from MaHoa_DonTu where ID like '" + DateTime.Now.ToString("yy") + "%'");
                                    if (checkExists != null)
                                    {
                                        object stt = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select MAX(SUBSTRING(CAST(ID as varchar(8)),3,5))+1 from MaHoa_DonTu where ID like '" + DateTime.Now.ToString("yy") + "%'");
                                        if (stt != null)
                                            ID = DateTime.Now.ToString("yy") + ((int)stt).ToString("00000");
                                    }
                                    else
                                    {
                                        ID = DateTime.Now.ToString("yy") + 1.ToString("00000");
                                    }
                                    string DinhMucHN = "NULL";
                                    if (dt.Rows[0]["DinhMucHN"].ToString() != "")
                                        DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
                                    string sql = "insert into MaHoa_DonTu(ID,MLT,DanhBo,HoTen,DiaChi,GiaBieu,DinhMuc,DinhMucHN,NoiDung,GhiChu,Dot,Ky,Nam,Phuong,Quan,CreateBy,CreateDate,HopDong)values"
                                        + "("
                                        + ID + ",'" + dt.Rows[0]["MLT"] + "','" + DanhBo + "',N'" + dt.Rows[0]["HoTen"] + "',N'" + dt.Rows[0]["DiaChi"] + "'"
                                        + "," + dt.Rows[0]["GiaBieu"] + "," + dt.Rows[0]["DinhMuc"] + "," + DinhMucHN + ",N'" + NoiDung + "',N'" + GhiChu + "'," + dt.Rows[0]["Dot"]
                                        + "," + dt.Rows[0]["Ky"] + "," + dt.Rows[0]["Nam"] + "," + dt.Rows[0]["Phuong"] + "," + dt.Rows[0]["Quan"] + "," + MaNV + ",getdate(),N'" + dt.Rows[0]["HopDong"] + "'"
                                        + ")";
                                    result.success = _cDAL_DocSo.ExecuteNonQuery(sql);
                                    if (Hinh != "")
                                        result.success = ghi_Hinh_DonTu_DHN(ID, Hinh, MaNV);
                                    CHoaDon hd = new CHoaDon();
                                    hd.TieuThu = int.Parse(ID);
                                    result.message = jss.Serialize(hd);
                                }
                            }
                            else
                            {
                                result.success = false;
                                result.error = "Danh Bộ đã lập Đơn cùng nội dung trong ngày";
                            }
                        }
                    }
                    else
                        result.success = false;
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public string getDS_DonTu_DHN(string DanhBo)
        {
            CResult result = new CResult();
            try
            {
                result.message = DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable("select CreateDate=CONVERT(char(10),CreateDate,103),NoiDung,TinhTrang from MaHoa_DonTu where DanhBo='" + DanhBo + "'"
+ " union all"
+ " select CreateDate=CONVERT(char(10),CreateDate,103),NoiDung,TinhTrang from MaHoa_PhieuChuyen_LichSu where DanhBo='" + DanhBo + "'"
+ " order by CreateDate desc"));
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        public bool ghi_Hinh_DonTu_DHN(string ID, string Hinh, string MaNV)
        {
            try
            {
                string filename = DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
                string sql = "insert into MaHoa_DonTu_Hinh(ID,IDParent,Name,Loai,CreateBy,CreateDate)values((select case when exists(select ID from MaHoa_DonTu_Hinh) then (select MAX(ID)+1 from MaHoa_DonTu_Hinh) else 1 end)," + ID + ",N'" + filename + "',N'.jpg'," + MaNV + ",getdate())";
                _cDAL_DocSo.ExecuteNonQuery(sql);
                ghi_Hinh_241(CGlobalVariable.pathHinhDHNMaHoa, "DonTu", ID, filename + ".jpg", Hinh);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //đọc số
        public DataTable getDocSo(string DanhBo, string Nam, string Ky)
        {
            string sql = "select TuNgay,DenNgay from DocSo where DanhBa='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky;
            return _cDAL_DocSo.ExecuteQuery_DataTable(sql);
        }

        public bool checkKhongTinhPBVMT(string DanhBo)
        {
            if (_cDAL_DocSo.ExecuteQuery_ReturnOneValue("select DanhBo from DanhBoKPBVMT where DanhBo='" + DanhBo + "'") != null)
                return true;
            else
                return false;
        }

        List<string> lstTBTT = new List<string> { "60", "61", "62", "63", "64", "66", "80", "F1", "F2", "F3", "F4" };

        public bool tinhCodeTieuThu(string DocSoID, string Code, int CSM, out int TieuThu, out int TienNuoc, out int ThueGTGT, out int TDVTN, out int ThueTDVTN)
        {
            try
            {
                TienNuoc = ThueGTGT = TDVTN = ThueTDVTN = 0;
                string sql = "EXEC [dbo].[spTinhTieuThu]"
                   + " @DANHBO = N'" + DocSoID.Substring(6, 11) + "',"
                   + " @KY = " + DocSoID.Substring(4, 2) + ","
                   + " @NAM = " + DocSoID.Substring(0, 4) + ","
                   + " @CODE = N'" + Code + "',"
                   + " @CSMOI = " + CSM;
                object result = _cDAL_DocSo.ExecuteQuery_ReturnOneValue(sql);
                if (result != null)
                    TieuThu = (int)result;
                else
                    TieuThu = -1;
                if (TieuThu < 0)
                    return true;
                DataTable dtDocSo = _cDAL_DocSo.ExecuteQuery_DataTable("select * from DocSo where DocSoID='" + DocSoID + "'");
                DataTable dtBienDong = _cDAL_DocSo.ExecuteQuery_DataTable("select * from BienDong where BienDongID='" + DocSoID + "'");
                if (dtDocSo != null && dtDocSo.Rows.Count > 0 && dtBienDong != null && dtBienDong.Rows.Count > 0)
                {
                    int DinhMuc = int.Parse(dtDocSo.Rows[0]["DM"].ToString());
                    int DinhMucHN = int.Parse(dtDocSo.Rows[0]["DMHN"].ToString());
                    if (lstTBTT.Contains(Code) && dtDocSo.Rows[0]["Nam"].ToString() == "2023" && dtDocSo.Rows[0]["Ky"].ToString() == "01")
                    {
                        TimeSpan Time = DateTime.Parse(dtDocSo.Rows[0]["DenNgay"].ToString()) - DateTime.Parse(dtDocSo.Rows[0]["TuNgay"].ToString());
                        int TongSoNgay = Time.Days;
                        double motngay = Math.Round(double.Parse(dtDocSo.Rows[0]["TBTT"].ToString()) / 30, 2, MidpointRounding.AwayFromZero);
                        TieuThu = (int)Math.Round(motngay * TongSoNgay);
                    }
                    int TienNuocNamCu = 0, TienNuocNamMoi = 0, PhiBVMTNamCu = 0, PhiBVMTNamMoi = 0, TieuThu_DieuChinhGia = 0;
                    string ChiTietA = "", ChiTietB = "", ChiTietPhiBVMTA = "", ChiTietPhiBVMTB = "";
                    TinhTienNuoc(false, false, false, 0, dtBienDong.Rows[0]["DanhBa"].ToString(), int.Parse(dtBienDong.Rows[0]["Ky"].ToString()), int.Parse(dtBienDong.Rows[0]["Nam"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["TuNgay"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["DenNgay"].ToString())
                         , int.Parse(dtDocSo.Rows[0]["GB"].ToString()), int.Parse(dtBienDong.Rows[0]["SH"].ToString()), int.Parse(dtBienDong.Rows[0]["SX"].ToString()), int.Parse(dtBienDong.Rows[0]["DV"].ToString()), int.Parse(dtBienDong.Rows[0]["HC"].ToString())
                         , DinhMuc, DinhMucHN, TieuThu, ref TienNuocNamCu, ref ChiTietA, ref TienNuocNamMoi, ref ChiTietB, ref TieuThu_DieuChinhGia, ref PhiBVMTNamCu, ref ChiTietPhiBVMTA, ref PhiBVMTNamMoi, ref ChiTietPhiBVMTB, ref TienNuoc, ref ThueGTGT, ref TDVTN, ref ThueTDVTN);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool tinhCodeTieuThu(string DocSoID, string Code, int TieuThu, out int TienNuoc, out int ThueGTGT, out int TDVTN, out int ThueTDVTN)
        {
            try
            {
                TienNuoc = ThueGTGT = TDVTN = ThueTDVTN = 0;
                if (TieuThu < 0)
                    return false;
                DataTable dtDocSo = _cDAL_DocSo.ExecuteQuery_DataTable("select * from DocSo where DocSoID='" + DocSoID + "'");
                DataTable dtBienDong = _cDAL_DocSo.ExecuteQuery_DataTable("select * from BienDong where BienDongID='" + DocSoID + "'");
                if (dtDocSo != null && dtDocSo.Rows.Count > 0 && dtBienDong != null && dtBienDong.Rows.Count > 0)
                {
                    int DinhMuc = int.Parse(dtDocSo.Rows[0]["DM"].ToString());
                    int DinhMucHN = int.Parse(dtDocSo.Rows[0]["DMHN"].ToString());
                    //if (dtDocSo.Rows[0]["Nam"].ToString() == "2023" && dtDocSo.Rows[0]["Ky"].ToString() == "01")
                    //{
                    //    TimeSpan Time = DateTime.Parse(dtDocSo.Rows[0]["DenNgay"].ToString()) - DateTime.Parse(dtDocSo.Rows[0]["TuNgay"].ToString());
                    //    int TongSoNgay = Time.Days;
                    //    double motngay = Math.Round(double.Parse(DinhMuc.ToString()) / 30, 2, MidpointRounding.AwayFromZero);
                    //    double motngayHN = Math.Round(double.Parse(DinhMucHN.ToString()) / 30, 2, MidpointRounding.AwayFromZero);
                    //    DinhMuc = (int)Math.Round(motngay * TongSoNgay);
                    //    DinhMucHN = (int)Math.Round(motngayHN * TongSoNgay);
                    //}
                    int TienNuocNamCu = 0, TienNuocNamMoi = 0, PhiBVMTNamCu = 0, PhiBVMTNamMoi = 0, TieuThu_DieuChinhGia = 0;
                    string ChiTietA = "", ChiTietB = "", ChiTietPhiBVMTA = "", ChiTietPhiBVMTB = "";
                    TinhTienNuoc(false, false, false, 0, dtBienDong.Rows[0]["DanhBa"].ToString(), int.Parse(dtBienDong.Rows[0]["Ky"].ToString()), int.Parse(dtBienDong.Rows[0]["Nam"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["TuNgay"].ToString()), DateTime.Parse(dtDocSo.Rows[0]["DenNgay"].ToString())
                         , int.Parse(dtDocSo.Rows[0]["GB"].ToString()), int.Parse(dtBienDong.Rows[0]["SH"].ToString()), int.Parse(dtBienDong.Rows[0]["SX"].ToString()), int.Parse(dtBienDong.Rows[0]["DV"].ToString()), int.Parse(dtBienDong.Rows[0]["HC"].ToString())
                         , DinhMuc, DinhMucHN, TieuThu, ref TienNuocNamCu, ref ChiTietA, ref TienNuocNamMoi, ref ChiTietB, ref TieuThu_DieuChinhGia, ref PhiBVMTNamCu, ref ChiTietPhiBVMTA, ref PhiBVMTNamMoi, ref ChiTietPhiBVMTB, ref TienNuoc, ref ThueGTGT, ref TDVTN, ref ThueTDVTN);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SendNotificationToClient_DHN(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            string responseMess = "";
            try
            {
                string serverKey = "AAAAEFkFujs:APA91bEkg2KLk53WsmZXHxTfU2AgElSDTq1GG5UsUAsCffgrXlex3wGU3rnp0iWX-GAgIm0JW9Qvq22aCQy0X-ns8LyrvPSHzmk1w2iSdK440VxRYHL9nOdNaKAAaAo_iXB7wlZCrdQi";
                string senderId = "70213024315";

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
                        Title = Title,
                        Body = Content,
                        Action = Action,
                        NameUpdate = NameUpdate,
                        ValueUpdate = ValueUpdate,
                        ID = ID,
                    }
                };


                var json = jss.Serialize(data);
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
                    _cDAL_DocSo.ExecuteNonQuery("insert into Temp(Name,Value,MaHD,Result)values(N'" + Title + "|" + Content + "|" + Action + "|" + NameUpdate + "|" + ValueUpdate + "',N'" + UID + "',N'" + ID + "',N'" + responseMess + "')");
                }
                else
                {
                    responseMess = "Error: " + respuesta.StatusCode;
                }
            }
            catch (Exception ex)
            {
                responseMess = "Error: " + ex.Message;
            }
            return responseMess;
        }

        public string dangKyQRCode(string DanhBo, string ID)
        {
            CResult result = new CResult();
            try
            {
                result.success = _cDAL_TTKH.ExecuteNonQuery("update QR_Dong set DanhBo='" + DanhBo + "' where DanhBo is null and ID=" + ID);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return jss.Serialize(result);
        }

        //quản lý
        public string getDS_TheoDoi_DHN(string MaTo, string Nam, string Ky, string Dot)
        {
            string sql = "select May,Tong=COUNT(DocSoID)"
                    + " ,DaDoc=COUNT(CASE WHEN CodeMoi not like '' THEN 1 END)"
                    + " ,ChuaDoc=COUNT(CASE WHEN CodeMoi like '' THEN 1 END)"
                    + " ,CodeF=COUNT(CASE WHEN CodeMoi like 'F%' THEN 1 END)"
                    + " from DocSo where Nam=" + Nam + " and Ky=" + Ky + " and Dot=" + Dot + " and (select TuMay from [To] where MaTo=" + MaTo + ")<=May and May<=(select DenMay from [To] where MaTo=" + MaTo + ")"
                    + " group by May";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_BatThuong_DHN(string MaTo, string Nam, string Ky, string Dot)
        {
            string sql = "DECLARE @LastNamKy INT;"
                        + " declare @Nam int"
                        + " declare @Ky char(2)"
                        + " declare @Dot char(2)"
                        + " declare @TuMay char(2)"
                        + " declare @DenMay char(2)"
                        + " set @Nam=" + Nam
                        + " set @Ky='" + Ky + "'"
                        + " set @Dot='" + Dot + "'"
                        + " set @TuMay=RIGHT('0' + CAST((select TuMay from [To] where MaTo=" + MaTo + ") AS VARCHAR(2)), 2)"
                        + " set @DenMay=RIGHT('0' + CAST((select DenMay from [To] where MaTo=" + MaTo + ") AS VARCHAR(2)), 2)"
                        + " SET @LastNamKy = @Nam * 12  + @Ky;"
                        + " IF (OBJECT_ID('tempdb.dbo.#ChiSo', 'U') IS NOT NULL) DROP TABLE #ChiSo;"
                        + " SELECT DanhBa, MAX([ChiSo0]) AS [ChiSo0], MAX([ChiSo1]) AS [ChiSo1], MAX([ChiSo2]) AS [ChiSo2], MAX([Code0]) AS [Code0],"
                        + "     MAX([Code1]) AS [Code1], MAX([Code2]) AS [Code2], MAX([TieuThu0]) AS [TieuThu0], MAX([TieuThu1]) AS [TieuThu1],"
                        + "     MAX([TieuThu2]) AS [TieuThu2]"
                        + "     INTO #ChiSo"
                        + "     FROM ("
                        + "         SELECT DanhBa, 'ChiSo'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS ChiSoKy, 'Code'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS CodeKy,"
                        + "             'TieuThu'+CAST(@LastNamKy-Nam*12-Ky AS CHAR) AS TieuThuKy, [CSCu], [CodeCu], [TieuThuCu]"
                        + "             FROM [DocSoTH].[dbo].[DocSo]"
                        + "             WHERE @LastNamKy-Nam*12-Ky between 0 and 2 and ((PhanMay>=@TuMay and PhanMay<=@DenMay) or (May>=@TuMay and May<=@DenMay))) src"
                        + "     PIVOT (MAX([CSCu]) FOR ChiSoKy IN ([ChiSo0],[ChiSo1],[ChiSo2])) piv_cs"
                        + "     PIVOT (MAX([CodeCu]) FOR CodeKy IN ([Code0],[Code1],[Code2])) piv_code"
                        + "     PIVOT (MAX([TieuThuCu]) FOR TieuThuKy IN ([TieuThu0],[TieuThu1],[TieuThu2])) piv_tt"
                        + "     GROUP BY DanhBa;"
                        + "     with sdt as("
                        + "        select g1.DanhBo"
                        + "        , stuff(("
                        + "            select ' | ' + g.DienThoai+' '+g.HoTen"
                        + "            from CAPNUOCTANHOA.dbo.SDT_DHN g"
                        + "            where g.DanhBo = g1.DanhBo and SoChinh=1"
                        + "            order by CreateDate desc"
                        + "            for xml path('')"
                        + "        ),1,2,'') as DienThoai"
                        + "        from CAPNUOCTANHOA.dbo.SDT_DHN g1"
                        + "        group by g1.DanhBo)"
                        + " select ds.DocSoID,MLT=kh.LOTRINH,DanhBo=ds.DanhBa,HoTen=kh.HOTEN,SoNha=kh.SONHA,TenDuong=kh.TENDUONG,ds.Nam,ds.Ky,ds.Dot,ds.PhanMay"
                        + "                          ,Hieu=kh.HIEUDH,Co=kh.CODH,SoThan=kh.SOTHANDH,ViTri=VITRIDHN,ViTriNgoai=ViTriDHN_Ngoai,ViTriHop=ViTriDHN_Hop,bd.SH,bd.SX,bd.DV,HCSN=bd.HC,ds.TienNuoc,ThueGTGT=ds.Thue,PhiBVMT=ds.BVMT,PhiBVMT_Thue=ds.BVMT_Thue,TongCong=ds.TongTien"
                        + "                          ,DiaChi=(select top 1 DiaChi=case when SO is null then DUONG else case when DUONG is null then SO else SO+' '+DUONG end end from HOADON_TA.dbo.HOADON where DanhBa=ds.DanhBa order by ID_HOADON desc)"
                        + "                          ,GiaBieu=ds.GB,DinhMuc=ds.DM,DinhMucHN=ds.DMHN,CSMoi,CodeMoi,TieuThuMoi,ds.TBTT,TuNgay=CONVERT(varchar(10),TuNgay,103),DenNgay=CONVERT(varchar(10),DenNgay,103),cs.*"
                        + "                          ,kh.Gieng,kh.KhoaTu,kh.AmSau,kh.XayDung,kh.DutChi_Goc,kh.DutChi_Than,kh.NgapNuoc,kh.KetTuong,kh.LapKhoaGoc,kh.BeHBV,kh.BeNapMatNapHBV,kh.GayTayVan"
                        + "                          ,kh.TroNgaiThay,kh.DauChungMayBom,kh.MauSacChiGoc,ds.ChuBao,DienThoai=sdt.DienThoai,kh.GhiChu,kh.KinhDoanh"
                        + "                          ,NgayThuTien=(select CONVERT(varchar(10),NgayThuTien,103) from Lich_DocSo ds,Lich_DocSo_ChiTiet dsct where ds.ID=dsct.IDDocSo and ds.Nam=@Nam and ds.Ky=@Ky and dsct.IDDot=@Dot)"
                        + "                          ,TinhTrang=(select"
                        + "                             case when exists (select top 1 MaKQDN from HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
                        + "                             then (select top 1 N'Thu Tiền đóng nước: '+CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108) from HOADON_TA.dbo.TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo=ds.DanhBa order by NgayDN desc)"
                        + "                             else ''"
                        + "                             end)"
                        + "                          ,CuaHangThuHo=(select CuaHangThuHo1+CHAR(10)+case when CuaHangThuHo2 is null or CuaHangThuHo2=CuaHangThuHo1 then '' else CuaHangThuHo2 end from HOADON_TA.dbo.TT_DichVuThu_DanhBo_CuaHang where DanhBo=ds.DanhBa)"
                        + "                          from DocSo ds left join CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG kh on ds.DanhBa=kh.DANHBO"
                        + "                          left join BienDong bd on ds.DocSoID=bd.BienDongID"
                        + "                          left join #ChiSo cs on ds.DanhBa=cs.DanhBa"
                        + "                          left join sdt on sdt.DanhBo=ds.DanhBa"
                        + "                          where ds.Nam=@Nam and ds.Ky=@Ky and ds.Dot=@Dot and ds.PhanMay>=@TuMay and ds.PhanMay<=@DenMay"
                        + "                          and (ds.TieuThuMoi=0 or ds.TieuThuMoi>=TBTT*1.4 or ds.TieuThuMoi<=TBTT-TBTT*0.4) order by ds.MLT1 asc";
            return DataTableToJSON(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
        }

        public string getDS_SoLieu_SanLuong_DHN(string Nam, string Ky, string Dot)
        {
            DataTable dtTo = _cDAL_DocSo.ExecuteQuery_DataTable("select MaTo,TenTo from [To] where HanhThu=1");
            DataTable dt = new DataTable();
            foreach (DataRow item in dtTo.Rows)
            {
                string sql = "DECLARE @LastNamKy INT;"
                        + " declare @Nam int"
                        + " declare @Ky char(2)"
                        + " declare @Dot char(2)"
                        + " declare @TuMay char(2)"
                        + " declare @DenMay char(2)"
                        + " set @Nam=" + Nam
                        + " set @Ky='" + Ky + "'"
                        + " set @Dot='" + Dot + "'"
                        + " set @TuMay=RIGHT('0' + CAST((select TuMay from [To] where MaTo=" + item["MaTo"].ToString() + ") AS VARCHAR(2)), 2)"
                        + " set @DenMay=RIGHT('0' + CAST((select DenMay from [To] where MaTo=" + item["MaTo"].ToString() + ") AS VARCHAR(2)), 2)"
                        + " SET @LastNamKy = @Nam * 12  + @Ky;"
                        + " select"
                        + " 'To'=(select TenTo from [To] where MaTo=" + item["MaTo"].ToString() + ")"
                        + " ,DHNTruoc=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy-1 and Dot=@Dot and May>=@TuMay and May<=@DenMay)"
                        + " ,SanLuongTruoc=(select SUM(TieuThuMoi) from DocSo where Nam*12+Ky=@LastNamKy-1 and Dot=@Dot and May>=@TuMay and May<=@DenMay)"
                        + " ,DHN=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy and Dot=@Dot and May>=@TuMay and May<=@DenMay)"
                        + " ,SanLuong=(select SUM(TieuThuMoi) from DocSo where Nam*12+Ky=@LastNamKy and Dot=@Dot and May>=@TuMay and May<=@DenMay)";
                dt.Merge(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
            }
            return DataTableToJSON(dt);
        }

        public string getDS_SoLieu_HD0_DHN(string Nam, string Ky, string Dot)
        {
            DataTable dtTo = _cDAL_DocSo.ExecuteQuery_DataTable("select MaTo,TenTo from [To] where HanhThu=1");
            DataTable dt = new DataTable();
            foreach (DataRow item in dtTo.Rows)
            {
                string sql = "DECLARE @LastNamKy INT;"
                        + " declare @Nam int"
                        + " declare @Ky char(2)"
                        + " declare @Dot char(2)"
                        + " declare @TuMay char(2)"
                        + " declare @DenMay char(2)"
                        + " set @Nam=" + Nam
                        + " set @Ky='" + Ky + "'"
                        + " set @Dot='" + Dot + "'"
                        + " set @TuMay=RIGHT('0' + CAST((select TuMay from [To] where MaTo=" + item["MaTo"].ToString() + ") AS VARCHAR(2)), 2)"
                        + " set @DenMay=RIGHT('0' + CAST((select DenMay from [To] where MaTo=" + item["MaTo"].ToString() + ") AS VARCHAR(2)), 2)"
                        + " SET @LastNamKy = @Nam * 12  + @Ky;"
                        + " select"
                        + " 'To'=(select TenTo from [To] where MaTo=" + item["MaTo"].ToString() + ")"
                        + " ,HD0Truoc=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy-1 and Dot=@Dot and TieuThuMoi=0 and May>=@TuMay and May<=@DenMay)"
                        + " ,HD4Truoc=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy-1 and Dot=@Dot and TieuThuMoi>=1 and TieuThuMoi<=4 and May>=@TuMay and May<=@DenMay)"
                        + " ,HD0=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy and Dot=@Dot and TieuThuMoi=0 and May>=@TuMay and May<=@DenMay)"
                        + " ,HD4=(select COUNT(DocSoID) from DocSo where Nam*12+Ky=@LastNamKy and Dot=@Dot and TieuThuMoi>=1 and TieuThuMoi<=4 and May>=@TuMay and May<=@DenMay)";
                dt.Merge(_cDAL_DocSo.ExecuteQuery_DataTable(sql));
            }
            return DataTableToJSON(dt);
        }

        //đồng hồ nước
        public string getPhuongQuan(string DanhBo)
        {
            string sql = "select ' P.'+p.TENPHUONG+' Q.'+q.TENQUAN from TB_DULIEUKHACHHANG dlkh,PHUONG p,QUAN q"
                    + " where DANHBO='" + DanhBo + "' and dlkh.QUAN=q.MAQUAN and dlkh.PHUONG=p.MAPHUONG"
                    + " and p.MAQUAN=q.MAQUAN";
            object result = _cDAL_DHN.ExecuteQuery_ReturnOneValue(sql);
            if (result != null)
                return result.ToString();
            else
                return "";
        }

        //billing
        OdbcConnection connTongCT = new OdbcConnection();

        public void OpenConnectionTCT()
        {
            try
            {
                connTongCT = new OdbcConnection(@"Dsn=Oracle7;uid=TH_HANDHELD;pwd=TH_HANDHELD;server=center");
                if (connTongCT.State != ConnectionState.Open)
                {
                    connTongCT.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnectionTCT()
        {
            try
            {
                if (connTongCT.State != ConnectionState.Closed)
                {
                    connTongCT.Close();
                }
                connTongCT.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool insertBilling(string DocSoID, string checksum)
        {
            try
            {
                if (checksum != "tanho@2022")
                    return false;
                string sql = "SELECT DanhBa,CSCu,CASE WHEN LEFT(CodeMoi, 1) = 'F' OR LEFT(CodeMoi, 1) = '6' THEN TieuThuMoi ELSE CSMOI END AS CSMoi,TieuThuMoi,CASE WHEN LEFT(CodeMoi,1) = '4' THEN '4' ELSE CodeMoi END AS CodeMoi,MLT2,TTDHNMoi"
                        + ",DenNgay=CONVERT(varchar(10),DenNgay,103),Nam,Ky,Dot FROM DocSo WHERE DocSoID='" + DocSoID + "'";
                DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
                string DanhBo = dt.Rows[0]["DanhBa"].ToString().Trim();
                string CodeMoi = "";
                if (dt.Rows[0]["CodeMoi"].ToString().Trim().Contains("X") == false)
                    CodeMoi = dt.Rows[0]["CodeMoi"].ToString().Trim();
                else
                    CodeMoi = "4";
                string CSC = dt.Rows[0]["CSCu"].ToString().Trim();
                string CSM = dt.Rows[0]["CSMoi"].ToString().Trim();
                string TieuThuMoi = dt.Rows[0]["TieuThuMoi"].ToString().Trim();
                string MLT = dt.Rows[0]["MLT2"].ToString().Trim();
                string May = MLT.Substring(2, 2);
                string STT = MLT.Substring(4, 5);
                string NgayDoc = dt.Rows[0]["DenNgay"].ToString().Trim();
                int ID = int.Parse(this.getID());
                string rST_ID = this.getRST_ID(CodeMoi);

                string cmdText = "INSERT INTO ADMIN.\"TMP$MR\" (ID, BRANCH_CODE, \"YEAR\", PERIOD, BC_CODE, CUSTOMER_NO, MR_STATUS, THIS_READING, CONSUMPTION, DATE_READING, CREATED_ON, CREATED_BY, BOOK_NO, OIB, EMP_ID, RST_ID) VALUES ("
                            + ID + ",'TH'," + dt.Rows[0]["Nam"].ToString().Trim() + "," + dt.Rows[0]["Ky"].ToString().Trim() + ",'" + dt.Rows[0]["Dot"].ToString().Trim() + "','" + DanhBo + "','" + CodeMoi + "'," + CSM + "," + TieuThuMoi + ",to_date('" + NgayDoc + "','DD/MM/YYYY'),to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','DD/MM/YYYY'),'TH_HANDHELD','" + May + "','" + STT + "',3824," + rST_ID + ")";
                if (CodeMoi.Length > 0 && (CodeMoi.Substring(0, 1) == "5" || CodeMoi.Substring(0, 1) == "8" || CodeMoi.Substring(0, 1) == "M"))
                {
                    cmdText = "INSERT INTO ADMIN.\"TMP$MR\" (ID, BRANCH_CODE, \"YEAR\", PERIOD, BC_CODE, CUSTOMER_NO, MR_STATUS, LAST_READING, THIS_READING, CONSUMPTION, DATE_READING, CREATED_ON, CREATED_BY, BOOK_NO, OIB, EMP_ID,RST_ID) VALUES ("
                            + ID + ",'TH'," + dt.Rows[0]["Nam"].ToString().Trim() + "," + dt.Rows[0]["Ky"].ToString().Trim() + ",'" + dt.Rows[0]["Dot"].ToString().Trim() + "','" + DanhBo + "','" + CodeMoi + "'," + CSC + "," + CSM + "," + TieuThuMoi + ",to_date('" + NgayDoc + "','DD/MM/YYYY'),to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','DD/MM/YYYY'),'TH_HANDHELD','" + May + "','" + STT + "',3824," + rST_ID + ")";
                }
                this.OpenConnectionTCT();
                OdbcCommand odbcCommand = new OdbcCommand(cmdText, this.connTongCT);
                int result = odbcCommand.ExecuteNonQuery();
                this.CloseConnectionTCT();
                if (result > 0)
                {
                    _cDAL_DocSo.ExecuteNonQuery("update DocSo set NgayChuyenListing=getdate() where DocSoID='" + DocSoID + "'");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getID()
        {
            string result = "0";
            try
            {
                OpenConnectionTCT();
                OdbcCommand odbcCommand = new OdbcCommand("SELECT ADMIN.\"TMP$MR_SEQ\".NEXTVAL AS ID FROM SYS.DUAL", this.connTongCT);
                OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
                if (odbcDataReader.Read())
                {
                    result = odbcDataReader["ID"].ToString();
                }
                odbcDataReader.Close();
                this.CloseConnectionTCT();
            }
            catch
            {
            }
            return result;
        }

        private string getRST_ID(string code)
        {
            string result = "";
            try
            {
                OpenConnectionTCT();
                OdbcCommand odbcCommand = new OdbcCommand("SELECT ID FROM READING_STATUS WHERE STATUS_CODE='" + code + "'", this.connTongCT);
                OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
                if (odbcDataReader.Read())
                {
                    result = odbcDataReader["ID"].ToString().Trim();
                }
                odbcDataReader.Close();
                this.CloseConnectionTCT();
            }
            catch
            {
            }
            return result;
        }

        #endregion

        #region Thương Vụ

        public byte[] get_Hinh_241(string pathroot, string FolderLoai, string FolderIDCT, string FileName)
        {
            try
            {
                byte[] hinh = null;
                if (File.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName) == true)
                    hinh = File.ReadAllBytes(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName);
                if (hinh.Length == 0)
                    return null;
                else
                    return hinh;
            }
            catch
            {
                return null;
            }
        }

        public bool ghi_Hinh_241(string pathroot, string FolderLoai, string FolderIDCT, string FileName, string HinhDHN)
        {
            try
            {
                if (Directory.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT) == false)
                    Directory.CreateDirectory(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
                if (File.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName) == true)
                    File.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName);
                byte[] hinh = System.Convert.FromBase64String(HinhDHN);
                File.WriteAllBytes(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName, hinh);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ghi_Hinh_241(string pathroot, string FolderLoai, string FolderIDCT, string FileName, byte[] HinhDHN)
        {
            try
            {
                if (Directory.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT) == false)
                    Directory.CreateDirectory(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
                if (File.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName) == true)
                    File.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName);
                File.WriteAllBytes(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName, HinhDHN);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool xoa_Hinh_241(string pathroot, string FolderLoai, string FolderIDCT, string FileName)
        {
            try
            {
                if (File.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName) == true)
                    File.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT + @"\" + FileName);
                bool flag = false;
                foreach (string files in Directory.GetFiles(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT))
                {
                    flag = true;
                }
                if (flag == false)
                    Directory.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool xoa_Folder_241(string pathroot, string FolderLoai, string FolderIDCT)
        {
            try
            {
                if (Directory.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT) == true)
                {
                    foreach (string files in Directory.GetFiles(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT))
                    {
                        if (File.Exists(files) == true)
                            File.Delete(files);
                    }
                    Directory.Delete(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] get_FileinFolder_241(string pathroot, string FolderLoai, string FolderIDCT)
        {
            try
            {
                if (Directory.Exists(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT) == true)
                {
                    return Directory.GetFiles(pathroot + @"\" + FolderLoai + @"\" + FolderIDCT);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //kinh doanh
        public DataTable getDS_GiaNuoc()
        {
            return _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from GiaNuoc2");
        }

        public bool checkExists_GiaNuocGiam(int Nam, int Ky, int GiaBieu)
        {
            return (bool)_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("if exists(select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky + "%' and GiaBieu like '%" + GiaBieu + "%') select 'true' else select 'false'");
        }

        public DataTable getGiaNuocGiam(int Nam, int Ky, int GiaBieu)
        {
            return _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky.ToString("00") + "%' and GiaBieu like '%" + GiaBieu + "%'");
        }

        public bool HasValue(double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }

        //chi tiết tiền nước
        private const int _GiamTienNuoc = 10;

        public int TinhTienNuoc(bool DieuChinhGia, int GiaDieuChinh, List<int> lstGiaNuoc, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out string ChiTiet, out int TieuThu_DieuChinhGia, out int PhiBVMT, out string ChiTietPhiBVMT)
        {
            try
            {
                string _chiTiet = "", _chiTietPhiBVMT = "";
                int DinhMuc = TongDinhMuc - DinhMucHN, _SH = 0, _SX = 0, _DV = 0, _HCSN = 0;
                TieuThu_DieuChinhGia = 0;
                ///Table GiaNuoc được thiết lập theo bảng giá nước
                ///1. Đến 4m3/người/tháng
                ///2. Trên 4m3 đến 6m3/người/tháng
                ///3. Trên 6m3/người/tháng
                ///4. Đơn vị sản xuất
                ///5. Cơ quan, đoàn thể HCSN
                ///6. Đơn vị kinh doanh, dịch vụ
                ///7. Hộ nghèo, cận nghèo
                ///List bắt đầu từ phần tử thứ 0
                int TongTien = 0, TongTienPhiBVMT = 0;
                switch (GiaBieu)
                {
                    ///TƯ GIA
                    case 10:
                        DinhMucHN = TongDinhMuc;
                        if (TieuThu <= DinhMucHN)
                        {
                            TongTien = TieuThu * lstGiaNuoc[6];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN <= Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((TieuThu - DinhMucHN) * lstGiaNuoc[1]);
                                    TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)
                                                + ((TieuThu - DinhMucHN) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if ((TieuThu - DinhMucHN) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((int)Math.Round((double)DinhMuc / 2) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((int)Math.Round((double)DinhMuc / 2) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if ((int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6])
                                            + ((TieuThu - DinhMucHN) * GiaDieuChinh);
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                           + ((TieuThu - DinhMucHN) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if ((TieuThu - DinhMucHN) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                    updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[6] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN;
                            }
                        break;
                    case 11:
                    case 21:///SH thuần túy
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            if (HasValue(TyLe) == false)
                                TyLe = 0;
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                        + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (TieuThuHN > 0)
                            {
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (TieuThuDC > 0)
                            {
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6])
                                            + (DinhMuc * lstGiaNuoc[0])
                                            + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + ((TieuThu - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (DinhMuc > 0)
                                {
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                    updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[0] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                            }
                        break;
                    case 12:
                    case 22:
                    case 32:
                    case 42:///SX thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[3];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 13:
                    case 23:
                    case 33:
                    case 43:///DV thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[5];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 14:
                    case 24:///SH + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeSX == 0)
                        {
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[3]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                               + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                               + ((TieuThu - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + SX
                        {
                            //int _SH = 0, _SX = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _SH;
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                          + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _SX * lstGiaNuoc[3];
                            TongTienPhiBVMT += _SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 15:
                    case 25:///SH + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeDV == 0)
                        {
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                //double TyLe = Math.Round((double)DinhMucHN / (DinhMucHN + DinhMuc), 2);
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[5]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + DV
                        {
                            //int _SH = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH;
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                            + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                            + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                            + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _DV * lstGiaNuoc[5];
                            TongTienPhiBVMT += _DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 16:
                    case 26:///SH + SX + DV
                        ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
                        if (TyLeSX != 0 && TyLeDV != 0 && TyLeSH == 0)
                        {
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SX;
                            TongTien = (_SX * lstGiaNuoc[3])
                                        + (_DV * lstGiaNuoc[5]);
                            TongTienPhiBVMT = (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                        + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_SX > 0)
                            {
                                _chiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                                _chiTietPhiBVMT = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                        ///Nếu có đủ 3 tỉ lệ SH + SX + DV
                        {
                            //int _SH = 0, _SX = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH - _SX;
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((_SH - DinhMuc) * lstGiaNuoc[1]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += (_SX * lstGiaNuoc[3])
                                        + (_DV * lstGiaNuoc[5]);
                            TongTienPhiBVMT += (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                        + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 17:
                    case 27:///SH ĐB
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[0];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 18:
                    case 28:
                    case 38:///SH + HCSN
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeHCSN == 0)
                        {
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                           + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMuc) * lstGiaNuoc[4]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + HCSN
                        {
                            //int _SH = 0, _HCSN = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _HCSN = TieuThu - _SH;
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                if (HasValue(TyLe) == false)
                                    TyLe = 0;
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (TieuThuHN > 0)
                                {
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (TieuThuDC > 0)
                                {
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                    + (DinhMuc * lstGiaNuoc[0])
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        if (DinhMucHN > 0)
                                        {
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                            _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                        }
                                        if (DinhMuc > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                            updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        {
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                            updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                        }
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _HCSN * lstGiaNuoc[4];
                            TongTienPhiBVMT += _HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (_HCSN > 0)
                            {
                                updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                                updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 19:
                    case 29:
                    case 39:///SH + HCSN + SX + DV
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            if (HasValue(TyLe) == false)
                                TyLe = 0;
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (TieuThuDC * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (TieuThuHN > 0)
                            {
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (TieuThuDC > 0)
                            {
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[1] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6])
                                            + (DinhMuc * lstGiaNuoc[0])
                                            + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (DinhMuc * (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[6] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (DinhMuc > 0)
                                {
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[0] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));
                                    updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[0] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * lstGiaNuoc[4])
                                    + (_SX * lstGiaNuoc[3])
                                    + (_DV * lstGiaNuoc[5]);
                        TongTienPhiBVMT += (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        if (_HCSN > 0)
                        {
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                            updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_SX > 0)
                        {
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_DV > 0)
                        {
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                            updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        break;
                    ///CƠ QUAN
                    case 31:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[4];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 34:///HCSN + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeSX == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[3];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _SX = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _HCSN;

                            TongTien = (_HCSN * lstGiaNuoc[4])
                                        + (_SX * lstGiaNuoc[3]);
                            TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_HCSN > 0)
                            {
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                                _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 35:///HCSN + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeDV == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[5];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _DV = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN;
                            TongTien = (_HCSN * lstGiaNuoc[4])
                                        + (_DV * lstGiaNuoc[5]);
                            TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_HCSN > 0)
                            {
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                                _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 36:///HCSN + SX + DV
                        {
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN - _SX;
                            TongTien = (_HCSN * lstGiaNuoc[4])
                                        + (_SX * lstGiaNuoc[3])
                                        + (_DV * lstGiaNuoc[5]);
                            TongTienPhiBVMT = (_HCSN * (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_HCSN > 0)
                            {
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                                _chiTietPhiBVMT = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[4] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    ///NƯỚC NGOÀI
                    case 41:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[2];
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)GiaDieuChinh * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 44:///SH + SX
                        {
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _SH;
                            TongTien = (_SH * lstGiaNuoc[2])
                                        + (_SX * lstGiaNuoc[3]);
                            TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_SH > 0)
                            {
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                                _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    case 45:///SH + DV
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;
                        TongTien = (_SH * lstGiaNuoc[2])
                                    + (_DV * lstGiaNuoc[5]);
                        TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        if (_SH > 0)
                        {
                            _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                            _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        }
                        if (_DV > 0)
                        {
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                            updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        break;
                    case 46:///SH + SX + DV
                        {
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH - _SX;
                            TongTien = (_SH * lstGiaNuoc[2])
                                        + (_SX * lstGiaNuoc[3])
                                        + (_DV * lstGiaNuoc[5]);
                            TongTienPhiBVMT = (_SH * (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_SX * (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (_DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (_SH > 0)
                            {
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                                _chiTietPhiBVMT = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[2] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (_SX > 0)
                            {
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[3] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                            if (_DV > 0)
                            {
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        break;
                    ///BÁN SỈ
                    case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            if (HasValue(TyLe) == false)
                                TyLe = 0;
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (TieuThuHN > 0)
                            {
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (TieuThuDC > 0)
                            {
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                          + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                          + ((TieuThu - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((TieuThu - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                  + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                  + ((TieuThu - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    _chiTietPhiBVMT = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (DinhMuc > 0)
                                {
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                            }
                        break;
                    case 52:///sỉ khu công nghiệp
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 53:///sỉ KD - TM
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 54:///sỉ HCSN
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            TongTienPhiBVMT = TieuThu * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                            if (TieuThu > 0)
                            {
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 58:
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;
                        TongTien = (_SH * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                    + (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
                                    + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
                                    + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        TongTienPhiBVMT = (_SH * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_HCSN * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_SX * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                            + (_DV * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        if (_SH > 0)
                        {
                            _chiTiet += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            _chiTietPhiBVMT += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        }
                        if (_HCSN > 0)
                        {
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_SX > 0)
                        {
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_DV > 0)
                        {
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        break;
                    case 59:///sỉ phức tạp
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            if (HasValue(TyLe) == false)
                                TyLe = 0;
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (TieuThuHN > 0)
                            {
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (TieuThuDC > 0)
                            {
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                       + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (DinhMuc > 0)
                                {
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
                                        + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
                                        + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        TongTienPhiBVMT += (_HCSN * (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                        + (_SX * (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                        + (_DV * (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                        if (_HCSN > 0)
                        {
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_SX > 0)
                        {
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        if (_DV > 0)
                        {
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                            updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        break;
                    case 68:///SH giá sỉ - KD giá lẻ
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            if (HasValue(TyLe) == false)
                                TyLe = 0;
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            TongTienPhiBVMT = (TieuThuHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (TieuThuDC * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            if (TieuThuHN > 0)
                            {
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                _chiTietPhiBVMT = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                            }
                            if (TieuThuDC > 0)
                            {
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                updateChiTiet(ref _chiTietPhiBVMT, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                            }
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                        + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    if (DinhMucHN > 0)
                                    {
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                        _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                    }
                                    if (DinhMuc > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                    {
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                        updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                    }
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                TongTienPhiBVMT = (DinhMucHN * (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + (DinhMuc * (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero))
                                                    + ((_SH - DinhMucHN - DinhMuc) * (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                if (DinhMucHN > 0)
                                {
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    _chiTietPhiBVMT = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero));
                                }
                                if (DinhMuc > 0)
                                {
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                {
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));
                                    updateChiTiet(ref _chiTietPhiBVMT, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)(GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100) * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                                }
                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMuc;
                            }
                        TongTien += _DV * lstGiaNuoc[5];
                        TongTienPhiBVMT += _DV * (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero);
                        if (_DV > 0)
                        {
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                            updateChiTiet(ref _chiTietPhiBVMT, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int)Math.Round((double)lstGiaNuoc[5] * lstGiaNuoc[7] / 100, 0, MidpointRounding.AwayFromZero)));
                        }
                        break;
                    default:
                        _chiTiet = "";
                        TongTien = 0;
                        break;
                }
                ChiTiet = _chiTiet;
                PhiBVMT = TongTienPhiBVMT;
                ChiTietPhiBVMT = _chiTietPhiBVMT;
                return TongTien;
            }
            catch (Exception ex)
            {
                ChiTiet = "";
                TieuThu_DieuChinhGia = 0;
                throw ex;
            }
        }

        public void TinhTienNuoc(bool KhongApGiaGiam, bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, ref int TienNuocNamCu, ref string ChiTietNamCu, ref int TienNuocNamMoi, ref string ChiTietNamMoi, ref int TieuThu_DieuChinhGia, ref int PhiBVMTNamCu, ref string ChiTietPhiBVMTNamCu, ref int PhiBVMTNamMoi, ref string ChiTietPhiBVMTNamMoi, ref int TienNuoc, ref int ThueGTGT, ref int TDVTN, ref int ThueTDVTN)
        {
            DataTable dtGiaNuoc = getDS_GiaNuoc();
            //check giảm giá
            if (KhongApGiaGiam == false)
                checkExists_GiamGiaNuoc(Nam, Ky, GiaBieu, ref dtGiaNuoc);

            int index = -1;
            TienNuocNamCu = TienNuocNamMoi = PhiBVMTNamCu = PhiBVMTNamMoi = TienNuoc = ThueGTGT = TDVTN = ThueTDVTN = 0;
            ChiTietNamCu = ChiTietNamMoi = ChiTietPhiBVMTNamCu = ChiTietPhiBVMTNamMoi = "";
            TieuThu_DieuChinhGia = 0;
            for (int i = 0; i < dtGiaNuoc.Rows.Count; i++)
                if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date < DenNgay.Date)
                {
                    index = i;
                }
                else
                    if (TuNgay.Date >= DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date)
                    {
                        index = i;
                    }
            if (index != -1)
            {
                if (DenNgay.Date < new DateTime(2019, 11, 15))
                {
                    List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
                    TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, 0, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
                }
                else
                    if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date < DenNgay.Date)
                    {
                        if (ApGiaNuocCu == false)
                        {
                            //int TieuThu_DieuChinhGia;
                            int TongSoNgay = (int)((DenNgay.Date - TuNgay.Date).TotalDays);

                            int SoNgayCu = (int)((DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date - TuNgay.Date).TotalDays);
                            int TieuThuCu = (int)Math.Round((double)TieuThu * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                            int TieuThuMoi = TieuThu - TieuThuCu;
                            int TongDinhMucCu = (int)Math.Round((double)TongDinhMuc * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                            int TongDinhMucMoi = TongDinhMuc - TongDinhMucCu;
                            int DinhMucHN_Cu = 0, DinhMucHN_Moi = 0;
                            if (TuNgay.Date > new DateTime(2019, 11, 15))
                                if (TongDinhMucCu != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
                                    DinhMucHN_Cu = (int)Math.Round((double)TongDinhMucCu * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
                            if (TongDinhMucMoi != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
                                DinhMucHN_Moi = (int)Math.Round((double)TongDinhMucMoi * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
                            List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["PhiBVMT"].ToString()) };
                            List<int> lstGiaNuocMoi = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
                            //lần đầu áp dụng giá biểu 10, tổng áp giá mới luôn
                            if (TuNgay.Date < new DateTime(2019, 11, 15) && new DateTime(2019, 11, 15) < DenNgay.Date && GiaBieu == 10)
                                TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
                            else
                                TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
                            TienNuocNamMoi = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucMoi, DinhMucHN_Moi, TieuThuMoi, out ChiTietNamMoi, out TieuThu_DieuChinhGia, out PhiBVMTNamMoi, out ChiTietPhiBVMTNamMoi);
                        }
                        else
                        {
                            List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["PhiBVMT"].ToString()) };
                            TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
                        }
                    }
                    else
                    {
                        List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["PhiBVMT"].ToString()) };
                        TienNuocNamCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietNamCu, out TieuThu_DieuChinhGia, out PhiBVMTNamCu, out ChiTietPhiBVMTNamCu);
                    }
                if (checkKhongTinhPBVMT(DanhBo) == true)
                {
                    PhiBVMTNamCu = PhiBVMTNamMoi = 0;
                    ChiTietPhiBVMTNamCu = ChiTietPhiBVMTNamMoi = "";
                }
                TienNuoc = TienNuocNamCu + TienNuocNamMoi;
                ThueGTGT = (int)Math.Round((double)(TienNuocNamCu + TienNuocNamMoi) * 5 / 100, 0, MidpointRounding.AwayFromZero);
                TDVTN = PhiBVMTNamCu + PhiBVMTNamMoi;
                //Từ 2022 Phí BVMT -> Tiền Dịch Vụ Thoát Nước
                int ThueTDVTN_VAT = 0;
                if (dtGiaNuoc.Rows[index]["VAT2_Ky"].ToString().Contains(Ky.ToString("00") + "/" + Nam))
                    ThueTDVTN_VAT = int.Parse(dtGiaNuoc.Rows[index]["VAT2"].ToString());
                else
                    ThueTDVTN_VAT = int.Parse(dtGiaNuoc.Rows[index]["VAT"].ToString());
                if ((TuNgay.Year < 2021) || (TuNgay.Year == 2021 && DenNgay.Year == 2021))
                {
                    ThueTDVTN = 0;
                }
                else
                    if (TuNgay.Year == 2021 && DenNgay.Year == 2022)
                    {
                        ThueTDVTN = (int)Math.Round((double)(PhiBVMTNamMoi) * ThueTDVTN_VAT / 100, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                        if (TuNgay.Year >= 2022)
                        {
                            ThueTDVTN = (int)Math.Round((double)(PhiBVMTNamCu + PhiBVMTNamMoi) * ThueTDVTN_VAT / 100, 0, MidpointRounding.AwayFromZero);
                        }
            }
        }

        //giảm giá nước
        public bool checkExists_GiamGiaNuoc(int Nam, int Ky, int GiaBieu, ref DataTable dt)
        {
            DataTable dtGiaNuocGiam = getGiaNuocGiam(Nam, Ky, GiaBieu);

            if (dtGiaNuocGiam != null && dtGiaNuocGiam.Rows.Count > 0)
            {
                double TyLeGiam = double.Parse(dtGiaNuocGiam.Rows[0]["TyLeGiam"].ToString());
                foreach (DataRow item in dt.Rows)
                {
                    item["SHN"] = int.Parse(item["SHN"].ToString()) - (int)(int.Parse(item["SHN"].ToString()) * TyLeGiam / 100);
                    item["SHTM"] = int.Parse(item["SHTM"].ToString()) - (int)(int.Parse(item["SHTM"].ToString()) * TyLeGiam / 100);
                    item["SHVM1"] = int.Parse(item["SHVM1"].ToString()) - (int)(int.Parse(item["SHVM1"].ToString()) * TyLeGiam / 100);
                    item["SHVM2"] = int.Parse(item["SHVM2"].ToString()) - (int)(int.Parse(item["SHVM2"].ToString()) * TyLeGiam / 100);
                    item["SX"] = int.Parse(item["SX"].ToString()) - (int)(int.Parse(item["SX"].ToString()) * TyLeGiam / 100);
                    item["HCSN"] = int.Parse(item["HCSN"].ToString()) - (int)(int.Parse(item["HCSN"].ToString()) * TyLeGiam / 100);
                    item["KDDV"] = int.Parse(item["KDDV"].ToString()) - (int)(int.Parse(item["KDDV"].ToString()) * TyLeGiam / 100);
                }
                return true;
            }
            else
                return false;
        }

        public void updateChiTiet(ref string main_value, string update_value)
        {
            if (main_value == "")
                main_value = update_value;
            else
                main_value += "\r\n" + update_value;
        }

        public string getDanhBo_CatTam(string ID)
        {
            object result = _cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatTam where MaCTCTDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public string getDanhBo_CatHuy(string ID)
        {
            object result = _cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatHuy where MaCTCHDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }

        //đơn từ
        public bool checkExists_DonTu(string DanhBo, string NoiDung, string SoNgay)
        {
            DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from DonTu where Name_NhomDon_PKH like N'%" + NoiDung + "%' and DATEADD(DAY, " + SoNgay + ", CreateDate)>=GETDATE()");

            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        //cccd tổng công ty
        public string getAccess_token_CCCD()
        {
            string strResponse = "";
            try
            {
                string url = "https://cskhapi.sawaco.com.vn/api/Login/LoginCSKH";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";

                var data = new
                {
                    grant_type = "password",
                    userName = "cskh.cnth",
                    Password = "123456@ABcd"
                };
                //Password = "cskh2022@tanhoa"
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
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = jss.Deserialize<dynamic>(result);
                    if (obj["ketQua"] == 1)
                    {
                        bool flag = _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["duLieu"]["token"] + "',expires_in=" + obj["duLieu"]["limitDayExpiresLoginAppStore"] + ",CreateDate=getdate() where ID='cskhapi'");
                        strResponse = flag.ToString();
                    }
                    else
                        strResponse = obj["thongBao"];
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

        private string getAccess_token()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='cskhapi'").ToString();
        }

        public int checkExists_CCCD(string DanhBo, string CCCD, out string result)
        {
            result = "";
            try
            {
                string url = "https://cskhapi.sawaco.com.vn/api/KhachHangDinhDanh/TraCuuKhachHangDinhDanh?danhBo=" + DanhBo + "&sdd=" + CCCD;
                //string url = "https://cskhapi.sawaco.com.vn/api/KhachHangDinhDanh/TraCuuKhachHangDinhDanh?danhBo=12101905044&sdd=040074001959";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = "Bearer " + getAccess_token();

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result1 = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = jss.Deserialize<dynamic>(result1);
                    object[] dulieu = obj["duLieu"];
                    if (dulieu != null && dulieu.Count() > 0)
                    {
                        result = "Đã tồn tại";
                        return 1;
                    }
                    else
                    {
                        result = "Không tồn tại";
                        return 0;
                    }
                }
                else
                {
                    result = "Lỗi kết nối";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return -1;
            }
        }

        public int them_CCCD(string DanhBo, string CCCD, out string result)
        {
            result = "";
            try
            {
                string url = "https://cskhapi.sawaco.com.vn/api/KhachHangDinhDanh/ThemDinhDanh";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token();
                DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select LoaiCapDM=case when TamTru=1 then '2' else '1' end,NgayHetHan=convert(char(4), NgayHetHan, 12),HoTen"
                + " from ChungTu a,ChungTu_ChiTiet b where a.MaCT=b.MaCT and a.MaLCT=b.MaLCT and DanhBo='" + DanhBo + "' and b.MaCT='" + CCCD + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = new
                    {
                        branch_code = "TH",
                        branch_name = "",
                        sodinhdanh = CCCD,
                        tenkh_codau = dt.Rows[0]["HoTen"],
                        tenkh_khongdau = "",
                        cmndcu = "",
                        danhbo = DanhBo,
                        sohokhau_stt = "",
                        hongheo = "0",
                        loaicapdm = dt.Rows[0]["LoaiCapDM"],
                        thoihantt = dt.Rows[0]["NgayHetHan"],
                        danhbo_tt = "",
                        diachikhachhang = "",
                        diachiemail = "",
                        sodienthoai = "",
                        ghichu = "",
                        dinhmuc = "",
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    json = "[" + json + "]";
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result1 = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = jss.Deserialize<dynamic>(result1);
                    if (obj["ketQua"] != null && obj["ketQua"] == 1)
                    {
                        result = "Thành Công";
                        return 1;
                    }
                    else
                    {
                        try
                        {
                            if (obj["duLieuKhongHopLe"][0]["ghichu"].ToString() != "")
                                result = obj["duLieuKhongHopLe"][0]["ghichu"];
                        }
                        catch
                        {

                        }
                        try
                        {
                            if (obj["duLieuTrung"][0]["id"].ToString() != "")
                                result = "Dữ liệu trùng: " + obj["duLieuTrung"][0]["branch_name"].ToString() + " " + obj["duLieuTrung"][0]["createdDate"].ToString();
                        }
                        catch
                        {

                        }
                        return 0;
                    }
                }
                else
                {
                    result = "Lỗi kết nối";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return -1;
            }
        }

        public int sua_CCCD(string DanhBo, string CCCD, out string result)
        {
            result = "";
            try
            {
                string url = "https://cskhapi.sawaco.com.vn/api/KhachHangDinhDanh/CapNhatDinhDanh";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token();
                DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select LoaiCapDM=case when TamTru=1 then '2' else '1' end,NgayHetHan=convert(char(4), NgayHetHan, 12),HoTen"
                + " from ChungTu a,ChungTu_ChiTiet b where a.MaCT=b.MaCT and a.MaLCT=b.MaLCT and DanhBo='" + DanhBo + "' and b.MaCT='" + CCCD + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = new
                    {
                        branch_code = "TH",
                        branch_name = "",
                        sodinhdanh = CCCD,
                        tenkh_codau = dt.Rows[0]["HoTen"],
                        tenkh_khongdau = "",
                        cmndcu = "",
                        danhbo = DanhBo,
                        sohokhau_stt = "",
                        hongheo = "0",
                        loaicapdm = dt.Rows[0]["LoaiCapDM"],
                        thoihantt = dt.Rows[0]["NgayHetHan"],
                        danhbo_tt = "",
                        diachikhachhang = "",
                        diachiemail = "",
                        sodienthoai = "",
                        ghichu = "",
                        dinhmuc = "",
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    json = "[" + json + "]";
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result1 = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = jss.Deserialize<dynamic>(result1);
                    if (obj["ketQua"] != null && obj["ketQua"] == 1)
                    {
                        result = "Thành Công";
                        return 1;
                    }
                    else
                    {
                        result = obj["duLieuKhongHopLe"][0]["ghichu"];
                        return 0;
                    }
                }
                else
                {
                    result = "Lỗi kết nối";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return -1;
            }
        }

        public int xoa_CCCD(string DanhBo, string CCCD, out string result)
        {
            result = "";
            try
            {
                string url = "https://cskhapi.sawaco.com.vn/api/KhachHangDinhDanh/XoaDinhDanh";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token();
                DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select HoTen"
                + " from ChungTu a,ChungTu_ChiTiet b where a.MaCT=b.MaCT and a.MaLCT=b.MaLCT and DanhBo='" + DanhBo + "' and b.MaCT='" + CCCD + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = new
                    {
                        branch_code = "TH",
                        branch_name = "",
                        sodinhdanh = CCCD,
                        tenkh_codau = dt.Rows[0]["HoTen"],
                        tenkh_khongdau = "",
                        cmndcu = "",
                        danhbo = DanhBo,
                        sohokhau_stt = "",
                        hongheo = "0",
                        loaicapdm = "1",
                        thoihantt = "",
                        danhbo_tt = "",
                        diachikhachhang = "",
                        diachiemail = "",
                        sodienthoai = "",
                        ghichu = "",
                        dinhmuc = "",
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    json = "[" + json + "]";
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    request.ContentLength = byteArray.Length;
                    //gắn data post
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result1 = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = jss.Deserialize<dynamic>(result1);
                    if (obj["ketQua"] != null && obj["ketQua"] == 1)
                    {
                        result = "Thành Công";
                        return 1;
                    }
                    else
                    {
                        result = "Thất Bại";
                        return 0;
                    }
                }
                else
                {
                    result = "Lỗi kết nối";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return -1;
            }
        }

        #endregion
    }
}
