interface CardProps {
  title?: string;
  children: React.ReactNode;
  action?: React.ReactNode;
  className?: string;
}

export function Card({ title, children, action, className = "" }: CardProps) {
  return (
    <div className={`rounded-xl border border-slate-200 bg-white shadow-sm ${className}`}>
      {(title || action) && (
        <div className="flex items-center justify-between border-b border-slate-100 px-6 py-4">
          {title && <h2 className="text-lg font-semibold text-slate-800">{title}</h2>}
          {action}
        </div>
      )}
      <div className="p-6">{children}</div>
    </div>
  );
}
