import { UserLayout } from "@/components/layout/UserLayout";

export default function AppLayout({ children }: { children: React.ReactNode }) {
  return <UserLayout>{children}</UserLayout>;
}
