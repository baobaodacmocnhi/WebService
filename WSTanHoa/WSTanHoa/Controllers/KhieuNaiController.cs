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
    public class KhieuNaiController : Controller
    {
        private ModelTrungTamKhachHang db = new ModelTrungTamKhachHang();

        // GET: KhieuNai
        public async Task<ActionResult> Index(decimal? id)
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
            //if (id != null)
            //{
            //    CConstantVariable.IDZalo = id.Value;
            //    ViewBag.IDZalo = CConstantVariable.IDZalo;
            //}
            //else
            //    ViewBag.IDZalo = "";
            return View(await db.KhieuNais.Where(item => item.IDZalo == CConstantVariable.IDZalo).ToListAsync());
        }

        // GET: KhieuNai/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhieuNai khieuNai = await db.KhieuNais.FindAsync(id);
            if (khieuNai == null)
            {
                return HttpNotFound();
            }
            return View(khieuNai);
        }

        // GET: KhieuNai/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhieuNai/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,DanhBo,HoTen,DiaChi,NoiDung,NguoiBao,DienThoai,IDZalo,CreateDate")] KhieuNai khieuNai)
        {
            if (ModelState.IsValid && khieuNai.IDZalo != null && CConstantVariable.IDZalo != 1)
            {
                if (db.KhieuNais.Count() == 0)
                    khieuNai.ID = 1;
                else
                    khieuNai.ID = db.KhieuNais.Max(item => item.ID) + 1;
                khieuNai.IDZalo = CConstantVariable.IDZalo;
                khieuNai.CreateDate = DateTime.Now;
                db.KhieuNais.Add(khieuNai);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(khieuNai);
        }

        // GET: KhieuNai/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhieuNai khieuNai = await db.KhieuNais.FindAsync(id);
            if (khieuNai == null)
            {
                return HttpNotFound();
            }
            return View(khieuNai);
        }

        // POST: KhieuNai/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,DanhBo,HoTen,DiaChi,NoiDung,NguoiBao,DienThoai,IDZalo,CreateDate")] KhieuNai khieuNai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khieuNai).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(khieuNai);
        }

        // GET: KhieuNai/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhieuNai khieuNai = await db.KhieuNais.FindAsync(id);
            if (khieuNai == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.KhieuNais.Remove(khieuNai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khieuNai);
        }

        // POST: KhieuNai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            KhieuNai khieuNai = await db.KhieuNais.FindAsync(id);
            db.KhieuNais.Remove(khieuNai);
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
