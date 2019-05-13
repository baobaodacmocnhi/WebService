using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class ZaloController : Controller
    {
        ModelTrungTamKhachHang db = new ModelTrungTamKhachHang();
        CConnection _cDAL_ThuTien = new CConnection(CConstantVariable.ThuTien);

        // GET: Zalo
        public async Task<ActionResult> Index(decimal? id, [Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi,DienThoai")] Zalo vZalo, string action)
        {
            if (CConstantVariable.IDZalo == -1)
            {
                if (id != null && id != -1)
                {
                    CConstantVariable.IDZalo = id.Value;
                    ViewBag.IDZalo = id.Value;
                }
                else
                {
                    CConstantVariable.IDZalo = -1;
                    if (id == -1)
                        ViewBag.IDZalo = "-1";
                    else
                        ViewBag.IDZalo = "";
                }
            }
            IEnumerable<Zalo> lstZalo = await db.Zaloes.Where(item => item.IDZalo == CConstantVariable.IDZalo).ToListAsync();
            Zalo zalo = new Zalo();
            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(action))
                if (CConstantVariable.IDZalo != -1)
                {
                    switch (action)
                    {
                        case "Kiểm Tra":
                            if (vZalo.DanhBo != null && vZalo.DanhBo != "")
                            {
                                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + vZalo.DanhBo.Replace(" ", "") + "' order by ID_HOADON desc");
                                if (dt.Rows.Count > 0)
                                {
                                    //vZalo = await db.Zaloes.FindAsync(CConstantVariable.IDZalo, vZalo.DanhBo);
                                    zalo.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                    zalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                    zalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                    return View(new ViewZalo(lstZalo, zalo));
                                }
                                else
                                {
                                    ModelState.AddModelError("vZalo.DanhBo", "Danh Bộ không đúng");
                                    return View(new ViewZalo(lstZalo, zalo));
                                }
                            }
                            break;
                        case "Đăng Ký":
                            if (vZalo.DanhBo != null && vZalo.DanhBo != "")
                                if (db.Zaloes.Count(item => item.IDZalo == CConstantVariable.IDZalo && item.DanhBo == vZalo.DanhBo.Replace(" ", "")) == 0)
                                {
                                    if (vZalo.DienThoai == null || vZalo.DienThoai == "")
                                    {
                                        ModelState.AddModelError("vZalo.DienThoai", "Vui lòng nhập số điện thoại");
                                        return View(new ViewZalo(lstZalo, vZalo));
                                    }
                                    if (vZalo.HoTen == null || vZalo.HoTen == "")
                                    {
                                        DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + vZalo.DanhBo + "' order by ID_HOADON desc");
                                        if (dt.Rows.Count > 0)
                                        {
                                            vZalo.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                            vZalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                            vZalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("vZalo.DanhBo", "Danh Bộ không đúng");
                                            return View(new ViewZalo(lstZalo, vZalo));
                                        }
                                    }
                                    vZalo.IDZalo = CConstantVariable.IDZalo;
                                    vZalo.DanhBo = vZalo.DanhBo.Replace(" ", "");
                                    vZalo.CreateDate = DateTime.Now;
                                    db.Zaloes.Add(vZalo);
                                    await db.SaveChangesAsync();
                                    return RedirectToAction("Index");
                                }
                            break;
                    }
                }

            return View(new ViewZalo(lstZalo, vZalo));
        }

        // GET: Zalo/Details/5
        public async Task<ActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zalo zalo = await db.Zaloes.FindAsync(id);
            if (zalo == null)
            {
                return HttpNotFound();
            }
            return View(zalo);
        }

        // GET: Zalo/Create
        public ActionResult Create(string DanhBo)
        {
            if (ViewBag == null)
                ViewBag.IDZalo = CConstantVariable.IDZalo;
            if (DanhBo != null)
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
                if (dt.Rows.Count > 0)
                {
                    Zalo en = new Zalo();
                    en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                    en.HoTen = dt.Rows[0]["HoTen"].ToString();
                    en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                    return View(en);
                }
                else
                {
                    ModelState.AddModelError("DanhBo", "Danh Bộ không đúng");
                    return View();
                }
            }

            return View();
        }

        // POST: Zalo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi,DienThoai")] Zalo zalo, string Loai)
        {
            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(Loai))
                if (CConstantVariable.IDZalo != -1)
                {
                    ViewBag.IDZalo = CConstantVariable.IDZalo;
                    switch (Loai)
                    {
                        case "Kiểm Tra":
                            if (zalo.DanhBo != null && zalo.DanhBo != "")
                            {
                                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + zalo.DanhBo + "' order by ID_HOADON desc");
                                if (dt.Rows.Count > 0)
                                {
                                    Zalo zalo1 = await db.Zaloes.FindAsync(1, "13051375570");
                                    //Zalo en = new Zalo();
                                    //en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                    zalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                    zalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                    return View(zalo1);
                                }
                                else
                                {
                                    ModelState.AddModelError("DanhBo", "Danh Bộ không đúng");
                                    return View(zalo);
                                }
                            }
                            break;
                        case "Đăng Ký":
                            if (zalo.DanhBo != null && zalo.DanhBo != "")
                                if (db.Zaloes.Count(item => item.IDZalo == zalo.IDZalo && item.DanhBo == zalo.DanhBo) == 0)
                                {
                                    if (zalo.DienThoai == null || zalo.DienThoai == "")
                                    {
                                        ModelState.AddModelError("DienThoai", "Vui lòng nhập số điện thoại");
                                        return View(zalo);
                                    }
                                    zalo.IDZalo = CConstantVariable.IDZalo;
                                    if (zalo.HoTen == null || zalo.HoTen == "")
                                    {
                                        DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + zalo.DanhBo + "' order by ID_HOADON desc");
                                        if (dt.Rows.Count > 0)
                                        {
                                            zalo.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                            zalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                            zalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                        }
                                        else
                                        {
                                            ModelState.AddModelError("DanhBo", "Danh Bộ không đúng");
                                            return View(zalo);
                                        }
                                    }
                                    zalo.CreateDate = DateTime.Now;
                                    db.Zaloes.Add(zalo);
                                    await db.SaveChangesAsync();
                                    return RedirectToAction("Index");
                                }
                            break;
                    }
                }
            return View(zalo);
        }

        // GET: Zalo/Edit/5
        public async Task<ActionResult> Edit(decimal? IDZalo, string DanhBo)
        {
            if (IDZalo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zalo zalo = await db.Zaloes.FindAsync(IDZalo, DanhBo);
            if (zalo == null)
            {
                return HttpNotFound();
            }
            return View(zalo);
        }

        // POST: Zalo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi,DienThoai")] Zalo zalo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zalo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(zalo);
        }

        // GET: Zalo/Delete/5
        public async Task<ActionResult> Delete1(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zalo zalo = await db.Zaloes.FindAsync(id);
            if (zalo == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Zaloes.Remove(zalo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(zalo);
        }

        public async Task<ActionResult> Delete(decimal? IDZalo, string DanhBo)
        {
            if (IDZalo == null && (DanhBo == null || DanhBo == ""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zalo zalo = await db.Zaloes.FindAsync(IDZalo, DanhBo);
            if (zalo == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Zaloes.Remove(zalo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(zalo);
        }

        // POST: Zalo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id)
        {
            Zalo zalo = await db.Zaloes.FindAsync(id);
            db.Zaloes.Remove(zalo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
