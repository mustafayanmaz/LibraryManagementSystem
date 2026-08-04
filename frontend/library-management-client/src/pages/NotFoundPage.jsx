import { Link } from "react-router";

export default function NotFoundPage() {
  return (
    <section className="mx-auto max-w-xl px-5 py-24 text-center">
      <p className="text-7xl font-bold text-blue-600">
        404
      </p>

      <h1 className="mt-5 text-3xl font-bold text-slate-900">
        Sayfa bulunamadı
      </h1>

      <p className="mt-3 text-slate-600">
        Aradığınız sayfa kaldırılmış veya adresi
        değiştirilmiş olabilir.
      </p>

      <Link
        to="/"
        className="mt-7 inline-block rounded-xl bg-blue-600 px-6 py-3 font-semibold text-white"
      >
        Ana sayfaya dön
      </Link>
    </section>
  );
}