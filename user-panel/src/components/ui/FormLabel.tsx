export function FormLabel({
  htmlFor,
  children,
  required,
}: {
  htmlFor?: string;
  children: React.ReactNode;
  required?: boolean;
}) {
  return (
    <label htmlFor={htmlFor} className="block text-sm font-medium text-slate-700">
      {children}
      {required && <span className="ml-0.5 text-red-500" aria-hidden="true">*</span>}
      {required && <span className="sr-only"> (required)</span>}
    </label>
  );
}
