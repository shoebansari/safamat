import { clearAuth, getToken } from "./auth";
import { getApiUrl } from "./config";
import type { ApiResponse } from "./types";

const API_URL = getApiUrl();

export class ApiError extends Error {
  status: number;
  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token) headers.Authorization = `Bearer ${token}`;

  const response = await fetch(`${API_URL}${endpoint}`, { ...options, headers });

  if (response.status === 401) {
    clearAuth();
    if (typeof window !== "undefined" && !window.location.pathname.includes("/login")) {
      window.location.href = "/login";
    }
    throw new ApiError("Unauthorized", 401);
  }

  const text = await response.text();
  if (!text) {
    if (!response.ok) throw new ApiError("Request failed", response.status);
    return undefined as T;
  }

  const json = JSON.parse(text) as ApiResponse<T>;
  if (!response.ok || !json.success) {
    throw new ApiError(json.message || "Request failed", response.status);
  }
  return json.data;
}

export const api = {
  get: <T>(endpoint: string) => request<T>(endpoint),
  post: <T>(endpoint: string, body: unknown) =>
    request<T>(endpoint, { method: "POST", body: JSON.stringify(body) }),
  put: <T>(endpoint: string, body: unknown) =>
    request<T>(endpoint, { method: "PUT", body: JSON.stringify(body) }),
  delete: <T>(endpoint: string) => request<T>(endpoint, { method: "DELETE" }),
};

export async function uploadFile<T>(endpoint: string, file: File, extra?: Record<string, string>): Promise<T> {
  const token = getToken();
  const form = new FormData();
  form.append("file", file);
  if (extra) Object.entries(extra).forEach(([k, v]) => form.append(k, v));

  const response = await fetch(`${API_URL}${endpoint}`, {
    method: "POST",
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    body: form,
  });

  const text = await response.text();
  let json: ApiResponse<T>;
  try {
    json = JSON.parse(text) as ApiResponse<T>;
  } catch {
    throw new ApiError(
      response.status === 413 ? "Could not upload: image must not be more than 2 MB." : "Upload failed",
      response.status
    );
  }
  if (!response.ok || !json.success) {
    throw new ApiError(json.message || "Upload failed", response.status);
  }
  return json.data;
}
