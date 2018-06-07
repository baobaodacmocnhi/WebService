using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace WSViettel
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        CConnection cDAL = new CConnection();

        private bool checkName(string name)
        {
            if (name == "FmXzcUCBB4iud6zKohiDZLeXLP5HnMNp8VSJ0Knm5MaPqmSpbt2n3VZOdEn8LZZYLT8581ME/e/AlUplsu4RZbe0tviAIVoavOFN/pqTTYQ1dFGUE7QLzhVG9Tp4htmTeYfu374wO//ViwMUJ9p1CprI61D4of2o12IuDplsusplsufIplsu2AY=")
            {
                name = "VIETTEL";
            }
            if (name.ToUpper() == "VIETTEL")
            {
                return true;
            }
            else
            { return false; }
        }

        private string getName(string name)
        {
            if (name == "FmXzcUCBB4iud6zKohiDZLeXLP5HnMNp8VSJ0Knm5MaPqmSpbt2n3VZOdEn8LZZYLT8581ME/e/AlUplsu4RZbe0tviAIVoavOFN/pqTTYQ1dFGUE7QLzhVG9Tp4htmTeYfu374wO//ViwMUJ9p1CprI61D4of2o12IuDplsusplsufIplsu2AY=")
            {
                return "VIETTEL";
            }
            return name;
        }

        [WebMethod]
        public DataSet getW_Bill_VT(string danhba, string ten, string matkhau)
        {
            DataSet bangNo = new DataSet();
            try
            {
                if (this.checkName(ten))
                {
                    //bangNo.Tables.Add(cDAL.ExecuteQuery_SqlDataAdapter_DataTable("SELECT ID_HOADON AS IDKey,DANHBA AS DBo,TENKH AS KHang,SO AS DChi1,DUONG AS DChi2,GB AS GBieu,DM AS DMuc,KY AS KyHD,NAM AS NamHD,PHI AS PBVMT,THUE AS TGTGT,TIEUTHU AS TThu,SOHOADON AS SHDon,TONGCONG FROM HoaDon WHERE (NGAYGIAITRACH IS NULL) AND (DANHBA = '" + danhba + "') and SOHOADON not in (select SoHoaDon from TT_DichVuThu) ORDER BY NAM, Ky"));
                    bangNo = cDAL.ExecuteQuery_SqlDataAdapter_DataSet("SELECT ID_HOADON AS IDKey,DANHBA AS DBo,TENKH AS KHang,SO AS DChi1,DUONG AS DChi2,GB AS GBieu,DM AS DMuc,KY AS KyHD,NAM AS NamHD,PHI AS PBVMT,THUE AS TGTGT,TIEUTHU AS TThu,SOHOADON AS SHDon,TONGCONG FROM HoaDon WHERE (NGAYGIAITRACH IS NULL) AND (DANHBA = '" + danhba + "') and SOHOADON not in (select SoHoaDon from TT_DichVuThu) ORDER BY NAM, Ky");
                }

                if (bangNo.Tables[0].Rows.Count < 1)
                {
                    bangNo.Tables.Remove(bangNo.Tables[0]);
                    //bangNo.Tables.Add(cDAL.ExecuteQuery_SqlDataAdapter_DataTable("SELECT TOP(1) ID_HOADON AS IDKey,DANHBA AS DBo,TENKH AS KHang,SO AS DChi1,DUONG AS DChi2,GB AS GBieu,DM AS  DMuc,0 AS KyHD,0 AS NamHD,0 AS PBVMT,0 AS TGTGT,0 AS TThu,'' AS SHDon,0 AS TONGCONG FROM HoaDon WHERE (DANHBA = '" + danhba + "') ORDER BY IDKey DESC"));
                    bangNo = cDAL.ExecuteQuery_SqlDataAdapter_DataSet("SELECT TOP(1) ID_HOADON AS IDKey,DANHBA AS DBo,TENKH AS KHang,SO AS DChi1,DUONG AS DChi2,GB AS GBieu,DM AS  DMuc,0 AS KyHD,0 AS NamHD,0 AS PBVMT,0 AS TGTGT,0 AS TThu,'' AS SHDon,0 AS TONGCONG FROM HoaDon WHERE (DANHBA = '" + danhba + "') ORDER BY IDKey DESC");
                }
            }
            catch (Exception) { }
            return bangNo;
        }

        [WebMethod]
        public bool payW_Bill_VT(string id, string ma_giaodich, string ten, string matkhau)
        {
            bool f = false;
            try
            {
                if (this.checkName(ten))
                {
                    string[] s = id.Trim().Split('#');
                    if (s.Length > 0)
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            try
                            {
                                string sql = "insert into TT_DichVuThu(MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,Phi,TienDu,TenDichVu,TenNganHang,ID_GiaoDich,CreateDate)"
                                    +"(select ID_HOADON,SoHoaDon,DanhBa,Nam,Ky,TongCong,'','','VIETTEL','VIETTEL','"+ma_giaodich+"',getdate() from HOADON where ID_HOADON="+s[i]+")";
                                cDAL.ExecuteNonQuery(sql);
                            }
                            catch (Exception)
                            { return false; }
                        }
                        try
                        {
                            f = true;
                        }
                        catch (Exception) { f = false; }
                    }
                }
                else
                {
                    return f;
                }
            }
            catch (Exception) { }
            return f;
        }

        [WebMethod]
        public DataSet checkPay_Bill_VT(string ma_gd, string ten, string matkhau)
        {
            DataSet bangNo = new DataSet();
            try
            {
                if (this.checkName(ten))
                {
                    string sql = "SELECT MaHD AS IDKey,DANHBA as DBo,TENKH as KHang,SO as DChi1,DUONG as DChi2,GB as GBieu,DM as DMuc,hd.KY as KyHD,hd.NAM as NamHD,hd.PHI as PBVMT,hd.THUE as TGTGT,TONGCONG,"
                            + "TIEUTHU as TThu,hd.SOHOADON as SHDon,dvt.CreateDate AS NGAYTHANHTOAN,TenDichVu as MANH "
                            + " FROM TT_DichVuThu dvt,HOADON hd WHERE (ID_GiaoDich = '" + ma_gd + "') and hd.ID_HOADON=dvt.MaHD";
                    //bangNo.Tables.Add(cDAL.ExecuteQuery_SqlDataAdapter_DataTable(sql));
                    bangNo = cDAL.ExecuteQuery_SqlDataAdapter_DataSet(sql);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception) { }
            return bangNo;
        }

        [WebMethod]
        public DataSet getCustomerInfo(string danhba, string ten, string matkhau)
        {
            DataSet bangNo = new DataSet();
            try
            {
                if (this.checkName(ten))
                {
                    //bangNo.Tables.Add(cDAL.ExecuteQuery_SqlDataAdapter_DataTable("SELECT TOP (1) DANHBA AS DBO,TENKH AS HOTEN,SO AS DC1,DUONG AS DC2,'' AS SDT FROM HOADON WHERE (DANHBA = '+" + danhba + "') ORDER BY ID_HOADON DESC"));
                    bangNo = cDAL.ExecuteQuery_SqlDataAdapter_DataSet("SELECT TOP (1) DANHBA AS DBO,TENKH AS HOTEN,SO AS DC1,DUONG AS DC2,'' AS SDT FROM HOADON WHERE (DANHBA = '" + danhba + "') ORDER BY ID_HOADON DESC");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception) { }
            return bangNo;
        }    

    }
}
