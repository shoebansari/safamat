import { getApiUrl } from "./config";

export function resolvePhotoUrl(url?: string | null): string {
  if (!url) return "";
  if (url.startsWith("http://") || url.startsWith("https://")) return url;
  const base = getApiUrl().replace(/\/$/, "");
  return `${base}${url.startsWith("/") ? url : `/${url}`}`;
}

export const PHOTO_MAX_COUNT = 3;
export const PHOTO_MAX_SIZE_MB = 2;
export const PHOTO_MAX_SIZE_BYTES = PHOTO_MAX_SIZE_MB * 1024 * 1024;
