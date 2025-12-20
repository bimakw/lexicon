'use client';

import { useState, useEffect } from 'react';
import { BookOpen, FolderTree, Tags, Users, MessageSquare } from 'lucide-react';
import { Header } from '@/components/layout/header';
import { Card, CardContent } from '@/components/ui/card';
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
    { name: 'Total Posts', value: stats.posts, icon: BookOpen, color: 'text-blue-600', bg: 'bg-blue-100' },
    { name: 'Categories', value: stats.categories, icon: FolderTree, color: 'text-green-600', bg: 'bg-green-100' },
    { name: 'Tags', value: stats.tags, icon: Tags, color: 'text-purple-600', bg: 'bg-purple-100' },
    { name: 'Authors', value: stats.authors, icon: Users, color: 'text-orange-600', bg: 'bg-orange-100' },
    { name: 'Pending Comments', value: stats.pendingComments, icon: MessageSquare, color: 'text-red-600', bg: 'bg-red-100' },
  ];

  return (
    <div>
      <Header title="Dashboard" description="Welcome to Lexicon CMS" />

      <div className="p-6">
        {loading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
          </div>
        ) : (
          <>
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
              {statCards.map((stat) => (
                <Card key={stat.name}>
                  <CardContent className="p-6">
                    <div className="flex items-center">
                      <div className={`p-3 rounded-lg ${stat.bg}`}>
                        <stat.icon className={`h-6 w-6 ${stat.color}`} />
                      </div>
                      <div className="ml-4">
                        <p className="text-sm font-medium text-gray-500">{stat.name}</p>
                        <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Quick Actions */}
            <Card>
              <CardContent className="p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h2>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <a
                    href="/posts/new"
                    className="flex items-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
                  >
                    <BookOpen className="h-8 w-8 text-blue-600" />
                    <div className="ml-4">
                      <p className="font-medium text-gray-900">Create Post</p>
                      <p className="text-sm text-gray-500">Write a new blog post</p>
                    </div>
                  </a>
                  <a
                    href="/categories"
                    className="flex items-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
                  >
                    <FolderTree className="h-8 w-8 text-green-600" />
                    <div className="ml-4">
                      <p className="font-medium text-gray-900">Manage Categories</p>
                      <p className="text-sm text-gray-500">Organize your content</p>
                    </div>
                  </a>
                  <a
                    href="/comments"
                    className="flex items-center p-4 bg-orange-50 rounded-lg hover:bg-orange-100 transition-colors"
                  >
                    <MessageSquare className="h-8 w-8 text-orange-600" />
                    <div className="ml-4">
                      <p className="font-medium text-gray-900">Review Comments</p>
                      <p className="text-sm text-gray-500">Moderate user comments</p>
                    </div>
                  </a>
                </div>
              </CardContent>
            </Card>
          </>
        )}
      </div>
    </div>
  );
}
