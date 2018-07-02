using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsTanHoa
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsTanHoa : System.Web.Services.WebService
    {
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public DataSet getHoaDonTon(string DanhBo, out string error)
        {
            error = "";
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(_cThuTien.getHoaDonTon(DanhBo));
                return ds;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        [WebMethod]
        public int getPhiMoNuoc(string DanhBo, out string error)
        {
            error = "";
            try
            {
                return _cThuTien.getPhiMoNuoc(DanhBo);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return -1;
            }
        }

        [WebMethod]
        public int getTienDu(string DanhBo, out string error)
        {
            error = "";
            try
            {
                return _cThuTien.getTienDu(DanhBo);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return -1;
            }
        }

        [WebMethod]
        public bool insertThuHo(string DanhBo, string MaHDs, int SoTien, int PhiMoNuoc, int TienDu, string TenDichVu, string IDGiaoDich, out string error)
        {
            error = "";
            try
            {
                return _cThuTien.insertThuHo(DanhBo, MaHDs, SoTien, PhiMoNuoc, TienDu, TenDichVu, IDGiaoDich);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        [WebMethod]
        public bool deleteThuHo(string TenDichVu, string IDGiaoDich, out string error)
        {
            error = "";
            try
            {
                return _cThuTien.deleteThuHo(TenDichVu, IDGiaoDich);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        [WebMethod]
        public DataSet getThuHo(string TenDichVu, string IDGiaoDich, out string error)
        {
            error = "";
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(_cThuTien.getThuHo(TenDichVu, IDGiaoDich));
                return ds;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        [WebMethod]
        public DataSet getThuHoTong(string TenDichVu, string IDGiaoDich, out string error)
        {
            error = "";
            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(_cThuTien.getThuHoTong(TenDichVu, IDGiaoDich));
                return ds;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

    }
}
