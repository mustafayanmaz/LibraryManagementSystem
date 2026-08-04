import { useEffect, useState } from "react";
import reservationApi from "../api/reservationApi";
import LoadingSpinner from "../components/LoadingSpinner";
import getApiErrorMessage from "../utils/getApiErrorMessage";

function getStatusClass(status) {
  const classes = {
    Pending: "bg-yellow-100 text-yellow-700",
    Approved: "bg-blue-100 text-blue-700",
    Cancelled: "bg-red-100 text-red-700",
    Completed: "bg-green-100 text-green-700",
    Expired: "bg-slate-200 text-slate-700",
  };

  return classes[status] ??
    "bg-slate-100 text-slate-700";
}

function formatDate(value) {
  return new Intl.DateTimeFormat("tr-TR").format(
    new Date(value),
  );
}

export default function MyReservationsPage() {
  const [bookReservations, setBookReservations] =
    useState([]);

  const [roomReservations, setRoomReservations] =
    useState([]);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function loadReservations() {
    setIsLoading(true);
    setError("");

    try {
      const [bookData, roomData] = await Promise.all([
        reservationApi.getMyBookReservations(),
        reservationApi.getMyRoomReservations(),
      ]);

      setBookReservations(bookData);
      setRoomReservations(roomData);
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Rezervasyonlar yüklenemedi.",
        ),
      );
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadReservations();
  }, []);

  async function cancelBookReservation(id) {
    setError("");
    setMessage("");

    try {
      await reservationApi.cancelBookReservation(id);

      setMessage(
        "Kitap rezervasyonu iptal edildi.",
      );

      await loadReservations();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Rezervasyon iptal edilemedi.",
        ),
      );
    }
  }

  async function cancelRoomReservation(id) {
    setError("");
    setMessage("");

    try {
      await reservationApi.cancelRoomReservation(id);

      setMessage(
        "Oda rezervasyonu iptal edildi.",
      );

      await loadReservations();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Rezervasyon iptal edilemedi.",
        ),
      );
    }
  }

  if (isLoading) {
    return (
      <LoadingSpinner message="Rezervasyonlar yükleniyor..." />
    );
  }

  return (
    <section className="mx-auto max-w-7xl px-5 py-12">
      <h1 className="text-3xl font-bold text-slate-900">
        Rezervasyonlarım
      </h1>

      {message && (
        <div className="mt-6 rounded-xl border border-green-200 bg-green-50 p-4 text-green-700">
          {message}
        </div>
      )}

      {error && (
        <div className="mt-6 rounded-xl border border-red-200 bg-red-50 p-4 text-red-700">
          {error}
        </div>
      )}

      <section className="mt-10">
        <h2 className="text-2xl font-bold text-slate-900">
          Kitap Rezervasyonları
        </h2>

        <div className="mt-5 space-y-4">
          {bookReservations.length === 0 ? (
            <p className="rounded-xl bg-white p-6 text-slate-500">
              Kitap rezervasyonunuz bulunmuyor.
            </p>
          ) : (
            bookReservations.map((reservation) => (
              <article
                key={reservation.id}
                className="flex flex-col justify-between gap-5 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm md:flex-row"
              >
                <div>
                  <h3 className="text-lg font-bold text-slate-900">
                    {reservation.bookTitle}
                  </h3>

                  <p className="mt-1 text-sm text-slate-600">
                    {reservation.authorName}
                  </p>

                  <p className="mt-3 text-sm text-slate-500">
                    {reservation.shelfLocation}
                  </p>

                  <p className="mt-2 text-sm text-slate-500">
                    Son tarih:{" "}
                    {formatDate(
                      reservation.expirationDate,
                    )}
                  </p>
                </div>

                <div className="flex flex-col items-start gap-3 md:items-end">
                  <span
                    className={`rounded-full px-3 py-1 text-xs font-semibold ${getStatusClass(
                      reservation.status,
                    )}`}
                  >
                    {reservation.status}
                  </span>

                  {reservation.canCancel && (
                    <button
                      type="button"
                      onClick={() =>
                        cancelBookReservation(
                          reservation.id,
                        )
                      }
                      className="rounded-lg border border-red-200 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50"
                    >
                      İptal Et
                    </button>
                  )}
                </div>
              </article>
            ))
          )}
        </div>
      </section>

      <section className="mt-12">
        <h2 className="text-2xl font-bold text-slate-900">
          Oda Rezervasyonları
        </h2>

        <div className="mt-5 space-y-4">
          {roomReservations.length === 0 ? (
            <p className="rounded-xl bg-white p-6 text-slate-500">
              Oda rezervasyonunuz bulunmuyor.
            </p>
          ) : (
            roomReservations.map((reservation) => (
              <article
                key={reservation.id}
                className="flex flex-col justify-between gap-5 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm md:flex-row"
              >
                <div>
                  <h3 className="text-lg font-bold text-slate-900">
                    {reservation.roomName}
                  </h3>

                  <p className="mt-1 text-sm text-slate-600">
                    Oda {reservation.roomNumber} ·{" "}
                    {reservation.floor}. Kat
                  </p>

                  <p className="mt-3 text-sm text-slate-500">
                    {formatDate(
                      reservation.reservationDate,
                    )}{" "}
                    · {reservation.startTime} –{" "}
                    {reservation.endTime}
                  </p>
                </div>

                <div className="flex flex-col items-start gap-3 md:items-end">
                  <span
                    className={`rounded-full px-3 py-1 text-xs font-semibold ${getStatusClass(
                      reservation.status,
                    )}`}
                  >
                    {reservation.status}
                  </span>

                  {reservation.canCancel && (
                    <button
                      type="button"
                      onClick={() =>
                        cancelRoomReservation(
                          reservation.id,
                        )
                      }
                      className="rounded-lg border border-red-200 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50"
                    >
                      İptal Et
                    </button>
                  )}
                </div>
              </article>
            ))
          )}
        </div>
      </section>
    </section>
  );
}