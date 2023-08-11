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
    public class TLMDController : Controller
    {
        private CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);

        // GET: TLMD
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThiCong()
        {
            return View();
        }

        public ActionResult updateThiCong()
        {
            return View();
        }

        public ActionResult DonViThiCong()
        {
            List<MDonViThiCong> lst = new List<MDonViThiCong>();
            DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_DonViThiCong");
            int STT = 1;
            foreach (DataRow item in dt.Rows)
            {
                MDonViThiCong en = new MDonViThiCong();
                en.STT = STT++;
                en.ID = int.Parse(item["ID"].ToString());
                en.Name = item["Name"].ToString();
                en.DaiDien = item["DaiDien"].ToString();
                en.DienThoai = item["DienThoai"].ToString();
                en.Username = item["Username"].ToString();
                en.Password = item["Password"].ToString();
                en.Active = bool.Parse(item["Active"].ToString());
                int a = -1;
                int.TryParse(item["CreateBy"].ToString(), out a);
                en.CreateBy = a;
                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                int.TryParse(item["ModifyBy"].ToString(), out a);
                en.ModifyBy = a;
                DateTime date;
                DateTime.TryParse(item["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                lst.Add(en);
            }
            return View(lst);
        }

        public ActionResult updateDonViThiCong(string Loai, int? ID, FormCollection form)
        {
            if (Loai == "insert")
            {
                _cDAL_TTKH.ExecuteNonQuery("insert into TLMD_DonViThiCong(ID,Name,DaiDien,DienThoai,Username,Password)values("
                    + "(select case when exists(select ID from TLMD_DonViThiCong) then (select MAX(ID)+1 from TLMD_DonViThiCong) else 1 end)"
                    + ",'" + form["inputName"].ToString() + "'"
                    + ",'" + form["inputDaiDien"].ToString() + "'"
                    + ",'" + form["inputDienThoai"].ToString() + "'"
                    + ",'" + form["inputUsername"].ToString() + "'"
                    + ",'" + form["inputPassword"].ToString() + "'"
                    + ")");
                return RedirectToAction("DonViThiCong");
            }
            else
            if (Loai == "viewUpdate")
            {
                MDonViThiCong en = new MDonViThiCong();
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_DonViThiCong where ID=" + ID);
                en.ID = int.Parse(dt.Rows[0]["ID"].ToString());
                en.Name = dt.Rows[0]["Name"].ToString();
                en.DaiDien = dt.Rows[0]["DaiDien"].ToString();
                en.DienThoai = dt.Rows[0]["DienThoai"].ToString();
                en.Username = dt.Rows[0]["Username"].ToString();
                en.Password = dt.Rows[0]["Password"].ToString();
                en.Active = bool.Parse(dt.Rows[0]["Active"].ToString());
                int a = -1;
                int.TryParse(dt.Rows[0]["CreateBy"].ToString(), out a);
                en.CreateBy = a;
                en.CreateDate = DateTime.Parse(dt.Rows[0]["CreateDate"].ToString());
                int.TryParse(dt.Rows[0]["ModifyBy"].ToString(), out a);
                en.ModifyBy = a;
                DateTime date;
                DateTime.TryParse(dt.Rows[0]["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                return View(en);
            }
            else
            if (Loai == "update")
            {
                string flag = "";
                if (form.AllKeys.Contains("inputActive"))
                    flag = "1";
                else
                    flag = "0";
                _cDAL_TTKH.ExecuteNonQuery("update TLMD_DonViThiCong set"
                    + " Name='" + form["inputName"].ToString() + "'"
                    + ",DaiDien='" + form["inputDaiDien"].ToString() + "'"
                    + ",DienThoai='" + form["inputDienThoai"].ToString() + "'"
                    + ",Username='" + form["inputUsername"].ToString() + "'"
                    + ",Password='" + form["inputPassword"].ToString() + "'"
                    + ",Active='" + flag + "'"
                    + ",ModifyBy=1"
                    + ",ModifyDate=getdate()"
                    + " where ID=" + form["inputID"].ToString());
                return RedirectToAction("DonViThiCong");
            }
            else
            if (Loai == "delete")
            {
                _cDAL_TTKH.ExecuteQuery_DataTable("delete TLMD_DonViThiCong where ID=" + ID);
                return RedirectToAction("DonViThiCong");
            }
            else
                return View();
        }

    }
}