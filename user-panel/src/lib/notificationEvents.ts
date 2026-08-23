export const NOTIFICATIONS_UPDATED_EVENT = "notifications-updated";

export function emitNotificationsUpdated() {
  if (typeof window !== "undefined") {
    window.dispatchEvent(new Event(NOTIFICATIONS_UPDATED_EVENT));
  }
}
