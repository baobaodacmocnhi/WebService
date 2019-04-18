using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using THService.LinQ;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;

namespace THService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class THService : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet getCustomerInfo(string db, string ten, string matkhau)
        {
            string sql = "SELECT TOP(1) hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG  ";
            sql += " FROM HOADON hd WHERE  hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += " ORDER BY NAM DESC, KY DESC";

            return LinQConnection.getDataset(sql);
        }

        [WebMethod]
        public DataSet W_Bill(string db, string ten, string matkhau)
        {
            string sql = "SELECT ID_HOADON AS IDkey, hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG,hd.GB,hd.DM, hd.DOT, hd.KY as KyHD, hd.NAM as NamHD, hd.PHI as PBVMT, hd.THUE as TGTGT,hd.GIABAN as TNuoc,hd.TONGCONG as TONGCONG  ";
            sql += " FROM HOADON hd ";
            sql += " WHERE NGAYGIAITRACH IS NULL AND hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += " AND hd.DANHBA NOT IN (SELECT DanhBo FROM TT_DichVuThu WHERE TT_DichVuThu.Ky=hd.KY and TT_DichVuThu.Nam=hd.NAM AND TT_DichVuThu.DanhBo=hd.DANHBA) ";
            sql += " AND hd.DANHBA NOT IN (SELECT Dbo FROM SimpayDB WHERE SimpayDB.KyHD=hd.KY and SimpayDB.NamHD=hd.NAM AND SimpayDB.Dbo=hd.DANHBA)   ";

            sql += " ORDER BY NAM DESC, KY DESC ";

            return LinQConnection.getDataset(sql);
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

                string sql2 = "insert into TT_DichVuThu(SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,TenNganHang,CreateDate) ";
                sql2 += " SELECT SOHOADON AS  SoHoaDon , hd.DANHBA AS DanhBo, hd.NAM as Nam,  hd.KY as Ky,hd.TONGCONG AS SoTien ,'AGRIBANK' AS TenDichVu,'AGRIBANK' AS TenNganHang, GETDATE() as CreateDate ";
                sql2 += " FROM HOADON hd ";
                sql2 += " WHERE  hd.ID_HOADON IN (" + id.Replace("#", ",") + ") AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
               
                if (LinQConnection.ExecuteCommand_(sql) == 0)
                    return false;           
               
                LinQConnection.ExecuteCommand_(sql2);

                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        [WebMethod]
        public DataSet checkPay_Bill(string id, string ten, string matkhau)
        {

            string sql = " SELECT ID_HOADON AS IDkey, DANHBO, KHACHHANG, SONHA, TENDUONG, GB, DM, DOT, KYHD, NAMHD, GIABAN, THUE, PBVMT, TONGCONG ";
            sql += " FROM Agribank_THUTAM hd ";
            sql += " WHERE GACHNO ='1' AND hd.ID_HOADON='" + id + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";
            sql += " ORDER BY NAM DESC, KY DESC ";
            return LinQConnection.getDataset(sql);
        }

        [WebMethod]
        public DataSet getListHoaDon(string Nam, string Ky, string Dot, string maNV, string ten, string matkhau)
        {

            string sql = " select * from HOADON where Nam=" + Nam + " and Ky=" + Ky + " and Dot=" + Dot + " and MaNV_HanhThu=" + maNV + " order by ID_HOADON desc ";
            return LinQConnection.getDataset(sql);
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
        //[WebMethod]
        //public bool ThemHDThu(string SoHoaDon, string DanhBo, int Nam, int Ky, int SoTien, string TenDichVu, string TenNganHang)
        //{
        //    try
        //    {
        //        string sql = "insert into TT_DichVuThu(SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,TenNganHang,CreateDate)"
        //                       + "values('" + SoHoaDon + "','" + DanhBo + "'," + Nam + "," + Ky + "," + SoTien + ",N'" + TenDichVu + "',N'" + TenNganHang + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + "')";

        //        if (LinQConnection.ExecuteCommand_(sql) == 0)
        //            return false;
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Ghi lại hóa đơn được chuyển cho Tân Hòa lúc nào, dịch vụ nào, ngân hàng nào
        /// </summary>
        /// <param name="SoHoaDon"></param>
        /// <param name="DanhBo"></param>
        /// <param name="Nam"></param>
        /// <param name="Ky"></param>
        /// <param name="SoTien"></param>
        /// <param name="TenDichVu"></param>
        /// <param name="TenNganHang"></param>
        /// <returns></returns>
        //[WebMethod]
        //public bool ThemHDChuyen(string SoHoaDon, string DanhBo, int Nam, int Ky, int SoTien, string TenDichVu, string TenNganHang)
        //{
        //    try
        //    {
        //        string sql = "insert into TT_CTDichVuChuyen(SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,TenNganHang,CreateDate)"
        //                       + "values('" + SoHoaDon + "','" + DanhBo + "'," + Nam + "," + Ky + "," + SoTien + ",N'" + TenDichVu + "',N'" + TenNganHang + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture) + "')";

        //        if (LinQConnection.ExecuteCommand_(sql) == 0)
        //            return false;
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Kiểm tra hóa đơn đã được đơn vị khác thu chưa
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="Nam"></param>
        /// <param name="Ky"></param>
        /// <returns></returns>
        //[WebMethod]
        //public bool KiemTraHDThu(string DanhBo, int Nam, int Ky)
        //{
        //    try
        //    {
        //        string sql = "select * from TT_DichVuThu where DanhBo='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky;

        //        if (LinQConnection.ExecuteCommand(sql) == 0)
        //            return false;
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Kiểm tra danh bộ có bị đóng nước hay không, nếu bị đóng nước phải đóng thêm phí mở nước
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="PhiMoNuoc"></param>
        /// <returns></returns>
      
        [WebMethod]
        public bool Check_PhiDN(string DanhBo, out int Phi)
        {
            Phi = 0;
            try
            {
                string sqlPhiMoNuoc = "select PhiMoNuoc from TT_KQDongNuoc kqdn where DanhBo='" + DanhBo + "' and kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and kqdn.TroNgaiMN=0 ";
                DataTable dt = LinQConnection.getDataTable(sqlPhiMoNuoc);

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


        [WebMethod]
        public DataSet getSearchKH(string shs, string diachi, string madot)
        {
            string sql = "SELECT * FROM TANHOA_WATER.dbo.DAT_SEARCH WHERE SHS IS NOT NULL ";
            if (!"".Equals(shs))
            {
                sql += " AND (SHS = '" + shs + "'  )";
            }
            if (!"".Equals(diachi))
            {
                sql += " AND DIACHI LIKE N'%" + diachi + "%'";
            }
            if (!"".Equals(madot))
            {
                sql += " AND MADOTTC LIKE N'%" + madot + "%'";
            }

            return LinQConnection.getDataset(sql);
        }

        /////////BAO BAO///////////

        KinhDoanhDataContext _db = new KinhDoanhDataContext();
        [WebMethod]
        public DataSet GetCTDB(decimal MaCTCTDB)
        {
            DataSet tb = new DataSet();
            tb.Tables.Add(LINQToDataTable(_db.CTCTDBs.Where(item => item.MaCTCTDB == MaCTCTDB).ToList()));
            return tb;
        }

        [WebMethod]
        public DataSet GetCHDB(decimal MaCTCHDB)
        {
            DataSet tb = new DataSet();
            tb.Tables.Add(LINQToDataTable(_db.CTCHDBs.Where(item => item.MaCTCHDB == MaCTCHDB).ToList()));
            return tb;
        }

        public static DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
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