using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class ThuHoTong
    {
        public string DanhBo { get; set; }
        public string MaHDs { get; set; }
        public string SoHoaDons { get; set; }
        public string Kys { get; set; }
        public int SoTien { get; set; }
        public int PhiMoNuoc { get; set; }
        public int TienDu { get; set; }
        public int TongCong { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class ThuHoChiTiet
    {
        public int MaHD { get; set; }
        public string DanhBo { get; set; }
        public string SoHoaDon { get; set; }
        public int Nam { get; set; }
        public int Ky { get; set; }
        public int SoTien { get; set; }
        public DateTime CreateDate { get; set; }
    }
}