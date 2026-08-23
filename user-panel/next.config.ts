import os from "os";
import type { NextConfig } from "next";

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
  return Array.from(origins);
}

const nextConfig: NextConfig = {
  allowedDevOrigins: getAllowedDevOrigins(),
};

export default nextConfig;
