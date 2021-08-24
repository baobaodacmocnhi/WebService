using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models.db
{
    public class ZaloView
    {
        public string STT { get; set; }
        public string IDZalo { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string CreateDate { get; set; }
        public string NguoiGui { get; set; }
        public string NoiDung { get; set; }
        public string Image { get; set; }

        [StringLength(13, MinimumLength = 11, ErrorMessage = "Danh bộ gồm 11 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập danh bộ")]
        [Display(Name = "Danh Bộ")]
        public string DanhBo { get; set; }

        [StringLength(500)]
        //[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; }

        [StringLength(500)]
        //[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Dịa Chỉ")]
        public string DiaChi { get; set; }

        [StringLength(10)]
        //[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Điện Thoại")]
        public string DienThoai { get; set; }
        public List<ZaloView> lst { get; set; }
        public ZaloView()
        {
            STT = "0";
            IDZalo = "-1";
            Avatar = Name = CreateDate =DanhBo= HoTen = DiaChi = DienThoai = "";
            lst = new List<ZaloView>();
        }
    }
}