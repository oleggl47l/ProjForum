import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Taiga UI Core - только проверенные компоненты
import {
  TuiButton,
  TuiLoader,
  TuiHint,
  TuiIcon,
  TuiTitle,
  TuiTextfield
} from '@taiga-ui/core';

// Taiga UI Kit - только ТЕ что точно есть
import {
  TuiAvatar,
  TuiBadge,
  TuiPagination
} from '@taiga-ui/kit';

// Временные интерфейсы
interface PostModel {
  id: string;
  title?: string;
  content?: string;
  authorId?: string;
  category?: string;
  createdAt: string;
  updatedAt?: string;
  isPublished: boolean;
  tagNames?: string[];
}

interface CategoryModel {
  id: string;
  name?: string;
  description?: string;
}

interface TagModel {
  id: string;
  name?: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,

    // Taiga UI Core
    TuiButton,
    TuiLoader,
    TuiHint,
    TuiIcon,
    TuiTitle,
    TuiTextfield,

    // Taiga UI Kit - минимальный набор
    TuiAvatar,
    TuiBadge,
    TuiPagination
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {
  // Данные (временные моки)
  posts: PostModel[] = [];
  allPosts: PostModel[] = [];
  categories: CategoryModel[] = [];
  tags: TagModel[] = [];

  // Состояние
  isLoading = false;
  isCreatingPost = false;
  searchQuery = '';
  selectedTags: string[] = [];
  sortBy: 'newest' | 'oldest' = 'newest';
  currentPage = 0;
  itemsPerPage = 10;

  // Фильтры
  showFilters = false;
  showDrafts = false;
  selectedCategory: string | null = null;

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.isLoading = true;

    // Временные моки данных
    setTimeout(() => {
      this.allPosts = [
        {
          id: '1',
          title: 'Пример первого поста',
          content: 'Это содержание первого поста на нашем форуме. Здесь обсуждаются интересные темы...',
          authorId: 'user-123',
          category: 'Обсуждение',
          createdAt: new Date().toISOString(),
          isPublished: true,
          tagNames: ['angular', 'frontend']
        },
        {
          id: '2',
          title: 'Вопрос по TypeScript',
          content: 'У меня возник вопрос по TypeScript generics. Как правильно типизировать...',
          authorId: 'user-456',
          category: 'Вопрос',
          createdAt: new Date(Date.now() - 86400000).toISOString(),
          isPublished: true,
          tagNames: ['typescript', 'programming']
        },
        {
          id: '3',
          title: 'Новости проекта',
          content: 'Мы выпустили новую версию нашего приложения с множеством улучшений...',
          authorId: 'user-789',
          category: 'Новости',
          createdAt: new Date(Date.now() - 172800000).toISOString(),
          isPublished: true,
          tagNames: ['announcement', 'update']
        }
      ];

      this.categories = [
        { id: '1', name: 'Обсуждение', description: 'Общие обсуждения' },
        { id: '2', name: 'Вопрос', description: 'Вопросы и ответы' },
        { id: '3', name: 'Новости', description: 'Новости проекта' }
      ];

      this.tags = [
        { id: '1', name: 'angular' },
        { id: '2', name: 'typescript' },
        { id: '3', name: 'frontend' }
      ];

      this.applyFilters();
      this.isLoading = false;
    }, 500);
  }

  applyFilters(): void {
    let filtered = [...this.allPosts];

    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      filtered = filtered.filter(post =>
        (post.title?.toLowerCase().includes(query)) ||
        (post.content?.toLowerCase().includes(query)) ||
        (post.category?.toLowerCase().includes(query))
      );
    }

    if (this.selectedTags.length > 0) {
      filtered = filtered.filter(post =>
        post.tagNames?.some(tag => this.selectedTags.includes(tag))
      );
    }

    if (this.selectedCategory) {
      filtered = filtered.filter(post =>
        post.category === this.selectedCategory
      );
    }

    if (!this.showDrafts) {
      filtered = filtered.filter(post => post.isPublished);
    }

    filtered.sort((a, b) => {
      switch (this.sortBy) {
        case 'newest':
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        case 'oldest':
          return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
        default:
          return 0;
      }
    });

    this.posts = filtered;
    this.currentPage = 0;
  }

  toggleTagFilter(tagName: string): void {
    const index = this.selectedTags.indexOf(tagName);
    if (index > -1) {
      this.selectedTags.splice(index, 1);
    } else {
      this.selectedTags.push(tagName);
    }
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedTags = [];
    this.selectedCategory = null;
    this.showDrafts = false;
    this.sortBy = 'newest';
    this.applyFilters();
  }

  get filteredPosts(): PostModel[] {
    const start = this.currentPage * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.posts.slice(start, end);
  }

  get totalPages(): number {
    return Math.ceil(this.posts.length / this.itemsPerPage);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('ru-RU', {
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }

  getPostExcerpt(content: string): string {
    const plainText = content.replace(/<[^>]*>/g, '');
    return plainText.length > 100 ? plainText.substring(0, 100) + '...' : plainText;
  }

  getCategoryColor(categoryName: string): string {
    const colors = ['#4CAF50', '#2196F3', '#FF9800', '#9C27B0'];
    const index = this.categories.findIndex(c => c.name === categoryName);
    return index !== -1 ? colors[index % colors.length] : colors[0];
  }

  openCreatePostDialog(): void {
    this.isCreatingPost = true;
  }

  onPostCreated(): void {
    this.isCreatingPost = false;
    this.loadInitialData();
  }
}
