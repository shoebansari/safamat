import { TenantLayout } from "@/components/layout/TenantLayout";

export default function TenantRootLayout({ children }: { children: React.ReactNode }) {
  return <TenantLayout>{children}</TenantLayout>;
}
