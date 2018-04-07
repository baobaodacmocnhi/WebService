using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
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

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select top 1 * from HOADON where DANHBA='"+DanhBo+"' order by ID_HOADON desc");
        }


    }
}
