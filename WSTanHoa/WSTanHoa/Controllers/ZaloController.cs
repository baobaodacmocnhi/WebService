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
    public class ZaloController : Controller
    {
        private ModelTrungTamKhachHang db = new ModelTrungTamKhachHang();

        // GET: Zalo
        public async Task<ActionResult> Index(int? id)
        {
           
            if (id != null && id != 0)
                Session["IDZalo"] = id;
            return View(await db.Zaloes.Where(item => item.IDZalo == id).ToListAsync());
        }

        // GET: Zalo/Details/5
        public async Task<ActionResult> Details(int? id)
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
            return View();
        }

        // POST: Zalo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDZalo,DanhBo,HoTen,DiaChi")] Zalo zalo)
        {
            if (ModelState.IsValid)
            {
                zalo.CreateDate = DateTime.Now;
                db.Zaloes.Add(zalo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(zalo);
        }

        // GET: Zalo/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
        public async Task<ActionResult> Delete1(int? id)
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

        public async Task<ActionResult> Delete(int? IDZalo, string DanhBo)
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
        public async Task<ActionResult> DeleteConfirmed(int id)
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
