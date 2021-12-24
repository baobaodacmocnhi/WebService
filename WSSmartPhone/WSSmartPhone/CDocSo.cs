using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Drawing;

namespace WSSmartPhone
{
    class CDocSo
    {
        //CConnection _DAL = new CConnection("Data Source=113.161.88.180,1833;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");
        CConnection _DAL = new CConnection("Data Source=hp_g7\\kd;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");

        //public bool CapNhat(string ID, string DanhBo, int Nam, int Ky, string CodeMoi, string TTDHNMoi, int CSMoi, int GiaBieu, int DinhMuc, string Latitude, string Longitude, out int TieuThu, out int TongCong)
        //{
        //    int GiaBan, PhiBVMT, ThueGTGT;
        //    string ChiTiet;
        //    TieuThu = TinhTieuThu(DanhBo, Nam, Ky, CodeMoi, CSMoi);
        //    GiaBan = TinhTienNuoc(DanhBo, GiaBieu, DinhMuc, TieuThu, out ChiTiet);
        //    if (_DAL.ExecuteQuery_ReturnOneValue("select DanhBo from DanhBoKPBVMT where DanhBo='" + DanhBo + "'") != null)
        //        PhiBVMT = 0;
        //    else
        //        PhiBVMT = (int)(GiaBan * 0.1);
        //    ThueGTGT = (int)(GiaBan * 0.05);
        //    TongCong = GiaBan + PhiBVMT + ThueGTGT;
        //    string sql = "update DocSo set NVGHI='nvds',GIOGHI=getdate(),SOLANGHI=1,GPSDATA='0',CSMoi=" + CSMoi + ",CodeMoi='" + CodeMoi + "',TTDHNMoi='" + TTDHNMoi + "',TieuThuMoi=" + TieuThu + ",TienNuoc=" + GiaBan + ",BVMT=" + PhiBVMT + ",Thue=" + ThueGTGT + ",TongTien=" + TongCong + ","
        //        + "ChiTiet='" + ChiTiet + "',Latitude='" + Latitude + "',Longitude='" + Longitude + "',NgayDS=getdate() where DocSoID=" + ID + " and (NgayDS is null or Cast(NgayDS as date)='1900-01-01' or Cast(NgayDS as date)=Cast(getdate() as date))";
        //    //return _DAL_Test.ExecuteNonQuery(sql);
        //    return _DAL.ExecuteNonQuery(sql);
        //}

        public bool CheckDangNhap(string TaiKhoan, string MatKhau)
        {
            if (_DAL.ExecuteQuery_ReturnOneValue("select MaND from NguoiDung where TaiKhoan='" + TaiKhoan + "' and MatKhau='" + MatKhau + "'") != null)
                return true;
            else
                return false;
        }

        public object GetCurrentVersion()
        {
            return _DAL.ExecuteQuery_ReturnOneValue("select Value from Configure where Name='CurrentVersion'");

        }

        public DataTable DangNhap(string TaiKhoan, string MatKhau)
        {
            return _DAL.ExecuteQuery_DataTable("select * from NguoiDung where TaiKhoan='" + TaiKhoan + "' and MatKhau='" + MatKhau + "'");
        }

        public DataTable GetDSCode()
        {
            return _DAL.ExecuteQuery_DataTable("select * from TTDHN where vitri is not null order by vitri asc");
        }

