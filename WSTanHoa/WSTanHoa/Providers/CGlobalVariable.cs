using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WSTanHoa.Models.db;

namespace WSTanHoa.Providers
{
    public class CGlobalVariable
    {
        public static decimal IDZalo = 4276209776391262580;
        public static string cheksum = "tanho@2022";
        public static string DHN = "Data Source=server9;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DHNWFH = "Data Source=113.161.88.180,1933;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSo = "Data Source=server9;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSoWFH = "Data Source=113.161.88.180,1933;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string sDHN = "Data Source=server9;Initial Catalog=sDHN;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string sDHNWFH = "Data Source=113.161.88.180,1933;Initial Catalog=sDHN;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string DocSo12 = "Data Source=server9;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db12@tanhoa";
        public static string GanMoi = "Data Source=server9;Initial Catalog=TANHOA_WATER;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTien = "Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTienWFH = "Data Source=113.161.88.180,1933;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTien_test = "Data Source=server11;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string ThuongVu = "Data Source=server9;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuongVuWFH = "Data Source=113.161.88.180,1933;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string TrungTamKhachHang = "Data Source=server9;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string TrungTamKhachHangWFH = "Data Source=113.161.88.180,1933;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string BauCu = "Data Source=113.161.88.180,1933;Initial Catalog=DH_CODONG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string pathHinhDHN = @"\\rackstation\HinhDHN";
        public static string pathHinhDHNMaHoa = @"\\rackstation\HinhDHN\MaHoa";
        public static string pathHinhTV = @"\\rackstation\HinhDHN\ThuongVu";
        public static JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        public static log4net.ILog log = log4net.LogManager.GetLogger("File");

        public static string getSHA256(string strData)
        {
            SHA256Managed crypt = new SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(strData), 0, Encoding.UTF8.GetByteCount(strData));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }

        public static string convertMoney(string money)
        {
            return String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", decimal.Parse(money));
        }

        public static string convertMoney(decimal money)
        {
            return String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", money);
        }
    }
}