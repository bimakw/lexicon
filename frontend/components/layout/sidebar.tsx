'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  BookOpen,
  FolderTree,
  Tags,
  Users,
  MessageSquare,
  LayoutDashboard,
  Settings,
  Sparkles,
} from 'lucide-react';
import { cn } from '@/lib/utils';

const navigation = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Posts', href: '/posts', icon: BookOpen },
  { name: 'Categories', href: '/categories', icon: FolderTree },
  { name: 'Tags', href: '/tags', icon: Tags },
  { name: 'Authors', href: '/authors', icon: Users },
  { name: 'Comments', href: '/comments', icon: MessageSquare },
];

export function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="fixed inset-y-0 left-0 z-50 w-72 gradient-dark glass-dark">
      <div className="flex h-full flex-col">
        {/* Logo */}
        <div className="flex h-20 items-center px-6 border-b border-white/10">
          <div className="relative">
            <div className="absolute inset-0 bg-gradient-to-r from-indigo-500 to-purple-500 rounded-xl blur-lg opacity-50" />
            <div className="relative flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 shadow-lg">
              <BookOpen className="h-6 w-6 text-white" />
            </div>
          </div>
          <div className="ml-4">
            <span className="text-xl font-bold text-white tracking-tight">Lexicon</span>
            <div className="flex items-center gap-1 text-xs text-indigo-300">
              <Sparkles className="h-3 w-3" />
              <span>CMS Platform</span>
            </div>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
          <p className="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider mb-4">
            Menu
          </p>
          {navigation.map((item) => {
            const isActive = pathname === item.href || pathname.startsWith(item.href + '/');
            return (
              <Link
                key={item.name}
                href={item.href}
                className={cn(
                  'group flex items-center px-4 py-3.5 text-sm font-medium rounded-xl transition-all duration-300',
                  isActive
                    ? 'bg-gradient-to-r from-indigo-500/20 to-purple-500/20 text-white border border-indigo-500/30 shadow-lg shadow-indigo-500/10'
                    : 'text-slate-400 hover:text-white hover:bg-white/5'
                )}
              >
                <div
                  className={cn(
                    'flex items-center justify-center w-9 h-9 rounded-lg mr-3 transition-all duration-300',
                    isActive
                      ? 'bg-gradient-to-br from-indigo-500 to-purple-600 shadow-lg shadow-indigo-500/30'
                      : 'bg-white/5 group-hover:bg-white/10'
                  )}
                >
                  <item.icon className={cn('h-5 w-5', isActive ? 'text-white' : 'text-slate-400 group-hover:text-white')} />
                </div>
                {item.name}
                {isActive && (
                  <div className="ml-auto w-1.5 h-1.5 rounded-full bg-indigo-400 animate-pulse" />
                )}
              </Link>
            );
          })}
        </nav>

        {/* Footer */}
        <div className="border-t border-white/10 p-4">
          <Link
            href="/settings"
            className="group flex items-center px-4 py-3.5 text-sm font-medium text-slate-400 hover:text-white hover:bg-white/5 rounded-xl transition-all duration-300"
          >
            <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-white/5 group-hover:bg-white/10 mr-3 transition-all duration-300">
              <Settings className="h-5 w-5" />
            </div>
            Settings
          </Link>

          {/* User Profile */}
          <div className="mt-4 p-4 rounded-xl bg-gradient-to-r from-indigo-500/10 to-purple-500/10 border border-white/10">
            <div className="flex items-center">
              <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white font-bold text-sm">
                A
              </div>
              <div className="ml-3 flex-1 min-w-0">
                <p className="text-sm font-medium text-white truncate">Admin User</p>
                <p className="text-xs text-slate-400 truncate">admin@lexicon.io</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </aside>
  );
}
