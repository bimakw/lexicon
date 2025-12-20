import { cn } from '@/lib/utils';
import { InputHTMLAttributes, forwardRef } from 'react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ className, label, error, id, ...props }, ref) => {
    return (
      <div className="space-y-2">
        {label && (
          <label htmlFor={id} className="block text-sm font-semibold text-slate-700">
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={id}
          className={cn(
            'block w-full px-4 py-3 bg-white/70 backdrop-blur-sm border border-slate-200/50 rounded-xl text-slate-800 placeholder:text-slate-400',
            'focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-300',
            'hover:border-slate-300 transition-all duration-300',
            'shadow-sm hover:shadow-md focus:shadow-md',
            error && 'border-rose-300 focus:border-rose-400 focus:ring-rose-500/20',
            className
          )}
          {...props}
        />
        {error && (
          <p className="text-sm text-rose-600 flex items-center gap-1">
            <span className="inline-block w-1 h-1 rounded-full bg-rose-500" />
            {error}
          </p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';
