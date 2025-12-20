import axios from 'axios';
import type {
  Post,
  PostList,
  Category,
  CategoryTree,
  Tag,
  Author,
  Comment,
  PagedResult,
  CreatePost,
  UpdatePost,
  CreateCategory,
  CreateTag,
  CreateAuthor,
  CreateComment,
  PostStatus,
} from '@/types';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:10180/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Posts
export const postsApi = {
  list: async (params?: {
    page?: number;
    pageSize?: number;
    status?: PostStatus;
    categoryId?: string;
    authorId?: string;
    search?: string;
  }) => {
    const { data } = await api.get<PagedResult<PostList>>('/posts', { params });
    return data;
  },
  getBySlug: async (slug: string) => {
    const { data } = await api.get<Post>(`/posts/${slug}`);
    return data;
  },
  getById: async (id: string) => {
    const { data } = await api.get<Post>(`/posts/id/${id}`);
    return data;
  },
  create: async (post: CreatePost) => {
    const { data } = await api.post<Post>('/posts', post);
    return data;
  },
  update: async (id: string, post: UpdatePost) => {
    const { data } = await api.put<Post>(`/posts/${id}`, post);
    return data;
  },
  delete: async (id: string) => {
    await api.delete(`/posts/${id}`);
  },
  publish: async (id: string) => {
    const { data } = await api.post<Post>(`/posts/${id}/publish`);
    return data;
  },
  unpublish: async (id: string) => {
    const { data } = await api.post<Post>(`/posts/${id}/unpublish`);
    return data;
  },
};

// Categories
export const categoriesApi = {
  list: async () => {
    const { data } = await api.get<Category[]>('/categories');
    return data;
  },
  tree: async () => {
    const { data } = await api.get<CategoryTree[]>('/categories/tree');
    return data;
  },
  getBySlug: async (slug: string) => {
    const { data } = await api.get<Category>(`/categories/${slug}`);
    return data;
  },
  getById: async (id: string) => {
    const { data } = await api.get<Category>(`/categories/id/${id}`);
    return data;
  },
  create: async (category: CreateCategory) => {
    const { data } = await api.post<Category>('/categories', category);
    return data;
  },
  update: async (id: string, category: CreateCategory) => {
    const { data } = await api.put<Category>(`/categories/${id}`, category);
    return data;
  },
  delete: async (id: string) => {
    await api.delete(`/categories/${id}`);
  },
};

// Tags
export const tagsApi = {
  list: async () => {
    const { data } = await api.get<Tag[]>('/tags');
    return data;
  },
  getBySlug: async (slug: string) => {
    const { data } = await api.get<Tag>(`/tags/${slug}`);
    return data;
  },
  create: async (tag: CreateTag) => {
    const { data } = await api.post<Tag>('/tags', tag);
    return data;
  },
  delete: async (id: string) => {
    await api.delete(`/tags/${id}`);
  },
};

// Authors
export const authorsApi = {
  list: async () => {
    const { data } = await api.get<Author[]>('/authors');
    return data;
  },
  getById: async (id: string) => {
    const { data } = await api.get<Author>(`/authors/${id}`);
    return data;
  },
  create: async (author: CreateAuthor) => {
    const { data } = await api.post<Author>('/authors', author);
    return data;
  },
  update: async (id: string, author: CreateAuthor) => {
    const { data } = await api.put<Author>(`/authors/${id}`, author);
    return data;
  },
  delete: async (id: string) => {
    await api.delete(`/authors/${id}`);
  },
};

// Comments
export const commentsApi = {
  getByPost: async (postId: string, approved?: boolean) => {
    const { data } = await api.get<Comment[]>(`/posts/${postId}/comments`, {
      params: { approved },
    });
    return data;
  },
  getPending: async () => {
    const { data } = await api.get<Comment[]>('/comments/pending');
    return data;
  },
  create: async (postId: string, comment: CreateComment) => {
    const { data } = await api.post<Comment>(`/posts/${postId}/comments`, comment);
    return data;
  },
  approve: async (id: string) => {
    const { data } = await api.put<Comment>(`/comments/${id}/approve`);
    return data;
  },
  delete: async (id: string) => {
    await api.delete(`/comments/${id}`);
  },
};

export default api;
