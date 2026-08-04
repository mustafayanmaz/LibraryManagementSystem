import { useEffect, useState } from "react";
import adminApi from "../../api/adminApi";
import LoadingSpinner from "../../components/LoadingSpinner";
import getApiErrorMessage from "../../utils/getApiErrorMessage";

export default function AdminDashboardPage() {
  const [statistics, setStatistics] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    let isActive = true;

    async function loadStatistics() {
      try {
        const [
          books,
          rooms,
          bookReservations,
          roomReservations,
        ] = await Promise.all([
          adminApi.getBooks(),
          adminApi.getRooms(),
          adminApi.getBookReservations(),
          adminApi.getRoomReservations(),
        ]);

        if (isActive) {
          setStatistics({
            books: books.length,
            rooms: rooms.length,
            pendingBookReservations:
              bookReservations.filter(
                (item) => item.status === "Pending",
              ).length,
            pendingRoomReservations:
              roomReservations.filter(
                (item) => item.status === "Pending",
              ).length,
          });
        }
      } catch (requestError) {
        if (isActive) {
          setError(
            getApiErrorMessage(
              requestError,
              "Yönetici bilgileri yüklenemedi.",
            ),
          );
        }
      }
    }

    loadStatistics();

    return () => {
      isActive = false;
    };
  }, []);

  if (!statistics && !error) {
    return (
      <LoadingSpinner message="Panel yükleniyor..." />
    );
  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-slate-900">
        Genel Bakış
      </h1>

      {error && (
        <div className="mt-6 rounded-xl bg-red-50 p-4 text-red-700">
          {error}
        </div>
      )}

      {statistics && (
        <div className="mt-8 grid gap-5 sm:grid-cols-2">
          {[
            ["Toplam Kitap", statistics.books],
            ["Çalışma Odası", statistics.rooms],
            [
              "Bekleyen Kitap Rezervasyonu",
              statistics.pendingBookReservations,
            ],
            [
              "Bekleyen Oda Rezervasyonu",
              statistics.pendingRoomReservations,
            ],
          ].map(([label, value]) => (
            <article
              key={label}
              className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm"
            >
              <p className="text-sm text-slate-500">
                {label}
              </p>

              <p className="mt-3 text-4xl font-bold text-blue-600">
                {value}
              </p>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}