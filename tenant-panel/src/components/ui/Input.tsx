import { type InputHTMLAttributes } from "react";
import { FormLabel } from "./FormLabel";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  requiredMark?: boolean;
}

export function Input({ label, error, requiredMark, className = "", id, required, ...props }: InputProps) {
  const inputId = id || label?.toLowerCase().replace(/\s/g, "-");
  const showRequired = requiredMark ?? required;

  return (
    <div className="space-y-1">
      {label && (
        <FormLabel htmlFor={inputId} required={showRequired}>
          {label}
        </FormLabel>
      )}
      <input
        id={inputId}
        required={required}
        aria-invalid={!!error}
        className={`w-full rounded-lg border px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-rose-500/20 ${
          error ? "border-red-400 focus:border-red-500" : "border-slate-300 focus:border-rose-500"
        } ${props.disabled ? "cursor-not-allowed bg-slate-100 text-slate-500" : ""} ${className}`}
        {...props}
      />
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
