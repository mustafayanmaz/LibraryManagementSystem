import { useEffect, useState } from "react";
import adminApi from "../../api/adminApi";
import getApiErrorMessage from "../../utils/getApiErrorMessage";

const emptyForm = {
  name: "",
  roomNumber: "",
  capacity: 1,
  floor: 1,
  description: "",
  isActive: true,
};

export default function ManageRoomsPage() {
  const [rooms, setRooms] = useState([]);
  const [formData, setFormData] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function loadRooms() {
    setRooms(await adminApi.getRooms());
  }

  useEffect(() => {
    loadRooms().catch((requestError) => {
      setError(
        getApiErrorMessage(
          requestError,
          "Odalar yüklenemedi.",
        ),
      );
    });
  }, []);

  function handleChange(event) {
    const {
      name,
      value,
      type,
      checked,
    } = event.target;

    setFormData((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value,
    }));
  }

  function editRoom(room) {
    setEditingId(room.id);

    setFormData({
      name: room.name,
      roomNumber: room.roomNumber,
      capacity: room.capacity,
      floor: room.floor,
      description: room.description ?? "",
      isActive: room.isActive,
    });
  }

  function resetForm() {
    setEditingId(null);
    setFormData(emptyForm);
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const payload = {
      ...formData,
      capacity: Number(formData.capacity),
      floor: Number(formData.floor),
    };

    try {
      if (editingId) {
        await adminApi.updateRoom(editingId, payload);
        setMessage("Çalışma odası güncellendi.");
      } else {
        await adminApi.createRoom(payload);
        setMessage("Çalışma odası eklendi.");
      }

      resetForm();
      await loadRooms();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Oda kaydedilemedi.",
        ),
      );
    }
  }

  async function deleteRoom(id) {
    if (!window.confirm("Oda silinsin mi?")) {
      return;
    }

    try {
      await adminApi.deleteRoom(id);
      setMessage("Oda silindi.");
      await loadRooms();
    } catch (requestError) {
      setError(
        getApiErrorMessage(
          requestError,
          "Oda silinemedi.",
        ),
      );
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold text-slate-900">
        Oda Yönetimi
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
        className="mt-8 grid gap-4 rounded-2xl border bg-white p-6 sm:grid-cols-2"
      >
        <input
          name="name"
          required
          value={formData.name}
          onChange={handleChange}
          placeholder="Oda adı"
          className="rounded-xl border p-3"
        />

        <input
          name="roomNumber"
          required
          value={formData.roomNumber}
          onChange={handleChange}
          placeholder="Oda numarası"
          className="rounded-xl border p-3"
        />

        <input
          name="capacity"
          type="number"
          min="1"
          value={formData.capacity}
          onChange={handleChange}
          className="rounded-xl border p-3"
        />

        <input
          name="floor"
          type="number"
          min="0"
          value={formData.floor}
          onChange={handleChange}
          className="rounded-xl border p-3"
        />

        <textarea
          name="description"
          value={formData.description}
          onChange={handleChange}
          placeholder="Açıklama"
          className="rounded-xl border p-3 sm:col-span-2"
        />

        <label className="flex items-center gap-3">
          <input
            name="isActive"
            type="checkbox"
            checked={formData.isActive}
            onChange={handleChange}
          />
          Oda kullanıma açık
        </label>

        <div className="flex gap-3 sm:col-span-2">
          <button
            type="submit"
            className="rounded-xl bg-blue-600 px-5 py-3 font-semibold text-white"
          >
            {editingId ? "Güncelle" : "Oda Ekle"}
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

      <div className="mt-8 grid gap-5 sm:grid-cols-2">
        {rooms.map((room) => (
          <article
            key={room.id}
            className="rounded-2xl border bg-white p-5"
          >
            <h2 className="font-bold text-slate-900">
              {room.name}
            </h2>

            <p className="mt-2 text-sm text-slate-600">
              Oda {room.roomNumber} · {room.floor}. Kat
            </p>

            <p className="mt-2 text-sm">
              {room.isActive ? "Aktif" : "Pasif"}
            </p>

            <div className="mt-4 flex gap-3">
              <button
                type="button"
                onClick={() => editRoom(room)}
                className="text-blue-600"
              >
                Düzenle
              </button>

              <button
                type="button"
                onClick={() => deleteRoom(room.id)}
                className="text-red-600"
              >
                Sil
              </button>
            </div>
          </article>
        ))}
      </div>
    </div>
  );
}