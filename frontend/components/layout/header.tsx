import { Bell, Search } from 'lucide-react';

interface HeaderProps {
  title: string;
  description?: string;
  children?: React.ReactNode;
}

export function Header({ title, description, children }: HeaderProps) {
  return (
    <header className="sticky top-0 z-40 glass border-b border-white/20">
      <div className="px-8 py-5">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-gradient">{title}</h1>
            {description && (
              <p className="mt-1 text-sm text-slate-500">{description}</p>
            )}
          </div>
          <div className="flex items-center gap-4">
            {/* Search */}
            <div className="relative hidden md:block">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400" />
              <input
                type="text"
                placeholder="Search..."
                className="w-64 pl-10 pr-4 py-2.5 rounded-xl bg-white/50 border border-slate-200/50 text-sm placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-300 transition-all duration-300"
              />
            </div>

            {/* Notifications */}
            <button className="relative p-2.5 rounded-xl bg-white/50 border border-slate-200/50 text-slate-500 hover:text-indigo-600 hover:border-indigo-200 transition-all duration-300">
              <Bell className="h-5 w-5" />
              <span className="absolute -top-1 -right-1 w-4 h-4 bg-gradient-to-r from-pink-500 to-rose-500 rounded-full text-[10px] font-bold text-white flex items-center justify-center">
                3
              </span>
            </button>

            {/* Actions */}
            {children && <div className="flex items-center gap-3">{children}</div>}
          </div>
        </div>
      </div>
    </header>
  );
}
