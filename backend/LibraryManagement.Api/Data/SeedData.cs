using LibraryManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Data;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                Name = "George Orwell",
                Biography = "İngiliz romancı ve gazeteci."
            },
            new Author
            {
                Id = 2,
                Name = "Sabahattin Ali",
                Biography = "Türk yazar ve şair."
            },
            new Author
            {
                Id = 3,
                Name = "Fyodor Dostoyevski",
                Biography = "Rus romancı ve düşünür."
            },
            new Author
            {
                Id = 4,
                Name = "Robert C. Martin",
                Biography = "Yazılım mühendisi ve teknik kitap yazarı."
            },
            new Author
            {
                Id = 5,
                Name = "Yuval Noah Harari",
                Biography = "Tarihçi ve yazar."
            },
            new Author
            {
                Id = 6,
                Name = "Thomas H. Cormen",
                Biography = "Bilgisayar bilimci ve akademisyen."
            }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Roman",
                Description = "Yerli ve yabancı romanlar."
            },
            new Category
            {
                Id = 2,
                Name = "Bilgisayar Bilimleri",
                Description = "Yazılım ve bilgisayar bilimleri kitapları."
            },
            new Category
            {
                Id = 3,
                Name = "Tarih",
                Description = "Tarih alanındaki kitaplar."
            },
            new Category
            {
                Id = 4,
                Name = "Bilim",
                Description = "Bilimsel ve popüler bilim kitapları."
            }
        );

        modelBuilder.Entity<Shelf>().HasData(
            new Shelf
            {
                Id = 1,
                Code = "A-01",
                Floor = 1,
                Section = "Roman",
                Description = "Yerli ve yabancı romanlar bölümü."
            },
            new Shelf
            {
                Id = 2,
                Code = "B-03",
                Floor = 1,
                Section = "Bilgisayar Bilimleri",
                Description = "Yazılım ve bilgisayar kitapları bölümü."
            },
            new Shelf
            {
                Id = 3,
                Code = "C-02",
                Floor = 2,
                Section = "Tarih",
                Description = "Tarih kitapları bölümü."
            },
            new Shelf
            {
                Id = 4,
                Code = "D-05",
                Floor = 2,
                Section = "Bilim",
                Description = "Bilim kitapları bölümü."
            }
        );

        modelBuilder.Entity<StudyRoom>().HasData(
            new StudyRoom
            {
                Id = 1,
                Name = "Bireysel Çalışma Odası 1",
                RoomNumber = "101",
                Capacity = 1,
                Floor = 1,
                Description = "Sessiz bireysel çalışma odası.",
                IsActive = true
            },
            new StudyRoom
            {
                Id = 2,
                Name = "Bireysel Çalışma Odası 2",
                RoomNumber = "102",
                Capacity = 1,
                Floor = 1,
                Description = "Sessiz bireysel çalışma odası.",
                IsActive = true
            },
            new StudyRoom
            {
                Id = 3,
                Name = "Bireysel Çalışma Odası 3",
                RoomNumber = "201",
                Capacity = 1,
                Floor = 2,
                Description = "Bilgisayar kullanımı için uygun oda.",
                IsActive = true
            },
            new StudyRoom
            {
                Id = 4,
                Name = "Bireysel Çalışma Odası 4",
                RoomNumber = "202",
                Capacity = 1,
                Floor = 2,
                Description = "Sessiz bireysel çalışma odası.",
                IsActive = true
            }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "1984",
                ISBN = "9789750718533",
                Description = "Distopik roman.",
                PublicationYear = 1949,
                Publisher = "Can Yayınları",
                TotalStock = 5,
                AvailableStock = 5,
                AuthorId = 1,
                CategoryId = 1,
                ShelfId = 1
            },
            new Book
            {
                Id = 2,
                Title = "Kürk Mantolu Madonna",
                ISBN = "9789753638029",
                Description = "Türk edebiyatının önemli romanlarından biri.",
                PublicationYear = 1943,
                Publisher = "Yapı Kredi Yayınları",
                TotalStock = 4,
                AvailableStock = 4,
                AuthorId = 2,
                CategoryId = 1,
                ShelfId = 1
            },
            new Book
            {
                Id = 3,
                Title = "Suç ve Ceza",
                ISBN = "9789754589023",
                Description = "Psikolojik ve felsefi roman.",
                PublicationYear = 1866,
                Publisher = "Türkiye İş Bankası Kültür Yayınları",
                TotalStock = 3,
                AvailableStock = 3,
                AuthorId = 3,
                CategoryId = 1,
                ShelfId = 1
            },
            new Book
            {
                Id = 4,
                Title = "Clean Code",
                ISBN = "9780132350884",
                Description = "Temiz ve sürdürülebilir kod yazma yöntemleri.",
                PublicationYear = 2008,
                Publisher = "Prentice Hall",
                TotalStock = 3,
                AvailableStock = 3,
                AuthorId = 4,
                CategoryId = 2,
                ShelfId = 2
            },
            new Book
            {
                Id = 5,
                Title = "Sapiens",
                ISBN = "9780062316097",
                Description = "İnsanlık tarihine genel bir bakış.",
                PublicationYear = 2015,
                Publisher = "Harper",
                TotalStock = 4,
                AvailableStock = 4,
                AuthorId = 5,
                CategoryId = 3,
                ShelfId = 3
            },
            new Book
            {
                Id = 6,
                Title = "Algoritmalara Giriş",
                ISBN = "9780262046305",
                Description = "Algoritma tasarımı ve analizine giriş.",
                PublicationYear = 2022,
                Publisher = "MIT Press",
                TotalStock = 2,
                AvailableStock = 2,
                AuthorId = 6,
                CategoryId = 2,
                ShelfId = 2
            }
        );
    }
}