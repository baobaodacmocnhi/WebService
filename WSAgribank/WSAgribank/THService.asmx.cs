using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Globalization;
using System.Transactions;

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
            string sql = "SELECT TOP(1) hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG  "
            + " FROM HOADON hd WHERE  hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' "
            + " ORDER BY NAM DESC, KY DESC";

            return _cDAL.ExecuteQuery_DataSet(sql);
        }

        [WebMethod]
        public DataSet W_Bill(string db, string ten, string matkhau)
        {
            string sql = "SELECT ID_HOADON AS IDkey, hd.DANHBA, hd.TENKH, SO as SONHA,DUONG as TENDUONG,hd.GB,hd.DM, hd.DOT, hd.KY as KyHD, hd.NAM as NamHD, hd.PHI as PBVMT, hd.THUE as TGTGT,hd.GIABAN as TNuoc,hd.TONGCONG as TONGCONG  "
            + " FROM HOADON hd "
            + " WHERE NGAYGIAITRACH IS NULL AND hd.DANHBA='" + db + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' "
            + " and ID_HOADON not in (select MaHD from TT_DichVuThu) "
            + " and (GB=10 and (NAM>2021 or (NAM=2021 and Ky<6)) or (GB!=10))"
            + " and ID_HOADON not in (select MaHD from TT_TraGop)"
            + " and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where CodeF2=1 and NGAYGIAITRACH is null and ID_HOADON=FK_HOADON)"
            + " and ID_HOADON not in (select FK_HOADON from DIEUCHINH_HD,HOADON where NGAYGIAITRACH is null and ID_HOADON=FK_HOADON and UpdatedHDDT=0)"
            + " and DANHBA not in (select DanhBo from server11.KTKS_DonKH.dbo.DonTu_ChiTiet dtct where ChanHoaDon=1 and not exists(select ID from server11.KTKS_DonKH.dbo.DonTu_LichSu where MaDon=dtct.MaDon and (ID_NoiNhan=20 or (ID_NoiChuyen=6 and IDCT is not null))))"
            + " ORDER BY NAM DESC, KY DESC ";
            DataSet ds = _cDAL.ExecuteQuery_DataSet(sql);
            int PhiMoNuoc = (int)_cDAL.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + db + ")");
            if (ds.Tables[0].Rows.Count > 0 && PhiMoNuoc > 0)
            {
                ds.Tables[0].Rows[0]["TONGCONG"] = int.Parse(ds.Tables[0].Rows[0]["TONGCONG"].ToString()) + PhiMoNuoc;
            }
            ds.AcceptChanges();
            return ds;
        }

        [WebMethod]
        public bool payW_Bill(string id, string ten, string matkhau)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    string sql = " INSERT INTO Agribank_THUTAM (ID_HOADON, DANHBO, KHACHHANG, SONHA, TENDUONG, GB, DM, DOT, KYHD, NAMHD, GIABAN, THUE, PBVMT, TONGCONG, SOHOADON, MAGIAODICH, NGAYTHANHTOAN, GACHNO)"
                    + " SELECT ID_HOADON , hd.DANHBA, hd.TENKH as KHACHHANG, SO as SONHA,DUONG as TENDUONG,hd.GB,hd.DM, hd.DOT, hd.KY as KYHD, hd.NAM as NAMHD,hd.GIABAN,hd.THUE, hd.PHI as PBVMT, hd.TONGCONG,hd.SOHOADON, '" + ten + "' AS MAGIAODICH,GETDATE() as  NGAYTHANHTOAN, '1' as GACHNO "
                    + " FROM HOADON hd"
                    + " WHERE  hd.ID_HOADON IN (" + id.Replace("#", ",") + ") AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";

                    //string sql2 = "insert into TT_DichVuThu(SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,TenNganHang,CreateDate) "
                    //+ " SELECT SOHOADON AS  SoHoaDon , hd.DANHBA AS DanhBo, hd.NAM as Nam,  hd.KY as Ky,hd.TONGCONG AS SoTien ,'AGRIBANK' AS TenDichVu,'AGRIBANK' AS TenNganHang, GETDATE() as CreateDate "
                    //+ " FROM HOADON hd "
                    //+ " WHERE  hd.ID_HOADON IN (" + id.Replace("#", ",") + ") AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' ";

                    if (_cDAL.ExecuteNonQuery(sql) == true)
                    {
                        int ID = (int)_cDAL.ExecuteQuery_ReturnOneValue("select MAX(ID)+1 from TT_DichVuThuTong");

                        string[] arrayMaHD = id.Split('#');
                        string MaHDs = "", SoHoaDons = "", Kys = "", sql_ChiTiet = "", DanhBo = "";
                        int TongCong = 0;
                        for (int i = 0; i < arrayMaHD.Length; i++)
                        {
                            DataTable dt = _cDAL.ExecuteQuery_DataTable("select MaHD=ID_HOADON,SOHOADON,DanhBo=DANHBA,NAM,KY,GIABAN,ThueGTGT=THUE,PhiBVMT=PHI,TONGCONG from HOADON where ID_HOADON=" + arrayMaHD[i]);
                            sql_ChiTiet += "insert into TT_DichVuThu(MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,IDDichVu,IDGiaoDich,CreateDate)"
                                + " values(" + dt.Rows[0]["MaHD"] + ",'" + dt.Rows[0]["SoHoaDon"] + "','" + dt.Rows[0]["DanhBo"] + "'," + dt.Rows[0]["Nam"] + "," + dt.Rows[0]["Ky"] + "," + dt.Rows[0]["TongCong"] + ",N'AGRIBANK'," + ID + ",'" + "AGRIBANK" + ID + "',getdate()) ";
                            //_cDAL.ExecuteNonQuery_Transaction(sql);
                            if (string.IsNullOrEmpty(SoHoaDons) == true)
                            {
                                MaHDs = dt.Rows[0]["MaHD"].ToString();
                                SoHoaDons = dt.Rows[0]["SoHoaDon"].ToString();
                                Kys = dt.Rows[0]["Ky"].ToString() + "/" + dt.Rows[0]["Nam"].ToString();
                                TongCong += int.Parse(dt.Rows[0]["TongCong"].ToString());
                            }
                            else
                            {
                                MaHDs += "," + dt.Rows[0]["MaHD"];
                                SoHoaDons += "," + dt.Rows[0]["SoHoaDon"];
                                Kys += "," + dt.Rows[0]["Ky"].ToString() + "/" + dt.Rows[0]["Nam"].ToString();
                                TongCong += int.Parse(dt.Rows[0]["TongCong"].ToString());
                            }
                            DanhBo = dt.Rows[0]["DanhBo"].ToString();
                        }
                        int PhiMoNuoc = (int)_cDAL.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + DanhBo + ")");
                        int TongThu = TongCong + PhiMoNuoc;
                        string sql_Tong = "insert into TT_DichVuThuTong(ID,DanhBo,MaHDs,SoHoaDons,Kys,SoTien,PhiMoNuoc,TienDu,TongCong,TenDichVu,IDGiaoDich,CreateDate)"
                                    + " values(" + ID + ",'" + DanhBo + "','" + MaHDs + "','" + SoHoaDons + "','" + Kys + "'," + TongCong + "," + PhiMoNuoc + ",0," + TongThu + ",N'AGRIBANK','" + "AGRIBANK" + ID + "',getdate())";
                        _cDAL.ExecuteNonQuery(sql_Tong);
                        _cDAL.ExecuteNonQuery(sql_ChiTiet);
                        ts.Complete();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        [WebMethod]
        public DataSet checkPay_Bill(string id, string ten, string matkhau)
        {

            string sql = " SELECT ID_HOADON AS IDkey, DANHBO, KHACHHANG, SONHA, TENDUONG, GB, DM, DOT, KYHD, NAMHD, GIABAN, THUE, PBVMT, TONGCONG "
            + " FROM Agribank_THUTAM hd "
            + " WHERE GACHNO ='1' AND hd.ID_HOADON='" + id + "' AND 'AGRIBANK'= '" + ten + "' AND DAY(GETDATE())='" + matkhau + "' "
            + " ORDER BY NAM DESC, KY DESC ";

            return _cDAL.ExecuteQuery_DataSet(sql);
        }

        [WebMethod]
        public bool Check_PhiDN(string DanhBo, out int Phi)
        {
            Phi = 0;
            try
            {
                string sqlPhiMoNuoc = "select PhiMoNuoc from TT_KQDongNuoc kqdn where DanhBo='" + DanhBo + "' and kqdn.DongNuoc=1 and kqdn.MoNuoc=0 and kqdn.TroNgaiMN=0 ";
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
