import { Link } from "react-router";

const features = [
  {
    title: "Kitap Ara",
    description:
      "Kitap adı, ISBN veya yazar bilgisiyle kütüphane kataloğunda arama yap.",
  },
  {
    title: "Raf Konumunu Bul",
    description:
      "Aradığın kitabın bulunduğu katı, bölümü ve raf kodunu görüntüle.",
  },
  {
    title: "Kitap Ayırt",
    description:
      "Stokta bulunan kitapları hesabın üzerinden ayırt.",
  },
  {
    title: "Oda Rezervasyonu",
    description:
      "Uygun bireysel çalışma odaları için tarih ve saat seç.",
  },
];

export default function HomePage() {
  return (
    <>
      <section className="bg-gradient-to-br from-slate-950 via-slate-900 to-blue-950 text-white">
        <div className="mx-auto grid max-w-7xl gap-12 px-5 py-20 lg:grid-cols-2 lg:items-center lg:py-28">
          <div>
            <span className="inline-flex rounded-full border border-blue-400/30 bg-blue-400/10 px-4 py-2 text-sm font-medium text-blue-200">
              Üniversite Kütüphanesi
            </span>

            <h1 className="mt-6 max-w-2xl text-4xl font-bold leading-tight sm:text-5xl">
              Kitaplara ve çalışma odalarına tek
              sistemden ulaş
            </h1>

            <p className="mt-6 max-w-xl text-lg leading-8 text-slate-300">
              Kitap ara, raf konumunu öğren, kitabını
              ayırt ve bireysel çalışma odası için
              rezervasyon oluştur.
            </p>

            <div className="mt-8 flex flex-wrap gap-4">
              <Link
                to="/books"
                className="rounded-xl bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-500"
              >
                Kitapları İncele
              </Link>

              <Link
                to="/rooms"
                className="rounded-xl border border-slate-600 px-6 py-3 font-semibold text-white transition hover:bg-white/10"
              >
                Odaları Gör
              </Link>
            </div>
          </div>

          <div className="rounded-3xl border border-white/10 bg-white/5 p-8 backdrop-blur">
            <p className="text-sm font-semibold uppercase tracking-widest text-blue-300">
              Hızlı erişim
            </p>

            <div className="mt-6 space-y-4">
              {[
                "Kitap adı, yazar ve ISBN ile arama",
                "Kategori ve stok durumuna göre filtreleme",
                "Kat, bölüm ve raf bilgisi",
                "Kullanıcı ve yönetici yetkilendirmesi",
              ].map((item) => (
                <div
                  key={item}
                  className="flex items-center gap-3 rounded-xl bg-white/5 p-4"
                >
                  <span className="flex h-7 w-7 items-center justify-center rounded-full bg-green-400/20 text-sm text-green-300">
                    ✓
                  </span>

                  <span className="text-slate-200">
                    {item}
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-5 py-16">
        <div className="max-w-2xl">
          <p className="font-semibold text-blue-600">
            Sistem özellikleri
          </p>

          <h2 className="mt-2 text-3xl font-bold text-slate-900">
            Kütüphane işlemlerini kolaylaştıran
            özellikler
          </h2>
        </div>

        <div className="mt-10 grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
          {features.map((feature, index) => (
            <article
              key={feature.title}
              className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm"
            >
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-blue-100 font-bold text-blue-700">
                {index + 1}
              </div>

              <h3 className="mt-5 text-lg font-bold text-slate-900">
                {feature.title}
              </h3>

              <p className="mt-2 text-sm leading-6 text-slate-600">
                {feature.description}
              </p>
            </article>
          ))}
        </div>
      </section>
    </>
  );
}