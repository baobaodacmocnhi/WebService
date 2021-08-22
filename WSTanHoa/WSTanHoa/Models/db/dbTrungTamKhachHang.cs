namespace WSTanHoa.Models.db
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dbTrungTamKhachHang : DbContext
    {
        public dbTrungTamKhachHang()
            : base("name=dbTrungTamKhachHang")
        {
        }

        public virtual DbSet<KhieuNai> KhieuNais { get; set; }
        public virtual DbSet<Zalo_Chat> Zalo_Chat { get; set; }
        public virtual DbSet<Zalo_DangKy> Zalo_DangKy { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Zalo_Chat>()
                .Property(e => e.IDZalo)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Zalo_Chat>()
                .Property(e => e.NguoiGui)
                .IsUnicode(false);

            modelBuilder.Entity<Zalo_DangKy>()
                .Property(e => e.IDZalo)
                .HasPrecision(20, 0);

            modelBuilder.Entity<Zalo_DangKy>()
                .Property(e => e.DanhBo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Zalo_DangKy>()
                .Property(e => e.DienThoai)
                .IsUnicode(false);
        }
    }
}
