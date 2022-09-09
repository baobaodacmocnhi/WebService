using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class MView
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string SoLuong { get; set; }
        public string ThoiGian { get; set; }
        public List<MView> lst { get; set; }
        public MView()
        {
            TieuDe = NoiDung = SoLuong = ThoiGian = "";
            lst = new List<MView>();
        }
    }

    public class MHoaDon
    {
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public int? MaHD { get; set; }
        public string SoHoaDon { get; set; }
        public string DanhBo { get; set; }
        public int? Nam { get; set; }
        public int? Ky { get; set; }
        public int TieuThu { get; set; }
        public int GiaBan { get; set; }
        public int ThueGTGT { get; set; }
        public int PhiBVMT { get; set; }
        public int PhiBVMT_Thue { get; set; }
        public int TongCong { get; set; }
        public int PhiMoNuoc { get; set; }
        public int TienDu { get; set; }
        //public DateTime? NgayGiaiTrach { get; set; }
        //public string KyHD { get; set; }
        public int CSC { get; set; }
        public int TieuThuMoi { get; set; }
        public string CodeMoi { get; set; }
        public int ChiSoMoi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }


        public MHoaDon()
        {
            HoTen = "";
            DiaChi = "";
            MaHD = null;
            SoHoaDon = "";
            DanhBo = "";
            Nam = null;
            Ky = null;
            GiaBan = 0;
            ThueGTGT = 0;
            PhiBVMT = 0;
            PhiBVMT_Thue = 0;
            TongCong = 0;
            PhiMoNuoc = 0;
            TienDu = 0;
            //NgayGiaiTrach = null;
            //KyHD = "";
            TieuThu = CSC = TieuThuMoi = ChiSoMoi = -1;
            CodeMoi = "";
        }

    }
}