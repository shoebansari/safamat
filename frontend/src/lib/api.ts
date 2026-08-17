import { clearAuth, getToken } from "./auth";
import type { ApiResponse } from "./types";

const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5116";

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

  let json: ApiResponse<T>;
  try {
    json = JSON.parse(text) as ApiResponse<T>;
  } catch {
    const message = text.startsWith("<")
      ? "Unexpected server response"
      : text.length > 200
        ? text.slice(0, 200)
        : text;
    throw new ApiError(message, response.status);
  }

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

export { API_URL };
