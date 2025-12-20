import { cn } from '@/lib/utils';
import { ButtonHTMLAttributes, forwardRef } from 'react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost' | 'outline' | 'gradient';
  size?: 'sm' | 'md' | 'lg';
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = 'primary', size = 'md', children, ...props }, ref) => {
    return (
      <button
        ref={ref}
        className={cn(
          'inline-flex items-center justify-center rounded-xl font-semibold transition-all duration-300 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:pointer-events-none active:scale-95',
          {
            // Primary - Gradient with glow
            'bg-gradient-to-r from-indigo-500 to-purple-600 text-white hover:from-indigo-600 hover:to-purple-700 focus:ring-indigo-500 shadow-lg shadow-indigo-500/30 hover:shadow-xl hover:shadow-indigo-500/40': variant === 'primary',
            // Secondary
            'bg-slate-100 text-slate-700 hover:bg-slate-200 focus:ring-slate-500': variant === 'secondary',
            // Danger
            'bg-gradient-to-r from-rose-500 to-pink-600 text-white hover:from-rose-600 hover:to-pink-700 focus:ring-rose-500 shadow-lg shadow-rose-500/30': variant === 'danger',
            // Ghost
            'text-slate-600 hover:bg-slate-100 hover:text-slate-800 focus:ring-slate-500': variant === 'ghost',
            // Outline
            'border-2 border-slate-200 bg-white/50 text-slate-700 hover:border-indigo-300 hover:bg-indigo-50 focus:ring-indigo-500': variant === 'outline',
            // Gradient (accent)
            'bg-gradient-to-r from-amber-500 to-orange-600 text-white hover:from-amber-600 hover:to-orange-700 focus:ring-amber-500 shadow-lg shadow-amber-500/30': variant === 'gradient',
            // Sizes
            'px-3.5 py-2 text-sm': size === 'sm',
            'px-5 py-2.5 text-sm': size === 'md',
            'px-7 py-3.5 text-base': size === 'lg',
          },
          className
        )}
        {...props}
      >
        {children}
      </button>
    );
  }
);

Button.displayName = 'Button';
