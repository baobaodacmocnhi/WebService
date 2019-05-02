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
        public string DanhBo { get; set; }

        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        [StringLength(200)]
        public string NoiDung { get; set; }

        [StringLength(100)]
        public string NguoiBao { get; set; }

        [StringLength(11)]
        public string DienThoai { get; set; }

        //public bool GiaiQuyet { get; set; }

        //public DateTime? NgayGiaiQuyet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? IDZalo { get; set; }

        //public int? CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        //public int? ModifyBy { get; set; }

        //public DateTime? ModifyDate { get; set; }
    }
}
