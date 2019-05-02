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

namespace WSTanHoa.Controllers
{
    public class KhieuNaiController : Controller
    {
        private ModelTrungTamKhachHang db = new ModelTrungTamKhachHang();

        // GET: KhieuNai
        public async Task<ActionResult> Index()
        {
            return View(await db.KhieuNais.ToListAsync());
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
            if (ModelState.IsValid)
            {
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
