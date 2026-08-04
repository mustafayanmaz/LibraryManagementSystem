import { useEffect, useState } from "react";
import adminApi from "../../api/adminApi";
import getApiErrorMessage from "../../utils/getApiErrorMessage";

const emptyForm = {
  title: "",
  isbn: "",
  description: "",
  publicationYear: new Date().getFullYear(),
  publisher: "",
  imageUrl: "",
  totalStock: 1,
  availableStock: 1,
  authorId: "",
  categoryId: "",
  shelfId: "",
};

export default function ManageBooksPage() {
  const [books, setBooks] = useState([]);
  const [authors, setAuthors] = useState([]);
  const [categories, setCategories] = useState([]);
  const [shelves, setShelves] = useState([]);

  const [formData, setFormData] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function loadData() {
    const [
      bookData,
      authorData,
      categoryData,
      shelfData,
    ] = await Promise.all([
      adminApi.getBooks(),
      adminApi.getAuthors(),
      adminApi.getCategories(),
      adminApi.getShelves(),
    ]);

    setBooks(bookData);
    setAuthors(authorData);
    setCategories(categoryData);
    setShelves(shelfData);
  }

  useEffect(() => {
    loadData().catch((requestError) => {
      setError(
        getApiErrorMessage(
          requestError,
          "Kitap bilgileri yüklenemedi.",
        ),
      );
    });
  }, []);

  function handleChange(event) {
    const { name, value } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: value,
    }));
  }

  function resetForm() {
    setFormData(emptyForm);
    setEditingId(null);
  }

  async function handleEdit(id) {
    setError("");
    setMessage("");

    try {
      const book = await adminApi.getBookById(id);

      setEditingId(id);

      setFormData({
        title: book.title,
        isbn: book.isbn,
        description: book.description ?? "",
        publicationYear: book.publicationYear,
        publisher: book.publisher ?? "",
        imageUrl: book.imageUrl ?? "",
        totalStock: book.totalStock,
        availableStock: book.availableStock,
        authorId: book.authorId,
        categoryId: book.categoryId,
        shelfId: book.shelfId,
      });
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Kitap bilgisi alınamadı.",
        ),
      );
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();

    setError("");
    setMessage("");

    const payload = {
      ...formData,
      publicationYear: Number(
        formData.publicationYear,
      ),
      totalStock: Number(formData.totalStock),
      availableStock: Number(
        formData.availableStock,
      ),
      authorId: Number(formData.authorId),
      categoryId: Number(formData.categoryId),
      shelfId: Number(formData.shelfId),
      imageUrl: formData.imageUrl || null,
    };

    try {
      if (editingId) {
        await adminApi.updateBook(editingId, payload);
        setMessage("Kitap güncellendi.");
      } else {
        await adminApi.createBook(payload);
        setMessage("Yeni kitap eklendi.");
      }

      resetForm();
      await loadData();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Kitap kaydedilemedi.",
        ),
      );
    }
  }

  async function handleDelete(id) {
    const confirmed = window.confirm(
      "Bu kitabı silmek istediğinizden emin misiniz?",
    );

    if (!confirmed) {
      return;
    }

    try {
      await adminApi.deleteBook(id);
      setMessage("Kitap silindi.");
      await loadData();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Kitap silinemedi.",
        ),
      );
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-slate-900">
        Kitap Yönetimi
      </h1>

      {message && (
        <div className="mt-5 rounded-xl bg-green-50 p-4 text-green-700">
          {message}
        </div>
      )}

      {error && (
        <div className="mt-5 rounded-xl bg-red-50 p-4 text-red-700">
          {error}
        </div>
      )}

      <form
        onSubmit={handleSubmit}
        className="mt-8 grid gap-4 rounded-2xl border border-slate-200 bg-white p-6 sm:grid-cols-2"
      >
        <input
          name="title"
          required
          value={formData.title}
          onChange={handleChange}
          placeholder="Kitap adı"
          className="rounded-xl border p-3"
        />

        <input
          name="isbn"
          required
          value={formData.isbn}
          onChange={handleChange}
          placeholder="ISBN"
          className="rounded-xl border p-3"
        />

        <input
          name="publisher"
          value={formData.publisher}
          onChange={handleChange}
          placeholder="Yayınevi"
          className="rounded-xl border p-3"
        />

        <input
          name="publicationYear"
          type="number"
          value={formData.publicationYear}
          onChange={handleChange}
          placeholder="Basım yılı"
          className="rounded-xl border p-3"
        />

        <input
          name="totalStock"
          type="number"
          min="0"
          value={formData.totalStock}
          onChange={handleChange}
          className="rounded-xl border p-3"
        />

        <input
          name="availableStock"
          type="number"
          min="0"
          value={formData.availableStock}
          onChange={handleChange}
          className="rounded-xl border p-3"
        />

        <select
          name="authorId"
          required
          value={formData.authorId}
          onChange={handleChange}
          className="rounded-xl border p-3"
        >
          <option value="">Yazar seçin</option>
          {authors.map((author) => (
            <option key={author.id} value={author.id}>
              {author.name}
            </option>
          ))}
        </select>

        <select
          name="categoryId"
          required
          value={formData.categoryId}
          onChange={handleChange}
          className="rounded-xl border p-3"
        >
          <option value="">Kategori seçin</option>
          {categories.map((category) => (
            <option
              key={category.id}
              value={category.id}
            >
              {category.name}
            </option>
          ))}
        </select>

        <select
          name="shelfId"
          required
          value={formData.shelfId}
          onChange={handleChange}
          className="rounded-xl border p-3"
        >
          <option value="">Raf seçin</option>
          {shelves.map((shelf) => (
            <option key={shelf.id} value={shelf.id}>
              {shelf.location}
            </option>
          ))}
        </select>

        <input
          name="imageUrl"
          value={formData.imageUrl}
          onChange={handleChange}
          placeholder="Kapak görseli adresi"
          className="rounded-xl border p-3"
        />

        <textarea
          name="description"
          value={formData.description}
          onChange={handleChange}
          placeholder="Açıklama"
          className="rounded-xl border p-3 sm:col-span-2"
        />

        <div className="flex gap-3 sm:col-span-2">
          <button
            type="submit"
            className="rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white"
          >
            {editingId ? "Güncelle" : "Kitap Ekle"}
          </button>

          {editingId && (
            <button
              type="button"
              onClick={resetForm}
              className="rounded-xl border px-5 py-3"
            >
              Vazgeç
            </button>
          )}
        </div>
      </form>

      <div className="mt-8 overflow-x-auto rounded-2xl border bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-50">
            <tr>
              <th className="p-4">Kitap</th>
              <th className="p-4">Yazar</th>
              <th className="p-4">Stok</th>
              <th className="p-4">İşlem</th>
            </tr>
          </thead>

          <tbody>
            {books.map((book) => (
              <tr
                key={book.id}
                className="border-t"
              >
                <td className="p-4 font-medium">
                  {book.title}
                </td>

                <td className="p-4">
                  {book.authorName}
                </td>

                <td className="p-4">
                  {book.availableStock}/
                  {book.totalStock}
                </td>

                <td className="p-4">
                  <div className="flex gap-2">
                    <button
                      type="button"
                      onClick={() =>
                        handleEdit(book.id)
                      }
                      className="text-blue-600"
                    >
                      Düzenle
                    </button>

                    <button
                      type="button"
                      onClick={() =>
                        handleDelete(book.id)
                      }
                      className="text-red-600"
                    >
                      Sil
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}