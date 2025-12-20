'use client';

import { useState, useEffect } from 'react';
import { Check, Trash2, MessageSquare, Clock } from 'lucide-react';
import { Header } from '@/components/layout/header';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { commentsApi } from '@/lib/api';
import type { Comment } from '@/types';

export default function CommentsPage() {
  const [comments, setComments] = useState<Comment[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadComments();
  }, []);

  const loadComments = async () => {
    try {
      const data = await commentsApi.getPending();
      setComments(data);
    } catch (error) {
      console.error('Failed to load comments:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (id: string) => {
    try {
      await commentsApi.approve(id);
      loadComments();
    } catch (error) {
      console.error('Failed to approve comment:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this comment?')) return;
    try {
      await commentsApi.delete(id);
      loadComments();
    } catch (error) {
      console.error('Failed to delete comment:', error);
    }
  };

  return (
    <div>
      <Header title="Comments" description="Moderate user comments">
        <Badge variant="warning" className="text-sm">
          <Clock className="h-4 w-4 mr-1" />
          {comments.length} pending
        </Badge>
      </Header>

      <div className="p-6">
        {loading ? (
          <div className="flex justify-center py-12">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
          </div>
        ) : comments.length === 0 ? (
          <Card>
            <CardContent className="p-12 text-center">
              <MessageSquare className="h-12 w-12 text-gray-300 mx-auto mb-4" />
              <p className="text-gray-500">No pending comments</p>
              <p className="text-sm text-gray-400 mt-2">
                All comments have been moderated
              </p>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            {comments.map((comment) => (
              <Card key={comment.id}>
                <CardContent className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center">
                          <span className="text-lg font-bold text-gray-400">
                            {comment.authorName.charAt(0).toUpperCase()}
                          </span>
                        </div>
                        <div>
                          <p className="font-medium text-gray-900">{comment.authorName}</p>
                          <p className="text-sm text-gray-500">{comment.email}</p>
                        </div>
                        <Badge variant="warning">Pending</Badge>
                      </div>
                      <p className="text-gray-700 mt-3 whitespace-pre-wrap">{comment.content}</p>
                      <div className="flex items-center gap-4 mt-4 text-sm text-gray-500">
                        <span>Post: {comment.postTitle}</span>
                        <span>{new Date(comment.createdAt).toLocaleString()}</span>
                      </div>
                    </div>
                    <div className="flex gap-2 ml-4">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => handleApprove(comment.id)}
                        className="text-green-600 hover:text-green-700 hover:bg-green-50"
                      >
                        <Check className="h-4 w-4 mr-1" />
                        Approve
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleDelete(comment.id)}
                        className="text-red-500 hover:text-red-700 hover:bg-red-50"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
