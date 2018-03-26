using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;

namespace WSSmartPhone
{
    class CThuTien
    {
        Connection _DAL = new Connection("Data Source=192.168.90.9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=P@ssW012d9");

        public DataTable DangNhap(string Username, string Password)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from TT_NguoiDung where TaiKhoan='"+Username+"' and MatKhau='"+Password+"' and An=0");
        }

        public string GetVersion()
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select Version from TT_ConfigAndroid").Rows[0]["Version"].ToString();
        }

        public DataTable GetDSHoaDon(string DanhBo)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
        }

        public DataTable GetDSHoaDon(string Nam, string Ky, string Dot, string MaNV_HanhThu)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from HOADON where Nam=" + Nam + " and Ky=" + Ky + " and Dot=" + Dot + " and MaNV_HanhThu=" + MaNV_HanhThu + " order by ID_HOADON desc");
        }

        public DataTable GetHDMoiNhat(string DanhBo)
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select top 1 * from HOADON where DANHBA='"+DanhBo+"' order by ID_HOADON desc");
        }


    }
}
