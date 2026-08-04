export default function RoomCard({
  room,
  onReserve,
  isReserving = false,
  showReserveButton = false,
}) {
  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <div className="flex items-start justify-between gap-4">
        <div>
          <span className="text-sm font-semibold text-blue-600">
            Oda {room.roomNumber}
          </span>

          <h2 className="mt-1 text-xl font-bold text-slate-900">
            {room.name}
          </h2>
        </div>

        <span className="rounded-full bg-green-100 px-3 py-1 text-xs font-semibold text-green-700">
          Uygun
        </span>
      </div>

      <p className="mt-4 min-h-12 text-sm leading-6 text-slate-600">
        {room.description ??
          "Bireysel çalışma için uygun sessiz oda."}
      </p>

      <div className="mt-5 grid grid-cols-2 gap-3">
        <div className="rounded-xl bg-slate-50 p-3">
          <p className="text-xs text-slate-500">
            Kat
          </p>

          <p className="mt-1 font-semibold text-slate-800">
            {room.floor}. Kat
          </p>
        </div>

        <div className="rounded-xl bg-slate-50 p-3">
          <p className="text-xs text-slate-500">
            Kapasite
          </p>

          <p className="mt-1 font-semibold text-slate-800">
            {room.capacity} kişi
          </p>
        </div>
      </div>

      {showReserveButton && (
        <button
          type="button"
          onClick={() => onReserve(room.id)}
          disabled={isReserving}
          className="mt-5 w-full rounded-xl bg-blue-600 px-4 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:bg-slate-300"
        >
          {isReserving
            ? "Oluşturuluyor..."
            : "Bu Odayı Ayırt"}
        </button>
      )}
    </article>
  );
}