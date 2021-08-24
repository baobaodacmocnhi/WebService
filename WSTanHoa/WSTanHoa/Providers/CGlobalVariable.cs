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
        public static string DHN = "Data Source=hp_g7\\KD;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string DocSo = "Data Source=hp_g7\\KD;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string DocSo12 = "Data Source=server12;Initial Catalog=DocSoTH;Persist Security Info=True;User ID=sa;Password=db12@tanhoa";
        public static string GanMoi = "Data Source=hp_g7\\KD;Initial Catalog=TANHOA_WATER;Persist Security Info=True;User ID=sa;Password=db8@tanhoa";
        public static string ThuTien = "Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTienWFH = "Data Source=113.161.88.180,1933;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
        public static string ThuTien_test = "Data Source=serverg8-01;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string KinhDoanh = "Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string TrungTamKhachHang = "Data Source=serverg8-01;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string TrungTamKhachHangWFH = "Data Source=113.161.88.180,1133;Initial Catalog=TRUNGTAMKHACHHANG;Persist Security Info=True;User ID=sa;Password=db11@tanhoa";
        public static string cheksum = "tanho@2022";       
        public static dbTrungTamKhachHang db = new dbTrungTamKhachHang();
        public static CConnection cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        public static CConnection cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        public static CConnection cDAL_DocSo12 = new CConnection(CGlobalVariable.DocSo12);
        public static CConnection cDAL_GanMoi = new CConnection(CGlobalVariable.GanMoi);
        public static CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        public static CConnection cDAL_KinhDoanh = new CConnection(CGlobalVariable.KinhDoanh);
        public static CConnection cDAL_TrungTam = new CConnection(CGlobalVariable.TrungTamKhachHangWFH);

        public static JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("File");

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