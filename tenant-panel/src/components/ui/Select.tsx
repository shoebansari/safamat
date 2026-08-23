import { type SelectHTMLAttributes } from "react";
import { FormLabel } from "./FormLabel";

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  requiredMark?: boolean;
  options: { value: string; label: string }[];
}

export function Select({ label, options, error, requiredMark, className = "", id, required, ...props }: SelectProps) {
  const selectId = id || label?.toLowerCase().replace(/\s/g, "-");
  const showRequired = requiredMark ?? required;

  return (
    <div className="space-y-1">
      {label && (
        <FormLabel htmlFor={selectId} required={showRequired}>
          {label}
        </FormLabel>
      )}
      <select
        id={selectId}
        required={required}
        aria-invalid={!!error}
        className={`w-full rounded-lg border px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-rose-500/20 ${
          error ? "border-red-400 focus:border-red-500" : "border-slate-300 focus:border-rose-500"
        } ${props.disabled ? "cursor-not-allowed bg-slate-100 text-slate-500" : ""} ${className}`}
        {...props}
      >
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}
