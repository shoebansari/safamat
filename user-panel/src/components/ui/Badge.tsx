interface BadgeProps {
  children: React.ReactNode;
  variant?: "success" | "danger" | "warning" | "info" | "default";
}

const styles = {
  success: "bg-green-100 text-green-700",
  danger: "bg-red-100 text-red-700",
  warning: "bg-amber-100 text-amber-700",
  info: "bg-blue-100 text-blue-700",
  default: "bg-slate-100 text-slate-700",
};

export function Badge({ children, variant = "default" }: BadgeProps) {
  return (
    <span className={`inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[variant]}`}>
      {children}
    </span>
  );
}

export function StatusBadge({ active, label }: { active: boolean; label?: string }) {
  const text = label ?? (active ? "Active" : "Inactive");
  let variant: "success" | "danger" | "warning" | "default" = active ? "success" : "danger";
  if (label) {
    if (label === "Approved" || label === "Paid" || label === "Active") variant = "success";
    else if (label === "Pending") variant = "warning";
    else variant = "danger";
  }
  return <Badge variant={variant}>{text}</Badge>;
}
