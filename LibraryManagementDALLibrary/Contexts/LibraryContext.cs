using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LibraryManagementModelLibrary.Models;

namespace LibraryManagementDALLibrary.Contexts;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookcategory> Bookcategories { get; set; }

    public virtual DbSet<Bookcopy> Bookcopies { get; set; }

    public virtual DbSet<Borrowing> Borrowings { get; set; }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Finepayment> Finepayments { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Membershiptype> Membershiptypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LibraryManagement;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("books_pkey");

            entity.ToTable("books");

            entity.HasIndex(e => e.Authorname, "idx_books_author");

            entity.HasIndex(e => e.Title, "idx_books_title");

            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.Authorname)
                .HasMaxLength(150)
                .HasColumnName("authorname");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Edition)
                .HasMaxLength(50)
                .HasColumnName("edition");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_books_category");
        });

        modelBuilder.Entity<Bookcategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookcategories_pkey");

            entity.ToTable("bookcategories");

            entity.HasIndex(e => e.Name, "bookcategories_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Bookcopy>(entity =>
        {
            entity.HasKey(e => e.Barcodeno).HasName("bookcopies_pkey");

            entity.ToTable("bookcopies");

            entity.HasIndex(e => e.Status, "idx_bookcopies_status");

            entity.Property(e => e.Barcodeno)
                .HasMaxLength(50)
                .HasColumnName("barcodeno");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .HasColumnName("isbn");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasConversion<string>()
                .HasDefaultValueSql("'Available'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.Bookcopies)
                .HasForeignKey(d => d.Isbn)
                .HasConstraintName("fk_bookcopies_book");
        });

        modelBuilder.Entity<Borrowing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("borrowings_pkey");

            entity.ToTable("borrowings");

            entity.HasIndex(e => e.Memberid, "idx_borrowings_member");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcodeno)
                .HasMaxLength(50)
                .HasColumnName("barcodeno");
            entity.Property(e => e.Borrowdate).HasColumnName("borrowdate");
            entity.Property(e => e.Borrowstatus)
                .HasMaxLength(20)
                .HasConversion<string>()
                .HasDefaultValueSql("'Borrowed'::character varying")
                .HasColumnName("borrowstatus");
            entity.Property(e => e.Duedate).HasColumnName("duedate");
            entity.Property(e => e.Memberid).HasColumnName("memberid");
            entity.Property(e => e.Returndate).HasColumnName("returndate");

            entity.HasOne(d => d.BarcodenoNavigation).WithMany(p => p.Borrowings)
                .HasForeignKey(d => d.Barcodeno)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_borrowings_bookcopy");

            entity.HasOne(d => d.Member).WithMany(p => p.Borrowings)
                .HasForeignKey(d => d.Memberid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_borrowings_member");
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fines_pkey");

            entity.ToTable("fines");

            entity.HasIndex(e => e.Borrowingid, "idx_fines_borrowing");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Borrowingid).HasColumnName("borrowingid");
            entity.Property(e => e.Fineamount)
                .HasPrecision(10, 2)
                .HasColumnName("fineamount");
            entity.Property(e => e.Finetype)
                .HasMaxLength(20)
                .HasConversion<string>()
                .HasColumnName("finetype");
            entity.Property(e => e.Ispaid)
                .HasDefaultValue(false)
                .HasColumnName("ispaid");

            entity.HasOne(d => d.Borrowing).WithMany(p => p.Fines)
                .HasForeignKey(d => d.Borrowingid)
                .HasConstraintName("fk_fines_borrowing");
        });

        modelBuilder.Entity<Finepayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("finepayments_pkey");

            entity.ToTable("finepayments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amountpaid)
                .HasPrecision(10, 2)
                .HasColumnName("amountpaid");
            entity.Property(e => e.Fineid).HasColumnName("fineid");
            entity.Property(e => e.Paymentdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");

            entity.HasOne(d => d.Fine).WithMany(p => p.Finepayments)
                .HasForeignKey(d => d.Fineid)
                .HasConstraintName("fk_finepayments_fine");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("members_pkey");

            entity.ToTable("members");

            entity.HasIndex(e => e.Email, "idx_members_email");

            entity.HasIndex(e => e.Phone, "idx_members_phone");

            entity.HasIndex(e => e.Email, "members_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "members_phone_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountstatus)
                .HasMaxLength(20)
                .HasConversion<string>()
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("accountstatus");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Membershiptypeid).HasColumnName("membershiptypeid");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");

            entity.HasOne(d => d.Membershiptype).WithMany(p => p.Members)
                .HasForeignKey(d => d.Membershiptypeid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_members_membershiptype");
        });

        modelBuilder.Entity<Membershiptype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("membershiptypes_pkey");

            entity.ToTable("membershiptypes");

            entity.HasIndex(e => e.Name, "membershiptypes_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Maximumborrowdays).HasColumnName("maximumborrowdays");
            entity.Property(e => e.Maximumborrowings).HasColumnName("maximumborrowings");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            // Seed initial membership types
            entity.HasData(
                new Membershiptype { Id = 1, Name = "Basic", Maximumborrowings = 2, Maximumborrowdays = 7 },
                new Membershiptype { Id = 2, Name = "Student", Maximumborrowings = 3, Maximumborrowdays = 10 },
                new Membershiptype { Id = 3, Name = "Premium", Maximumborrowings = 5, Maximumborrowdays = 15 }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
