using LibraryManagement.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Shelf> Shelves => Set<Shelf>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookReservation> BookReservations => Set<BookReservation>();
    public DbSet<StudyRoom> StudyRooms => Set<StudyRoom>();
    public DbSet<RoomReservation> RoomReservations => Set<RoomReservation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasIndex(user => user.StudentNumber)
            .IsUnique();

        builder.Entity<Book>()
            .HasIndex(book => book.ISBN)
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();

        builder.Entity<Shelf>()
            .HasIndex(shelf => shelf.Code)
            .IsUnique();

        builder.Entity<StudyRoom>()
            .HasIndex(room => room.RoomNumber)
            .IsUnique();

        builder.Entity<Book>()
            .HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(book => book.Category)
            .WithMany(category => category.Books)
            .HasForeignKey(book => book.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(book => book.Shelf)
            .WithMany(shelf => shelf.Books)
            .HasForeignKey(book => book.ShelfId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BookReservation>()
            .HasOne(reservation => reservation.User)
            .WithMany(user => user.BookReservations)
            .HasForeignKey(reservation => reservation.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BookReservation>()
            .HasOne(reservation => reservation.Book)
            .WithMany(book => book.Reservations)
            .HasForeignKey(reservation => reservation.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoomReservation>()
            .HasOne(reservation => reservation.User)
            .WithMany(user => user.RoomReservations)
            .HasForeignKey(reservation => reservation.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoomReservation>()
            .HasOne(reservation => reservation.StudyRoom)
            .WithMany(room => room.Reservations)
            .HasForeignKey(reservation => reservation.StudyRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BookReservation>()
            .Property(reservation => reservation.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Entity<RoomReservation>()
            .Property(reservation => reservation.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Entity<RoomReservation>()
            .HasIndex(reservation => new
            {
                reservation.StudyRoomId,
                reservation.ReservationDate,
                reservation.StartTime,
                reservation.EndTime
            });

        builder.Entity<Book>()
            .ToTable("Books", table =>
            {
                table.HasCheckConstraint(
                    "CK_Books_TotalStock",
                    "\"TotalStock\" >= 0");

                table.HasCheckConstraint(
                    "CK_Books_AvailableStock",
                    "\"AvailableStock\" >= 0 AND \"AvailableStock\" <= \"TotalStock\"");
            });

        builder.Entity<StudyRoom>()
            .ToTable("StudyRooms", table =>
            {
                table.HasCheckConstraint(
                    "CK_StudyRooms_Capacity",
                    "\"Capacity\" > 0");
            });

        builder.Entity<RoomReservation>()
            .HasIndex(reservation => new
            {
                reservation.UserId,
                reservation.ReservationDate,
                reservation.StartTime,
                reservation.EndTime
            })
            .HasDatabaseName(
                "IX_RoomReservations_User_Date_Time");
                    
        builder.Entity<BookReservation>()
            .HasIndex(reservation => new
            {
                reservation.UserId,
                reservation.BookId
            })
            .IsUnique()
            .HasDatabaseName(
                "UX_BookReservations_User_Book_Active")
            .HasFilter(
                "\"Status\" IN ('Pending', 'Approved')");

        SeedData.Seed(builder);
    }
}