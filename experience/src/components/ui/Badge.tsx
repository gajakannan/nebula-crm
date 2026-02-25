import { cn } from '@/lib/utils';

type BadgeVariant = 'default' | 'success' | 'warning' | 'error' | 'info' | 'gradient';

const variantClasses: Record<BadgeVariant, string> = {
  default: 'bg-zinc-800 text-zinc-300',
  success: 'bg-status-success/15 text-status-success',
  warning: 'bg-status-warning/15 text-status-warning',
  error: 'bg-status-error/15 text-status-error',
  info: 'bg-status-info/15 text-status-info',
  gradient: 'bg-gradient-to-r from-nebula-violet/20 to-nebula-fuchsia/20 text-nebula-fuchsia border border-nebula-fuchsia/20',
};

interface BadgeProps {
  children: React.ReactNode;
  variant?: BadgeVariant;
  className?: string;
}

export function Badge({ children, variant = 'default', className }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium',
        variantClasses[variant],
        className,
      )}
    >
      {children}
    </span>
  );
}
