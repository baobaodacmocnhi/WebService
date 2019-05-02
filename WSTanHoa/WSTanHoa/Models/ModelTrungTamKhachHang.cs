namespace WSTanHoa.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelTrungTamKhachHang : DbContext
    {
        public ModelTrungTamKhachHang()
            : base("name=ModelTrungTamKhachHang")
        {
        }

        public virtual DbSet<Zalo> Zaloes { get; set; }

        public virtual DbSet<KhieuNai> KhieuNais { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Zalo>()
                .Property(e => e.IDZalo)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Zalo>()
                .Property(e => e.DanhBo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<KhieuNai>()
               .Property(e => e.DanhBo)
               .IsFixedLength()
               .IsUnicode(false);

            modelBuilder.Entity<KhieuNai>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);

            modelBuilder.Entity<KhieuNai>()
                .Property(e => e.IDZalo)
                .HasPrecision(20, 0);
        }
    }
}
