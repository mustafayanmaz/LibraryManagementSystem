import { useEffect, useState } from "react";
import roomApi from "../api/roomApi";
import LoadingSpinner from "../components/LoadingSpinner";
import RoomCard from "../components/RoomCard";
import getApiErrorMessage from "../utils/getApiErrorMessage";

export default function RoomsPage() {
  const [rooms, setRooms] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

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

  return (
    <section className="mx-auto max-w-7xl px-5 py-12">
      <div>
        <p className="font-semibold text-blue-600">
          Bireysel çalışma alanları
        </p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">
          Çalışma odaları
        </h1>

        <p className="mt-3 max-w-2xl leading-7 text-slate-600">
          Kütüphanedeki aktif çalışma odalarının kat,
          oda numarası ve kapasite bilgilerini
          inceleyebilirsiniz.
        </p>
      </div>

      {error && (
        <div className="mt-8 rounded-xl border border-red-200 bg-red-50 p-4 text-red-700">
          {error}
        </div>
      )}

      {isLoading ? (
        <LoadingSpinner message="Odalar yükleniyor..." />
      ) : rooms.length === 0 ? (
        <div className="mt-10 rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-500">
          Kullanıma açık çalışma odası bulunmuyor.
        </div>
      ) : (
        <div className="mt-8 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {rooms.map((room) => (
            <RoomCard key={room.id} room={room} />
          ))}
        </div>
      )}
    </section>
  );
}