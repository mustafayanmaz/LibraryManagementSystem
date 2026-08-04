export default function LoadingSpinner({
  message = "Yükleniyor...",
}) {
  return (
    <div className="flex min-h-64 flex-col items-center justify-center gap-4">
      <div className="h-10 w-10 animate-spin rounded-full border-4 border-slate-200 border-t-blue-600" />

      <p className="text-sm text-slate-500">{message}</p>
    </div>
  );
}