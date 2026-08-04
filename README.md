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

## Book Reservation Endpoints

- `POST /api/book-reservations`: Kitap rezervasyonu oluşturma
- `GET /api/book-reservations/my`: Kullanıcının rezervasyonları
- `GET /api/book-reservations/{id}`: Rezervasyon detayı
- `PUT /api/book-reservations/{id}/cancel`: Rezervasyon iptali
- `GET /api/book-reservations`: Tüm rezervasyonlar (Admin)
- `PUT /api/book-reservations/{id}/status`: Durum güncelleme (Admin)

Aktif rezervasyon oluşturulduğunda kullanılabilir kitap stoğu azaltılır.
İptal edilen, tamamlanan veya süresi dolan rezervasyonlarda stok yeniden artırılır.

## Study Room Endpoints

- `GET /api/study-rooms`: Çalışma odalarını listeleme
- `GET /api/study-rooms/available`: Uygun odaları sorgulama
- `POST /api/study-rooms`: Oda ekleme (Admin)
- `PUT /api/study-rooms/{id}`: Oda güncelleme (Admin)
- `DELETE /api/study-rooms/{id}`: Oda silme (Admin)

## Room Reservation Endpoints

- `POST /api/room-reservations`: Oda rezervasyonu oluşturma
- `GET /api/room-reservations/my`: Kullanıcının rezervasyonları
- `PUT /api/room-reservations/{id}/cancel`: Rezervasyon iptali
- `GET /api/room-reservations`: Tüm rezervasyonlar (Admin)
- `PUT /api/room-reservations/{id}/status`: Durum güncelleme (Admin)

Çakışan oda ve kullanıcı rezervasyonları engellenmektedir.
Rezervasyonlar 08.00–22.00 saatleri arasında ve en fazla iki saat
olacak şekilde oluşturulabilir.

## Frontend

Frontend uygulaması React, Vite ve Tailwind CSS ile
geliştirilmiştir.

### Frontend Sayfaları

- `/`: Ana sayfa
- `/login`: Kullanıcı girişi
- `/register`: Öğrenci kaydı
- `/books`: Kitap arama ve filtreleme
- `/books/{id}`: Kitap detayları ve raf konumu
- `/rooms`: Aktif çalışma odaları

### Frontend Kurulumu

```bash
cd frontend/library-management-client
npm install
npm run dev