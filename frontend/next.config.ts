import os from "os";
import type { NextConfig } from "next";

/**
 * Next.js 16 blocks dev assets when the browser origin differs from the dev
 * server host (e.g. opening http://192.168.x.x:3000 instead of localhost).
 * Auto-detect local IPs so npm run dev works on any network without edits.
 */
function getAllowedDevOrigins(): string[] {
  const origins = new Set<string>(["localhost", "127.0.0.1", "[::1]"]);

  for (const addresses of Object.values(os.networkInterfaces())) {
    if (!addresses) continue;
    for (const address of addresses) {
      if (address.internal) continue;
      const family = String(address.family);
      if (family === "IPv4" || family === "4") origins.add(address.address);
    }
  }

  const extra = process.env.EXTRA_DEV_ORIGINS;
  if (extra) {
    extra
      .split(",")
      .map((origin) => origin.trim())
      .filter(Boolean)
      .forEach((origin) => origins.add(origin));
  }

  return Array.from(origins);
}

const nextConfig: NextConfig = {
  allowedDevOrigins: getAllowedDevOrigins(),
};

export default nextConfig;
