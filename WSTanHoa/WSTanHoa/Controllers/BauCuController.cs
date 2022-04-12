using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class BauCuController : Controller
    {
        private CConnection cDAL_BauCu = new CConnection(CGlobalVariable.BauCu);

        // GET: BauCu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BCH(string function, string BoPhieu, FormCollection form)
        {
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dtUngVien = cDAL_BauCu.ExecuteQuery_DataTable("select * from BCH_UngCu where BCH=1");
            foreach (DataRow item in dtUngVien.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["HoTen"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_UngCu = model;
            model = new List<ThongTinKhachHang>();
            DataTable dt = cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY ID asc),ID,CreateDate=convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) from BCH_BoPhieu where BCH=1 and CreateBy=0 order by ID desc");
            foreach (DataRow item in dt.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.MLT = item["STT"].ToString();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["CreateDate"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_BoPhieu = model;
            if (function != null)
            {
                if (function == "Gui")
                {
                    if (form.AllKeys.Contains("0"))
                    {
                        string sql = "declare @tbID table (id int);"
                                  + " insert into BCH_BoPhieu(KhongHopLe,BCH,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(1,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        foreach (DataRow item in dtUngVien.Rows)
                            sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0,0)";
                        cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                    else
                    if (form.AllKeys.Count() > 1)
                    {
                        string sql = "declare @tbID table (id int);"
                                  + " insert into BCH_BoPhieu(KhongHopLe,BCH,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(0,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        foreach (DataRow item in dtUngVien.Rows)
                            if (form.AllKeys.Contains(item["ID"].ToString()))
                                sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0,0)";
                            else
                                sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",1,0)";
                        cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                }
                else
            if (function == "Xoa")
                {
                    string sql = "delete BCH_BoPhieu_ChiTiet where IDBoPhieu=" + BoPhieu
                        + " delete BCH_BoPhieu where ID=" + BoPhieu;
                    cDAL_BauCu.ExecuteNonQuery(sql);
                }
                return RedirectToAction("BCH", "BauCu");
            }
            return View();
        }

        public ActionResult BCH_View()
        {
            DataTable dt = cDAL_BauCu.ExecuteQuery_DataTable("select Tong=COUNT(*),HopLe=COUNT(NULLIF(1, KhongHopLe)),KhongHopLe=COUNT(NULLIF(0, KhongHopLe)) from BCH_BoPhieu where BCH=1");
            ViewBag.Tong = dt.Rows[0]["Tong"].ToString();
            ViewBag.HopLe = dt.Rows[0]["HopLe"].ToString();
            ViewBag.KhongHopLe = dt.Rows[0]["KhongHopLe"].ToString();
            dt = cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY t1.ID asc),t1.* from (select uc.ID,uc.HoTen,Chon=COUNT(NULLIF(0, Chon)) from BCH_BoPhieu bp,BCH_BoPhieu_ChiTiet bpct,BCH_UngCu uc"
                + " where bp.ID=bpct.IDBoPhieu and uc.ID=bpct.IDUngVien and bp.BCH=1"
                + " group by uc.ID,uc.HoTen)t1");
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            foreach (DataRow item in dt.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.MLT = item["STT"].ToString();
                en.HoTen = item["HoTen"].ToString();
                en.DiaChi = item["Chon"].ToString();
                en.DienThoai = ((double)(double.Parse(item["Chon"].ToString()) / double.Parse(ViewBag.Tong) * 100)).ToString("0.00") + " %";
                en.DinhMuc = ((int)(int.Parse(ViewBag.Tong) - int.Parse(item["Chon"].ToString()))).ToString();
                en.DinhMucHN = ((double)((double.Parse(ViewBag.Tong) - double.Parse(item["Chon"].ToString())) / double.Parse(ViewBag.Tong) * 100)).ToString("0.00") + " %";
                model.Add(en);
            }
            ViewBag.BCH_BoPhieu = model;
            return View();
        }

        public ActionResult BiThu()
        {
            return View();
        }

        public ActionResult BiThu_View()
        {
            return View();
        }

        public ActionResult DaiBieu()
        {
            return View();
        }

        public ActionResult DaiBieu_View()
        {
            return View();
        }

    }
}