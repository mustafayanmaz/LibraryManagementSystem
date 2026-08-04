import { NavLink, Outlet } from "react-router";

function getLinkClass({ isActive }) {
  return [
    "rounded-xl px-4 py-3 text-sm font-medium",
    isActive
      ? "bg-blue-600 text-white"
      : "text-slate-600 hover:bg-slate-100",
  ].join(" ");
}

export default function AdminLayout() {
  return (
    <section className="mx-auto grid max-w-7xl gap-8 px-5 py-10 lg:grid-cols-[230px_1fr]">
      <aside className="h-fit rounded-2xl border border-slate-200 bg-white p-4">
        <p className="px-4 py-2 text-sm font-bold text-slate-900">
          Yönetici Paneli
        </p>

        <nav className="mt-2 flex flex-col gap-1">
          <NavLink
            to="/admin"
            end
            className={getLinkClass}
          >
            Genel Bakış
          </NavLink>

          <NavLink
            to="/admin/books"
            className={getLinkClass}
          >
            Kitap Yönetimi
          </NavLink>

          <NavLink
            to="/admin/rooms"
            className={getLinkClass}
          >
            Oda Yönetimi
          </NavLink>

          <NavLink
            to="/admin/reservations"
            className={getLinkClass}
          >
            Rezervasyonlar
          </NavLink>
        </nav>
      </aside>

      <div>
        <Outlet />
      </div>
    </section>
  );
}