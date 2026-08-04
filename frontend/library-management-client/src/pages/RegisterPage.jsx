import { useState } from "react";
import { Link, useNavigate } from "react-router";
import useAuth from "../hooks/useAuth";
import getApiErrorMessage from "../utils/getApiErrorMessage";

const initialFormData = {
  firstName: "",
  lastName: "",
  studentNumber: "",
  email: "",
  password: "",
  passwordConfirmation: "",
};

export default function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();

  const [formData, setFormData] =
    useState(initialFormData);

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

    if (
      formData.password !==
      formData.passwordConfirmation
    ) {
      setError("Parola ve parola tekrarı aynı olmalıdır.");
      return;
    }

    setIsSubmitting(true);

    try {
      await register({
        firstName: formData.firstName,
        lastName: formData.lastName,
        studentNumber: formData.studentNumber,
        email: formData.email,
        password: formData.password,
      });

      navigate("/books", {
        replace: true,
      });
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Kayıt işlemi tamamlanamadı.",
        ),
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="mx-auto max-w-2xl px-5 py-16">
      <div className="rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
        <p className="font-semibold text-blue-600">
          Öğrenci kaydı
        </p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">
          Yeni hesap oluşturun
        </h1>

        {error && (
          <div className="mt-6 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            {error}
          </div>
        )}

        <form
          onSubmit={handleSubmit}
          className="mt-7 grid gap-5 sm:grid-cols-2"
        >
          <div>
            <label
              htmlFor="firstName"
              className="text-sm font-medium text-slate-700"
            >
              Ad
            </label>

            <input
              id="firstName"
              name="firstName"
              required
              minLength={2}
              value={formData.firstName}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

          <div>
            <label
              htmlFor="lastName"
              className="text-sm font-medium text-slate-700"
            >
              Soyad
            </label>

            <input
              id="lastName"
              name="lastName"
              required
              minLength={2}
              value={formData.lastName}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

          <div>
            <label
              htmlFor="studentNumber"
              className="text-sm font-medium text-slate-700"
            >
              Öğrenci numarası
            </label>

            <input
              id="studentNumber"
              name="studentNumber"
              required
              minLength={5}
              value={formData.studentNumber}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

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
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
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
              minLength={8}
              autoComplete="new-password"
              value={formData.password}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />

            <p className="mt-2 text-xs leading-5 text-slate-500">
              Büyük harf, küçük harf, rakam ve özel
              karakter kullanın.
            </p>
          </div>

          <div>
            <label
              htmlFor="passwordConfirmation"
              className="text-sm font-medium text-slate-700"
            >
              Parola tekrarı
            </label>

            <input
              id="passwordConfirmation"
              name="passwordConfirmation"
              type="password"
              required
              minLength={8}
              autoComplete="new-password"
              value={formData.passwordConfirmation}
              onChange={handleChange}
              className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-100"
            />
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:opacity-60 sm:col-span-2"
          >
            {isSubmitting
              ? "Hesap oluşturuluyor..."
              : "Hesap Oluştur"}
          </button>
        </form>

        <p className="mt-6 text-center text-sm text-slate-600">
          Zaten hesabınız var mı?{" "}
          <Link
            to="/login"
            className="font-semibold text-blue-600"
          >
            Giriş yapın
          </Link>
        </p>
      </div>
    </section>
  );
}