using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class ResultThuTien
    {
        public bool success { get; set; }

        public string message { get; set; }

        public Logger logger { get; set}

        public ResultThuTien()
        {
            success = false;
            message = "Không Có Kết Quả";
            nguoidung = new Logger();
        }

        public ResultThuTien(bool success, string message)
        {
            this.success = success;
            this.message = message;
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