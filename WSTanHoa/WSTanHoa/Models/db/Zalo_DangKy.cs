﻿namespace WSTanHoa.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Zalo_DangKy
    {
        [Key]
        [Column(Order = 0, TypeName = "numeric")]
        [Display(Name = "ID Zalo")]
        public decimal IDZalo { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(13, MinimumLength = 11, ErrorMessage = "Danh bộ gồm 11 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập danh bộ")]
        [Display(Name = "Danh bộ")]
        public string DanhBo { get; set; }

        [StringLength(10)]
        //[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Điện Thoại")]
        public string DienThoai { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }
    }

}
