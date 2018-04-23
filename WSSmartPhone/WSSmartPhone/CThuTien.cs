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
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from HOADON where Nam=" + Nam + " and Ky=" + Ky + " and Dot>=" + FromDot + " and Dot<="+ToDot+" and MaNV_HanhThu=" + MaNV_HanhThu + " order by ID_HOADON desc"));
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
            return DataTableToJSON(_DAL.ExecuteQuery_SqlDataAdapter_DataTable("select MaDN,DanhBo,HoTen,DiaChi,MLT from TT_DongNuoc where Huy=0 and MaNV_DongNuoc=" + MaNV_DongNuoc + " and CAST(NgayGiao as DATE)>='" + FromNgayGiao.ToString("yyyy-MM-dd") + "' and CAST(NgayGiao as DATE)>='" + ToNgayGiao.ToString("yyyy-MM-dd") + "'"));
        }

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select top 1 * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

    }
}
