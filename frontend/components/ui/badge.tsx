import { cn } from '@/lib/utils';

interface BadgeProps {
  children: React.ReactNode;
  variant?: 'default' | 'success' | 'warning' | 'danger' | 'info' | 'secondary' | 'gradient';
  className?: string;
}

export function Badge({ children, variant = 'default', className }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold transition-all duration-300',
        {
          'bg-slate-100 text-slate-700 border border-slate-200': variant === 'default',
          'bg-gradient-to-r from-emerald-500/10 to-teal-500/10 text-emerald-700 border border-emerald-200': variant === 'success',
          'bg-gradient-to-r from-amber-500/10 to-orange-500/10 text-amber-700 border border-amber-200': variant === 'warning',
          'bg-gradient-to-r from-rose-500/10 to-pink-500/10 text-rose-700 border border-rose-200': variant === 'danger',
          'bg-gradient-to-r from-blue-500/10 to-cyan-500/10 text-blue-700 border border-blue-200': variant === 'info',
          'bg-slate-50 text-slate-500 border border-slate-200': variant === 'secondary',
          'bg-gradient-to-r from-indigo-500 to-purple-600 text-white shadow-lg shadow-indigo-500/30': variant === 'gradient',
        },
        className
      )}
    >
      {children}
    </span>
  );
}
