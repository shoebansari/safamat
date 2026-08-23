import { User } from "lucide-react";

interface DefaultAvatarProps {
  name?: string;
  className?: string;
}

export function DefaultAvatar({ name, className = "" }: DefaultAvatarProps) {
  const initial = name?.trim().charAt(0).toUpperCase();
  return (
    <div
      className={`flex items-center justify-center bg-gradient-to-br from-rose-100 to-rose-200 font-semibold text-rose-600 ${className}`}
      aria-hidden
    >
      {initial || <User size={20} />}
    </div>
  );
}
