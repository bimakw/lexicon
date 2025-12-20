export type PostStatus = 'Draft' | 'Published' | 'Archived';

export interface Author {
  id: string;
  name: string;
  email: string;
  bio?: string;
  avatarUrl?: string;
  postCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  parentId?: string;
  parentName?: string;
  postCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CategoryTree {
  id: string;
  name: string;
  slug: string;
  description?: string;
  children: CategoryTree[];
}

export interface Tag {
  id: string;
  name: string;
  slug: string;
  postCount: number;
  createdAt: string;
}

export interface Post {
  id: string;
  title: string;
  slug: string;
  content: string;
  excerpt?: string;
  featuredImage?: string;
  status: PostStatus;
  publishedAt?: string;
  authorId: string;
  authorName?: string;
  categoryId?: string;
  categoryName?: string;
  tags: Tag[];
  createdAt: string;
  updatedAt: string;
}

export interface PostList {
  id: string;
  title: string;
  slug: string;
  excerpt?: string;
  featuredImage?: string;
  status: PostStatus;
  publishedAt?: string;
  authorName?: string;
  categoryName?: string;
  createdAt: string;
}

export interface Comment {
  id: string;
  postId: string;
  postTitle?: string;
  authorName: string;
  email: string;
  content: string;
  isApproved: boolean;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreatePost {
  title: string;
  content: string;
  excerpt?: string;
  featuredImage?: string;
  authorId: string;
  categoryId?: string;
  tagIds?: string[];
}

export interface UpdatePost {
  title: string;
  content: string;
  excerpt?: string;
  featuredImage?: string;
  categoryId?: string;
  tagIds?: string[];
}

export interface CreateCategory {
  name: string;
  description?: string;
  parentId?: string;
}

export interface CreateTag {
  name: string;
}

export interface CreateAuthor {
  name: string;
  email: string;
  bio?: string;
  avatarUrl?: string;
}

export interface CreateComment {
  authorName: string;
  email: string;
  content: string;
}
