"use client";

import { LogOut, Menu, User } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { Button } from "@/components/ui/Button";

interface HeaderProps {
  onMenuClick: () => void;
}

export function Header({ onMenuClick }: HeaderProps) {
  const { user, logout } = useAuth();

  return (
    <header className="sticky top-0 z-20 flex h-14 items-center justify-between gap-2 border-b border-slate-200 bg-white px-3 sm:px-6 lg:h-16">
      <div className="flex min-w-0 items-center gap-2">
        <button
          type="button"
          onClick={onMenuClick}
          className="rounded-lg p-2 text-slate-600 hover:bg-slate-100 lg:hidden"
          aria-label="Open menu"
        >
          <Menu size={22} />
        </button>
        <p className="truncate text-sm font-medium text-slate-800 lg:hidden">{user?.companyName}</p>
      </div>
      <div className="flex shrink-0 items-center gap-1 sm:gap-4">
        <div className="hidden items-center gap-3 lg:flex">
          <div className="flex h-9 w-9 items-center justify-center rounded-full bg-rose-100 text-rose-600">
            <User size={18} />
          </div>
          <div>
            <p className="text-sm font-medium text-slate-800">{user?.companyName}</p>
            <p className="text-xs text-slate-500">{user?.userName}</p>
          </div>
        </div>
        <Button variant="ghost" size="sm" onClick={logout} className="px-2 sm:px-3">
          <LogOut size={16} />
          <span className="hidden sm:inline">Logout</span>
        </Button>
      </div>
    </header>
  );
}
