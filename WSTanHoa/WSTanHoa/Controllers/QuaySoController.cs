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
    public class QuaySoController : Controller
    {
        private CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=DH_CODONG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        // GET: QuaySo
        public ActionResult Index()
        {
            //Random random = new Random();
            //DataTable dt = null;
            //while (dt == null || dt.Rows.Count == 0)
            //{
            //    dt = _cDAL.ExecuteQuery_DataTable("select top 1 STT=right('000' + cast(STT as varchar(3)), 3),DonVi,HoTen from QuaySo where Quay=0 ORDER BY NEWID()");
            //}
            ////_cDAL.ExecuteNonQuery("update QuaySo set Quay=1 where STT=" + dt.Rows[0]["STT"].ToString());
            //ViewBag.num1 = dt.Rows[0]["STT"].ToString().Substring(0, 1);
            //ViewBag.num2 = dt.Rows[0]["STT"].ToString().Substring(1, 1);
            //ViewBag.num3 = dt.Rows[0]["STT"].ToString().Substring(2, 1);
            //ViewBag.donvi = dt.Rows[0]["DonVi"].ToString();
            //ViewBag.hoten = dt.Rows[0]["HoTen"].ToString();
            return View();
        }

        //public string getQuay()
        //{
        //    try
        //    {
        //        List<MView> vKhongTinHieu = new List<MView>();
        //        DataTable dt = null;
        //        while (dt == null || dt.Rows.Count == 0)
        //        {
        //            dt = _cDAL.ExecuteQuery_DataTable("select top 1 STT=right('000' + cast(STT as varchar(3)), 3),DonVi,HoTen from QuaySo where Quay=0 ORDER BY NEWID()");
        //        }
        //        _cDAL.ExecuteNonQuery("update QuaySo set Quay=1 where STT=" + dt.Rows[0]["STT"].ToString());
        //        MView en = new MView();
        //        en.ChiSo = dt.Rows[0]["STT"].ToString().Substring(0, 1);
        //        en.DanhBo = dt.Rows[0]["STT"].ToString().Substring(1, 1);
        //        en.DiaChi = dt.Rows[0]["STT"].ToString().Substring(2, 1);
        //        en.HoTen = dt.Rows[0]["DonVi"].ToString();
        //        en.NoiDung = dt.Rows[0]["HoTen"].ToString().ToUpper();
        //        vKhongTinHieu.Add(en);
        //        return CGlobalVariable.jsSerializer.Serialize(vKhongTinHieu);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        public string getQuay(string IDGiaiThuong)
        {
            try
            {
                List<MView> vKhongTinHieu = new List<MView>();
                DataTable dt = null;
                int QDCBCNV = -1, QDKhachMoi = -1, SLCBCNV = -1, SLKhachMoi = -1;
                QDCBCNV = (int)_cDAL.ExecuteQuery_ReturnOneValue("select SoLuongTrong from QuaySo_GiaiThuong where ID=" + IDGiaiThuong);
                QDKhachMoi = (int)_cDAL.ExecuteQuery_ReturnOneValue("select SoLuongNgoai from QuaySo_GiaiThuong where ID=" + IDGiaiThuong);
                SLCBCNV = (int)_cDAL.ExecuteQuery_ReturnOneValue("select count(*) from QuaySo where KhachMoi=0 and IDGiaiThuong=" + IDGiaiThuong);
                SLKhachMoi = (int)_cDAL.ExecuteQuery_ReturnOneValue("select count(*) from QuaySo where KhachMoi=1 and IDGiaiThuong=" + IDGiaiThuong);
                if (QDCBCNV == SLCBCNV && QDKhachMoi == SLKhachMoi)
                {
                    return null;
                }
                while (dt == null || dt.Rows.Count == 0 || QDCBCNV < SLCBCNV || QDKhachMoi < SLKhachMoi)
                {
                    dt = _cDAL.ExecuteQuery_DataTable("select top 1 STT=right('000' + cast(STT as varchar(3)), 3),DonVi,HoTen,KhachMoi from QuaySo where Quay=0 ORDER BY NEWID()");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (!bool.Parse(dt.Rows[0]["KhachMoi"].ToString()))
                            SLCBCNV = (int)_cDAL.ExecuteQuery_ReturnOneValue("select count(*) from QuaySo where KhachMoi=0 and IDGiaiThuong=" + IDGiaiThuong) + 1;
                        else
                            SLKhachMoi = (int)_cDAL.ExecuteQuery_ReturnOneValue("select count(*) from QuaySo where KhachMoi=1 and IDGiaiThuong=" + IDGiaiThuong) + 1;
                    }
                }
                _cDAL.ExecuteNonQuery("update QuaySo set Quay=1 where STT=" + dt.Rows[0]["STT"].ToString());
                MView en = new MView();
                en.ChiSo = dt.Rows[0]["STT"].ToString().Substring(0, 1);
                en.DanhBo = dt.Rows[0]["STT"].ToString().Substring(1, 1);
                en.DiaChi = dt.Rows[0]["STT"].ToString().Substring(2, 1);
                en.HoTen = dt.Rows[0]["DonVi"].ToString();
                en.NoiDung = dt.Rows[0]["HoTen"].ToString().ToUpper();
                vKhongTinHieu.Add(en);
                return CGlobalVariable.jsSerializer.Serialize(vKhongTinHieu);
            }
            catch
            {
                return null;
            }
        }

        public string trung(string STT, string IDGiaiThuong)
        {
            try
            {
                _cDAL.ExecuteNonQuery("update QuaySo set Quay=1,IDGiaiThuong=" + IDGiaiThuong + " where STT=" + STT);
                return "1";
            }
            catch
            {
                return null;
            }
        }

    }
}