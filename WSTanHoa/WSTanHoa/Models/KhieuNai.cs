namespace WSTanHoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KhieuNai")]
    public partial class KhieuNai
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [StringLength(11)]
        [Display(Name = "Danh bộ")]
        public string DanhBo { get; set; }

        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [StringLength(200)]
        [Display(Name = "Nội dung khiếu nại")]
        public string NoiDung { get; set; }

        [StringLength(100)]
        [Display(Name = "Họ tên người liên hệ")]
        public string NguoiBao { get; set; }

        [StringLength(11)]
        [Display(Name = "Điện thoại")]
        public string DienThoai { get; set; }

        //public bool GiaiQuyet { get; set; }

        //public DateTime? NgayGiaiQuyet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDZalo { get; set; }

        //public int? CreateBy { get; set; }

        [Display(Name = "Ngày lập")]
        public DateTime? CreateDate { get; set; }

        //public int? ModifyBy { get; set; }

        //public DateTime? ModifyDate { get; set; }
    }
}
