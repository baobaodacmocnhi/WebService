using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSSmartPhone
{
    public class CGlobalVariable
    {
        public static string cheksum = "tanho@2022";
        public static string DHN = "Data Source=server9;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSo = "Data Source=server9;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSoWFH = "Data Source=113.161.88.180,1933;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSo12 = "Data Source=server12;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db12@tanhoa";
        public static string ThuTien = "Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTienWFH = "Data Source=113.161.88.180,1933;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string KinhDoanh = "Data Source=server9;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string TTKH = "Data Source=server9;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string pathHinhDHN = @"\\rackstation\HinhDHN\DocSo";
        public static string pathHinhDHNMaHoa = @"\\rackstation\HinhDHN\MaHoa";
        public static string pathHinhTV = @"\\rackstation\HinhDHN\ThuongVu";
    }

    public class CResult
    {
        public bool success;
        public string error;
        public string alert;
        public string message;
        public string hoadonton;
        public CResult()
        {
            success = false;
            error = message = alert = hoadonton = "";
        }
    }

    public class CHoaDon
    {
        public int TieuThu, TienNuoc, ThueGTGT, PhiBVMT, PhiBVMT_Thue, TongCong;
        public string Ky, Nam, DanhBo, MLT, HoTen, DiaChi, GiaBieu, DinhMuc, CSC, CodeMoi, ChiSoMoi, TuNgay, DenNgay, TieuThuMoi;
        public CHoaDon()
        {
            TieuThu = TienNuoc = ThueGTGT = PhiBVMT = PhiBVMT_Thue = TongCong = 0;
            Ky = Nam = DanhBo = MLT = HoTen = DiaChi = GiaBieu = DinhMuc = CSC = CodeMoi = ChiSoMoi = TuNgay = DenNgay = TieuThuMoi = "";
        }
    }
}