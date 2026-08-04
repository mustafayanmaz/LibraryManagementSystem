import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import roomApi from "../api/roomApi";
import reservationApi from "../api/reservationApi";
import LoadingSpinner from "../components/LoadingSpinner";
import RoomCard from "../components/RoomCard";
import useAuth from "../hooks/useAuth";
import getApiErrorMessage from "../utils/getApiErrorMessage";

function getTomorrowDate() {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  return tomorrow.toISOString().split("T")[0];
}

function normalizeTime(value) {
  return value.length === 5 ? `${value}:00` : value;
}

export default function RoomsPage() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [rooms, setRooms] = useState([]);
  const [availableRooms, setAvailableRooms] =
    useState([]);

  const [formData, setFormData] = useState({
    date: getTomorrowDate(),
    startTime: "10:00",
    endTime: "11:00",
  });

  const [searchedPeriod, setSearchedPeriod] =
    useState(null);

  const [isLoading, setIsLoading] = useState(true);
  const [isSearching, setIsSearching] =
    useState(false);

  const [reservingRoomId, setReservingRoomId] =
    useState(null);

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    let isActive = true;

    async function loadRooms() {
      try {
        const data = await roomApi.getRooms(true);

        if (isActive) {
          setRooms(data);
        }
      } catch (requestError) {
        if (isActive) {
          setError(
            getApiErrorMessage(
              requestError,
              "Çalışma odaları yüklenemedi.",
            ),
          );
        }
      } finally {
        if (isActive) {
          setIsLoading(false);
        }
      }
    }

    loadRooms();

    return () => {
      isActive = false;
    };
  }, []);

  function handleChange(event) {
    const { name, value } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: value,
    }));
  }

  async function searchAvailableRooms(event) {
    event.preventDefault();

    if (!isAuthenticated) {
      navigate("/login", {
        state: {
          from: {
            pathname: "/rooms",
          },
        },
      });

      return;
    }

    setIsSearching(true);
    setError("");
    setMessage("");

    try {
      const normalizedStartTime =
        normalizeTime(formData.startTime);

      const normalizedEndTime =
        normalizeTime(formData.endTime);

      const data = await roomApi.getAvailableRooms(
        formData.date,
        normalizedStartTime,
        normalizedEndTime,
      );

      setAvailableRooms(data);

      setSearchedPeriod({
        date: formData.date,
        startTime: normalizedStartTime,
        endTime: normalizedEndTime,
      });
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Uygun odalar sorgulanamadı.",
        ),
      );
    } finally {
      setIsSearching(false);
    }
  }

  async function handleReserveRoom(roomId) {
    if (!searchedPeriod) {
      return;
    }

    setReservingRoomId(roomId);
    setError("");
    setMessage("");

    try {
      await reservationApi.reserveRoom({
        studyRoomId: roomId,
        reservationDate: searchedPeriod.date,
        startTime: searchedPeriod.startTime,
        endTime: searchedPeriod.endTime,
      });

      setMessage(
        "Oda rezervasyonu başarıyla oluşturuldu.",
      );

      setAvailableRooms((currentRooms) =>
        currentRooms.filter((room) => room.id !== roomId),
      );
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Oda rezervasyonu oluşturulamadı.",
        ),
      );
    } finally {
      setReservingRoomId(null);
    }
  }

  return (
    <section className="mx-auto max-w-7xl px-5 py-12">
      <div>
        <p className="font-semibold text-blue-600">
          Bireysel çalışma alanları
        </p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">
          Çalışma odası rezervasyonu
        </h1>

        <p className="mt-3 max-w-2xl leading-7 text-slate-600">
          Tarih ve saat seçerek uygun çalışma
          odalarını görüntüleyebilirsiniz.
        </p>
      </div>

      <form
        onSubmit={searchAvailableRooms}
        className="mt-8 grid gap-4 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm md:grid-cols-4"
      >
        <div>
          <label className="text-sm font-medium text-slate-700">
            Tarih
          </label>

          <input
            name="date"
            type="date"
            required
            value={formData.date}
            onChange={handleChange}
            className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Başlangıç
          </label>

          <input
            name="startTime"
            type="time"
            required
            value={formData.startTime}
            onChange={handleChange}
            className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3"
          />
        </div>

        <div>
          <label className="text-sm font-medium text-slate-700">
            Bitiş
          </label>

          <input
            name="endTime"
            type="time"
            required
            value={formData.endTime}
            onChange={handleChange}
            className="mt-2 w-full rounded-xl border border-slate-300 px-4 py-3"
          />
        </div>

        <button
          type="submit"
          disabled={isSearching}
          className="self-end rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white disabled:bg-slate-300"
        >
          {isSearching
            ? "Sorgulanıyor..."
            : "Uygun Odaları Bul"}
        </button>
      </form>

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

      {isLoading ? (
        <LoadingSpinner message="Odalar yükleniyor..." />
      ) : searchedPeriod ? (
        <>
          <h2 className="mt-10 text-2xl font-bold text-slate-900">
            Uygun Odalar
          </h2>

          {availableRooms.length === 0 ? (
            <div className="mt-5 rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-500">
              Seçilen zaman aralığında uygun oda
              bulunamadı.
            </div>
          ) : (
            <div className="mt-6 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {availableRooms.map((room) => (
                <RoomCard
                  key={room.id}
                  room={room}
                  showReserveButton
                  onReserve={handleReserveRoom}
                  isReserving={
                    reservingRoomId === room.id
                  }
                />
              ))}
            </div>
          )}
        </>
      ) : (
        <>
          <h2 className="mt-10 text-2xl font-bold text-slate-900">
            Aktif Çalışma Odaları
          </h2>

          <div className="mt-6 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {rooms.map((room) => (
              <RoomCard key={room.id} room={room} />
            ))}
          </div>
        </>
      )}
    </section>
  );
}