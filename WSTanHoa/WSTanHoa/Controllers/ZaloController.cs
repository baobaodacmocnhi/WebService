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
        public async Task<ActionResult> Index(decimal? id)
        {
            if (id != null && id != -1)
            {
                CConstantVariable.IDZalo = id.Value;
                ViewBag.IDZalo = CConstantVariable.IDZalo;
            }
            else
                if (id == -1)
                ViewBag.IDZalo = "-1";
            else
                ViewBag.IDZalo = "";
            //if (id != null)
            //{
            //    CConstantVariable.IDZalo = id.Value;
            //    ViewBag.IDZalo = CConstantVariable.IDZalo;
            //}
            //else
            //    ViewBag.IDZalo = "";
            return View(await db.Zaloes.Where(item => item.IDZalo == CConstantVariable.IDZalo).ToListAsync());
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
        public ActionResult Create()
        {
            ViewBag.IDZalo = CConstantVariable.IDZalo;

            return View();
        }

        // POST: Zalo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi")] Zalo zalo, string Loai)
        {
            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(Loai))
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
                                Zalo en = new Zalo();
                                en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                en.HoTen = dt.Rows[0]["HoTen"].ToString();
                                en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                return View(en);
                            }
                        }
                        break;
                    case "Đăng Ký":
                        if (CConstantVariable.IDZalo != -1)
                        {
                            if (db.Zaloes.Count(item => item.IDZalo == zalo.IDZalo && item.DanhBo == zalo.DanhBo) == 0)
                            {
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
                                }
                                zalo.CreateDate = DateTime.Now;
                                db.Zaloes.Add(zalo);
                                await db.SaveChangesAsync();
                                return RedirectToAction("Index");
                            }
                        }
                        break;
                }

            }
            return View(zalo);
        }

        // GET: Zalo/Edit/5
        public async Task<ActionResult> Edit(decimal? id)
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

        // POST: Zalo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi")] Zalo zalo)
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
