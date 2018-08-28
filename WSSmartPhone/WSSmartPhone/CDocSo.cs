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
        CConnection _DAL = new CConnection("Data Source=hp_g7\\kd;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");
        CThuTien _cThuTien = new CThuTien();
        CKinhDoanh _cKinhDoanh = new CKinhDoanh();

        public bool CapNhat(string ID, string DanhBo, int Nam, int Ky, string CodeMoi, string TTDHNMoi, int CSMoi, int GiaBieu, int DinhMuc, string Latitude, string Longitude, out int TieuThu, out int TongCong)
        {
            int GiaBan, PhiBVMT, ThueGTGT;
            string ChiTiet;
            TieuThu = TinhTieuThu(DanhBo, Nam, Ky, CodeMoi, CSMoi);
            GiaBan = TinhTienNuoc(DanhBo, GiaBieu, DinhMuc, TieuThu, out ChiTiet);
            if (_DAL.ExecuteQuery_ReturnOneValue("select DanhBo from DanhBoKPBVMT where DanhBo='" + DanhBo + "'") != null)
                PhiBVMT = 0;
            else
                PhiBVMT = (int)(GiaBan * 0.1);
            ThueGTGT = (int)(GiaBan * 0.05);
            TongCong = GiaBan + PhiBVMT + ThueGTGT;
            string sql = "update DocSo set NVGHI='nvds',GIOGHI=getdate(),SOLANGHI=1,GPSDATA='0',CSMoi=" + CSMoi + ",CodeMoi='" + CodeMoi + "',TTDHNMoi='" + TTDHNMoi + "',TieuThuMoi=" + TieuThu + ",TienNuoc=" + GiaBan + ",BVMT=" + PhiBVMT + ",Thue=" + ThueGTGT + ",TongTien=" + TongCong + ","
                + "ChiTiet='" + ChiTiet + "',Latitude='" + Latitude + "',Longitude='" + Longitude + "',NgayDS=getdate() where DocSoID=" + ID + " and (NgayDS is null or Cast(NgayDS as date)='1900-01-01' or Cast(NgayDS as date)=Cast(getdate() as date))";
            //return _DAL_Test.ExecuteNonQuery(sql);
            return _DAL.ExecuteNonQuery(sql);
        }

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

        private const int _GiamTienNuoc = 10;

        /// <summary>
        /// Công thức tính tiền nước theo giá biểu
        /// </summary>
        /// <param name="DieuChinhGia">true là điều chỉnh giá/ false là không</param>
        /// <param name="GiaDieuChinh"></param>
        /// <param name="DanhBo">Danh Bộ được dùng để lấy SH,SX,DV,HCSN</param>
        /// <param name="GiaBieu"></param>
        /// <param name="DinhMuc"></param>
        /// <param name="TieuThu"></param>
        /// <param name="ChiTiet"></param>
        /// <returns></returns>
        public void TinhTienNuoc(string DanhBo, int GiaBieu, int DinhMuc, int TieuThu, out int GiaBan, out int PhiBVMT, out int ThueGTGT, out int TongCong, out string ChiTiet)
        {
            try
            {
                DataTable dtHoaDon = _cThuTien.GetHDMoiNhat(DanhBo);

                DataTable dtGiaNuoc = _cKinhDoanh.GetDSGiaNuoc();
                ///Table GiaNuoc được thiết lập theo bảng giá nước
                ///1. Đến 4m3/người/tháng
                ///2. Trên 4m3 đến 6m3/người/tháng
                ///3. Trên 6m3/người/tháng
                ///4. Đơn vị sản xuất
                ///5. Cơ quan, đoàn thể HCSN
                ///6. Đơn vị kinh doanh, dịch vụ
                ///List bắt đầu từ phần tử thứ 0
                GiaBan = 0;
                PhiBVMT = 0;
                ThueGTGT = 0;
                TongCong = 0;
                ChiTiet = "";
                switch (GiaBieu)
                {
                    ///TƯ GIA
                    case 11:
                    case 21:///SH thuần túy
                        if (DinhMuc == 0)
                        {
                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                        }
                        else if (TieuThu <= DinhMuc)
                        {
                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                        }
                        else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                        {
                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                        + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                        }
                        else
                        {
                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                        + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                        + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                        }
                        //else
                        //{
                        //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                        //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                        //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                        //        TieuThu_DieuChinhGia = TieuThu;
                        //    else
                        //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                        //}
                        break;
                    case 12:
                    case 22:
                    case 32:
                    case 42:///SX thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 13:
                    case 23:
                    case 33:
                    case 43:///DV thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 14:
                    case 24:///SH + SX
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }

                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //if (!DieuChinhGia)
                                //{
                                //GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                //ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //           + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //               + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}

                            }
                            else
                            ///Nếu có tỉ lệ SH + SX
                            //if (hoadon.TILESH!=null && hoadon.TILESX!=null)
                            {
                                int _SH = 0, _SX = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _SX = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                {
                                    if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                                    {
                                        if (DinhMuc == 0)
                                        {
                                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                        }
                                        else if (TieuThu <= DinhMuc)
                                        {
                                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                        }
                                        else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                        {
                                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                        + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        }
                                        else
                                        {
                                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                        + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                        + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        }
                                    }
                                }
                                //if (!DieuChinhGia)
                                //if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                        break;
                    case 15:
                    case 25:///SH + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILEDV"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }
                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //    //if (!DieuChinhGia)
                                //    //{
                                //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}
                            }
                            else
                            ///Nếu có tỉ lệ SH + DV
                            //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                            {
                                int _SH = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _DV = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                    //if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 16:
                    case 26:///SH + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "" && dtHoaDon.Rows[0]["TILEDV"].ToString() != "" && dtHoaDon.Rows[0]["TILESH"].ToString() == "")
                            {
                                int _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                                int _DV = TieuThu - _SX;
                                GiaBan = (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                            + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có đủ 3 tỉ lệ SH + SX + DV
                            //if (hoadon.TILESX!=null && hoadon.TILEDV!=null && hoadon.TILESH!=null)
                            {
                                int _SH = 0, _SX = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                    _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                                _DV = TieuThu - _SH - _SX;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                    // if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet += "\r\n" + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                             + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 17:
                    case 27:///SH ĐB
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 18:
                    case 28:
                    case 38:///SH + HCSN
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }
                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //    //if (!DieuChinhGia)
                                //    //{
                                //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                                //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                                //    //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}
                            }
                            else
                            ///Nếu có tỉ lệ SH + HCSN
                            //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null)
                            {
                                int _SH = 0, _HCSN = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _HCSN = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                {
                                    if (DinhMuc == 0)
                                    {
                                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                    }
                                    else if (TieuThu <= DinhMuc)
                                    {
                                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                    }
                                    else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                }
                                //if (!DieuChinhGia)
                                //if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                            }
                        break;
                    case 19:
                    case 29:
                    case 39:///SH + HCSN + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _HCSN - _SX;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                            }
                            else
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }

                            }
                            ////if (!DieuChinhGia)
                            //    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                            //    {
                            //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            //    }
                            //    else
                            //    {
                            //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                            //                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            //    }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    ///TẬP THỂ
                    //case 21:///SH thuần túy
                    //    if (TieuThu <= DinhMuc)
                    //        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                    //    else
                    //        if (TieuThu - DinhMuc <= DinhMuc / 2)
                    //            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                    //        else
                    //            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + (DinhMuc / 2 * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                    //    break;
                    //case 22:///SX thuần túy
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 23:///DV thuần túy
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    //case 24:///SH + SX
                    //    hoadon = _cThuTien.GetMoiNhat(DanhBo);
                    //    if (dtHoaDon.Rows.Count != 0)
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
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                    //    break;
                    //case 28:///SH + HCSN

                    //    break;
                    //case 29:///SH + HCSN + SX + DV

                    //    break;
                    ///CƠ QUAN
                    case 31:///SHVM thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    //case 32:///SX
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 33:///DV
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    case 34:///HCSN + SX
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                                ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có tỉ lệ
                            //if (hoadon.TILEHCSN!=null && hoadon.TILESX!=null)
                            {
                                int _HCSN = 0, _SX = 0;
                                if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                    _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                                _SX = TieuThu - _HCSN;

                                GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                            + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                        break;
                    case 35:///HCSN + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                                ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có tỉ lệ
                            //if (hoadon.TILEHCSN!=null && hoadon.TILEDV!=null)
                            {
                                int _HCSN = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                    _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                                _DV = TieuThu - _HCSN;

                                GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                            + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 36:///HCSN + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _HCSN - _SX;

                            GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    //case 38:///SH + HCSN

                    //    break;
                    //case 39:///SH + HCSN + SX + DV

                    //    break;
                    ///NƯỚC NGOÀI
                    case 41:///SHVM thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    //case 42:///SX
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 43:///DV
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    case 44:///SH + SX
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILESX!=null)
                        {
                            int _SH = 0, _SX = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _SX = TieuThu - _SH;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                        }
                        break;
                    case 45:///SH + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _DV = TieuThu - _SH;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    case 46:///SH + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _SX;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    ///BÁN SỈ
                    case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
                        //if (TieuThu <= DinhMuc)
                        //    GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100));
                        //else
                        //    if (TieuThu - DinhMuc <= DinhMuc / 2)
                        //        GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100))) + ((TieuThu - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * 10 / 100)));
                        //    else
                        //        GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100))) + (DinhMuc / 2 * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * 10 / 100))) + ((TieuThu - DinhMuc - DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * 10 / 100)));
                        if (TieuThu <= DinhMuc)
                        {
                            GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                        }
                        else
                            //if (!DieuChinhGia)
                            if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                            {
                                GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((TieuThu - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                            + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            }
                            else
                            {
                                GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                            + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                            + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            }
                        //else
                        //{
                        //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((TieuThu - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                        //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                        //        TieuThu_DieuChinhGia = TieuThu;
                        //    else
                        //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 52:///sỉ khu công nghiệp
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 53:///sỉ KD - TM
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 54:///sỉ HCSN
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 59:///sỉ phức tạp
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _HCSN - _SX;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            }
                            else
                                //if (!DieuChinhGia)
                                if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((_SH - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((_SH - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                            //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += (_HCSN * (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + (_SX * (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + +(_DV * (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                         + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                         + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            //GiaBan -= GiaBan * 10 / 100;
                        }
                        break;
                    case 68:///SH giá sỉ - KD giá lẻ
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _DV = TieuThu - _SH;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100);
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                            }
                            else
                                //if (!DieuChinhGia)
                                if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((_SH - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                         + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                    ChiTiet = (DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * _GiamTienNuoc / 100))) + "\r\n"
                                         + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * _GiamTienNuoc / 100)) + "\r\n"
                                         + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * _GiamTienNuoc / 100));
                                }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((_SH - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                            //         + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += _DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                            ChiTiet += "\r\n" + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) );
                            //GiaBan -= GiaBan * 10 / 100;
                        }
                        break;
                    default:
                        GiaBan = 0;
                        PhiBVMT = 0;
                        ThueGTGT = 0;
                        TongCong = 0;
                        ChiTiet = "";
                        break;
                }
                PhiBVMT = (int)(GiaBan * 0.1);
                ThueGTGT = (int)(GiaBan * 0.05);
                TongCong = GiaBan + PhiBVMT + ThueGTGT;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GiaBan = 0;
                PhiBVMT = 0;
                ThueGTGT = 0;
                TongCong = 0;
                ChiTiet = "";
            }
        }

        public int TinhTienNuoc(string DanhBo, int GiaBieu, int DinhMuc, int TieuThu, out string ChiTiet)
        {
            try
            {
                DataTable dtHoaDon = _cThuTien.GetHDMoiNhat(DanhBo);

                DataTable dtGiaNuoc = _cKinhDoanh.GetDSGiaNuoc();
                ///Table GiaNuoc được thiết lập theo bảng giá nước
                ///1. Đến 4m3/người/tháng
                ///2. Trên 4m3 đến 6m3/người/tháng
                ///3. Trên 6m3/người/tháng
                ///4. Đơn vị sản xuất
                ///5. Cơ quan, đoàn thể HCSN
                ///6. Đơn vị kinh doanh, dịch vụ
                ///List bắt đầu từ phần tử thứ 0
               int GiaBan = 0;
               ChiTiet = "";
                switch (GiaBieu)
                {
                    ///TƯ GIA
                    case 11:
                    case 21:///SH thuần túy
                        if (DinhMuc == 0)
                        {
                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                        }
                        else if (TieuThu <= DinhMuc)
                        {
                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                        }
                        else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                        {
                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                        + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                        }
                        else
                        {
                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                        + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                        + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                        }
                        //else
                        //{
                        //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                        //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                        //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                        //        TieuThu_DieuChinhGia = TieuThu;
                        //    else
                        //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                        //}
                        break;
                    case 12:
                    case 22:
                    case 32:
                    case 42:///SX thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 13:
                    case 23:
                    case 33:
                    case 43:///DV thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 14:
                    case 24:///SH + SX
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }

                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //if (!DieuChinhGia)
                                //{
                                //GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                //ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //           + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //               + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}

                            }
                            else
                            ///Nếu có tỉ lệ SH + SX
                            //if (hoadon.TILESH!=null && hoadon.TILESX!=null)
                            {
                                int _SH = 0, _SX = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _SX = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                {
                                    if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                                    {
                                        if (DinhMuc == 0)
                                        {
                                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                        }
                                        else if (TieuThu <= DinhMuc)
                                        {
                                            GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                        }
                                        else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                        {
                                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                        + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        }
                                        else
                                        {
                                            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                            ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                        + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                        + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        }
                                    }
                                }
                                //if (!DieuChinhGia)
                                //if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                        break;
                    case 15:
                    case 25:///SH + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILEDV"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }
                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //    //if (!DieuChinhGia)
                                //    //{
                                //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}
                            }
                            else
                            ///Nếu có tỉ lệ SH + DV
                            //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                            {
                                int _SH = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _DV = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                    //if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 16:
                    case 26:///SH + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "" && dtHoaDon.Rows[0]["TILEDV"].ToString() != "" && dtHoaDon.Rows[0]["TILESH"].ToString() == "")
                            {
                                int _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                                int _DV = TieuThu - _SX;
                                GiaBan = (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                            + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có đủ 3 tỉ lệ SH + SX + DV
                            //if (hoadon.TILESX!=null && hoadon.TILEDV!=null && hoadon.TILESH!=null)
                            {
                                int _SH = 0, _SX = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                    _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                                _DV = TieuThu - _SH - _SX;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                    // if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet += "\r\n" + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                             + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 17:
                    case 27:///SH ĐB
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    case 18:
                    case 28:
                    case 38:///SH + HCSN
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() == "" && dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "")
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }
                                //if (TieuThu <= DinhMuc)
                                //{
                                //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                //}
                                //else
                                //    //if (!DieuChinhGia)
                                //    //{
                                //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                                //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                                //    //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = TieuThu;
                                //    else
                                //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                                //}
                            }
                            else
                            ///Nếu có tỉ lệ SH + HCSN
                            //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null)
                            {
                                int _SH = 0, _HCSN = 0;
                                if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                    _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                                _HCSN = TieuThu - _SH;

                                if (_SH <= DinhMuc)
                                {
                                    GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else
                                {
                                    if (DinhMuc == 0)
                                    {
                                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                    }
                                    else if (TieuThu <= DinhMuc)
                                    {
                                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                    }
                                    else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    }
                                    else
                                    {
                                        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                    + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    }
                                }
                                //if (!DieuChinhGia)
                                //if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                //}
                                //else
                                //{
                                //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                                //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                                //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                                //        TieuThu_DieuChinhGia = _SH;
                                //    else
                                //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                                //}
                                GiaBan += _HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString());
                                ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                            }
                        break;
                    case 19:
                    case 29:
                    case 39:///SH + HCSN + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _HCSN - _SX;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                            }
                            else
                            {
                                if (DinhMuc == 0)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));

                                }
                                else if (TieuThu <= DinhMuc)
                                {
                                    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                                    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()));
                                }
                                else if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                                                + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                                }

                            }
                            ////if (!DieuChinhGia)
                            //    if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                            //    {
                            //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                    + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                            //    }
                            //    else
                            //    {
                            //        GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((int)Math.Round((double)DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            //        ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                    + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + "\r\n"
                            //                    + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                            //    }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((_SH - DinhMuc) * GiaDieuChinh);
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + "\r\n"
                            //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) == GiaDieuChinh)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    ///TẬP THỂ
                    //case 21:///SH thuần túy
                    //    if (TieuThu <= DinhMuc)
                    //        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                    //    else
                    //        if (TieuThu - DinhMuc <= DinhMuc / 2)
                    //            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + ((TieuThu - DinhMuc) * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()));
                    //        else
                    //            GiaBan = (DinhMuc * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())) + (DinhMuc / 2 * int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString())) + ((TieuThu - DinhMuc - DinhMuc / 2) * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                    //    break;
                    //case 22:///SX thuần túy
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 23:///DV thuần túy
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    //case 24:///SH + SX
                    //    hoadon = _cThuTien.GetMoiNhat(DanhBo);
                    //    if (dtHoaDon.Rows.Count != 0)
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
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString());
                    //    break;
                    //case 28:///SH + HCSN

                    //    break;
                    //case 29:///SH + HCSN + SX + DV

                    //    break;
                    ///CƠ QUAN
                    case 31:///SHVM thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    //case 32:///SX
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 33:///DV
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    case 34:///HCSN + SX
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                                ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có tỉ lệ
                            //if (hoadon.TILEHCSN!=null && hoadon.TILESX!=null)
                            {
                                int _HCSN = 0, _SX = 0;
                                if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                    _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                                _SX = TieuThu - _HCSN;

                                GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                                ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                            + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            }
                        break;
                    case 35:///HCSN + DV
                        if (dtHoaDon.Rows.Count != 0)
                            ///Nếu không có tỉ lệ
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() == "" && dtHoaDon.Rows[0]["TILESX"].ToString() == "")
                            {
                                GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                                ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                            else
                            ///Nếu có tỉ lệ
                            //if (hoadon.TILEHCSN!=null && hoadon.TILEDV!=null)
                            {
                                int _HCSN = 0, _DV = 0;
                                if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                    _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                                _DV = TieuThu - _HCSN;

                                GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                                ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                            + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            }
                        break;
                    case 36:///HCSN + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _HCSN - _SX;

                            GiaBan = (_HCSN * int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    //case 38:///SH + HCSN

                    //    break;
                    //case 39:///SH + HCSN + SX + DV

                    //    break;
                    ///NƯỚC NGOÀI
                    case 41:///SHVM thuần túy
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString());
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * GiaDieuChinh;
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        break;
                    //case 42:///SX
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString());
                    //    break;
                    //case 43:///DV
                    //    GiaBan = TieuThu * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString());
                    //    break;
                    case 44:///SH + SX
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILESX!=null)
                        {
                            int _SH = 0, _SX = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _SX = TieuThu - _SH;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()));
                        }
                        break;
                    case 45:///SH + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _DV = TieuThu - _SH;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    case 46:///SH + SX + DV
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _SX;

                            GiaBan = (_SH * int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + (_SX * int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + (_DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                            ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString())) + "\r\n"
                                        + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString())) + "\r\n"
                                        + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()));
                        }
                        break;
                    ///BÁN SỈ
                    case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
                        //if (TieuThu <= DinhMuc)
                        //    GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100));
                        //else
                        //    if (TieuThu - DinhMuc <= DinhMuc / 2)
                        //        GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100))) + ((TieuThu - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * 10 / 100)));
                        //    else
                        //        GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) * 10 / 100))) + (DinhMuc / 2 * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) * 10 / 100))) + ((TieuThu - DinhMuc - DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) * 10 / 100)));
                        if (TieuThu <= DinhMuc)
                        {
                            GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100);
                            ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100));
                        }
                        else
                            //if (!DieuChinhGia)
                            if (TieuThu - DinhMuc <= Math.Round((double)DinhMuc / 2))
                            {
                                GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((TieuThu - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                                ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + "\r\n"
                                            + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                            }
                            else
                            {
                                GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + ((TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                                ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + "\r\n"
                                            + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + "\r\n"
                                            + (TieuThu - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                            }
                        //else
                        //{
                        //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((TieuThu - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                        //                + (TieuThu - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                        //        TieuThu_DieuChinhGia = TieuThu;
                        //    else
                        //        TieuThu_DieuChinhGia = TieuThu - DinhMuc;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 52:///sỉ khu công nghiệp
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 53:///sỉ KD - TM
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 54:///sỉ HCSN
                        //if (!DieuChinhGia)
                        //{
                        GiaBan = TieuThu * (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) / 100);
                        ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) / 100));
                        //}
                        //else
                        //{
                        //    GiaBan = TieuThu * (GiaDieuChinh - GiaDieuChinh  / 100);
                        //    ChiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                        //    TieuThu_DieuChinhGia = TieuThu;
                        //}
                        //GiaBan -= GiaBan * 10 / 100;
                        break;
                    case 59:///sỉ phức tạp
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEHCSN!=null && hoadon.TILESX!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILEHCSN"].ToString() != "")
                                _HCSN = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILEHCSN"].ToString()) / 100);
                            if (dtHoaDon.Rows[0]["TILESX"].ToString() != "")
                                _SX = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESX"].ToString()) / 100);
                            _DV = TieuThu - _SH - _HCSN - _SX;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100);
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100));
                            }
                            else
                                //if (!DieuChinhGia)
                                if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((_SH - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + "\r\n"
                                                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + "\r\n"
                                                + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + "\r\n"
                                                + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                                }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((_SH - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                            //                + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += (_HCSN * (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) / 100)) + (_SX * (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) / 100)) + +(_DV * (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) / 100));
                            ChiTiet += "\r\n" + _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[4]["DonGia"].ToString()) / 100)) + "\r\n"
                                         + _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[3]["DonGia"].ToString()) / 100)) + "\r\n"
                                         + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) / 100));
                            //GiaBan -= GiaBan * 10 / 100;
                        }
                        break;
                    case 68:///SH giá sỉ - KD giá lẻ
                        if (dtHoaDon.Rows.Count != 0)
                        //if (hoadon.TILESH!=null && hoadon.TILEDV!=null)
                        {
                            int _SH = 0, _DV = 0;
                            if (dtHoaDon.Rows[0]["TILESH"].ToString() != "")
                                _SH = (int)Math.Round((double)TieuThu * int.Parse(dtHoaDon.Rows[0]["TILESH"].ToString()) / 100);
                            _DV = TieuThu - _SH;

                            if (_SH <= DinhMuc)
                            {
                                GiaBan = _SH * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100);
                                ChiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100));
                            }
                            else
                                //if (!DieuChinhGia)
                                if (_SH - DinhMuc <= Math.Round((double)DinhMuc / 2))
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((_SH - DinhMuc) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                                    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + "\r\n"
                                         + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100));
                                }
                                else
                                {
                                    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100)) + ((int)Math.Round((double)DinhMuc / 2) * (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + ((_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) * (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                                    ChiTiet = (DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) / 100))) + "\r\n"
                                         + (int)Math.Round((double)DinhMuc / 2) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[1]["DonGia"].ToString()) / 100)) + "\r\n"
                                         + (_SH - DinhMuc - (int)Math.Round((double)DinhMuc / 2)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[2]["DonGia"].ToString()) / 100));
                                }
                            //else
                            //{
                            //    GiaBan = (DinhMuc * (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + ((_SH - DinhMuc) * (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    ChiTiet = DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100)) + "\r\n"
                            //         + (_SH - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh  / 100));
                            //    if (int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString()) - int.Parse(dtGiaNuoc.Rows[0]["DonGia"].ToString())  / 100 == GiaDieuChinh - GiaDieuChinh  / 100)
                            //        TieuThu_DieuChinhGia = _SH;
                            //    else
                            //        TieuThu_DieuChinhGia = _SH - DinhMuc;
                            //}
                            GiaBan += _DV * int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) ;
                            ChiTiet += "\r\n" + _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(dtGiaNuoc.Rows[5]["DonGia"].ToString()) );
                            //GiaBan -= GiaBan * 10 / 100;
                        }
                        break;
                    default:
                        GiaBan = 0;
                        ChiTiet = "";
                        break;
                }
                return GiaBan;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ChiTiet = "";
                return 0;
            }
        }

        public bool ThemHinhDHN(string DanhBo, string CreateBy, string imageStr,string Latitude, string Longitude)
        {
            //int ID = 0;
            //if (int.Parse(_DAL.ExecuteQuery_DataTable("select COUNT(ID) from HinhDHN").Rows[0][0].ToString()) == 0)
            //    ID = 1;
            //else
            //    ID = int.Parse(_DAL.ExecuteQuery_DataTable("select MAX(ID)+1 from HinhDHN").Rows[0][0].ToString());
            //string sql = "insert into HinhDHN(ID,DanhBo,Hinh,CreateBy,CreateDate)values(" + ID + ",'" + DanhBo + "'," + image + "," + CreateBy + ",GETDATE())";
            //return _DAL.ExecuteNonQuery(sql);

            try
            {
                //dbDocSoDataContext db = new dbDocSoDataContext();
                //Byte[] image = System.Convert.FromBase64String(imageStr);

                //HinhDHN entity = new HinhDHN();
                //if (db.HinhDHNs.Count() == 0)
                //    entity.ID = 1;
                //else
                //    entity.ID = db.HinhDHNs.Max(item => item.ID) + 1;

                //entity.DanhBo = DanhBo;
                //entity.Image = image;
                //entity.Latitude = Latitude;
                //entity.Longitude = Longitude;
                //entity.CreateBy = int.Parse(CreateBy);
                //entity.CreateDate = DateTime.Now;
                //db.HinhDHNs.InsertOnSubmit(entity);
                //db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}