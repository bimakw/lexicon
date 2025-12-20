'use client';

import { useState, useEffect } from 'react';
import { BookOpen, FolderTree, Tags, Users, MessageSquare, TrendingUp, ArrowUpRight, Sparkles } from 'lucide-react';
import { Header } from '@/components/layout/header';
import { postsApi, categoriesApi, tagsApi, authorsApi, commentsApi } from '@/lib/api';

interface Stats {
  posts: number;
  categories: number;
  tags: number;
  authors: number;
  pendingComments: number;
}

export default function Dashboard() {
  const [stats, setStats] = useState<Stats>({
    posts: 0,
    categories: 0,
    tags: 0,
    authors: 0,
    pendingComments: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const [postsRes, categoriesRes, tagsRes, authorsRes, commentsRes] = await Promise.all([
        postsApi.list({ pageSize: 1 }),
        categoriesApi.list(),
        tagsApi.list(),
        authorsApi.list(),
        commentsApi.getPending().catch(() => []),
      ]);

      setStats({
        posts: postsRes.totalCount,
        categories: categoriesRes.length,
        tags: tagsRes.length,
        authors: authorsRes.length,
        pendingComments: commentsRes.length,
      });
    } catch (error) {
      console.error('Failed to load stats:', error);
    } finally {
      setLoading(false);
    }
  };

  const statCards = [
    {
      name: 'Total Posts',
      value: stats.posts,
      icon: BookOpen,
      gradient: 'from-indigo-500 to-purple-600',
      bgGradient: 'from-indigo-500/10 to-purple-500/10',
      iconBg: 'bg-indigo-500',
      change: '+12%'
    },
    {
      name: 'Categories',
      value: stats.categories,
      icon: FolderTree,
      gradient: 'from-emerald-500 to-teal-600',
      bgGradient: 'from-emerald-500/10 to-teal-500/10',
      iconBg: 'bg-emerald-500',
      change: '+5%'
    },
    {
      name: 'Tags',
      value: stats.tags,
      icon: Tags,
      gradient: 'from-amber-500 to-orange-600',
      bgGradient: 'from-amber-500/10 to-orange-500/10',
      iconBg: 'bg-amber-500',
      change: '+8%'
    },
    {
      name: 'Authors',
      value: stats.authors,
      icon: Users,
      gradient: 'from-cyan-500 to-blue-600',
      bgGradient: 'from-cyan-500/10 to-blue-500/10',
      iconBg: 'bg-cyan-500',
      change: '+2%'
    },
    {
      name: 'Pending',
      value: stats.pendingComments,
      icon: MessageSquare,
      gradient: 'from-rose-500 to-pink-600',
      bgGradient: 'from-rose-500/10 to-pink-500/10',
      iconBg: 'bg-rose-500',
      change: '-3%'
    },
  ];

  const quickActions = [
    {
      title: 'Create Post',
      description: 'Write a new blog post',
      href: '/posts/new',
      icon: BookOpen,
      gradient: 'from-indigo-500 to-purple-600',
    },
    {
      title: 'Manage Categories',
      description: 'Organize your content',
      href: '/categories',
      icon: FolderTree,
      gradient: 'from-emerald-500 to-teal-600',
    },
    {
      title: 'Review Comments',
      description: 'Moderate user comments',
      href: '/comments',
      icon: MessageSquare,
      gradient: 'from-rose-500 to-pink-600',
    },
  ];

  return (
    <div className="min-h-screen">
      <Header title="Dashboard" description="Welcome back! Here's what's happening." />

      <div className="p-8">
        {loading ? (
          <div className="flex items-center justify-center py-20">
            <div className="relative">
              <div className="w-16 h-16 border-4 border-indigo-200 rounded-full animate-spin border-t-indigo-600" />
              <Sparkles className="absolute inset-0 m-auto w-6 h-6 text-indigo-600" />
            </div>
          </div>
        ) : (
          <>
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
              {statCards.map((stat, index) => (
                <div
                  key={stat.name}
                  className="group card-modern rounded-2xl p-6 cursor-pointer"
                  style={{ animationDelay: `${index * 100}ms` }}
                >
                  <div className="flex items-start justify-between mb-4">
                    <div className={`p-3 rounded-xl bg-gradient-to-br ${stat.gradient} shadow-lg`}>
                      <stat.icon className="h-6 w-6 text-white" />
                    </div>
                    <div className="flex items-center gap-1 text-xs font-medium text-emerald-600 bg-emerald-50 px-2 py-1 rounded-full">
                      <TrendingUp className="h-3 w-3" />
                      {stat.change}
                    </div>
                  </div>
                  <div>
                    <p className="text-3xl font-bold text-slate-800 mb-1">{stat.value}</p>
                    <p className="text-sm text-slate-500">{stat.name}</p>
                  </div>
                  <div className={`absolute inset-0 rounded-2xl bg-gradient-to-br ${stat.bgGradient} opacity-0 group-hover:opacity-100 transition-opacity duration-300 -z-10`} />
                </div>
              ))}
            </div>

            {/* Quick Actions */}
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-6">
                <div className="p-2 rounded-lg bg-gradient-to-br from-indigo-500 to-purple-600">
                  <Sparkles className="h-5 w-5 text-white" />
                </div>
                <h2 className="text-xl font-bold text-slate-800">Quick Actions</h2>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {quickActions.map((action) => (
                  <a
                    key={action.title}
                    href={action.href}
                    className="group card-modern rounded-2xl p-6 flex items-center gap-5"
                  >
                    <div className={`p-4 rounded-2xl bg-gradient-to-br ${action.gradient} shadow-lg group-hover:scale-110 transition-transform duration-300`}>
                      <action.icon className="h-7 w-7 text-white" />
                    </div>
                    <div className="flex-1">
                      <h3 className="font-semibold text-slate-800 group-hover:text-indigo-600 transition-colors">
                        {action.title}
                      </h3>
                      <p className="text-sm text-slate-500">{action.description}</p>
                    </div>
                    <ArrowUpRight className="h-5 w-5 text-slate-400 group-hover:text-indigo-600 group-hover:translate-x-1 group-hover:-translate-y-1 transition-all duration-300" />
                  </a>
                ))}
              </div>
            </div>

            {/* Activity & Overview */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Recent Activity */}
              <div className="card-modern rounded-2xl p-6">
                <h2 className="text-lg font-bold text-slate-800 mb-4">Recent Activity</h2>
                <div className="space-y-4">
                  {[1, 2, 3, 4].map((i) => (
                    <div key={i} className="flex items-center gap-4 p-3 rounded-xl hover:bg-slate-50 transition-colors">
                      <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white font-bold text-sm">
                        {['A', 'B', 'C', 'D'][i - 1]}
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-slate-800 truncate">
                          {['New post published', 'Comment approved', 'Category created', 'Tag updated'][i - 1]}
                        </p>
                        <p className="text-xs text-slate-500">{i} hour{i > 1 ? 's' : ''} ago</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Content Overview */}
              <div className="card-modern rounded-2xl p-6">
                <h2 className="text-lg font-bold text-slate-800 mb-4">Content Overview</h2>
                <div className="space-y-4">
                  {[
                    { label: 'Published Posts', value: 65, color: 'bg-indigo-500' },
                    { label: 'Draft Posts', value: 25, color: 'bg-amber-500' },
                    { label: 'Scheduled', value: 10, color: 'bg-emerald-500' },
                  ].map((item) => (
                    <div key={item.label}>
                      <div className="flex justify-between text-sm mb-2">
                        <span className="text-slate-600">{item.label}</span>
                        <span className="font-semibold text-slate-800">{item.value}%</span>
                      </div>
                      <div className="h-2 bg-slate-100 rounded-full overflow-hidden">
                        <div
                          className={`h-full ${item.color} rounded-full transition-all duration-500`}
                          style={{ width: `${item.value}%` }}
                        />
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
