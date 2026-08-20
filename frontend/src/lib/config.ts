/** Live API on Render — update here if the URL changes */
export const PRODUCTION_API_URL =
  "https://matrimonial-admin-api-3syo.onrender.com";

/** API base URL: .env.local overrides in dev; production uses env or Render URL */
export function getApiUrl(): string {
  const fromEnv = process.env.NEXT_PUBLIC_API_URL?.trim();
  if (fromEnv) return fromEnv.replace(/\/$/, "");

  if (process.env.NODE_ENV === "production") return PRODUCTION_API_URL;

  return "http://localhost:5116";
}
