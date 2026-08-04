function App() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-100 px-6">
      <section className="w-full max-w-2xl rounded-2xl bg-white p-10 shadow-lg">
        <span className="text-sm font-semibold uppercase tracking-wider text-blue-600">
          Library Management System
        </span>

        <h1 className="mt-3 text-4xl font-bold text-slate-900">
          Kütüphane Yönetim Sistemi
        </h1>

        <p className="mt-4 leading-7 text-slate-600">
          Öğrencilerin kitap arayabileceği, kitap ayırtabileceği ve bireysel
          çalışma odaları için rezervasyon oluşturabileceği full stack
          kütüphane uygulaması.
        </p>

        <div className="mt-8 flex flex-wrap gap-3">
          <span className="rounded-full bg-blue-100 px-4 py-2 text-sm font-medium text-blue-700">
            React
          </span>

          <span className="rounded-full bg-purple-100 px-4 py-2 text-sm font-medium text-purple-700">
            ASP.NET Core
          </span>

          <span className="rounded-full bg-cyan-100 px-4 py-2 text-sm font-medium text-cyan-700">
            Tailwind CSS
          </span>

          <span className="rounded-full bg-indigo-100 px-4 py-2 text-sm font-medium text-indigo-700">
            PostgreSQL
          </span>
        </div>

        <div className="mt-8 rounded-xl border border-green-200 bg-green-50 p-4">
          <p className="font-medium text-green-700">
            ✓ Frontend kurulumu başarıyla tamamlandı.
          </p>
        </div>
      </section>
    </main>
  );
}

export default App;