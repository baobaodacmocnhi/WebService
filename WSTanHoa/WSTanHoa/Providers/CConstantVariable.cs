using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WSTanHoa.Providers
{
    public class CConstantVariable
    {
        public static decimal IDZalo1 = -1;
        public static string DHN = "Data Source=hp_g7\\KD;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string DocSo = "Data Source=hp_g7\\KD;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string DocSo12 = "Data Source=server12;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=P@ssword";
        public static string GanMoi = "Data Source=hp_g7\\KD;Initial Catalog=TANHOA_WATER;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string ThuTien = "Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTien_test = "Data Source=serverg8-01;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string KinhDoanh = "Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string TrungTamKhachHang = "Data Source=serverg8-01;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        

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
    }
}