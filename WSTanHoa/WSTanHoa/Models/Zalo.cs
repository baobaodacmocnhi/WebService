namespace WSTanHoa.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Zalo")]
    public partial class Zalo
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "ID Zalo")]
        public int IDZalo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(11,MinimumLength =11,ErrorMessage ="Danh bộ gồm 11 ký tự")]
        [Required(ErrorMessage = "Nhập danh bộ")]
        
        [Display(Name = "Danh bộ")]
        public string DanhBo { get; set; }

        [StringLength(500)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }
    }
}
