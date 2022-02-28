using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSSmartPhone
{
    public class CGlobalVariable
    {
        public static string cheksum = "tanho@2022";
        public static string DHN = "Data Source=hp_g7\\KD;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string DocSo = "Data Source=hp_g7\\KD;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string ThuTien = "Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string KinhDoanh = "Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string pathHinhDHN = @"\\192.168.90.241\hinhdhn";

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
        public CHoaDon()
        {
            TieuThu = TienNuoc = ThueGTGT = PhiBVMT = PhiBVMT_Thue = TongCong = 0;
        }
    }
}