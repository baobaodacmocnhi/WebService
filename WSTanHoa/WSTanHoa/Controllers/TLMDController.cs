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

        public ActionResult DangNhap(FormCollection form)
        {
            if (form.AllKeys.Contains("username"))
            {
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name from TLMD_User where Active=1 and Username='" + form["username"].ToString().Trim() + "' and Password='" + form["password"].ToString().Trim() + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    Session["ID"] = dt.Rows[0]["ID"].ToString();
                    Session["Name"] = dt.Rows[0]["Name"].ToString();
                    return Redirect(Session["Url"].ToString());
                }
            }
            return View();
        }

        public ActionResult DoiMatKhau(FormCollection form)
        {
            if (form.AllKeys.Contains("passwordCu"))
            {
                if (_cDAL_TTKH.ExecuteNonQuery("update TLMD_User set Password='" + form["passwordMoi"].ToString().Trim() + "' where ID=" + Session["ID"] + " and Password='" + form["passwordCu"].ToString().Trim() + "'") == true)
                    ModelState.AddModelError("", "Thành công");
                else
                    ModelState.AddModelError("", "Thất bại");
            }
            return View();
        }

        public string getTTKH(string DanhBo)
        {
            try
            {
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select SoNha,TenDuong from [CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] where DanhBo='" + DanhBo + "'");
                MView en = new MView();
                en.SoNha = dt.Rows[0]["SoNha"].ToString();
                en.TenDuong = dt.Rows[0]["TenDuong"].ToString();
                return CGlobalVariable.jsSerializer.Serialize(en);
            }
            catch
            {
                return null;
            }
        }

        public ActionResult ThiCong()
        {
            //if (Session["ID"] == null)
            //{
            //    Session["Url"] = Request.Url;
            //    return RedirectToAction("DangNhap");
            //}
            if
            return View();
        }

        public ActionResult updateThiCong(string Loai, int? ID, FormCollection form)
        {
            ViewBag.KetCau = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name from TLMD_KetCau"), "ID", "Name");
            ViewBag.DonViThiCong = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name from TLMD_DonViThiCong"), "ID", "Name");
            if (Loai == "insert")
            {
                _cDAL_TTKH.ExecuteNonQuery("insert into TLMD_ThiCong(ID,Name,IDDonViThiCong,IDKetCau,DanhBo,DiemDau,DiemCuoi,TenDuong)values("
                    + "(select case when exists(select ID from TLMD_ThiCong) then (select MAX(ID)+1 from TLMD_ThiCong) else 1 end)"
                    + ",'" + form["inputName"].ToString() + "'"
                    + "," + form["IDDonViThiCong"].ToString() + ""
                    + "," + form["IDKetCau"].ToString() + ""
                    + ",'" + form["inputDanhBo"].ToString() + "'"
                    + ",'" + form["inputDiemDau"].ToString() + "'"
                    + ",'" + form["inputDiemCuoi"].ToString() + "'"
                    + ",'" + form["inputTenDuong"].ToString() + "'"
                    + ")");
                return RedirectToAction("ThiCong");
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

        public ActionResult DonViThiCong()
        {
            //if (Session["ID"] == null)
            //{
            //    Session["Url"] = Request.Url;
            //    return RedirectToAction("DangNhap");
            //}
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