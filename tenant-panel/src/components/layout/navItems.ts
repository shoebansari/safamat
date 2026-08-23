import type { LucideIcon } from "lucide-react";
import { ClipboardList, LayoutDashboard, UserCheck, Wallet } from "lucide-react";

export interface NavItem {
  href: string;
  label: string;
  icon: LucideIcon;
  mobileNav?: boolean;
  shortLabel?: string;
}

export const navItems: NavItem[] = [
  { href: "/dashboard", label: "Dashboard", shortLabel: "Home", icon: LayoutDashboard, mobileNav: true },
  { href: "/plans", label: "Member Plans", shortLabel: "Plans", icon: Wallet, mobileNav: true },
  { href: "/member-plans", label: "Assign Plans", shortLabel: "Assign", icon: ClipboardList, mobileNav: true },
  { href: "/profile-approvals", label: "Profile Approvals", shortLabel: "Approve", icon: UserCheck, mobileNav: true },
];

export const mobileNavItems = navItems.filter((item) => item.mobileNav);
