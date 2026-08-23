import type { LucideIcon } from "lucide-react";
import {
  Bell,
  Compass,
  CreditCard,
  Heart,
  LayoutDashboard,
  MessageCircle,
  Search,
  Star,
  User,
  Users,
} from "lucide-react";

export interface NavItem {
  href: string;
  label: string;
  icon: LucideIcon;
  mobileNav?: boolean;
  shortLabel?: string;
}

export const navItems: NavItem[] = [
  { href: "/dashboard", label: "Dashboard", shortLabel: "Home", icon: LayoutDashboard, mobileNav: true },
  { href: "/discover", label: "Discover", shortLabel: "Discover", icon: Compass, mobileNav: true },
  { href: "/interests", label: "Interests", shortLabel: "Interests", icon: Heart, mobileNav: true },
  { href: "/matches", label: "Matches", icon: Users },
  { href: "/messages", label: "Messages", shortLabel: "Chats", icon: MessageCircle, mobileNav: true },
  { href: "/favorites", label: "Favorites", icon: Star },
  { href: "/profile", label: "My Profile", shortLabel: "Profile", icon: User, mobileNav: true },
  { href: "/plan", label: "My Plan", icon: CreditCard },
  { href: "/preferences", label: "Preferences", icon: Search },
  { href: "/notifications", label: "Notifications", icon: Bell },
];

export const mobileNavItems = navItems.filter((item) => item.mobileNav);
