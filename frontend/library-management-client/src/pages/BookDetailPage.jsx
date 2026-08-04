import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import bookApi from "../api/bookApi";
import LoadingSpinner from "../components/LoadingSpinner";
import getApiErrorMessage from "../utils/getApiErrorMessage";

export default function BookDetailPage() {
  const { id } = useParams();

  const [book, setBook] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isActive = true;

    async function loadBook() {
      setIsLoading(true);
      setError("");

      try {
        const data = await bookApi.getBookById(id);

        if (isActive) {
          setBook(data);
        }
      } catch (requestError) {
        if (isActive) {
          setError(
            getApiErrorMessage(
              requestError,
              "Kitap bilgileri yüklenemedi.",
            ),
          );
        }
      } finally {
        if (isActive) {
          setIsLoading(false);
        }
      }
    }

    loadBook();

    return () => {
      isActive = false;
    };
  }, [id]);

  if (isLoading) {
    return (
      <LoadingSpinner message="Kitap bilgileri yükleniyor..." />
    );
  }

  if (error || !book) {
    return (
      <section className="mx-auto max-w-4xl px-5 py-16">
        <div className="rounded-2xl border border-red-200 bg-red-50 p-8 text-center">
          <p className="text-red-700">
            {error || "Kitap bulunamadı."}
          </p>

          <Link
            to="/books"
            className="mt-5 inline-block font-semibold text-blue-600"
          >
            Kitaplara dön
          </Link>
        </div>
      </section>
    );
  }

  return (
    <section className="mx-auto max-w-6xl px-5 py-12">
      <Link
        to="/books"
        className="text-sm font-semibold text-blue-600 hover:text-blue-700"
      >
        ← Kitaplara dön
      </Link>

      <div className="mt-6 grid gap-10 rounded-3xl border border-slate-200 bg-white p-7 shadow-sm md:grid-cols-[280px_1fr]">
        <div className="flex min-h-96 items-center justify-center rounded-2xl bg-gradient-to-br from-blue-50 to-slate-200 p-8">
          <span className="text-center text-3xl font-bold text-blue-700">
            {book.title}
          </span>
        </div>

        <div>
          <span
            className={[
              "inline-flex rounded-full px-3 py-1 text-sm font-semibold",
              book.isAvailable
                ? "bg-green-100 text-green-700"
                : "bg-red-100 text-red-700",
            ].join(" ")}
          >
            {book.isAvailable
              ? `${book.availableStock} kitap müsait`
              : "Stokta bulunmuyor"}
          </span>

          <h1 className="mt-5 text-4xl font-bold text-slate-900">
            {book.title}
          </h1>

          <p className="mt-2 text-lg text-slate-600">
            {book.authorName}
          </p>

          <p className="mt-6 leading-7 text-slate-600">
            {book.description ??
              "Bu kitap için açıklama bulunmuyor."}
          </p>

          <dl className="mt-8 grid gap-4 sm:grid-cols-2">
            {[
              ["ISBN", book.isbn],
              ["Kategori", book.categoryName],
              ["Yayınevi", book.publisher ?? "-"],
              ["Basım yılı", book.publicationYear],
              ["Toplam stok", book.totalStock],
              ["Kullanılabilir stok", book.availableStock],
            ].map(([label, value]) => (
              <div
                key={label}
                className="rounded-xl bg-slate-50 p-4"
              >
                <dt className="text-xs font-medium uppercase tracking-wide text-slate-500">
                  {label}
                </dt>

                <dd className="mt-1 font-semibold text-slate-800">
                  {value}
                </dd>
              </div>
            ))}
          </dl>

          <div className="mt-6 rounded-2xl border border-blue-200 bg-blue-50 p-5">
            <p className="text-sm font-medium text-blue-600">
              Kitabın konumu
            </p>

            <p className="mt-2 text-lg font-bold text-blue-950">
              {book.shelfLocation}
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}