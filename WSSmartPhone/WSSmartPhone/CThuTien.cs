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

namespace WSSmartPhone
{
    class CThuTien
    {
        CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        CKinhDoanh _cKinhDoanh = new CKinhDoanh();
        CDHN _cDHN = new CDHN();
        CDocSo _cDocSo = new CDocSo();
        dbThuTienDataContext _dbThuTien = new dbThuTienDataContext();

        public string DataTableToJSON(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = Int32.MaxValue;
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

        private bool checkActiveMobile(string MaNV)
        {
            return (bool)_cDAL.ExecuteQuery_ReturnOneValue("select ActiveMobile from TT_NguoiDung where MaND=" + MaNV);
        }

        private bool checkChotDangNgan(string NgayGiaiTrach)
        {
            if ((int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(*) from TT_ChotDangNgan where CAST(NgayChot as date)='" + NgayGiaiTrach + "' and Chot=1") > 0)
                return true;
            else
                return false;
        }

        public string DangNhaps(string Username, string Password, string IDMobile, string UID)
        {
            try
            {
                object MaNV = null;
                MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

                if (MaNV == null || MaNV.ToString() == "")
                    return "false;Sai mật khẩu hoặc IDMobile";

                //xóa máy đăng nhập MaNV khác
                object MaNV_UID_Old = _cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
                if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
                    _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

                //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                //{
                //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                //    }
                //}

                object MaNV_UID = _cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                if (MaNV_UID != null)
                    if ((int)MaNV_UID == 0)
                        _cDAL.ExecuteNonQuery("insert TT_DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
                    else
                        _cDAL.ExecuteNonQuery("update TT_DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

                //_cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);

                return "true;" + DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Admin,HanhThu,DongNuoc,Doi,ToTruong,MaTo,DienThoai,Zalo,InPhieuBao,TestApp,SyncNopTien from TT_NguoiDung where MaND=" + MaNV));
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
                MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
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

                return "true;" + DataTableToJSON(_cDAL.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Admin,HanhThu,DongNuoc,Doi,ToTruong,MaTo,DienThoai,Zalo,InPhieuBao,TestApp,SyncNopTien from TT_NguoiDung where MaND=" + MaNV));
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

                return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
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
                object MaNV = _cDAL.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and An=0").ToString();

                if (MaNV != null)
                    _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where MaNV=" + MaNV + " and UID='" + UID + "'");

                return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'").ToString() + ";";
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
            return _cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);
        }

        public bool updateLogin(string MaNV, string UID)
        {
            return _cDAL.ExecuteNonQuery("update TT_DeviceSigned set ModifyDate=getdate() where UID='" + UID + "' and MaNV=" + MaNV);
        }

        public string getDS_To()
        {
            string sql = "select MaTo,TenTo,HanhThu,DongNuoc from TT_To where An=0";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien_HanhThu()
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and HanhThu=1 and DongNuoc=0 and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien()
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and An=0 order by STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string getDS_NhanVien(string MaTo)
        {
            string sql = "select MaND,HoTen,HanhThu,DongNuoc,MaTo,DienThoai,Zalo from TT_NguoiDung where MaND!=0 and MaTo=" + MaTo + " and An=0 order by STT asc";
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
                responseMess = "Error: " + ex.Message;
            }
            return responseMess;
        }

        //hành thu
        public string getDSHoaDonTon_NhanVien(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end,CoDH"
                            + " ,GiaBieu=GB,DinhMuc=DM,DinhMucHN,CSC=CSCU,CSM=CSMOI,Code,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong"
                            + " ,GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end"
                //+ " ,DangNgan_DienThoai=case when MaNV_DangNgan=" + MaNV + " then DangNgan_DienThoai else 'false' end"
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
                //+ " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null) or (Nam=2020 and DANHBA in (select DanhBo from TT_ThoatNgheo)))"
                            + " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null))"
                            + " and hd.SOHOADON not in (select SoHoaDon from TT_HoaDon_KhongThu)"
                            + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                            + " order by MLT asc,ID_HOADON desc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string getDSHoaDonTon_May(string MaNV, string Nam, string Ky, string FromDot, string ToDot, string TuMay, string DenMay)
        {
            string sql = "select ID=ID_HOADON,MaHD=ID_HOADON,MLT=MALOTRINH,hd.SoHoaDon,Ky=CAST(hd.KY as varchar)+'/'+CAST(hd.NAM as varchar),DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=case when hd.SO is null then hd.DUONG else case when hd.DUONG is null then hd.SO else hd.SO+' '+hd.DUONG end end,CoDH"
                            + " ,GiaBieu=GB,DinhMuc=DM,DinhMucHN,CSC=CSCU,CSM=CSMOI,Code,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong"
                            + " ,GiaiTrach=case when hd.NgayGiaiTrach is not null then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                            + " ,ModifyDate=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then (select CreateDate from TT_DichVuThu where MaHD=hd.ID_HOADON) else NULL end"
                //+ " ,DangNgan_DienThoai=case when MaNV_DangNgan=" + MaNV + " then DangNgan_DienThoai else 'false' end"
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
                //+ " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null) or (Nam=2020 and DANHBA in (select DanhBo from TT_ThoatNgheo)))"
                            + " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null))"
                            + " and hd.SOHOADON not in (select SoHoaDon from TT_HoaDon_KhongThu)"
                            + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                            + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
                            + " order by MLT asc,ID_HOADON desc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
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
                    DataTable dt = _cDAL.ExecuteQuery_DataTable(sqlCheck);
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
                        return "false;Tính năng đã bị Khóa";
                        if (checkActiveMobile(MaNV) == false)
                            return "false;Chưa Active Mobile";
                        if (checkChotDangNgan(Ngay.ToString("yyyyMMdd")) == true)
                        {
                            Ngay = Ngay.AddDays(1);
                            TimeSpan ts = new TimeSpan(1, 0, 0);
                            Ngay = Ngay.Date + ts;
                            //return "false;Đã Chốt Ngày Giải Trách";
                        }
                        if (bool.Parse(_cDAL.ExecuteQuery_ReturnOneValue("if exists(select ID_HOADON from HOADON where ID_HOADON=" + MaHDs + " and (NAM<2020 or (NAM=2020 and KY<=6)))"
                                                                    + "	select 'true'"
                                                                    + "else"
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
                        return "false;Tính năng đã bị Khóa";
                        //if (checkActiveMobile(MaNV) == false)
                        //    return "false,Chưa Active Mobile";
                        //if (checkChotDangNgan(Ngay) == true)
                        //    return "false,Đã Chốt Ngày Giải Trách";
                        sql += " update TT_KQDongNuoc set DongPhi=1,MaNV_DongPhi=" + MaNV + ",NgayDongPhi='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where MaKQDN=" + MaKQDN + " and NgayDongPhi is null ";
                        break;
                    case "PhieuBao":
                        sql += " update HOADON set InPhieuBao_MaNV=" + MaNV + ",InPhieuBao_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null and InPhieuBao_Ngay is null ";
                        break;
                    case "PhieuBao2":
                        sql += " update HOADON set InPhieuBao2_MaNV=" + MaNV + ",InPhieuBao2_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao2_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',InPhieuBao2_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null and InPhieuBao2_Ngay is null ";
                        break;
                    case "TBDongNuoc":
                        //insert table TBDongNuoc
                        if ((int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(*) from TT_DongNuoc a,TT_CTDongNuoc b where a.Huy=0 and b.MaHD in (" + MaHDs + ") and a.MaDN=b.MaDN") > 0)
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
                            dongnuoc.DiaChi = lstHDTemp[0].SO + " " + lstHDTemp[0].DUONG + _cDHN.getPhuongQuan(lstHDTemp[0].DANHBA);
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

                        sql += " update HOADON set TBDongNuoc_MaNV=" + MaNV + ",TBDongNuoc_Ngay='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',TBDongNuoc_NgayHen='" + NgayHen.ToString("yyyyMMdd HH:mm:ss") + "',TBDongNuoc_Location='" + Location + "',ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is null ";
                        break;
                    case "XoaDangNgan":
                        return "false;Tính năng đã bị Khóa";
                        if (checkQuyenXoa(MaNV) == false)
                            return "false;Tính năng này tạm thời ẩn";
                        if (checkActiveMobile(MaNV) == false)
                            return "false;Chưa Active Mobile";
                        if (checkChotDangNgan(_cDAL.ExecuteQuery_ReturnOneValue("select convert(varchar, NGAYGIAITRACH, 112) from HOADON where ID_HOADON=" + MaHDs).ToString()) == true)
                            return "false;Đã Chốt Ngày Giải Trách";
                        sql += " update HOADON set XoaDangNgan_MaNV_DienThoai=" + MaNV + ",XoaDangNgan_Ngay_DienThoai='" + Ngay.ToString("yyyyMMdd HH:mm:ss") + "',XoaDangNgan_Location_DienThoai='" + Location + "',DangNgan_DienThoai=0,DangNgan_Ton=0,MaNV_DangNgan=NULL,NGAYGIAITRACH=NULL,ModifyBy=" + MaNV + ",ModifyDate=getDate() where ID_HOADON in (" + MaHDs + ") and NGAYGIAITRACH is not null ";
                        break;
                    case "XoaDongPhi":
                        return "false;Tính năng đã bị Khóa";
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
                    if (_cDAL.ExecuteNonQuery(sql) == true)
                    {
                        _cDAL.ExecuteNonQuery("insert into TT_Location_NhanVien(MaNV,Location,Action)values(" + MaNV + ",'" + Location + "','" + LoaiXuLy + "')");
                        if (XoaDCHD == true)
                        {
                            if (_cDAL.ExecuteNonQuery("insert into TT_DieuChinhTienDuXoa(MaHD,CreateBy,CreateDate)values(" + MaHDs + "," + MaNV + ",getdate())") == true)
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
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
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
                if (_cDAL.ExecuteNonQuery(sql) == true)
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
            return bool.Parse(_cDAL.ExecuteQuery_ReturnOneValue(sql).ToString());
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

        public string GetDSDongNuoc(string MaNV_DongNuoc)
        {
            string query = "select ID=dn.MaDN,dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,dn.CreateDate"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=dn.DanhBo)"
                            + " ,Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end"
                            + " ,DongNuoc=case when kqdn.DongNuoc is null then 'false' else case when kqdn.DongNuoc=1 then 'true' else 'false' end end"
                            + " ,DongNuoc2=case when kqdn.DongNuoc2 is null then 'false' else case when kqdn.DongNuoc2=1 then 'true' else 'false' end end"
                            + " ,MoNuoc=case when kqdn.MoNuoc is null then 'false' else case when kqdn.MoNuoc=1 then 'true' else 'false' end end"
                            + " ,DongPhi=case when kqdn.DongPhi is null then 'false' else case when kqdn.DongPhi=1 then 'true' else 'false' end end"
                            + " ,ButChi=case when kqdn.ButChi is null then 'false' else case when kqdn.ButChi=1 then 'true' else 'false' end end"
                            + " ,KhoaTu=case when kqdn.KhoaTu is null then 'false' else case when kqdn.KhoaTu=1 then 'true' else 'false' end end"
                            + " ,KhoaKhac=case when kqdn.KhoaKhac is null then 'false' else case when kqdn.KhoaKhac=1 then 'true' else 'false' end end"
                            + " ,kqdn.NgayDN,kqdn.ChiSoDN,kqdn.NiemChi,kqdn.KhoaKhac_GhiChu,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.ViTri,kqdn.LyDo,kqdn.NgayDN1,kqdn.ChiSoDN1,kqdn.NiemChi1,kqdn.NgayMN,kqdn.ChiSoMN,kqdn.MaKQDN"
                            + " ,CuaHangThuHo1=(select top 1 CuaHangThuHo1 from HOADON where DANHBA=dn.DanhBo order by ID_HOADON desc)"
                            + " ,CuaHangThuHo2=(select top 1 CuaHangThuHo2 from HOADON where DANHBA=dn.DanhBo order by ID_HOADON desc)"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc
                            + " and exists(select * from HOADON a,TT_CTDongNuoc b where a.ID_HOADON=b.MaHD and b.MaDN=dn.MaDN)"
                            + " and (select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and ChuyenNoKhoDoi=0 and (NGAYGIAITRACH is null or CAST(NGAYGIAITRACH as DATE)=CAST(getdate() as DATE)))>0"
                            + " and (kqdn.MaDN is null or ((kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and TroNgaiMN=0) or (CAST(kqdn.NgayMN as date)=CAST(GETDATE() as date))))"
                            + " order by dn.MLT";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public string GetDSDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select ID=dn.MaDN,dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,dn.CreateDate"
                            + " DiaChiDHN=(select [SONHA]+' '+[TENDUONG] FROM [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] where DanhBo=dn.DanhBo),"
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
                            + " kqdn.NgayDN,kqdn.ChiSoDN,kqdn.NiemChi,kqdn.KhoaKhac_GhiChu,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.ViTri,kqdn.LyDo,kqdn.NgayDN1,kqdn.ChiSoDN1,kqdn.NiemChi1,kqdn.NgayMN,kqdn.ChiSoMN,kqdn.MaKQDN"
                            + " ,DiaChiDHN=(select DiaChi from TT_DiaChiDHN where DanhBo=dn.DanhBo)"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyyMMdd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyyMMdd") + "'"
                            + " and (kqdn.DongNuoc is null and (select COUNT(MaHD) from TT_CTDongNuoc where MaDN=dn.MaDN)=(select COUNT(MaHD) from TT_CTDongNuoc ctdn,HOADON hd where MaDN=dn.MaDN and ctdn.MaHD=hd.ID_HOADON and NGAYGIAITRACH is null)"
                            + " or kqdn.MoNuoc=0 or CAST(kqdn.NgayMN as DATE)=CAST(getdate() as DATE))"
                            + " order by dn.MLT";
            DataTable dt = _cDAL.ExecuteQuery_DataTable(query);

            return DataTableToJSON(dt);
        }

        public string GetDSCTDongNuoc(string MaNV_DongNuoc)
        {
            string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,MLT=MALOTRINH,ctdn.Ky,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG"
                            + " ,GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,hd.Code,hd.TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),hd.GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,hd.TongCong,hd.DCHD,hd.TienDuTruoc_DCHD,hd.ChiTietTienNuoc"
                            + " ,DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen"
                            + " ,GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end"
                            + " ,TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end"
                            + " ,ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end"
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
            DataTable dt = _cDAL.ExecuteQuery_DataTable(sql);

            return DataTableToJSON(dt);
        }

        public string GetDSCTDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string sql = "select ID=dn.MaDN,dn.MaDN,MaHD,MLT=MALOTRINH,ctdn.Ky,DanhBo=hd.DANHBA,HoTen=hd.TENKH,DiaChi=hd.SO+' '+hd.DUONG,"
                            + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,hd.Code,hd.TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),hd.GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,hd.TongCong,hd.DCHD,hd.TienDuTruoc_DCHD,hd.ChiTietTienNuoc"
                            + " DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen,"
                            + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=ctdn.MaHD) then 'true' else 'false' end,"
                            + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=ctdn.MaHD) then 'true' else 'false' end,"
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
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
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
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
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
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
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
            string sql = "select MaHD=ID_HOADON,Ky=CAST(KY as varchar)+'/'+CAST(NAM as varchar),MLT=MALOTRINH,DanhBo=DANHBA,HoTen=TENKH,DiaChi=SO+' '+DUONG,"
                 + " GiaBieu=GB,DinhMuc=DM,CSC=CSCU,CSM=CSMOI,TieuThu,TuNgay=CONVERT(varchar(10),TUNGAY,103),DenNgay=CONVERT(varchar(10),DenNgay,103),GiaBan,ThueGTGT=Thue,PhiBVMT=Phi,TongCong,hd.DCHD,hd.TienDuTruoc_DCHD,"
                 + " DangNgan_DienThoai,NgayGiaiTrach,XoaDangNgan_Ngay_DienThoai,InPhieuBao_Ngay,InPhieuBao2_Ngay,InPhieuBao2_NgayHen,TBDongNuoc_Ngay,TBDongNuoc_NgayHen,"
                 + " GiaiTrach=case when exists(select ID_HOADON from HOADON where NGAYGIAITRACH is not null and ID_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                 + " TamThu=case when exists(select ID_TAMTHU from TAMTHU where FK_HOADON=hd.ID_HOADON) then 'true' else 'false' end,"
                 + " ThuHo=case when exists(select MaHD from TT_DichVuThu where MaHD=hd.ID_HOADON) then 'true' else 'false' end,"
                 + " PhiMoNuoc=(select dbo.fnGetPhiMoNuoc(hd.DANHBA)),"
                 + " PhiMoNuocThuHo=(select PhiMoNuoc from TT_DichVuThuTong where MaHDs like '%'+CONVERT(varchar(8),hd.ID_HOADON)+'%'),"
                 + " LenhHuy=case when exists(select MaHD from TT_LenhHuy where MaHD=hd.ID_HOADON) then 'true' else 'false' end"
                 + " from HOADON hd where DANHBA='" + DanhBo + "' and NGAYGIAITRACH is null and hd.ID_HOADON not in (" + MaHDs + ")"
                //+ " and ((GB!=10 and DinhMucHN is null) or (Nam=2020 and KY in (4,5,6) and DANHBA in (select DanhBo from TT_ThoatNgheo)))"
                //+ " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null) or (Nam=2020 and DANHBA in (select DanhBo from TT_ThoatNgheo)))"
                 + " and ((NAM>2020 or (NAM=2020 and Ky>=7)) or (GB!=10 and DinhMucHN is null))"
                 + " and hd.SOHOADON not in (select SoHoaDon from TT_HoaDon_KhongThu)"
                 + " and hd.ID_HOADON not in (select MaHD from TT_TraGop)"
                 + " and hd.ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
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

        public string GetTongTon_DenKy(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            string sql = "select t1.*,t2.HoTen,TyLe=CAST(ROUND(CONVERT(float,t1.TongHD)/(select COUNT(ID_HOADON) from HOADON a where (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot + " and MaNV_HanhThu=t1.MaNV_HanhThu)*100,2)as varchar(5)) from"
                        + " (select MaNV_HanhThu,TongHD=COUNT(ID_HOADON),TongCong=SUM(TONGCONG) from HOADON"
                        + " where NgayGiaiTrach is null and (NAM<" + Nam + " or (NAM=" + Nam + " and KY<=" + Ky + ")) and DOT>=" + FromDot + " and DOT<=" + ToDot
                        + " and MAY>=(select TuCuonGCS from TT_To where MaTo=" + MaTo + ") and MAY<=(select DenCuonGCS from TT_To where MaTo=" + MaTo + ")"
                        + " group by MaNV_HanhThu) t1,TT_NguoiDung t2"
                        + " where t1.MaNV_HanhThu=t2.MaND"
                        + " order by t2.STT asc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
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

        //admin
        public string truyvan(string sql)
        {
            try
            {
                return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
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
                return _cDAL.ExecuteNonQuery(sql).ToString();
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
                var transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
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
                        + " SLHDDTSach=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is null),"
                        + " TCHDDTSach=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and (NAM>2020 or (NAM=2020 and KY>=7)) and MaNV_DangNgan is not null and SoHoaDonCu is null),"
                        + " SLNopTien=(select COUNT(ID_HOADON) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and SyncNopTien=1),"
                        + " TCNopTien=(select SUM(TONGCONG) from HOADON where CAST(NGAYGIAITRACH as date)=CAST(NgayChot as date) and SyncNopTien=1)"
                        + " from TT_ChotDangNgan where CAST(NgayChot as date)>='" + FromNgayGiaiTrach.ToString("yyyyMMdd") + "' and CAST(NgayChot as date)<='" + ToNgayGiaiTrach.ToString("yyyyMMdd") + "'"
                        + " group by ID,NgayChot,Chot order by ID desc";
            return DataTableToJSON(_cDAL.ExecuteQuery_DataTable(sql));
        }

        public string them_ChotDangNgan(DateTime NgayGiaiTrach, string CreateBy)
        {
            try
            {
                if (bool.Parse(_cDAL.ExecuteQuery_ReturnOneValue("select case when exists(select ID from TT_ChotDangNgan where CAST(NgayChot as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "') then 'true' else 'false' end").ToString()) == true)
                    return "false;Ngày Chốt đã tồn tại";
                if (_cDAL.ExecuteNonQuery("insert into TT_ChotDangNgan(ID,NgayChot,Chot,CreateBy,CreateDate)values((select case when exists(select ID from TT_ChotDangNgan) then (select MAX(ID)+1 from TT_ChotDangNgan) else 1 end),'" + NgayGiaiTrach.ToString("yyyyMMdd HH:mm:ss") + "',0," + CreateBy + ",getdate())") == true)
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
                if (_cDAL.ExecuteNonQuery("update TT_ChotDangNgan set Chot=" + value + ",ModifyBy=" + CreateBy + ",ModifyDate=getdate() where ID=" + ID) == true)
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
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select DanhBo=hd.DANHBA,Ky=(convert(varchar(2),KY)+'/'+convert(varchar(4),NAM)),tmp.Result from HOADON hd,Temp_SyncHoaDon tmp where CAST(NGAYGIAITRACH as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and SyncNopTien=0 and hd.SOHOADON=tmp.SoHoaDon");
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
        #region Class

        string urlTong = "https://hoadon.sawaco.com.vn";
        string taxCode = "0301129367";
        string userName = "tanhoaapi";
        string passWord = "tanhoaapi@sawaco.com.vn#2020";
        string branchcode = "TH";
        string pattern = "01GTKT0/002";
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

        public string syncThanhToan(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                    request.Method = "POST";
                    request.Headers.Add("taxcode", taxCode);
                    request.Headers.Add("username", userName);
                    request.Headers.Add("password", passWord);
                    request.ContentType = "application/json; charset=utf-8";

                    string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        NgayThanhToan = dt.Rows[0]["NgayGiaiTrach"].ToString();
                    else
                        NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                    if (bool.Parse(dt.Rows[0]["DangNgan_Ton"].ToString()) == true)
                        LoaiThuTien = "0";
                    else
                        if (bool.Parse(dt.Rows[0]["DangNgan_ChuyenKhoan"].ToString()) == true)
                            LoaiThuTien = "2";
                        else
                            if (bool.Parse(dt.Rows[0]["DangNgan_Quay"].ToString()) == true)
                                LoaiThuTien = "1";

                    if (GiaiTrach == true)
                        ThanhToan = "1";
                    else
                        ThanhToan = "0";

                    if (dt.Rows[0]["DangNgan"].ToString() != "")
                        TenThuTien = dt.Rows[0]["DangNgan"].ToString();
                    else
                        TenThuTien = "NULL";

                    //var data = new
                    //{
                    //    branchcode = branchcode,
                    //    pattern = pattern,
                    //    serial = serial,
                    //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
                    //    NgayThanhToan = NgayThanhToan,
                    //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
                    //    LoaiThuTien = LoaiThuTien,
                    //    TenThuTien = TenThuTien,
                    //    ThanhToan = ThanhToan,
                    //};
                    HoaDonThanhToan en = new HoaDonThanhToan();
                    en.branchcode = branchcode;
                    en.pattern = pattern;
                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
                    en.NgayThanhToan = NgayThanhToan;
                    en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
                    en.LoaiThuTien = LoaiThuTien;
                    en.TenThuTien = TenThuTien;
                    en.ThanhToan = ThanhToan;

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(en);
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(result);
                        if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                        {
                            _cDAL.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                            _cDAL.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                        }
                        else
                        {
                            _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                        }
                        result = "true;" + obj["status"] + " = " + obj["message"];
                    }
                    else
                        result = "false;" + respuesta.StatusCode;
                }
                else
                {
                    result = "false;Hóa Đơn không có";
                    DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=1 and ID_HOADON=" + MaHD);
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string ThanhToan = "-1";
                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";
                        _cDAL.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                        _cDAL.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncThanhToan(string SoHoaDon, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                    request.Method = "POST";
                    request.Headers.Add("taxcode", taxCode);
                    request.Headers.Add("username", userName);
                    request.Headers.Add("password", passWord);
                    request.ContentType = "application/json; charset=utf-8";

                    string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        NgayThanhToan = dt.Rows[0]["NgayGiaiTrach"].ToString();
                    else
                        NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                    if (bool.Parse(dt.Rows[0]["DangNgan_Ton"].ToString()) == true)
                        LoaiThuTien = "0";
                    else
                        if (bool.Parse(dt.Rows[0]["DangNgan_ChuyenKhoan"].ToString()) == true)
                            LoaiThuTien = "2";
                        else
                            if (bool.Parse(dt.Rows[0]["DangNgan_Quay"].ToString()) == true)
                                LoaiThuTien = "1";

                    if (GiaiTrach == true)
                        ThanhToan = "1";
                    else
                        ThanhToan = "0";

                    if (dt.Rows[0]["DangNgan"].ToString() != "")
                        TenThuTien = dt.Rows[0]["DangNgan"].ToString();
                    else
                        TenThuTien = "NULL";

                    //var data = new
                    //{
                    //    branchcode = branchcode,
                    //    pattern = pattern,
                    //    serial = serial,
                    //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
                    //    NgayThanhToan = NgayThanhToan,
                    //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
                    //    LoaiThuTien = LoaiThuTien,
                    //    TenThuTien = TenThuTien,
                    //    ThanhToan = ThanhToan,
                    //};
                    HoaDonThanhToan en = new HoaDonThanhToan();
                    en.branchcode = branchcode;
                    en.pattern = pattern;
                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
                    en.NgayThanhToan = NgayThanhToan;
                    en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
                    en.LoaiThuTien = LoaiThuTien;
                    en.TenThuTien = TenThuTien;
                    en.ThanhToan = ThanhToan;

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(en);
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(result);
                        if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                        {
                            _cDAL.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and SOHOADON='" + SoHoaDon + "'");
                            _cDAL.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                        }
                        else
                        {
                            _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                        }
                        result = "true;" + obj["status"] + " = " + obj["message"];
                    }
                    else
                        result = "false;" + respuesta.StatusCode;
                }
                else
                {
                    result = "false;Hóa Đơn không có";
                    DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,DangNgan_Ton,DangNgan_ChuyenKhoan,DangNgan_Quay,DangNgan=(select HoTen from TT_NguoiDung where MaND=MaNV_DangNgan) from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string ThanhToan = "-1";
                        if (GiaiTrach == true)
                            ThanhToan = "1";
                        else
                            ThanhToan = "0";
                        _cDAL.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and SOHOADON='" + SoHoaDon + "'");
                        _cDAL.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncThanhToan_ThuHo(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt;
                if (GiaiTrach == true)
                    dt = _cDAL.ExecuteQuery_DataTable("select SoHoaDon,NGAYGIAITRACH=(select convert(varchar, CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu where EXISTS(select * from HOADON where MaHD=" + MaHD + " and BaoCaoThue=0) and MaHD=" + MaHD);
                else
                    dt = _cDAL.ExecuteQuery_DataTable("select SoHoaDon,NGAYGIAITRACH=(select convert(varchar, CreateDate, 112)),TONGCONG=SoTien,DangNgan_Ton='false',DangNgan_ChuyenKhoan='true',DangNgan_Quay='false',DangNgan=TenDichVu from TT_DichVuThu_Huy where EXISTS(select * from HOADON where MaHD=" + MaHD + " and BaoCaoThue=0) and MaHD=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/thanhtoan");
                    request.Method = "POST";
                    request.Headers.Add("taxcode", taxCode);
                    request.Headers.Add("username", userName);
                    request.Headers.Add("password", passWord);
                    request.ContentType = "application/json; charset=utf-8";

                    string NgayThanhToan = "", LoaiThuTien = "0", ThanhToan = "-1", TenThuTien = "";
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        NgayThanhToan = dt.Rows[0]["NgayGiaiTrach"].ToString();
                    else
                        NgayThanhToan = DateTime.Now.ToString("yyyyMMdd");

                    if (bool.Parse(dt.Rows[0]["DangNgan_Ton"].ToString()) == true)
                        LoaiThuTien = "0";
                    else
                        if (bool.Parse(dt.Rows[0]["DangNgan_ChuyenKhoan"].ToString()) == true)
                            LoaiThuTien = "2";
                        else
                            if (bool.Parse(dt.Rows[0]["DangNgan_Quay"].ToString()) == true)
                                LoaiThuTien = "1";

                    if (GiaiTrach == true)
                        ThanhToan = "1";
                    else
                        ThanhToan = "0";

                    if (dt.Rows[0]["DangNgan"].ToString() != "")
                        TenThuTien = dt.Rows[0]["DangNgan"].ToString();
                    else
                        TenThuTien = "NULL";

                    //var data = new
                    //{
                    //    branchcode = branchcode,
                    //    pattern = pattern,
                    //    serial = serial,
                    //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
                    //    NgayThanhToan = NgayThanhToan,
                    //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
                    //    LoaiThuTien = LoaiThuTien,
                    //    TenThuTien = TenThuTien,
                    //    ThanhToan = ThanhToan,
                    //};
                    HoaDonThanhToan en = new HoaDonThanhToan();
                    en.branchcode = branchcode;
                    en.pattern = pattern;
                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
                    en.NgayThanhToan = NgayThanhToan;
                    en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
                    en.LoaiThuTien = LoaiThuTien;
                    en.TenThuTien = TenThuTien;
                    en.ThanhToan = ThanhToan;

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(en);
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(result);
                        if (obj["status"] == "OK" || obj["status"] == "ERR:4" || obj["status"] == "ERR:6" || obj["status"] == "ERR:7")
                        {
                            _cDAL.ExecuteNonQuery("update HOADON set SyncThanhToan=" + ThanhToan + ",SyncThanhToan_Ngay=getdate() where SyncThanhToan=0 and ID_HOADON=" + MaHD);
                            _cDAL.ExecuteNonQuery("delete Temp_SyncHoaDon where ID=" + IDTemp_SyncHoaDon);
                        }
                        else
                        {
                            _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                        }
                        result = "true;" + obj["status"] + " = " + obj["message"];
                    }
                    else
                    {
                        result = "false;" + respuesta.StatusCode;
                        _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + result + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                    }
                }
                else
                {
                    result = "false;Hóa Đơn không có";
                    _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + result + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
                }
            }
            catch (Exception ex)
            {
                result = "false; " + ex.Message;
                _cDAL.ExecuteNonQuery("update Temp_SyncHoaDon set Result=N'" + result + "',ModifyDate=getdate() where ID=" + IDTemp_SyncHoaDon);
            }
            return result;
        }

        public string syncNopTien(int MaHD)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=0 and ID_HOADON=" + MaHD);
                if (dt != null && dt.Rows.Count > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptien");
                    request.Method = "POST";
                    request.Headers.Add("taxcode", taxCode);
                    request.Headers.Add("username", userName);
                    request.Headers.Add("password", passWord);
                    request.ContentType = "application/json; charset=utf-8";

                    string NgayNopTien = "", HinhThucThanhToan = "";
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        NgayNopTien = dt.Rows[0]["NgayGiaiTrach"].ToString();
                    else
                        NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                    if (bool.Parse(dt.Rows[0]["ChuyenNoKhoDoi"].ToString()) == true)
                        HinhThucThanhToan = "2";
                    else
                        HinhThucThanhToan = "1";

                    //var data = new
                    //{
                    //    branchcode = branchcode,
                    //    pattern = pattern,
                    //    serial = serial,
                    //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
                    //    NgayNopTien = NgayNopTien,
                    //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
                    //    HinhThucThanhToan = HinhThucThanhToan,
                    //};
                    HoaDonNopTien en = new HoaDonNopTien();
                    en.branchcode = branchcode;
                    en.pattern = pattern;
                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
                    en.NgayNopTien = NgayNopTien;
                    en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
                    en.HinhThucThanhToan = HinhThucThanhToan;

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(en);
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(result);
                        if (obj["status"] == "OK" || obj["status"] == "ERR:7" || obj["status"] == "ERR:8")
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD;
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            //_cDAL.ExecuteNonQuery("insert into Temp_SyncHoaDon(ID,[Action],SoHoaDon,Result)values((select ID=case when not exists (select ID from Temp_SyncHoaDon) then 1 else MAX(ID)+1 end from Temp_SyncHoaDon),'NopTien','" + dt.Rows[0]["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')");
                            _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "')"
                                           + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + dt.Rows[0]["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')"
                                           + " else update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "'");
                        }
                        result = "true;" + obj["status"] + " = " + obj["message"];
                    }
                    else
                    {
                        result = "false;" + respuesta.StatusCode;
                        DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=1 and ID_HOADON=" + MaHD);
                        if (dtBCT != null && dtBCT.Rows.Count > 0)
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and ID_HOADON=" + MaHD;
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
                        }
                    }
                }
                else
                    result = "false;Hóa Đơn không có";
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncNopTien(string SoHoaDon)
        {
            string result = "";
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=0 and SOHOADON='" + SoHoaDon + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptien");
                    request.Method = "POST";
                    request.Headers.Add("taxcode", taxCode);
                    request.Headers.Add("username", userName);
                    request.Headers.Add("password", passWord);
                    request.ContentType = "application/json; charset=utf-8";

                    string NgayNopTien = "", HinhThucThanhToan = "";
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        NgayNopTien = dt.Rows[0]["NgayGiaiTrach"].ToString();
                    else
                        NgayNopTien = DateTime.Now.ToString("yyyyMMdd");

                    if (bool.Parse(dt.Rows[0]["ChuyenNoKhoDoi"].ToString()) == true)
                        HinhThucThanhToan = "2";
                    else
                        HinhThucThanhToan = "1";

                    //var data = new
                    //{
                    //    branchcode = branchcode,
                    //    pattern = pattern,
                    //    serial = serial,
                    //    SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6),
                    //    NgayNopTien = NgayNopTien,
                    //    TongSoTien = dt.Rows[0]["TongCong"].ToString(),
                    //    HinhThucThanhToan = HinhThucThanhToan,
                    //};
                    HoaDonNopTien en = new HoaDonNopTien();
                    en.branchcode = branchcode;
                    en.pattern = pattern;
                    en.serial = dt.Rows[0]["SoHoaDon"].ToString().Substring(0, 6);
                    en.SoHD = dt.Rows[0]["SoHoaDon"].ToString().Substring(6);
                    en.NgayNopTien = NgayNopTien;
                    en.TongSoTien = dt.Rows[0]["TongCong"].ToString();
                    en.HinhThucThanhToan = HinhThucThanhToan;

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(en);
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
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var obj = js.Deserialize<dynamic>(result);
                        if (obj["status"] == "OK" || obj["status"] == "ERR:7" || obj["status"] == "ERR:8")
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SOHOADON='" + SoHoaDon + "'";
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            //_cDAL.ExecuteNonQuery("insert into Temp_SyncHoaDon(ID,[Action],SoHoaDon,Result)values((select ID=case when not exists (select ID from Temp_SyncHoaDon) then 1 else MAX(ID)+1 end from Temp_SyncHoaDon),'NopTien','" + dt.Rows[0]["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')");
                            _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "')"
                                           + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + dt.Rows[0]["SoHoaDon"].ToString() + "',N'" + obj["status"] + " = " + obj["message"] + "')"
                                           + " else update Temp_SyncHoaDon set Result=N'" + obj["status"] + " = " + obj["message"] + "',ModifyDate=getdate() where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "'");
                        }
                        result = "true;" + obj["status"] + " = " + obj["message"];
                    }
                    else
                        result = "false;" + respuesta.StatusCode;
                }
                else
                {
                    result = "false;Hóa Đơn không có";
                    DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where BaoCaoThue=1 and SOHOADON='" + SoHoaDon + "'");
                    if (dtBCT != null && dtBCT.Rows.Count > 0)
                    {
                        string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SOHOADON='" + SoHoaDon + "'";
                        sql += " delete Temp_SyncHoaDon where SoHoaDon='" + dt.Rows[0]["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                        _cDAL.ExecuteNonQuery(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "false;" + ex.Message;
            }
            return result;
        }

        public string syncNopTienLo(DateTime NgayGiaiTrach)
        {
            string result = "";
            try
            {
                DataTable dtSerial = _cDAL.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 group by SUBSTRING(SOHOADON,0,7)");
                if (dtSerial == null || dtSerial.Rows.Count == 0)
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(pattern) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
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

                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(lstHD);
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
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                HoaDonNopTienLoResult deserializedResult = serializer.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'";
                                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'";
                                            _cDAL.ExecuteNonQuery(sql);
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
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
                    else
                    {
                        result = "false;" + "Hóa Đơn không có";
                        DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                        foreach (DataRow item in dtBCT.Rows)
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'";
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
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
                DataTable dtSerial = _cDAL.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY<12 and DCHD=0 group by SUBSTRING(SOHOADON,0,7)");
                if (dtSerial == null || dtSerial.Rows.Count == 0)
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY<12 and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(pattern) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY<12 and DCHD=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
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

                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(lstHD);
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
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                HoaDonNopTienLoResult deserializedResult = serializer.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'";
                                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'";
                                            _cDAL.ExecuteNonQuery(sql);
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
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
                    else
                    {
                        result = "false;" + "Hóa Đơn không có";
                        DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY<12 and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                        foreach (DataRow item in dtBCT.Rows)
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'";
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
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
                DataTable dtSerial = _cDAL.ExecuteQuery_DataTable("select serial=SUBSTRING(SOHOADON,0,7) from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY=12 and DCHD=0 and BaoCaoThue=0 group by SUBSTRING(SOHOADON,0,7)");
                if (dtSerial == null || dtSerial.Rows.Count == 0)
                    result = "false;" + "Đã Nộp Tiền rồi";
                foreach (DataRow itemSerial in dtSerial.Rows)
                {
                    DataTable dt = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY=12 and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int SL = (int)Math.Ceiling((double)dt.Rows.Count / 1000);
                        for (int i = 0; i < SL; i++)
                        {
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlTong + "/api/sawacobusiness/noptienlo?branchCode=" + branchcode + "&pattern=" + HttpUtility.UrlEncode(pattern) + "&serial=" + HttpUtility.UrlEncode(itemSerial["serial"].ToString()));
                            request.Method = "POST";
                            request.Headers.Add("taxcode", taxCode);
                            request.Headers.Add("username", userName);
                            request.Headers.Add("password", passWord);
                            request.ContentType = "application/json; charset=utf-8";

                            var lstHD = new List<HoaDonNopTienLo>();
                            dt = _cDAL.ExecuteQuery_DataTable("select top 1000 SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY=12 and DCHD=0 and BaoCaoThue=0 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
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

                            var serializer = new JavaScriptSerializer();
                            var json = serializer.Serialize(lstHD);
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
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                HoaDonNopTienLoResult deserializedResult = serializer.Deserialize<HoaDonNopTienLoResult>(result);
                                if (deserializedResult.Status == "OK")
                                {
                                    foreach (HoaDonNopTienResult item in deserializedResult.result)
                                    {
                                        if (item.Status == "OK" || item.Status == "ERR:7" || item.Status == "ERR:8")
                                        {
                                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "'";
                                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "' and [Action]='NopTien'";
                                            _cDAL.ExecuteNonQuery(sql);
                                        }
                                        //else
                                        //if (item.Status == "ERR:6")
                                        //{
                                        //syncThanhToan(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"), true, 0);
                                        //syncNopTien(itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000"));
                                        //}
                                        else
                                        {
                                            _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "')"
                                            + " insert into Temp_SyncHoaDon([Action],SoHoaDon,Result)values('NopTien','" + itemSerial["serial"].ToString() + ((int)item.SoHD).ToString("0000000") + "',N'" + item.Status + " = " + item.Message + "')"
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
                    else
                    {
                        result = "false;" + "Hóa Đơn không có";
                        DataTable dtBCT = _cDAL.ExecuteQuery_DataTable("select SOHOADON,NGAYGIAITRACH=(select convert(varchar, NGAYGIAITRACH, 112)),TONGCONG,ChuyenNoKhoDoi from HOADON where Cast(NgayGiaiTrach as date)='" + NgayGiaiTrach.ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null and syncNopTien=0 and (NAM>2020 or (NAM=2020 and KY>=7)) and KY=12 and DCHD=0 and BaoCaoThue=1 and SUBSTRING(SOHOADON,0,7)='" + itemSerial["serial"].ToString() + "' order by NGAYGIAITRACH asc");
                        foreach (DataRow item in dtBCT.Rows)
                        {
                            string sql = "update HOADON set SyncNopTien=1,SyncNopTien_Ngay=getdate() where SyncNopTien=0 and SoHoaDon='" + item["SoHoaDon"].ToString() + "'";
                            sql += " delete Temp_SyncHoaDon where SoHoaDon='" + item["SoHoaDon"].ToString() + "' and [Action]='NopTien'";
                            _cDAL.ExecuteNonQuery(sql);
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
                    _cDAL.ExecuteNonQuery(sql);
                }
                else
                {
                    _cDAL.ExecuteNonQuery("if not exists (select ID from Temp_SyncHoaDon where SoHoaDon='" + itemSerial["serial"].ToString() + ((int)itemResult.result[i].SoHD).ToString("0000000") + "')"
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

        //chi tiết tiền nước
        private const int _GiamTienNuoc = 10;

        public string updateChiTietTienNuoc(int Nam, int Ky, int Dot)
        {
            try
            {
                string ChiTietCuA = "", ChiTietCuB = "";
                int TyleSH = 0, TyLeSX = 0, TyLeDV = 0, TyLeHCSN = 0, TongTienCuA = 0, TongTienCuB = 0, TieuThu_DieuChinhGia = 0, DinhMucHN = 0;
                List<HOADON> lst = _dbThuTien.HOADONs.Where(item => item.NAM == Nam && item.KY == Ky && item.DOT == Dot && item.ChiTietTienNuoc == null).ToList();
                foreach (HOADON item in lst)
                {
                    ChiTietCuA = ChiTietCuB = "";
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
                        TinhTienNuoc(false, false, 0, item.DANHBA, Ky, Nam, item.TUNGAY.Value, item.DENNGAY.Value, item.GB.Value, TyleSH, TyLeSX, TyLeDV, TyLeHCSN, (int)item.DM.Value, DinhMucHN, (int)item.TIEUTHU.Value, out TongTienCuA, out ChiTietCuA, out TongTienCuB, out ChiTietCuB, out TieuThu_DieuChinhGia);
                    else
                    {
                        DataTable dt = _cDocSo.get(item.DANHBA, item.NAM.Value.ToString(), item.KY.ToString());
                        DateTime TuNgay = DateTime.Parse(dt.Rows[0]["TuNgay"].ToString()), DenNgay = DateTime.Parse(dt.Rows[0]["DenNgay"].ToString());
                        TinhTienNuoc(false, false, 0, item.DANHBA, Ky, Nam, TuNgay, DenNgay, item.GB.Value, TyleSH, TyLeSX, TyLeDV, TyLeHCSN, (int)item.DM.Value, DinhMucHN, (int)item.TIEUTHU.Value, out TongTienCuA, out ChiTietCuA, out TongTienCuB, out ChiTietCuB, out TieuThu_DieuChinhGia);
                    }
                    item.ChiTietTienNuoc = ChiTietCuA + "\r\n" + ChiTietCuB;
                    _dbThuTien.SubmitChanges();
                }

                return "true; ";
            }
            catch (Exception ex)
            {
                return "false;" + ex.Message;
            }
        }

        public void TinhTienNuoc(bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out int TienNuocCu, out string ChiTietCu, out int TienNuocMoi, out string ChiTietMoi, out int TieuThu_DieuChinhGia)
        {
            List<GiaNuoc2> lst = _dbThuTien.GiaNuoc2s.ToList();
            int index = -1;
            TienNuocCu = TienNuocMoi = 0;
            ChiTietCu = ChiTietMoi = "";
            TieuThu_DieuChinhGia = 0;
            for (int i = 0; i < lst.Count; i++)
                if (TuNgay.Date < lst[i].NgayTangGia.Value.Date && lst[i].NgayTangGia.Value.Date < DenNgay.Date)
                {
                    index = i;
                }
                else
                    if (TuNgay.Date >= lst[i].NgayTangGia.Value.Date)
                    {
                        index = i;
                    }
            if (index != -1)
            {
                if (DenNgay.Date < new DateTime(2019, 11, 15))
                {
                    //int TieuThu_DieuChinhGia;
                    List<int> lstGiaNuoc = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
                    TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, 0, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                }
                else
                    if (TuNgay.Date < lst[index].NgayTangGia.Value.Date && lst[index].NgayTangGia.Value.Date < DenNgay.Date)
                    {
                        if (ApGiaNuocCu == false)
                        {
                            //int TieuThu_DieuChinhGia;
                            int TongSoNgay = (int)((DenNgay.Date - TuNgay.Date).TotalDays);

                            int SoNgayCu = (int)((lst[index].NgayTangGia.Value.Date - TuNgay.Date).TotalDays);
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
                            List<int> lstGiaNuocCu = new List<int> { lst[index - 1].SHTM.Value, lst[index - 1].SHVM1.Value, lst[index - 1].SHVM2.Value, lst[index - 1].SX.Value, lst[index - 1].HCSN.Value, lst[index - 1].KDDV.Value, lst[index - 1].SHN.Value };
                            List<int> lstGiaNuocMoi = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
                            //lần đầu áp dụng giá biểu 10, tổng áp giá mới luôn
                            if (TuNgay.Date < new DateTime(2019, 11, 15) && new DateTime(2019, 11, 15) < DenNgay.Date && GiaBieu == 10)
                                TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
                            else
                                TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
                            TienNuocMoi = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucMoi, DinhMucHN_Moi, TieuThuMoi, out ChiTietMoi, out TieuThu_DieuChinhGia);
                        }
                        else
                        {
                            List<int> lstGiaNuocCu = new List<int> { lst[index - 1].SHTM.Value, lst[index - 1].SHVM1.Value, lst[index - 1].SHVM2.Value, lst[index - 1].SX.Value, lst[index - 1].HCSN.Value, lst[index - 1].KDDV.Value, lst[index - 1].SHN.Value };
                            TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                        }
                    }
                    else
                    {
                        //int TieuThu_DieuChinhGia;
                        List<int> lstGiaNuoc = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
                        TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                    }
            }
            else
            {

            }
        }

        public int TinhTienNuoc(bool DieuChinhGia, int GiaDieuChinh, List<int> lstGiaNuoc, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out string ChiTiet, out int TieuThu_DieuChinhGia)
        {
            try
            {
                string _chiTiet = "";
                int DinhMuc = TongDinhMuc - DinhMucHN, _SH = 0, _SX = 0, _DV = 0, _HCSN = 0;
                TieuThu_DieuChinhGia = 0;
                //HOADON hoadon = _cThuTien.Get(DanhBo, Ky, Nam);
                //List<GiaNuoc> lstGiaNuoc = db.GiaNuocs.ToList();
                ///Table GiaNuoc được thiết lập theo bảng giá nước
                ///1. Đến 4m3/người/tháng
                ///2. Trên 4m3 đến 6m3/người/tháng
                ///3. Trên 6m3/người/tháng
                ///4. Đơn vị sản xuất
                ///5. Cơ quan, đoàn thể HCSN
                ///6. Đơn vị kinh doanh, dịch vụ
                ///7. Hộ nghèo, cận nghèo
                ///List bắt đầu từ phần tử thứ 0
                int TongTien = 0;
                switch (GiaBieu)
                {
                    ///TƯ GIA
                    case 10:
                        DinhMucHN = TongDinhMuc;
                        if (TieuThu <= DinhMucHN)
                        {
                            TongTien = TieuThu * lstGiaNuoc[6];
                            if (TieuThu > 0)
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN <= Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((TieuThu - DinhMucHN) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if ((TieuThu - DinhMucHN) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((int)Math.Round((double)DinhMuc / 2) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if ((int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6])
                                            + ((TieuThu - DinhMucHN) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if ((TieuThu - DinhMucHN) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                if (lstGiaNuoc[6] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN;
                            }
                        break;
                    case 11:
                    case 21:///SH thuần túy
                        //if (TieuThu <= DinhMucHN)
                        //{
                        //    TongTien = TieuThu * lstGiaNuoc[6];
                        //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        //}
                        //else
                        //    if (TieuThu - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * lstGiaNuoc[6])
                        //                    + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                        //        _chiTiet = (DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                        //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        //    }
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0])
                                            + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

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
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
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
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 14:
                    case 24:///SH + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeSX == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[3]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

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

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _SX * lstGiaNuoc[3];
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 15:
                    case 25:///SH + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeDV == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (TieuThu * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                //double TyLe = Math.Round((double)DinhMucHN / (DinhMucHN + DinhMuc), 2);
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[5]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

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

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _DV * lstGiaNuoc[5];
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
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
                            TongTien = (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SX > 0)
                                _chiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
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

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    case 17:
                    case 27:///SH ĐB
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[0];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 18:
                    case 28:
                    case 38:///SH + HCSN
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeHCSN == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[4]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

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

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[0]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _HCSN * lstGiaNuoc[4];
                            if (_HCSN > 0)
                                updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
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

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * lstGiaNuoc[6];
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                if (lstGiaNuoc[0] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        break;
                    ///TẬP THỂ
                    //case 21:///SH thuần túy
                    //    if (TieuThu <= DinhMuc)
                    //        TongTien = TieuThu * lstGiaNuoc[0];
                    //    else
                    //        if (TieuThu - DinhMuc <= DinhMuc / 2)
                    //            TongTien = (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[1]);
                    //        else
                    //            TongTien = (DinhMuc * lstGiaNuoc[0]) + (DinhMuc / 2 * lstGiaNuoc[1]) + ((TieuThu - DinhMuc - DinhMuc / 2) * lstGiaNuoc[2]);
                    //    break;
                    //case 22:///SX thuần túy
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 23:///DV thuần túy
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    //case 24:///SH + SX
                    //    hoadon = _cThuTien.GetMoiNhat(DanhBo);
                    //    if (hoadon != null)
                    //        ///Nếu không có tỉ lệ
                    //        if (hoadon.TILESH==null && hoadon.TILESX==null)
                    //        {

                    //        }
                    //    break;
                    //case 25:///SH + DV

                    //    break;
                    //case 26:///SH + SX + DV

                    //    break;
                    //case 27:///SH ĐB
                    //    TongTien = TieuThu * lstGiaNuoc[0];
                    //    break;
                    //case 28:///SH + HCSN

                    //    break;
                    //case 29:///SH + HCSN + SX + DV

                    //    break;
                    ///CƠ QUAN
                    case 31:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[4];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    //case 32:///SX
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 33:///DV
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    case 34:///HCSN + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeSX == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[3];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _SX = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _HCSN;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 35:///HCSN + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeDV == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[5];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _DV = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_DV * lstGiaNuoc[5]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    case 36:///HCSN + SX + DV
                        {
                            //int _HCSN = 0, _SX = 0, _DV = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN - _SX;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    //case 38:///SH + HCSN

                    //    break;
                    //case 39:///SH + HCSN + SX + DV

                    //    break;
                    ///NƯỚC NGOÀI
                    case 41:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[2];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    //case 42:///SX
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 43:///DV
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    case 44:///SH + SX
                        {
                            //int _SH = 0, _SX = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _SH;

                            TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]);
                            if (_SH > 0)
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 45:///SH + DV
                        //int _SH = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;

                        TongTien = (_SH * lstGiaNuoc[2]) + (_DV * lstGiaNuoc[5]);
                        if (_SH > 0)
                            _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        break;
                    case 46:///SH + SX + DV
                        {
                            //int _SH = 0, _SX = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH - _SX;

                            TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SH > 0)
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    ///BÁN SỈ
                    case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
                        //if (TieuThu <= DinhMuc)
                        //    TongTien = TieuThu * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100));
                        //else
                        //    if (TieuThu - DinhMuc <= DinhMuc / 2)
                        //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100)));
                        //    else
                        //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + (DinhMuc / 2 * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100))) + ((TieuThu - DinhMuc - DinhMuc / 2) * (lstGiaNuoc[2] - (lstGiaNuoc[2] * 10 / 100)));
                        //if (TieuThu <= DinhMucHN)
                        //{
                        //    TongTien = TieuThu * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (TieuThu - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (TieuThu - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                        //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((TieuThu - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                            }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 52:///sỉ khu công nghiệp
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 53:///sỉ KD - TM
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 54:///sỉ HCSN
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 58:
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;

                        TongTien += (_SH * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                    + (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
                                    + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
                                    + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        if (_SH > 0)
                            _chiTiet += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                        break;
                    case 59:///sỉ phức tạp
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (_SH - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + "\r\n"
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)) + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)) + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 68:///SH giá sỉ - KD giá lẻ
                        //int _SH = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + ((_SH - DinhMucHN) * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMuc;
                            }
                        TongTien += _DV * lstGiaNuoc[5];
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    default:
                        _chiTiet = "";
                        TongTien = 0;
                        break;
                }
                ChiTiet = _chiTiet;
                return TongTien;
            }
            catch (Exception)
            {
                ChiTiet = "";
                TieuThu_DieuChinhGia = 0;
                return 0;
            }
        }

        public void updateChiTiet(ref string main_value, string update_value)
        {
            if (main_value == "")
                main_value = update_value;
            else
                main_value += "\r\n" + update_value;
        }

    }
}
