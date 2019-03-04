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
        private ModelTrungTamKhachHang db = new ModelTrungTamKhachHang();
        CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");

        // GET: Zalo
        public async Task<ActionResult> Index(decimal? id)
        {
            if (id != null && id != -1)
            {
                CConstantVariable.IDZalo = id;
                ViewBag.IDZalo = CConstantVariable.IDZalo;
            }
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
        public ActionResult Create(string DanhBo)
        {
            ViewBag.IDZalo = CConstantVariable.IDZalo;
            if (DanhBo != null && DanhBo != "")
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
                if (dt.Rows.Count > 0)
                {
                    Zalo en = new Zalo();
                    en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                    en.HoTen = dt.Rows[0]["HoTen"].ToString();
                    en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                    return View(en);
                }
            }
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
                if (zalo.IDZalo != null || zalo.IDZalo != -1 || zalo.IDZalo != 0)
                {
                    zalo.CreateDate = DateTime.Now;
                    db.Zaloes.Add(zalo);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
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
