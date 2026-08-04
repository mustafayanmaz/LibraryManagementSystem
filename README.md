# Library Management System

Öğrencilerin kitap arayabildiği, kitap ayırtabildiği, kitapların raf
konumlarını görüntüleyebildiği ve bireysel çalışma odaları için
rezervasyon oluşturabildiği full stack kütüphane yönetim sistemi.

## Kullanılan Teknolojiler

- ASP.NET Core Web API
- React
- Tailwind CSS
- PostgreSQL
- Entity Framework Core

## Proje Yapısı

- `backend`: ASP.NET Core Web API
- `frontend`: React kullanıcı arayüzü

## Authentication Endpoints

- `POST /api/auth/register`: Öğrenci kaydı
- `POST /api/auth/login`: Kullanıcı girişi
- `GET /api/auth/profile`: Giriş yapan kullanıcı profili
- `GET /api/auth/admin-check`: Admin rolü kontrolü

Kimlik doğrulama işlemlerinde JWT Bearer kullanılmaktadır.

## Book Management Endpoints

### Books

- `GET /api/books`: Kitapları listeleme, arama ve filtreleme
- `GET /api/books/{id}`: Kitap detaylarını görüntüleme
- `POST /api/books`: Yeni kitap ekleme
- `PUT /api/books/{id}`: Kitap güncelleme
- `DELETE /api/books/{id}`: Kitap silme

### Supporting Data

- `/api/authors`: Yazar yönetimi
- `/api/categories`: Kategori yönetimi
- `/api/shelves`: Raf yönetimi

Kitap ekleme, güncelleme ve silme işlemleri yalnızca Admin rolüne açıktır.