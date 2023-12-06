using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class MResult
    {
        public bool success { set; get; }
        public string error { set; get; }
        public string alert { set; get; }
        public string message { set; get; }
        public string hoadonton { set; get; }
        public string data { set; get; }
        public MResult()
        {
            success = false;
            error = message = alert = hoadonton = data = "";
        }

        public class Logger
        {
            public string TaiKhoan { get; set; }

            public string MatKhau { get; set; }

            public int MaND { get; set; }

            public int MaTo { get; set; }

            public string HoTen { get; set; }

            public string DienThoai { get; set; }

            public bool Admin { get; set; }

            public bool HanhThu { get; set; }

            public bool DongNuoc { get; set; }

            public bool Doi { get; set; }

            public bool ToTruong { get; set; }

            public bool InPhieuBao { get; set; }

            public bool TestApp { get; set; }

            public bool SyncNopTien { get; set; }

            public Logger()
            {
                TaiKhoan = "";
                MatKhau = "";
                MaND = -1;
                MaTo = -1;
                HoTen = "";
                DienThoai = "";
                Admin = false;
                HanhThu = false;
                DongNuoc = false;
                Doi = false;
                ToTruong = false;
                InPhieuBao = false;
                TestApp = false;
                SyncNopTien = false;
            }

        }
    }
}