        public DataTable GetDSDocSo(string Nam, string Ky, string Dot, string May)
        {
            string sql = "declare @Nam int;"
                        + " declare @Ky int;"
                        + " declare @Dot int;"
                        + " declare @May int;"
                        + " set @Nam=" + Nam + ";"
                        + " set @Ky=" + Ky + ";"
                        + " set @Dot=" + Dot + ";"
                        + " set @May=" + May + ";"
                        + " select a.*,b.*,c.*,d.HoTen from"
                        + " (select * from DocSo where Nam=@Nam and Ky=@Ky and Dot=@Dot and May=@May ) a";
            switch (int.Parse(Ky))
            {
                case 1:
                    sql += " left join"
                        + " (select DanhBa,CSCu2=CSCu,CodeCu2=CodeCu,TieuThuCu2=TieuThuCu from DocSo where Nam=@Nam-1 and Ky=12 and Dot=@Dot and May=@May) b on a.DanhBa=b.DanhBa"
                        + " left join"
                        + " (select DanhBa,CSCu3=CSCu,CodeCu3=CodeCu,TieuThuCu3=TieuThuCu from DocSo where Nam=@Nam-1 and Ky=11 and Dot=@Dot and May=@May) c on a.DanhBa=c.DanhBa";
                    break;
                case 2:
                    sql += " left join"
                        + " (select DanhBa,CSCu2=CSCu,CodeCu2=CodeCu,TieuThuCu2=TieuThuCu from DocSo where Nam=@Nam-1 and Ky=@Ky-1 and Dot=@Dot and May=@May) b on a.DanhBa=b.DanhBa"
                        + " left join"
                        + " (select DanhBa,CSCu3=CSCu,CodeCu3=CodeCu,TieuThuCu3=TieuThuCu from DocSo where Nam=@Nam-1 and Ky=12 and Dot=@Dot and May=@May) c on a.DanhBa=c.DanhBa";
                    break;
                default:
                    sql += " left join"
                        + " (select DanhBa,CSCu2=CSCu,CodeCu2=CodeCu,TieuThuCu2=TieuThuCu from DocSo where Nam=@Nam and Ky=@Ky-1 and Dot=@Dot and May=@May) b on a.DanhBa=b.DanhBa"
                        + " left join"
                        + " (select DanhBa,CSCu3=CSCu,CodeCu3=CodeCu,TieuThuCu3=TieuThuCu from DocSo where Nam=@Nam and Ky=@Ky-2 and Dot=@Dot and May=@May) c on a.DanhBa=c.DanhBa";
                    break;
            }
            sql += " left join"
                + " (select DanhBa,HoTen=TenKH from KhachHang) d on a.DanhBa=d.DanhBa"
                + " order by MLT1 asc";
            //return _DAL_Test.ExecuteQuery_DataTable(sql);
            return _DAL.ExecuteQuery_DataTable(sql);
        }

        public DataTable get(string DanhBo, string Nam, string Ky)
        {
            string sql = "select TuNgay,DenNgay from DocSo where DanhBa='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky;
            return _DAL.ExecuteQuery_DataTable(sql);
        }

        public int TinhTieuThu(string DanhBo, int nam, int ky, string code, int csmoi)
        {
            int tieuthu = 0;
            try
            {
                //_DAL_Test.Connect();
                _DAL.Connect();

                //SqlCommand cmd = new SqlCommand("calTieuTHu", _DAL_Test.connection);
                SqlCommand cmd = new SqlCommand("calTieuTHu", _DAL.Connection);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _db = cmd.Parameters.Add("@DANHBO", SqlDbType.VarChar);
                _db.Direction = ParameterDirection.Input;
                _db.Value = DanhBo;

                SqlParameter _ky = cmd.Parameters.Add("@KY", SqlDbType.Int);
                _ky.Direction = ParameterDirection.Input;
                _ky.Value = ky;

                SqlParameter _nam = cmd.Parameters.Add("@NAM", SqlDbType.Int);
                _nam.Direction = ParameterDirection.Input;
                _nam.Value = nam;

                SqlParameter _code = cmd.Parameters.Add("@CODE", SqlDbType.VarChar);
                _code.Direction = ParameterDirection.Input;
                _code.Value = code;

                SqlParameter _csmoi = cmd.Parameters.Add("@CSMOI", SqlDbType.Int);
                _csmoi.Direction = ParameterDirection.Input;
                _csmoi.Value = csmoi;

                SqlParameter _tieuthu = cmd.Parameters.Add("@TIEUTHU", SqlDbType.Int);
                _tieuthu.Direction = ParameterDirection.Output;


                cmd.ExecuteNonQuery();

                tieuthu = int.Parse(cmd.Parameters["@TIEUTHU"].Value + "");
            }
            catch (Exception)
            {
                //MessageBox.Show(this, ex.ToString());
            }
            finally
            {
                //_DAL_Test.Disconnect();
                _DAL.Disconnect();
            }
            return tieuthu;
        }

        public bool checkKhongTinhPBVMT(string DanhBo)
        {
            if (_DAL.ExecuteQuery_ReturnOneValue("select DanhBo from DanhBoKPBVMT where DanhBo='" + DanhBo + "'") != null)
                return true;
            else
                return false;
        }


    }
}