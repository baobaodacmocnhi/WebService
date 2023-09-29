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
        private wrThuongVu.wsThuongVu _wsThuongVu = new wrThuongVu.wsThuongVu();

        // GET: TLMD
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DangNhap(FormCollection form)
        {
            if (form.AllKeys.Contains("username"))
            {
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name,Admin,TruongPhong,IDPhong from TLMD_User where Active=1 and Username='" + form["username"].ToString().Trim() + "' and Password='" + form["password"].ToString().Trim() + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    Session["ID"] = dt.Rows[0]["ID"].ToString();
                    Session["Name"] = dt.Rows[0]["Name"].ToString();
                    Session["Admin"] = dt.Rows[0]["Admin"].ToString();
                    Session["TruongPhong"] = dt.Rows[0]["TruongPhong"].ToString();
                    Session["IDPhong"] = dt.Rows[0]["IDPhong"].ToString();
                    Session["Rules"] = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_PhanQuyenUser where IDUser=" + dt.Rows[0]["ID"].ToString());
                    return Redirect(Session["Url"].ToString());
                }
            }
            return View();
        }

        public ActionResult DoiMatKhau(FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("DangNhap");
            }
            if (form.AllKeys.Contains("passwordCu"))
            {
                if (_cDAL_TTKH.ExecuteNonQuery("update TLMD_User set Password='" + form["passwordMoi"].ToString().Trim() + "' where ID=" + Session["ID"] + " and Password='" + form["passwordCu"].ToString().Trim() + "'") == true)
                    ModelState.AddModelError("", "Thành công");
                else
                    ModelState.AddModelError("", "Thất bại");
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("ThiCong");
        }

        public bool checkRules(string IDMenu, string action)
        {
            DataTable dt = (DataTable)Session["Rules"];
            DataRow[] dr = dt.Select("IDMenu=" + IDMenu);
            return bool.Parse(dr[0][action].ToString());
        }

        public bool checkUsersThiCong(string IDUser, string IDThiCong)
        {
            try
            {
                DataTable dtUser = _cDAL_TTKH.ExecuteQuery_DataTable("select ID,Admin,TruongPhong,IDPhong from TLMD_User where ID=" + IDUser);
                DataTable dtUserCreateBy = _cDAL_TTKH.ExecuteQuery_DataTable("select ID,Admin,TruongPhong,IDPhong from TLMD_User where ID=(select CreateBy from TLMD_ThiCong where ID=" + IDThiCong + ")");
                if (dtUser.Rows[0]["ID"].ToString() == dtUserCreateBy.Rows[0]["ID"].ToString())
                    return true;
                else
                    if (dtUser.Rows[0]["IDPhong"].ToString() == dtUserCreateBy.Rows[0]["IDPhong"].ToString() && (bool.Parse(dtUser.Rows[0]["Admin"].ToString()) || bool.Parse(dtUser.Rows[0]["TruongPhong"].ToString())))
                    return true;
                else
                    if (dtUser.Rows[0]["IDPhong"].ToString() != dtUserCreateBy.Rows[0]["IDPhong"].ToString() && bool.Parse(dtUser.Rows[0]["Admin"].ToString()))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
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

        public string getPhuong(string Quan)
        {
            try
            {
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("SELECT ID=[MAPHUONG],Name=[TENPHUONG] FROM[CAPNUOCTANHOA].[dbo].[PHUONG] where MaQuan=" + Quan);
                List<MView> lst = new List<MView>();
                foreach (DataRow item in dt.Rows)
                {
                    MView en = new MView();
                    en.SoNha = item["ID"].ToString();
                    en.TenDuong = item["Name"].ToString();
                    lst.Add(en);
                }
                return CGlobalVariable.jsSerializer.Serialize(lst);
            }
            catch
            {
                return null;
            }
        }

        public ActionResult ThiCong(FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("DangNhap");
            }
            if (!checkRules("2", "Xem"))
                return RedirectToAction("Denied");
            if (form.AllKeys.Contains("TuNgay"))
            {
                Session["TuNgay"] = form["TuNgay"].ToString();
                Session["DenNgay"] = form["DenNgay"].ToString();
            }
            else
                if (Session["TuNgay"] == null)
                Session["TuNgay"] = Session["DenNgay"] = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.TuNgay = Session["TuNgay"];
            ViewBag.DenNgay = Session["DenNgay"];
            string[] dateTus = Session["TuNgay"].ToString().Split('/');
            string[] dateDens = Session["DenNgay"].ToString().Split('/');
            List<MThiCong> lst = new List<MThiCong>();
            string sql = "select CreateBy=(select Name from TLMD_User where TLMD_User.ID=thicong.CreateBy),ModifyBy=(select Name from TLMD_User where TLMD_User.ID=thicong.ModifyBy),thicong.*,DonViThiCong=donvithicong.Name,KetCau=ketcau.Name from TLMD_ThiCong thicong,TLMD_DonViThiCong donvithicong,TLMD_KetCau ketcau"
            + " where thicong.IDDonViThiCong=donvithicong.ID and thicong.IDKetCau=ketcau.ID";
            if (!form.AllKeys.Contains("radLoai") || form["radLoai"].ToString() == "radAll")
            {
                sql += " and cast(thicong.createdate as date)>='" + dateTus[2] + dateTus[1] + dateTus[0] + "' and cast(thicong.createdate as date)<='" + dateDens[2] + dateDens[1] + dateDens[0] + "'";
            }
            else
            if (form["radLoai"].ToString() == "radDaXuLy")
            {
                sql += " and cast((select top 1 CreateDate from TLMD_ThiCong_LichSu where IDThiCong=thicong.ID order by CreateDate desc) as date)>='" + dateTus[2] + dateTus[1] + dateTus[0] + "' and cast((select top 1 CreateDate from TLMD_ThiCong_LichSu where IDThiCong=thicong.ID  order by CreateDate desc) as date)<='" + dateDens[2] + dateDens[1] + dateDens[0] + "'"
                    + " and (select COUNT(*) from TLMD_ThiCong_LichSu where IDThiCong=thicong.ID)>0";
            }
            else
            if (form["radLoai"].ToString() == "radTon")
            {
                sql += " and (select COUNT(*) from TLMD_ThiCong_LichSu where IDThiCong=thicong.ID)=0";
            }
            if (bool.Parse(Session["Admin"].ToString()))
                sql += "";
            else
                if (bool.Parse(Session["TruongPhong"].ToString()))
                sql += " and (select IDPhong from TLMD_User where TLMD_User.ID=thicong.CreateBy)=" + Session["IDPhong"];
            else
                sql += " and thicong.CreateBy=" + Session["ID"];
            DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable(sql);
            int STT = 1;
            foreach (DataRow item in dt.Rows)
            {
                MThiCong en = new MThiCong();
                en.STT = STT++;
                en.ID = int.Parse(item["ID"].ToString());
                en.Name = item["Name"].ToString();
                en.DonViThiCong = item["DonViThiCong"].ToString();
                en.KetCau = item["KetCau"].ToString();
                en.DanhBo = item["DanhBo"].ToString();
                en.DiemDau = item["DiemDau"].ToString();
                en.DiemCuoi = item["DiemCuoi"].ToString();
                en.TenDuong = item["TenDuong"].ToString();
                DateTime date;
                DateTime.TryParse(item["NgayBatDau"].ToString(), out date);
                en.NgayBatDau = date;
                en.CreateBy = item["CreateBy"].ToString();
                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                en.ModifyBy = item["ModifyBy"].ToString();
                DateTime.TryParse(item["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                lst.Add(en);
            }
            return View(lst);
        }

        public ActionResult updateThiCong(string Loai, int? ID, FormCollection form)
        {
            try
            {
                if (Session["ID"] == null)
                {
                    Session["Url"] = Request.Url;
                    return RedirectToAction("DangNhap");
                }
                ViewBag.KetCau = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name from TLMD_KetCau"), "ID", "Name");
                ViewBag.DonViThiCong = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("select ID,Name from TLMD_DonViThiCong"), "ID", "Name");
                ViewBag.Phuong = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("SELECT ID=[MAPHUONG],Name=[TENPHUONG] FROM[CAPNUOCTANHOA].[dbo].[PHUONG] where MaQuan=22"), "ID", "Name");
                ViewBag.Quan = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("SELECT ID=[MAQUAN],Name=[TENQUAN] FROM[CAPNUOCTANHOA].[dbo].[QUAN]"), "ID", "Name");
                if (Loai == "insert")
                {
                    if (!checkRules("2", "Them"))
                        return RedirectToAction("Denied");
                    string[] NgayBatDaus = form["inputNgayBatDau"].ToString().Split('/');
                    if (_cDAL_TTKH.ExecuteNonQuery("insert into TLMD_ThiCong(ID,Name,IDDonViThiCong,IDKetCau,DanhBo,DiemDau,DiemCuoi,TenDuong,Phuong,Quan,NgayBatDau,CreateBy)values("
                          + "(select case when exists(select ID from TLMD_ThiCong) then (select MAX(ID)+1 from TLMD_ThiCong) else 1 end)"
                          + ",N'" + form["inputName"].ToString() + "'"
                          + "," + form["IDDonViThiCong"].ToString() + ""
                          + "," + form["IDKetCau"].ToString() + ""
                          + ",N'" + form["inputDanhBo"].ToString() + "'"
                          + ",N'" + form["inputDiemDau"].ToString() + "'"
                          + ",N'" + form["inputDiemCuoi"].ToString() + "'"
                          + ",N'" + form["inputTenDuong"].ToString() + "'"
                          + ",N'" + form["IDPhuong"].ToString() + "'"
                          + ",N'" + form["IDQuan"].ToString() + "'"
                          + ",'" + NgayBatDaus[2] + "-" + NgayBatDaus[1] + "-" + NgayBatDaus[0] + "'"
                          + "," + Session["ID"]
                          + ")"))
                        return RedirectToAction("ThiCong");
                }
                else
                if (Loai == "viewUpdate")
                {
                    MThiCong en = new MThiCong();
                    DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select CreateBy=(select Name from TLMD_User where TLMD_User.ID=TLMD_ThiCong.CreateBy),ModifyBy=(select Name from TLMD_User where TLMD_User.ID=TLMD_ThiCong.ModifyBy),*"
                          + " from TLMD_ThiCong where ID=" + ID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataTable dtLichSu = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_ThiCong_LichSu where IDThiCong=" + ID);
                        if (dtLichSu != null && dtLichSu.Rows.Count > 0)
                        {
                            foreach (DataRow itemLS in dtLichSu.Rows)
                            {
                                MThiCong_LichSu enLS = new MThiCong_LichSu();
                                enLS.Name = itemLS["Name"].ToString();
                                DateTime dateLS;
                                DateTime.TryParse(itemLS["NgayKetThuc"].ToString(), out dateLS);
                                enLS.NgayKetThuc = dateLS;
                                DateTime.TryParse(itemLS["NgayNghiemThu"].ToString(), out dateLS);
                                enLS.NgayNghiemThu = dateLS;
                                DataTable dtLichSu_Hinh = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_ThiCong_LichSu_Hinh where IDThiCongLichSu=" + itemLS["ID"].ToString());
                                foreach (DataRow itemLSH in dtLichSu_Hinh.Rows)
                                {
                                    MThiCong_LichSu_Hinh enLSH = new MThiCong_LichSu_Hinh();
                                    enLSH.ID = itemLSH["ID"].ToString();
                                    byte[] bytes = _wsThuongVu.get_Hinh_Root("TLMD", "", "", itemLSH["ID"].ToString() + ".jpg");
                                    if (bytes != null)
                                        enLSH.image = "data:image/png;base64," + Convert.ToBase64String(bytes);
                                    enLS.lstLichSu_Hinh.Add(enLSH);
                                }
                                en.lstLichSu.Add(enLS);
                            }
                        }
                        en.ID = int.Parse(dt.Rows[0]["ID"].ToString());
                        en.Name = dt.Rows[0]["Name"].ToString();
                        en.IDDonViThiCong = int.Parse(dt.Rows[0]["IDDonViThiCong"].ToString());
                        en.IDKetCau = int.Parse(dt.Rows[0]["IDKetCau"].ToString());
                        en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                        en.DiemDau = dt.Rows[0]["DiemDau"].ToString();
                        en.DiemCuoi = dt.Rows[0]["DiemCuoi"].ToString();
                        en.TenDuong = dt.Rows[0]["TenDuong"].ToString();
                        en.IDPhuong = dt.Rows[0]["Phuong"].ToString();
                        en.IDQuan = dt.Rows[0]["Quan"].ToString();
                        DateTime date;
                        DateTime.TryParse(dt.Rows[0]["NgayBatDau"].ToString(), out date);
                        en.NgayBatDau = date;
                        en.CreateBy = dt.Rows[0]["CreateBy"].ToString();
                        en.CreateDate = DateTime.Parse(dt.Rows[0]["CreateDate"].ToString());
                        en.ModifyBy = dt.Rows[0]["ModifyBy"].ToString();
                        DateTime.TryParse(dt.Rows[0]["ModifyDate"].ToString(), out date);
                        en.ModifyDate = date;
                        ViewBag.Phuong = _cDAL_TTKH.ToSelectList(_cDAL_TTKH.ExecuteQuery_DataTable("SELECT ID=[MAPHUONG],Name=[TENPHUONG] FROM[CAPNUOCTANHOA].[dbo].[PHUONG] where MaQuan=" + en.IDQuan), "ID", "Name");
                        return View(en);
                    }
                }
                else
                if (Loai == "update")
                {
                    if (!checkRules("2", "Sua"))
                        return RedirectToAction("Denied");
                    if (!checkUsersThiCong(Session["ID"].ToString(), form["inputID"].ToString()))
                        return RedirectToAction("Denied");
                    string[] date = form["inputNgayBatDau"].ToString().Split(' ');
                    string[] NgayBatDaus = date[0].Split('/');
                    if (_cDAL_TTKH.ExecuteNonQuery("update TLMD_ThiCong set"
                        + " Name=N'" + form["inputName"].ToString() + "'"
                        + ",IDDonViThiCong=" + form["IDDonViThiCong"].ToString() + ""
                        + ",IDKetCau=" + form["IDKetCau"].ToString() + ""
                        + ",DanhBo='" + form["inputDanhBo"].ToString() + "'"
                        + ",DiemDau=N'" + form["inputDiemDau"].ToString() + "'"
                        + ",DiemCuoi=N'" + form["inputDiemCuoi"].ToString() + "'"
                        + ",TenDuong=N'" + form["inputTenDuong"].ToString() + "'"
                        + ",Phuong=N'" + form["IDPhuong"].ToString() + "'"
                        + ",Quan=N'" + form["IDQuan"].ToString() + "'"
                        + ",NgayBatDau='" + NgayBatDaus[2] + "-" + NgayBatDaus[1] + "-" + NgayBatDaus[0] + "'"
                        + ",ModifyBy=" + Session["ID"]
                        + ",ModifyDate=getdate()"
                        + " where ID=" + form["inputID"].ToString()))
                        return RedirectToAction("ThiCong");
                }
                else
                if (Loai == "delete")
                {
                    if (!checkRules("2", "Xoa"))
                        return RedirectToAction("Denied");
                    if (!checkUsersThiCong(Session["ID"].ToString(), ID.Value.ToString()))
                        return RedirectToAction("Denied");
                    if (_cDAL_TTKH.ExecuteNonQuery("delete TLMD_ThiCong where ID=" + ID))
                        return RedirectToAction("ThiCong");
                }
            }
            catch (Exception ex)
            {
                CGlobalVariable.log.Error("updateThiCong (ID=" + form["inputID"].ToString() + ") " + ex.Message);
                ModelState.AddModelError("", "Lỗi updateThiCong (ID=" + form["inputID"].ToString() + ") " + ex.Message);
            }
            return View();
        }

        public string getTimKiemThiCong(string NoiDungTimKiem)
        {
            List<MThiCong> lst = new List<MThiCong>();
            string sql = "select CreateBy=(select Name from TLMD_User where TLMD_User.ID=thicong.CreateBy),ModifyBy=(select Name from TLMD_User where TLMD_User.ID=thicong.ModifyBy),thicong.*,DonViThiCong=donvithicong.Name,KetCau=ketcau.Name from TLMD_ThiCong thicong,TLMD_DonViThiCong donvithicong,TLMD_KetCau ketcau"
            + " where thicong.IDDonViThiCong=donvithicong.ID and thicong.IDKetCau=ketcau.ID and (thicong.Name like N'%" + NoiDungTimKiem + "%' or thicong.DanhBo like N'%" + NoiDungTimKiem + "%' or thicong.DiemDau like N'%" + NoiDungTimKiem + "%' or thicong.DiemCuoi like N'%" + NoiDungTimKiem + "%' or thicong.TenDuong like N'%" + NoiDungTimKiem + "%')";
            DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable(sql);
            int STT = 1;
            foreach (DataRow item in dt.Rows)
            {
                MThiCong en = new MThiCong();
                en.STT = STT++;
                en.ID = int.Parse(item["ID"].ToString());
                en.Name = item["Name"].ToString();
                en.DonViThiCong = item["DonViThiCong"].ToString();
                en.KetCau = item["KetCau"].ToString();
                en.DanhBo = item["DanhBo"].ToString();
                en.DiemDau = item["DiemDau"].ToString();
                en.DiemCuoi = item["DiemCuoi"].ToString();
                en.TenDuong = item["TenDuong"].ToString();
                DateTime date;
                DateTime.TryParse(item["NgayBatDau"].ToString(), out date);
                en.NgayBatDau = date;
                en.CreateBy = item["CreateBy"].ToString();
                en.CreateDateW = DateTime.Parse(item["CreateDate"].ToString()).ToString("dd/MM/yyyy");
                en.ModifyBy = item["ModifyBy"].ToString();
                DateTime.TryParse(item["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                lst.Add(en);
            }
            return CGlobalVariable.jsSerializer.Serialize(lst);
        }

        public ActionResult DonViThiCong()
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("DangNhap");
            }
            if (!checkRules("1", "Xem"))
                return RedirectToAction("Denied");
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
                en.CreateBy = item["CreateBy"].ToString();
                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                en.ModifyBy = item["ModifyBy"].ToString();
                DateTime date;
                DateTime.TryParse(item["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                lst.Add(en);
            }
            return View(lst);
        }

        public ActionResult updateDonViThiCong(string Loai, int? ID, FormCollection form)
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("DangNhap");
            }
            if (Loai == "insert")
            {
                if (!checkRules("1", "Them"))
                    return RedirectToAction("Denied");
                if (_cDAL_TTKH.ExecuteNonQuery("insert into TLMD_DonViThiCong(ID,Name,DaiDien,DienThoai,Username,Password,CreateBy)values("
                      + "(select case when exists(select ID from TLMD_DonViThiCong) then (select MAX(ID)+1 from TLMD_DonViThiCong) else 1 end)"
                      + ",N'" + form["inputName"].ToString() + "'"
                      + ",N'" + form["inputDaiDien"].ToString() + "'"
                      + ",'" + form["inputDienThoai"].ToString() + "'"
                      + ",'" + form["inputUsername"].ToString() + "'"
                      + ",'" + form["inputPassword"].ToString() + "'"
                      + "," + Session["ID"]
                      + ")"))
                    return RedirectToAction("DonViThiCong");
            }
            else
            if (Loai == "viewUpdate")
            {
                MDonViThiCong en = new MDonViThiCong();
                DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select CreateBy=(select ID from TLMD_User where ID=TLMD_DonViThiCong.CreateBy),ModifyBy=(select ID from TLMD_User where ID=TLMD_DonViThiCong.ModifyBy),* from TLMD_DonViThiCong where ID=" + ID);
                en.ID = int.Parse(dt.Rows[0]["ID"].ToString());
                en.Name = dt.Rows[0]["Name"].ToString();
                en.DaiDien = dt.Rows[0]["DaiDien"].ToString();
                en.DienThoai = dt.Rows[0]["DienThoai"].ToString();
                en.Username = dt.Rows[0]["Username"].ToString();
                en.Password = dt.Rows[0]["Password"].ToString();
                en.Active = bool.Parse(dt.Rows[0]["Active"].ToString());
                en.CreateBy = dt.Rows[0]["CreateBy"].ToString();
                en.CreateDate = DateTime.Parse(dt.Rows[0]["CreateDate"].ToString());
                en.ModifyBy = dt.Rows[0]["ModifyBy"].ToString();
                DateTime date;
                DateTime.TryParse(dt.Rows[0]["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                return View(en);
            }
            else
            if (Loai == "update")
            {
                if (!checkRules("1", "Sua"))
                    return RedirectToAction("Denied");
                string flag = "";
                if (form.AllKeys.Contains("inputActive"))
                    flag = "1";
                else
                    flag = "0";
                if (_cDAL_TTKH.ExecuteNonQuery("update TLMD_DonViThiCong set"
                      + " Name=N'" + form["inputName"].ToString() + "'"
                      + ",DaiDien=N'" + form["inputDaiDien"].ToString() + "'"
                      + ",DienThoai='" + form["inputDienThoai"].ToString() + "'"
                      + ",Username='" + form["inputUsername"].ToString() + "'"
                      + ",Password='" + form["inputPassword"].ToString() + "'"
                      + ",Active='" + flag + "'"
                      + ",ModifyBy=" + Session["ID"]
                      + ",ModifyDate=getdate()"
                      + " where ID=" + form["inputID"].ToString()))
                    return RedirectToAction("DonViThiCong");
            }
            else
            if (Loai == "delete")
            {
                if (!checkRules("1", "Xoa"))
                    return RedirectToAction("Denied");
                if (_cDAL_TTKH.ExecuteNonQuery("delete TLMD_DonViThiCong where ID=" + ID))
                    return RedirectToAction("DonViThiCong");
            }

            return View();
        }

        public ActionResult Denied()
        {
            return View();
        }

    }
}