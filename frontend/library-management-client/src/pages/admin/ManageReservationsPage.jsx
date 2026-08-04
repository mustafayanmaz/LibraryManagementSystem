import { useEffect, useState } from "react";
import adminApi from "../../api/adminApi";
import getApiErrorMessage from "../../utils/getApiErrorMessage";

function getNextStatuses(status) {
  if (status === "Pending") {
    return ["Approved", "Cancelled", "Expired"];
  }

  if (status === "Approved") {
    return ["Completed", "Cancelled", "Expired"];
  }

  return [];
}

export default function ManageReservationsPage() {
  const [bookReservations, setBookReservations] =
    useState([]);

  const [roomReservations, setRoomReservations] =
    useState([]);

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function loadReservations() {
    const [bookData, roomData] = await Promise.all([
      adminApi.getBookReservations(),
      adminApi.getRoomReservations(),
    ]);

    setBookReservations(bookData);
    setRoomReservations(roomData);
  }

  useEffect(() => {
    loadReservations().catch((requestError) => {
      setError(
        getApiErrorMessage(
          requestError,
          "Rezervasyonlar yüklenemedi.",
        ),
      );
    });
  }, []);

  async function updateBookStatus(id, status) {
    if (!status) {
      return;
    }

    try {
      await adminApi.updateBookReservationStatus(
        id,
        status,
      );

      setMessage("Kitap rezervasyonu güncellendi.");
      await loadReservations();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Durum güncellenemedi.",
        ),
      );
    }
  }

  async function updateRoomStatus(id, status) {
    if (!status) {
      return;
    }

    try {
      await adminApi.updateRoomReservationStatus(
        id,
        status,
      );

      setMessage("Oda rezervasyonu güncellendi.");
      await loadReservations();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Durum güncellenemedi.",
        ),
      );
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-slate-900">
        Rezervasyon Yönetimi
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

      <section className="mt-8">
        <h2 className="text-xl font-bold">
          Kitap Rezervasyonları
        </h2>

        <div className="mt-4 overflow-x-auto rounded-2xl border bg-white">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50">
              <tr>
                <th className="p-4">Öğrenci</th>
                <th className="p-4">Kitap</th>
                <th className="p-4">Durum</th>
                <th className="p-4">Yeni Durum</th>
              </tr>
            </thead>

            <tbody>
              {bookReservations.map((item) => (
                <tr key={item.id} className="border-t">
                  <td className="p-4">
                    {item.studentName}
                  </td>

                  <td className="p-4">
                    {item.bookTitle}
                  </td>

                  <td className="p-4">
                    {item.status}
                  </td>

                  <td className="p-4">
                    <select
                      defaultValue=""
                      disabled={
                        getNextStatuses(item.status)
                          .length === 0
                      }
                      onChange={(event) =>
                        updateBookStatus(
                          item.id,
                          event.target.value,
                        )
                      }
                      className="rounded-lg border p-2"
                    >
                      <option value="">
                        İşlem seçin
                      </option>

                      {getNextStatuses(
                        item.status,
                      ).map((status) => (
                        <option
                          key={status}
                          value={status}
                        >
                          {status}
                        </option>
                      ))}
                    </select>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      <section className="mt-10">
        <h2 className="text-xl font-bold">
          Oda Rezervasyonları
        </h2>

        <div className="mt-4 overflow-x-auto rounded-2xl border bg-white">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50">
              <tr>
                <th className="p-4">Öğrenci</th>
                <th className="p-4">Oda</th>
                <th className="p-4">Durum</th>
                <th className="p-4">Yeni Durum</th>
              </tr>
            </thead>

            <tbody>
              {roomReservations.map((item) => (
                <tr key={item.id} className="border-t">
                  <td className="p-4">
                    {item.studentName}
                  </td>

                  <td className="p-4">
                    Oda {item.roomNumber}
                  </td>

                  <td className="p-4">
                    {item.status}
                  </td>

                  <td className="p-4">
                    <select
                      defaultValue=""
                      disabled={
                        getNextStatuses(item.status)
                          .length === 0
                      }
                      onChange={(event) =>
                        updateRoomStatus(
                          item.id,
                          event.target.value,
                        )
                      }
                      className="rounded-lg border p-2"
                    >
                      <option value="">
                        İşlem seçin
                      </option>

                      {getNextStatuses(
                        item.status,
                      ).map((status) => (
                        <option
                          key={status}
                          value={status}
                        >
                          {status}
                        </option>
                      ))}
                    </select>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}