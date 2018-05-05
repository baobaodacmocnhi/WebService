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

namespace WSSmartPhone
{
    class CThuTien
    {
        Connection _DAL = new Connection("Data Source=192.168.90.9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=P@ssW012d9");

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

        public string DangNhap(string Username, string Password,string UID)
        {
            _DAL.ExecuteQuery_SqlDataAdapter_DataTable("update TT_NguoiDung set UID='" + UID + "' where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0"));
        }

        public bool DangXuat(string Username)
        {
           return _DAL.ExecuteNonQuery("update TT_NguoiDung set UID='' where TaiKhoan='" + Username + "'");
        }

        public bool UpdateUID(string MaNV, string UID)
        {
            return _DAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "' where MaND=" + MaNV);
        }

        public string GetVersion()
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select Version from TT_ConfigAndroid").Rows[0]["Version"].ToString();
        }

        public string GetDSHoaDon(string DanhBo)
        {
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc"));
        }

        public string GetDSHoaDon(string Nam, string Ky, string FromDot,string ToDot, string MaNV_HanhThu)
        {
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from HOADON where Nam=" + Nam + " and Ky=" + Ky + " and Dot>=" + FromDot + " and Dot<="+ToDot+" and MaNV_HanhThu=" + MaNV_HanhThu + " order by MALOTRINH asc"));
        }

        public string SendNotificationToClient(string Title,string Content,string MaNV,string SoHoaDon)
        {
            string response;
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
                string deviceId = (string)_DAL.ExecuteQuery_ReturnOneValue("select UID from TT_NguoiDung where MaND="+MaNV); // Also something very long, 
                // got from android
                //string deviceId = "//topics/all";             // Use this to notify all devices, 
                // but App must be subscribed to 
                // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    data = new
                    {
                        title = Title,
                        body = Content,
                        SoHoaDon=SoHoaDon,
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

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
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            return response;
        }

        public string GetDSDongNuoc(string MaNV_DongNuoc, DateTime FromNgayGiao, DateTime ToNgayGiao)
        {
            string query = "select dn.MaDN,dn.DanhBo,dn.HoTen,dn.DiaChi,dn.MLT,"
                            + " Hieu=case when kqdn.Hieu is not null then kqdn.Hieu else (select Hieu=ttkh.HIEUDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " Co=case when kqdn.Co is not null then kqdn.Co else (select ttkh.CODH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " SoThan=case when kqdn.SoThan is not null then kqdn.SoThan else (select SoThan=ttkh.SOTHANDH from [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=dn.DanhBo) end,"
                            + " kqdn.NgayDN,kqdn.ChiSoDN,kqdn.ChiMatSo,kqdn.ChiKhoaGoc,kqdn.LyDo,kqdn.NgayMN,kqdn.ChiSoMN"
                            + " from TT_DongNuoc dn left join TT_KQDongNuoc kqdn on dn.MaDN=kqdn.MaDN"
                            + " where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyy-MM-dd") + "' and CAST(NgayGiao as DATE)<='" + ToNgayGiao.ToString("yyyy-MM-dd") + "'"
                            + " order by dn.MLT";
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable(query));
        }

        public bool KiemTraHoaDon_DongNuoc(string MaDN)
        {
            DataTable dt = _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select ctdn.*,hd.NGAYGIAITRACH from TT_CTDongNuoc ctdn, HOADON hd where MaDN=" + MaDN + " and ctdn.MaHD=hd.ID_HOADON");
            int CountGiaiTrach = 0;
            foreach (DataRow item in dt.Rows)
                if (item["NgayGiaiTrach"].ToString() != "")
                {
                    CountGiaiTrach++;
                }
            if (CountGiaiTrach == dt.Rows.Count)
                return true;
            return false;
        }

        public bool CheckExist_DongNuoc(string MaDN)
        {
            if (int.Parse(_DAL.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where DongNuoc=1 MaDN=" + MaDN).ToString()) == 0)
                return false;
            else
                return true;
        }

        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, DateTime NgayDN, string ChiSoDN, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            string sql = "insert into TT_KQDongNuoc(MaKQDN,MaDN,DanhBo,MLT,HoTen,DiaChi,DongNuoc,HinhDN,NgayDN,NgayDN_ThucTe,ChiSoDN,Hieu,Co,SoThan,ChiMatSo,ChiKhoaGoc,LyDo,PhiMoNuoc,CreateBy,CreateDate)values("
                + "(select case when (select COUNT(MaKQDN) from TT_KQDongNuoc)=0 then 1 else (select MAX(MaKQDN) from TT_KQDongNuoc)+1 end)," + MaDN + ",'" + DanhBo + "','" + MLT + "','" + HoTen + "','" + DiaChi + "',1,CONVERT(VARBINARY(max), '" + HinhDN + "')"
                + ",'" + NgayDN.ToString("yyyy-MM-dd") +" "+ DateTime.Now.ToString("HH:mm:ss.fff")+"',getDate()," + ChiSoDN + ",'" + Hieu + "'," + Co + ",'" + SoThan + "',N'" + ChiMatSo + "',N'" + ChiKhoaGoc + "',N'" + LyDo + "',(select top 1 PhiMoNuoc from TT_CacLoaiPhi)," + CreateBy + ",getDate())";
            return _DAL.ExecuteNonQuery(sql);
        }

        public bool CheckExist_MoNuoc(string MaDN)
        {
            if (int.Parse(_DAL.ExecuteQuery_ReturnOneValue("select COUNT(MaKQDN) from TT_KQDongNuoc where MoNuoc=0 and MaDN=" + MaDN).ToString()) == 0)
                return false;
            else
                return true;
        }

        public bool ThemMoNuoc(string MaDN,  string HinhMN, DateTime NgayMN, string ChiSoMN, string CreateBy)
        {
            string sql = "update TT_KQDongNuoc set HinhMN=CONVERT(VARBINARY(max), '" + HinhMN + "'),NgayMN='" + NgayMN.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss.fff") + "',NgayMN_ThucTe=getDate(),ChiSoMN=" + ChiSoMN + ",ModifyBy="+CreateBy+",ModifyDate=getDate() where MaDN=" + MaDN;
            return _DAL.ExecuteNonQuery(sql);
        }




        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select top 1 * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

    }
}
