import { useEffect, useState } from "react";
import bookApi from "../api/bookApi";
import BookCard from "../components/BookCard";
import LoadingSpinner from "../components/LoadingSpinner";
import getApiErrorMessage from "../utils/getApiErrorMessage";

const initialFilters = {
  search: "",
  categoryId: "",
  availableOnly: false,
};

export default function BooksPage() {
  const [books, setBooks] = useState([]);
  const [categories, setCategories] = useState([]);

  const [formValues, setFormValues] =
    useState(initialFilters);

  const [filters, setFilters] =
    useState(initialFilters);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let isActive = true;

    async function loadCategories() {
      try {
        const data = await bookApi.getCategories();

        if (isActive) {
          setCategories(data);
        }
      } catch {
        if (isActive) {
          setCategories([]);
        }
      }
    }

    loadCategories();

    return () => {
      isActive = false;
    };
  }, []);

  useEffect(() => {
    let isActive = true;

    async function loadBooks() {
      setIsLoading(true);
      setError("");

      try {
        const params = {};

        if (filters.search) {
          params.search = filters.search;
        }

        if (filters.categoryId) {
          params.categoryId = filters.categoryId;
        }

        if (filters.availableOnly) {
          params.availableOnly = true;
        }

        const data = await bookApi.getBooks(params);

        if (isActive) {
          setBooks(data);
        }
      } catch (requestError) {
        if (isActive) {
          setError(
            getApiErrorMessage(
              requestError,
              "Kitaplar yüklenemedi.",
            ),
          );
        }
      } finally {
        if (isActive) {
          setIsLoading(false);
        }
      }
    }

    loadBooks();

    return () => {
      isActive = false;
    };
  }, [filters]);

  function handleChange(event) {
    const { name, value, checked, type } =
      event.target;

    setFormValues((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value,
    }));
  }

  function handleSubmit(event) {
    event.preventDefault();

    setFilters({
      search: formValues.search.trim(),
      categoryId: formValues.categoryId,
      availableOnly: formValues.availableOnly,
    });
  }

  function handleReset() {
    setFormValues(initialFilters);
    setFilters(initialFilters);
  }

  return (
    <section className="mx-auto max-w-7xl px-5 py-12">
      <div>
        <p className="font-semibold text-blue-600">
          Kütüphane kataloğu
        </p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">
          Kitapları ara ve raf konumunu öğren
        </h1>

        <p className="mt-3 text-slate-600">
          Kitap adı, ISBN veya yazar bilgisiyle arama
          yapabilirsiniz.
        </p>
      </div>

      <form
        onSubmit={handleSubmit}
        className="mt-8 grid gap-4 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm lg:grid-cols-[2fr_1fr_auto_auto]"
      >
        <input
          name="search"
          value={formValues.search}
          onChange={handleChange}
          placeholder="Kitap, ISBN veya yazar ara..."
          className="rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
        />

        <select
          name="categoryId"
          value={formValues.categoryId}
          onChange={handleChange}
          className="rounded-xl border border-slate-300 bg-white px-4 py-3 outline-none focus:border-blue-500"
        >
          <option value="">Tüm kategoriler</option>

          {categories.map((category) => (
            <option
              key={category.id}
              value={category.id}
            >
              {category.name}
            </option>
          ))}
        </select>

        <label className="flex items-center gap-3 rounded-xl border border-slate-300 px-4 py-3 text-sm font-medium text-slate-700">
          <input
            name="availableOnly"
            type="checkbox"
            checked={formValues.availableOnly}
            onChange={handleChange}
            className="h-4 w-4"
          />
          Yalnızca müsait
        </label>

        <button
          type="submit"
          className="rounded-xl bg-blue-600 px-6 py-3 font-semibold text-white transition hover:bg-blue-700"
        >
          Ara
        </button>

        <button
          type="button"
          onClick={handleReset}
          className="text-left text-sm font-medium text-slate-500 hover:text-slate-800 lg:col-span-4"
        >
          Filtreleri temizle
        </button>
      </form>

      {error && (
        <div className="mt-8 rounded-xl border border-red-200 bg-red-50 p-4 text-red-700">
          {error}
        </div>
      )}

      {isLoading ? (
        <LoadingSpinner message="Kitaplar yükleniyor..." />
      ) : books.length === 0 ? (
        <div className="mt-10 rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center">
          <h2 className="text-xl font-bold text-slate-800">
            Kitap bulunamadı
          </h2>

          <p className="mt-2 text-slate-500">
            Arama bilgilerinizi değiştirerek tekrar
            deneyin.
          </p>
        </div>
      ) : (
        <>
          <p className="mt-8 text-sm text-slate-500">
            {books.length} kitap bulundu.
          </p>

          <div className="mt-5 grid gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {books.map((book) => (
              <BookCard key={book.id} book={book} />
            ))}
          </div>
        </>
      )}
    </section>
  );
}