import {
  Link,
  NavLink,
  useNavigate,
} from "react-router";
import useAuth from "../hooks/useAuth";

function getNavLinkClass({ isActive }) {
  return [
    "rounded-lg px-3 py-2 text-sm font-medium transition",
    isActive
      ? "bg-blue-50 text-blue-700"
      : "text-slate-600 hover:bg-slate-100 hover:text-slate-900",
  ].join(" ");
}

export default function Navbar() {
  const navigate = useNavigate();

  const {
    user,
    isAuthenticated,
    isAdmin,
    isInitializing,
    logout,
  } = useAuth();

  function handleLogout() {
    logout();
    navigate("/");
  }

  return (
    <header className="border-b border-slate-200 bg-white">
      <div className="mx-auto flex max-w-7xl flex-wrap items-center justify-between gap-4 px-5 py-4">
        <Link
          to="/"
          className="flex items-center gap-3"
        >
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-lg font-bold text-white">
            K
          </div>

          <div>
            <p className="font-bold text-slate-900">
              Kütüphane Sistemi
            </p>

            <p className="text-xs text-slate-500">
              Üniversite Kütüphanesi
            </p>
          </div>
        </Link>

        <nav className="flex flex-wrap items-center gap-1">
          <NavLink to="/" className={getNavLinkClass} end>
            Ana Sayfa
          </NavLink>

          <NavLink
            to="/books"
            className={getNavLinkClass}
          >
            Kitaplar
          </NavLink>

          <NavLink
            to="/rooms"
            className={getNavLinkClass}
          >
            Çalışma Odaları
          </NavLink>
        </nav>

        <div className="flex items-center gap-3">
          {isInitializing ? (
            <span className="text-sm text-slate-500">
              Oturum kontrol ediliyor...
            </span>
          ) : isAuthenticated ? (
            <>
              <div className="hidden text-right sm:block">
                <p className="text-sm font-semibold text-slate-800">
                  {user.firstName} {user.lastName}
                </p>

                <p className="text-xs text-slate-500">
                  {isAdmin ? "Yönetici" : "Öğrenci"}
                </p>
              </div>

              <button
                type="button"
                onClick={handleLogout}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100"
              >
                Çıkış Yap
              </button>
            </>
          ) : (
            <>
              <Link
                to="/login"
                className="rounded-lg px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100"
              >
                Giriş Yap
              </Link>

              <Link
                to="/register"
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-blue-700"
              >
                Kayıt Ol
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}