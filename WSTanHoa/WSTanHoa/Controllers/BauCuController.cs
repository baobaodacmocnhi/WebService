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
            return View();
        }

        public ActionResult BiThu()
        {
            return View();
        }

        public ActionResult DaiBieu()
        {
            return View();
        }

    }
}