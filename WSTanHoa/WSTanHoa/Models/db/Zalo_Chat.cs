namespace WSTanHoa.Models.db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Zalo_Chat
    {
        [Key]
        [Column(Order = 0)]
        public decimal IDZalo { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime CreateDate { get; set; }

        [StringLength(10)]
        public string NguoiGui { get; set; }

        [StringLength(500)]
        public string NoiDung { get; set; }

        [StringLength(100)]
        public string Image { get; set; }
    }
}
