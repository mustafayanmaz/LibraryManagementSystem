import { useState } from "react";
import {
  Link,
  useLocation,
  useNavigate,
} from "react-router";
import useAuth from "../hooks/useAuth";
import getApiErrorMessage from "../utils/getApiErrorMessage";

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] =
    useState(false);

  function handleChange(event) {
    const { name, value } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();

    setError("");
    setIsSubmitting(true);

    try {
      await login(formData);

      const destination =
        location.state?.from?.pathname ?? "/";

      navigate(destination, {
        replace: true,
      });
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Giriş işlemi tamamlanamadı.",
        ),
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="mx-auto max-w-md px-5 py-16">
      <div className="rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
        <p className="font-semibold text-blue-600">
          Hoş geldiniz
        </p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">
          Hesabınıza giriş yapın
        </h1>

        <p className="mt-3 text-sm leading-6 text-slate-600">
          Kitap ve oda işlemlerinize ulaşmak için
          bilgilerinizi girin.
        </p>

        {error && (
          <div className="mt-6 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            {error}
          </div>
        )}

        <form
          onSubmit={handleSubmit}
          className="mt-7 space-y-5"
        >
          <div>
            <label
              htmlFor="email"
              className="text-sm font-medium text-slate-700"
            >
              E-posta adresi
            </label>

            <input
              id="email"
              name="email"
              type="email"
              required
              autoComplete="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="ogrenci@library.local"
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

          <div>
            <label
              htmlFor="password"
              className="text-sm font-medium text-slate-700"
            >
              Parola
            </label>

            <input
              id="password"
              name="password"
              type="password"
              required
              autoComplete="current-password"
              value={formData.password}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
          >
            {isSubmitting
              ? "Giriş yapılıyor..."
              : "Giriş Yap"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-slate-600">
          Henüz hesabınız yok mu?{" "}
          <Link
            to="/register"
            className="font-semibold text-blue-600 hover:text-blue-700"
          >
            Kayıt olun
          </Link>
        </p>
      </div>
    </section>
  );
}