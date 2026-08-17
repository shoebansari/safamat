# Matrimonial Admin Frontend

Next.js admin dashboard for the Matrimonial SaaS platform.

## Prerequisites

- Node.js 18+
- Backend API running at `http://localhost:5116`

## Setup

```bash
npm install
npm run dev
```

Open **http://localhost:3000** (recommended on the same PC).

The dev server also prints a **Network** URL (e.g. `http://192.168.x.x:3000`) for other devices on your LAN. Local IPs are auto-allowed in `next.config.ts` — no manual edits when your IP changes.

Use `npm run dev:local` if you only need localhost access.

## Default Login

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `Admin@123` |

## Pages

| Route | Description |
|-------|-------------|
| `/login` | Admin login |
| `/dashboard` | Overview stats |
| `/admin-users` | Manage admin accounts |
| `/tenants` | Manage tenants |
| `/subscription-plans` | Manage plans |
| `/tenant-subscriptions` | Manage subscriptions |
| `/payments` | Payment records |
| `/email-templates` | Email templates |
| `/system-settings` | System configuration |

## Environment

Create `.env.local`:

```
NEXT_PUBLIC_API_URL=http://localhost:5116
```
