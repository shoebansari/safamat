import { FormLabel } from "./FormLabel";

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  requiredMark?: boolean;
}

export function Textarea({ label, error, requiredMark, className = "", id, required, ...props }: TextareaProps) {
  const textareaId = id || label?.toLowerCase().replace(/\s/g, "-");
  const showRequired = requiredMark ?? required;

  return (
    <div className="space-y-1">
      {label && (
        <FormLabel htmlFor={textareaId} required={showRequired}>
          {label}
        </FormLabel>
      )}
      <textarea
        id={textareaId}
        required={required}
        aria-invalid={!!error}
        className={`w-full rounded-lg border px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-rose-500/20 ${
          error ? "border-red-400 focus:border-red-500" : "border-slate-300 focus:border-rose-500"
        } ${className}`}
        {...props}
      />
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
