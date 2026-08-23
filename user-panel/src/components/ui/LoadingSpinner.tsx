"use client";

export function LoadingSpinner({ fullPage }: { fullPage?: boolean }) {
  const spinner = (
    <div className="flex items-center justify-center">
      <div className="h-8 w-8 animate-spin rounded-full border-4 border-rose-200 border-t-rose-600" />
    </div>
  );

  if (fullPage) {
    return <div className="flex min-h-screen items-center justify-center">{spinner}</div>;
  }

  return <div className="py-12">{spinner}</div>;
}

export function EmptyState({ message }: { message: string }) {
  return (
    <div className="py-12 text-center text-slate-500">
      <p>{message}</p>
    </div>
  );
}

export function Alert({ message, type = "error" }: { message: string; type?: "error" | "success" }) {
  const styles = type === "error" ? "bg-red-50 text-red-700 border-red-200" : "bg-green-50 text-green-700 border-green-200";
  return (
    <div className={`mb-4 rounded-lg border px-4 py-3 text-sm ${styles}`}>{message}</div>
  );
}
