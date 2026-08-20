export type FieldErrors = Partial<Record<string, string>>;

export type SetFieldErrors = (value: FieldErrors | ((prev: FieldErrors) => FieldErrors)) => void;

/** Clear or set a single field error (used for live validation while typing). */
export function patchFieldError(setErrors: SetFieldErrors, field: string, error?: string) {
  setErrors((prev) => {
    if (!error) {
      if (!prev[field]) return prev;
      const next = { ...prev };
      delete next[field];
      return next;
    }
    if (prev[field] === error) return prev;
    return { ...prev, [field]: error };
  });
}

export function required(value: string | undefined | null, fieldLabel: string): string | undefined {
  if (!value?.trim()) return `${fieldLabel} is required`;
}

export function email(value: string, requiredField = false): string | undefined {
  if (!value.trim()) return requiredField ? "Email is required" : undefined;
  const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!pattern.test(value.trim())) return "Enter a valid email address";
}

export function phone(value: string): string | undefined {
  if (!value.trim()) return undefined;
  const cleaned = value.replace(/[\s\-()]/g, "");
  if (!/^\+?[0-9]{7,15}$/.test(cleaned)) return "Enter a valid phone number (7-15 digits)";
}

export function minLength(value: string, min: number, fieldLabel: string): string | undefined {
  if (value.trim().length < min) return `${fieldLabel} must be at least ${min} characters`;
}

export function maxLength(value: string, max: number, fieldLabel: string): string | undefined {
  if (value.trim().length > max) return `${fieldLabel} must be at most ${max} characters`;
}

export function password(value: string, requiredField = true): string | undefined {
  if (!value) return requiredField ? "Password is required" : undefined;
  if (value.length < 6) return "Password must be at least 6 characters";
}

export function positiveNumber(value: number, fieldLabel: string, allowZero = false): string | undefined {
  if (Number.isNaN(value)) return `${fieldLabel} must be a valid number`;
  if (allowZero ? value < 0 : value <= 0) return `${fieldLabel} must be greater than ${allowZero ? "or equal to 0" : "0"}`;
}

export function requiredSelect(value: string, fieldLabel: string): string | undefined {
  if (!value) return `${fieldLabel} is required`;
}

export function hasErrors(errors: FieldErrors): boolean {
  return Object.keys(errors).length > 0;
}
