import { Link } from "react-router";

export default function BookCard({ book }) {
  return (
    <article className="flex h-full flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-md">
      <div className="flex h-44 items-center justify-center bg-gradient-to-br from-blue-50 to-slate-100">
        <span className="px-5 text-center text-2xl font-bold text-blue-700">
          {book.title}
        </span>
      </div>

      <div className="flex flex-1 flex-col p-5">
        <div className="mb-3">
          <span
            className={[
              "inline-flex rounded-full px-3 py-1 text-xs font-semibold",
              book.isAvailable
                ? "bg-green-100 text-green-700"
                : "bg-red-100 text-red-700",
            ].join(" ")}
          >
            {book.isAvailable
              ? `${book.availableStock} adet müsait`
              : "Stokta yok"}
          </span>
        </div>

        <h2 className="text-lg font-bold text-slate-900">
          {book.title}
        </h2>

        <p className="mt-1 text-sm text-slate-600">
          {book.authorName}
        </p>

        <dl className="mt-5 space-y-2 text-sm">
          <div className="flex justify-between gap-3">
            <dt className="text-slate-500">Kategori</dt>
            <dd className="text-right font-medium text-slate-700">
              {book.categoryName}
            </dd>
          </div>

          <div className="flex justify-between gap-3">
            <dt className="text-slate-500">Raf</dt>
            <dd className="text-right font-medium text-slate-700">
              {book.floor}. Kat · {book.shelfCode}
            </dd>
          </div>
        </dl>

        <Link
          to={`/books/${book.id}`}
          className="mt-6 block rounded-xl bg-slate-900 px-4 py-3 text-center text-sm font-semibold text-white transition hover:bg-slate-700"
        >
          Kitap Detayını Gör
        </Link>
      </div>
    </article>
  );
}