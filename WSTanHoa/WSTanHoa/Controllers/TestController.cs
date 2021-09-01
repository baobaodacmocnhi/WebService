using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models.db;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class TestController : Controller
    {
        private CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTienWFH);

        // GET: Test
        public ActionResult Index(string action, string radLoai, string TuNgay, string DenNgay)
        {
            //if (Session["ID"] == null)
            //{
            //    Session["Url"] = Request.Url;
            //    return RedirectToAction("Login", "Home");
            //}
            ZaloView zv = new ZaloView();
            if (action == "Xem" && TuNgay != "" && DenNgay != "")
            {
                DateTime dateTu = DateTime.Parse(TuNgay), dateDen = DateTime.Parse(DenNgay);
                zv.TuNgay = TuNgay;
                zv.DenNgay = DenNgay;
                string sql;
                DataTable dt;
                int rowCount = 11, SoLuong = 0;
                decimal TongCong = 0;
                if (radLoai == "radNgayThu")
                {
                    sql = "select TenDichVu,SoLuong=COUNT(MaHD),TongCong=SUM(CAST(SoTien as decimal(12,0))) from TT_DichVuThu"
                           + " where CAST(CreateDate as date)>='" + dateTu.ToString("yyyyMMdd") + "' and CAST(CreateDate as date)<='" + dateDen.ToString("yyyyMMdd") + "'"
                           + " group by TenDichVu"
                           + " order by TenDichVu";
                    dt = cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    DataRow[] dr;
                    for (int i = 0; i < rowCount; i++)
                    {
                        ZaloView en = new ZaloView();
                        switch (i)
                        {
                            case 0:
                                en.Image = "acb.png";
                                break;
                            case 1:
                                en.Image = "agr.png";
                                dr = dt.Select("TenDichVu='AGRIBANK'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            case 2:
                                en.Image = "da.png";
                                break;
                            case 3:
                                en.Image = "kb.png";
                                break;
                            case 4:
                                en.Image = "mb.png";
                                dr = dt.Select("TenDichVu='MB' or TenDichVu='VIETTEL'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            case 5:
                                en.Image = "momo.png";
                                dr = dt.Select("TenDichVu='MOMO'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            case 6:
                                en.Image = "payoo.png";
                                dr = dt.Select("TenDichVu='PAYOO'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            case 7:
                                en.Image = "shopeepay.png";
                                dr = dt.Select("TenDichVu='AIRPAY'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            case 8:
                                en.Image = "vcb.png";
                                break;
                            case 9:
                                en.Image = "vnpay.svg";
                                dr = dt.Select("TenDichVu='VNPAY'");
                                if (dr != null && dr.Count() > 0)
                                {
                                    en.SoLuong = int.Parse(dr[0]["SoLuong"].ToString());
                                    en.TongCong = decimal.Parse(dr[0]["TongCong"].ToString());
                                }
                                break;
                            default:
                                break;
                        }
                        zv.SoLuong += en.SoLuong;
                        zv.TongCong += en.TongCong;
                        zv.lst.Add(en);
                    }
                }
                else
                    if (radLoai == "radNgayGiaiTrach")
                {
                    sql = "select Bank = bank.NGANHANG, giaitrach.SoLuong,giaitrach.TongCong from NGANHANG bank left join"
                           + " (select MaNH,SoLuong = COUNT(ID_HOADON),TongCong = SUM(TongCong) from HOADON hd"
                           + " left join TAMTHU tt on tt.FK_HOADON = hd.ID_HOADON"
                           + " where CAST(NGAYGIAITRACH as date)>='" + dateTu.ToString("yyyyMMdd") + "' and CAST(NGAYGIAITRACH as date)<='" + dateDen.ToString("yyyyMMdd") + "'"
                           + " and MaNH is not null"
                           + " group by MaNH) giaitrach on bank.ID_NGANHANG = giaitrach.MaNH"
                           + " where bank.An = 0"
                           + " union"
                           + " select Bank='zzz',SoLuong = COUNT(ID_HOADON),TongCong = SUM(TongCong) from HOADON hd"
                           + " left join TAMTHU tt on tt.FK_HOADON = hd.ID_HOADON"
                           + " where CAST(NGAYGIAITRACH as date)>='20210601' and CAST(NGAYGIAITRACH as date)<='20210831' and MaNH is null"
                           + " group by MaNH"
                           + " order by bank.NGANHANG";
                    dt = cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                    for (int i = 0; i < rowCount; i++)
                    {
                        ZaloView en = new ZaloView();
                        switch (i)
                        {
                            case 0:
                                en.Image = "acb.png";
                                break;
                            case 1:
                                en.Image = "agr.png";
                                break;
                            case 2:
                                en.Image = "da.png";
                                break;
                            case 3:
                                en.Image = "kb.png";
                                break;
                            case 4:
                                en.Image = "mb.png";
                                break;
                            case 5:
                                en.Image = "momo.png";
                                break;
                            case 6:
                                en.Image = "payoo.png";
                                break;
                            case 7:
                                en.Image = "shopeepay.png";
                                break;
                            case 8:
                                en.Image = "vcb.png";
                                break;
                            case 9:
                                en.Image = "vnpay.svg";
                                break;
                            default:
                                break;
                        }
                        if (dt.Rows[i]["SoLuong"].ToString() != "")
                        {
                            en.SoLuong = int.Parse(dt.Rows[i]["SoLuong"].ToString());
                            SoLuong += int.Parse(dt.Rows[i]["SoLuong"].ToString());
                        }
                        if (dt.Rows[i]["TongCong"].ToString() != "")
                        {
                            en.TongCong = decimal.Parse(dt.Rows[i]["TongCong"].ToString());
                            TongCong += int.Parse(dt.Rows[i]["TongCong"].ToString());
                        }
                        zv.SoLuong += SoLuong;
                        zv.TongCong += TongCong;
                        zv.lst.Add(en);
                    }
                }

            }

            return View(zv);
        }
    }
}