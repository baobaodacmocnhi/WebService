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
        private CConnection _cDAL_BauCu = new CConnection(CGlobalVariable.BauCu);

        // GET: BauCu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BCH(string function, string BoPhieu, FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("Login", "Home");
            }
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dtUngVien = _cDAL_BauCu.ExecuteQuery_DataTable("select * from BCH_UngCu where BCH=1");
            foreach (DataRow item in dtUngVien.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.MLT = item["STT"].ToString();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["HoTen"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_UngCu = model;
            model = new List<ThongTinKhachHang>();
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY ID asc),ID,CreateDate=convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) from BCH_BoPhieu where BCH=1 and CreateBy="+ Session["ID"] + " order by ID desc");
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
                            sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0,"+ Session["ID"] + ")";
                        _cDAL_BauCu.ExecuteNonQuery(sql);
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
                                sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                            else
                                sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",1," + Session["ID"] + ")";
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                }
                else
            if (function == "Xoa")
                {
                    string sql = "delete BCH_BoPhieu_ChiTiet where IDBoPhieu=" + BoPhieu
                        + " delete BCH_BoPhieu where ID=" + BoPhieu;
                    _cDAL_BauCu.ExecuteNonQuery(sql);
                }
                return RedirectToAction("BCH", "BauCu");
            }
            return View();
        }

        public ActionResult BCH_View(string function)
        {
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select Tong=COUNT(*),HopLe=COUNT(NULLIF(1, KhongHopLe)),KhongHopLe=COUNT(NULLIF(0, KhongHopLe)) from BCH_BoPhieu where BCH=1");
            ViewBag.Tong = dt.Rows[0]["Tong"].ToString();
            ViewBag.HopLe = dt.Rows[0]["HopLe"].ToString();
            ViewBag.KhongHopLe = dt.Rows[0]["KhongHopLe"].ToString();
            dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY t1.ID asc),t1.* from (select uc.ID,uc.HoTen,Chon=COUNT(NULLIF(0, Chon)) from BCH_BoPhieu bp,BCH_BoPhieu_ChiTiet bpct,BCH_UngCu uc"
              + " where bp.ID=bpct.IDBoPhieu and uc.ID=bpct.IDUngVien and bp.BCH=1"
              + " group by uc.ID,uc.HoTen)t1");
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
            if (function != null)
            {
                if (function == "Sort")
                {
                    List<ThongTinKhachHang> SortedList = model.OrderByDescending(o => o.DiaChi).ThenBy(o=>o.MLT).ToList();
                    for (int i = 0; i < SortedList.Count; i++)
                    {
                        SortedList[i].MLT = (i + 1).ToString();
                    }
                    ViewBag.BCH_BoPhieu = SortedList;
                }
            }
            else
            {
                ViewBag.BCH_BoPhieu = model;
            }
           
            return View();
        }

        public ActionResult BiThu(string function, string BoPhieu, FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("Login", "Home");
            }
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dtUngVien = _cDAL_BauCu.ExecuteQuery_DataTable("select * from BCH_UngCu where BiThu=1");
            foreach (DataRow item in dtUngVien.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.MLT = item["STT"].ToString();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["HoTen"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_UngCu = model;
            model = new List<ThongTinKhachHang>();
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY ID asc),ID,CreateDate=convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) from BCH_BoPhieu where BiThu=1 and CreateBy="+ Session["ID"] + " order by ID desc");
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
                                  + " insert into BCH_BoPhieu(KhongHopLe,BiThu,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(1,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        foreach (DataRow item in dtUngVien.Rows)
                            sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                    else
                    if (form.AllKeys.Count() > 1)
                    {
                        string sql = "declare @tbID table (id int);"
                                  + " insert into BCH_BoPhieu(KhongHopLe,BiThu,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(0,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        var YourRadioButton = Request.Form["1"];
                        foreach (DataRow item in dtUngVien.Rows)
                            if (form.AllKeys.Contains(item["ID"].ToString()))
                            {
                                string value = Request.Form[item["ID"].ToString()];
                                if (value.Equals("Yes"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",1," + Session["ID"] + ")";
                                else
                                    if (value.Equals("No"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                            }
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                }
                else
            if (function == "Xoa")
                {
                    string sql = "delete BCH_BoPhieu_ChiTiet where IDBoPhieu=" + BoPhieu
                        + " delete BCH_BoPhieu where ID=" + BoPhieu;
                    _cDAL_BauCu.ExecuteNonQuery(sql);
                }
                return RedirectToAction("BiThu", "BauCu");
            }
            return View();
        }

        public ActionResult BiThu_View()
        {
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select Tong=COUNT(*),HopLe=COUNT(NULLIF(1, KhongHopLe)),KhongHopLe=COUNT(NULLIF(0, KhongHopLe)) from BCH_BoPhieu where BiThu=1");
            ViewBag.Tong = dt.Rows[0]["Tong"].ToString();
            ViewBag.HopLe = dt.Rows[0]["HopLe"].ToString();
            ViewBag.KhongHopLe = dt.Rows[0]["KhongHopLe"].ToString();
            dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY t1.ID asc),t1.* from (select uc.ID,uc.HoTen,Chon=COUNT(NULLIF(0, Chon)) from BCH_BoPhieu bp,BCH_BoPhieu_ChiTiet bpct,BCH_UngCu uc"
                + " where bp.ID=bpct.IDBoPhieu and uc.ID=bpct.IDUngVien and bp.BiThu=1"
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

        public ActionResult DaiBieuChinhThuc(string function, string BoPhieu, FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("Login", "Home");
            }
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dtUngVien = _cDAL_BauCu.ExecuteQuery_DataTable("select * from BCH_UngCu where DaiBieuChinhThuc=1");
            foreach (DataRow item in dtUngVien.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["HoTen"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_UngCu = model;
            model = new List<ThongTinKhachHang>();
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY ID asc),ID,CreateDate=convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) from BCH_BoPhieu where DaiBieuChinhThuc=1 and CreateBy="+ Session["ID"] + " order by ID desc");
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
                                  + " insert into BCH_BoPhieu(KhongHopLe,DaiBieuChinhThuc,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(1,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        foreach (DataRow item in dtUngVien.Rows)
                            sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                    else
                    if (form.AllKeys.Count() > 1)
                    {
                        string sql = "declare @tbID table (id int);"
                                  + " insert into BCH_BoPhieu(KhongHopLe,DaiBieuChinhThuc,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(0,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        var YourRadioButton = Request.Form["1"];
                        foreach (DataRow item in dtUngVien.Rows)
                            if (form.AllKeys.Contains(item["ID"].ToString()))
                            {
                                string value = Request.Form[item["ID"].ToString()];
                                if (value.Equals("Yes"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",1," + Session["ID"] + ")";
                                else
                                    if (value.Equals("No"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                            }
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                }
                else
            if (function == "Xoa")
                {
                    string sql = "delete BCH_BoPhieu_ChiTiet where IDBoPhieu=" + BoPhieu
                        + " delete BCH_BoPhieu where ID=" + BoPhieu;
                    _cDAL_BauCu.ExecuteNonQuery(sql);
                }
                return RedirectToAction("DaiBieuChinhThuc", "BauCu");
            }
            return View();
        }

        public ActionResult DaiBieuChinhThuc_View()
        {
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select Tong=COUNT(*),HopLe=COUNT(NULLIF(1, KhongHopLe)),KhongHopLe=COUNT(NULLIF(0, KhongHopLe)) from BCH_BoPhieu where DaiBieuChinhThuc=1");
            ViewBag.Tong = dt.Rows[0]["Tong"].ToString();
            ViewBag.HopLe = dt.Rows[0]["HopLe"].ToString();
            ViewBag.KhongHopLe = dt.Rows[0]["KhongHopLe"].ToString();
            dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY t1.ID asc),t1.* from (select uc.ID,uc.HoTen,Chon=COUNT(NULLIF(0, Chon)) from BCH_BoPhieu bp,BCH_BoPhieu_ChiTiet bpct,BCH_UngCu uc"
                + " where bp.ID=bpct.IDBoPhieu and uc.ID=bpct.IDUngVien and bp.DaiBieuChinhThuc=1"
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

        public ActionResult DaiBieuDuKhuyet(string function, string BoPhieu, FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("Login", "Home");
            }
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            DataTable dtUngVien = _cDAL_BauCu.ExecuteQuery_DataTable("select * from BCH_UngCu where DaiBieuDuKhuyet=1");
            foreach (DataRow item in dtUngVien.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.DanhBo = item["ID"].ToString();
                en.HoTen = item["HoTen"].ToString();
                model.Add(en);
            }
            ViewBag.BCH_UngCu = model;
            model = new List<ThongTinKhachHang>();
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY ID asc),ID,CreateDate=convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) from BCH_BoPhieu where DaiBieuDuKhuyet=1 and CreateBy="+ Session["ID"] + " order by ID desc");
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
                                  + " insert into BCH_BoPhieu(KhongHopLe,DaiBieuDuKhuyet,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(1,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        foreach (DataRow item in dtUngVien.Rows)
                            sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                    else
                    if (form.AllKeys.Count() > 1)
                    {
                        string sql = "declare @tbID table (id int);"
                                  + " insert into BCH_BoPhieu(KhongHopLe,DaiBieuDuKhuyet,CreateBy)"
                                  + " output inserted.ID into @tbID"
                                  + " values(0,1,0);"
                                  + " declare @ID int"
                                  + " select @ID=id from @tbID;";
                        var YourRadioButton = Request.Form["1"];
                        foreach (DataRow item in dtUngVien.Rows)
                            if (form.AllKeys.Contains(item["ID"].ToString()))
                            {
                                string value = Request.Form[item["ID"].ToString()];
                                if (value.Equals("Yes"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",1," + Session["ID"] + ")";
                                else
                                    if (value.Equals("No"))
                                    sql += " insert into BCH_BoPhieu_ChiTiet(IDBoPhieu,IDUngVien,Chon,CreateBy)values(@ID," + item["ID"].ToString() + ",0," + Session["ID"] + ")";
                            }
                        _cDAL_BauCu.ExecuteNonQuery(sql);
                    }
                }
                else
            if (function == "Xoa")
                {
                    string sql = "delete BCH_BoPhieu_ChiTiet where IDBoPhieu=" + BoPhieu
                        + " delete BCH_BoPhieu where ID=" + BoPhieu;
                    _cDAL_BauCu.ExecuteNonQuery(sql);
                }
                return RedirectToAction("DaiBieuDuKhuyet", "BauCu");
            }
            return View();
        }

        public ActionResult DaiBieuDuKhuyet_View()
        {
            DataTable dt = _cDAL_BauCu.ExecuteQuery_DataTable("select Tong=COUNT(*),HopLe=COUNT(NULLIF(1, KhongHopLe)),KhongHopLe=COUNT(NULLIF(0, KhongHopLe)) from BCH_BoPhieu where DaiBieuDuKhuyet=1");
            ViewBag.Tong = dt.Rows[0]["Tong"].ToString();
            ViewBag.HopLe = dt.Rows[0]["HopLe"].ToString();
            ViewBag.KhongHopLe = dt.Rows[0]["KhongHopLe"].ToString();
            dt = _cDAL_BauCu.ExecuteQuery_DataTable("select STT=ROW_NUMBER() OVER(ORDER BY t1.ID asc),t1.* from (select uc.ID,uc.HoTen,Chon=COUNT(NULLIF(0, Chon)) from BCH_BoPhieu bp,BCH_BoPhieu_ChiTiet bpct,BCH_UngCu uc"
                + " where bp.ID=bpct.IDBoPhieu and uc.ID=bpct.IDUngVien and bp.DaiBieuDuKhuyet=1"
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

    }
}