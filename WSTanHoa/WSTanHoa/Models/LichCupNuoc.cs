using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class LichCupNuoc
    {
        public int? ID { get; set; }
        public string NoiDung { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? MaQuan { get; set; }
        public string TenQuan { get; set; }
        public string MaPhuong { get; set; }
        public string TenPhuong { get; set; }
        public bool Gui { get; set; }

        public LichCupNuoc()
        {
            ID = null;
            NoiDung = "";
            TuNgay = null;
            DenNgay = null;
            MaQuan = null;
            TenQuan = "";
            MaPhuong = "";
            TenPhuong = "";
            Gui = false;
        }
    }
}