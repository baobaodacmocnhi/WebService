using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Globalization;

namespace WSAgribank
{
    /// <summary>
    /// Summary description for THService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class THService : System.Web.Services.WebService
    {
        CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public DataSet getCustomerInfo(string db, string ten, string matkhau)
        {
            string sql = "SELECT TOP(1) hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG  ";
            sql += " FROM HOADON hd WHERE  hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += " ORDER BY NAM DESC, KY DESC";

            return _cDAL.ExecuteQuery_DataSet(sql);
        }

        [WebMethod]
        public DataSet W_Bill(string db, string ten, string matkhau)
        {
            string sql = "SELECT ID_HOADON AS IDkey, hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG,hd.GB,hd.DM, hd.DOT, hd.KY as KyHD, hd.NAM as NamHD, hd.PHI as PBVMT, hd.THUE as TGTGT,hd.GIABAN as TNuoc,hd.TONGCONG as TONGCONG  ";
            sql += " FROM HOADON hd ";
            sql += " WHERE NGAYGIAITRACH IS NULL AND hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += "   AND hd.DANHBA NOT IN (SELECT DANHBA FROM BGW_HOADON WHERE BGW_HOADON.KY=hd.KY and BGW_HOADON.NAM=hd.NAM AND BGW_HOADON.DANHBA=hd.DANHBA) ";
            sql += "  AND hd.DANHBA NOT IN (SELECT Dbo FROM ThuOnline WHERE ThuOnline.KyHD=hd.KY and ThuOnline.NamHD=hd.NAM AND ThuOnline.Dbo=hd.DANHBA) ";
            sql += " AND hd.DANHBA NOT IN (SELECT Dbo FROM SimpayDB WHERE SimpayDB.KyHD=hd.KY and SimpayDB.NamHD=hd.NAM AND SimpayDB.Dbo=hd.DANHBA)   ";
            sql += " AND hd.DANHBA NOT IN (SELECT DANHBA FROM Agribank_THUTAM WHERE Agribank_THUTAM.KyHD=hd.KY and Agribank_THUTAM.NamHD=hd.NAM AND Agribank_THUTAM.DANHBO=hd.DANHBA ) ";
            sql += " AND hd.DANHBA NOT IN (SELECT DANHBA FROM dbo_VNPAY WHERE dbo_VNPAY.KY=hd.KY and dbo_VNPAY.NAM=hd.NAM AND dbo_VNPAY.DANHBA=hd.DANHBA) ";
            sql += " AND hd.DANHBA NOT IN (SELECT DANHBO FROM MOMO_SERVICE WHERE MOMO_SERVICE.KY=hd.KY and MOMO_SERVICE.NAM=hd.NAM AND MOMO_SERVICE.DANHBO=hd.DANHBA)  ";

            sql += " ORDER BY NAM DESC, KY DESC ";

            return _cDAL.ExecuteQuery_DataSet(sql);
        }

        [WebMethod]
        public bool payW_Bill(string id, string ten, string matkhau)
        {
            try
            {
                string sql = " INSERT INTO Agribank_THUTAM (ID_HOADON, DANHBO, KHACHHANG, SONHA, TENDUONG, GB, DM, DOT, KYHD, NAMHD, GIABAN, THUE, PBVMT, TONGCONG, SOHOADON, MAGIAODICH, NGAYTHANHTOAN, GACHNO)";
                sql += " SELECT ID_HOADON , hd.DANHBA, hd.TENKH as KHACHHANG, SO as SONHA,DUONG as TENDUONG,hd.GB,hd.DM, hd.DOT, hd.KY as KYHD, hd.NAM as NAMHD,hd.GIABAN,hd.THUE, hd.PHI as PBVMT, hd.TONGCONG,hd.SOHOADON, '" + ten + "' AS MAGIAODICH,GETDATE() as  NGAYTHANHTOAN, '1' as GACHNO ";
                sql += " FROM HOADON hd";
                sql += " WHERE  hd.ID_HOADON IN (" + id.Replace("#", ",") + ") AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";

                return _cDAL.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return false;
            }
            //return true;
        }

        [WebMethod]
        public DataSet checkPay_Bill(string id, string ten, string matkhau)
        {

            string sql = " SELECT ID_HOADON AS IDkey, DANHBO, KHACHHANG, SONHA, TENDUONG, GB, DM, DOT, KYHD, NAMHD, GIABAN, THUE, PBVMT, TONGCONG ";
            sql += " FROM Agribank_THUTAM hd ";
            sql += " WHERE GACHNO ='1' AND hd.ID_HOADON='" + id + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += " ORDER BY NAM DESC, KY DESC ";

            return _cDAL.ExecuteQuery_DataSet(sql);
        }

        /// <summary>
        /// Ghi lại hóa đơn được thu lúc nào, dịch vụ nào, ngân hàng nào
        /// </summary>
        /// <param name="SoHoaDon"></param>
        /// <param name="DanhBo"></param>
        /// <param name="Nam"></param>
        /// <param name="Ky"></param>
        /// <param name="SoTien"></param>
        /// <param name="TenDichVu"></param>
        /// <param name="TenNganHang"></param>
        /// <returns></returns>
        [WebMethod]
        public bool Insert_DichVuThu(string SoHoaDon, string DanhBo, int Nam, int Ky, int SoTien, int Phi, string TenDichVu, string TenNganHang)
        {
            try
            {
                string sql = "insert into TT_DichVuThu(SoHoaDon,DanhBo,Nam,Ky,SoTien,Phi,TenDichVu,TenNganHang,CreateDate)"
                               + "values('" + SoHoaDon + "','" + DanhBo + "'," + Nam + "," + Ky + "," + SoTien + "," + Phi + ",N'" + TenDichVu + "',N'" + TenNganHang + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + "')";

                return _cDAL.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra Kỳ hóa đơn có được dịch vụ nào thu chưa
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="Nam"></param>
        /// <param name="Ky"></param>
        /// <returns></returns>
        [WebMethod]
        public bool Check_DichVuThu(string DanhBo, int Nam, int Ky)
        {
            try
            {
                string sql = "select * from TT_DichVuThu where DanhBo='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky;

                return _cDAL.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra danh bộ có phí phát sinh thêm không
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="PhiMoNuoc"></param>
        /// <returns></returns>
        [WebMethod]
        public bool Check_Phi(string DanhBo, out int Phi)
        {
            Phi = 0;
            try
            {
                string sqlPhiMoNuoc = "select PhiMoNuoc from TT_KQDongNuoc kqdn where DanhBo='" + DanhBo + "' and kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and kqdn.TroNgaiMN=0";
                DataTable dt = _cDAL.ExecuteQuery_DataTable(sqlPhiMoNuoc);

                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    Phi = int.Parse(dt.Rows[0]["PhiMoNuoc"].ToString());
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //[WebMethod]
        //public bool W_ThanhToan(int ID_HOADON, string db, string kh, string snha,
        //    string tduong, string gb, string dm, int dot, int ky,
        //    int nam, double gban, double thue, double phibv, double tongcong,
        //    string sohd, string magd, DateTime ngaytt, string user, string matkhau)
        //{
        //    if ("AGRIBANK".Equals(user) && DateTime.Now.Day.ToString().Equals(matkhau))
        //    {
        //        Agribank_THUTAM tmp = new Agribank_THUTAM();
        //        tmp.ID_HOADON = ID_HOADON;
        //        tmp.DANHBO = db;
        //        tmp.KHACHHANG = kh;
        //        tmp.SONHA = snha;
        //        tmp.TENDUONG = tduong;
        //        tmp.GB = gb;
        //        tmp.DM = dm;
        //        tmp.DOT = dot;
        //        tmp.KYHD = ky;
        //        tmp.NAMHD = nam;
        //        tmp.THUE = thue;
        //        tmp.GIABAN = gban;
        //        tmp.PBVMT = phibv;
        //        tmp.TONGCONG = tongcong;
        //        tmp.SOHOADON = sohd;
        //        tmp.MAGIAODICH = magd;
        //        tmp.NGAYTHANHTOAN = ngaytt;
        //        tmp.GACHNO = false;
        //        return LinQConnection.Insert(tmp);
        //    }
        //    return false;
        //}

    }
}
