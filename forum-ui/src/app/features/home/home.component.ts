import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Taiga UI Core для standalone
import {
  TuiButton,
  TuiLoader,
  TuiHint,
  TuiTitle
} from '@taiga-ui/core';

// Taiga UI Kit для standalone
import {
  TuiAvatar,
  TuiBadge,
  TuiPagination
} from '@taiga-ui/kit';

// Импортируем компонент диалога и его интерфейс
import { CreatePostDialogComponent, NewPostData } from '../post/create-post-dialog/create-post-dialog.component';

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

    // Taiga UI Core - компоненты
    TuiButton,
    TuiLoader,
    TuiHint,
    TuiTitle,

    // Taiga UI Kit - компоненты
    TuiAvatar,
    TuiBadge,
    TuiPagination,

    // Диалог создания поста
    CreatePostDialogComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {
  // Данные (временные моки)
  posts: PostModel[] = [];
  allPosts: PostModel[] = [];
  categories: CategoryModel[] = [];
  allTags: TagModel[] = []; // Все теги
  filteredTags: TagModel[] = []; // Отфильтрованные теги для отображения

  // Состояние
  isLoading = false;
  searchQuery = '';
  tagSearchQuery = ''; // Поиск по тегам
  selectedTags: string[] = [];
  sortBy: 'newest' | 'oldest' = 'newest';
  currentPage = 0;
  itemsPerPage = 10;

  // Фильтры
  showFilters = false;
  showDrafts = false;
  selectedCategory: string | null = null;

  // Диалог создания поста
  isCreatePostDialogOpen = false;

  // Состояние развернутости тегов
  isTagsExpanded = false;

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
        { id: '3', name: 'Новости', description: 'Новости проекта' },
        { id: '4', name: 'Руководство', description: 'Инструкции и руководства' },
        { id: '5', name: 'Объявление', description: 'Официальные объявления' }
      ];

      // Начальный список тегов
      this.allTags = [
        { id: '1', name: 'angular' },
        { id: '2', name: 'typescript' },
        { id: '3', name: 'frontend' },
        { id: '4', name: 'programming' },
        { id: '5', name: 'announcement' },
        { id: '6', name: 'update' },
        { id: '7', name: 'javascript' },
        { id: '8', name: 'css' },
        { id: '9', name: 'html' },
        { id: '10', name: 'web' }
      ];

      this.filteredTags = [...this.allTags];
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

  // Поиск по тегам
  applyTagSearch(): void {
    if (this.tagSearchQuery.trim()) {
      const query = this.tagSearchQuery.toLowerCase();
      this.filteredTags = this.allTags.filter(tag =>
        tag.name?.toLowerCase().includes(query)
      );
    } else {
      this.filteredTags = [...this.allTags];
    }
  }

  // Очистить поиск по тегам
  clearTagSearch(): void {
    this.tagSearchQuery = '';
    this.applyTagSearch();
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

  // Проверка, является ли тег выбранным
  isTagSelected(tagName: string): boolean {
    return this.selectedTags.includes(tagName);
  }

  // Получить стиль для подсветки тега
  getTagAppearance(tagName: string): string {
    return this.isTagSelected(tagName) ? 'primary' : 'secondary';
  }

  // Переключение состояния развернутости тегов
  toggleTagsExpanded(): void {
    this.isTagsExpanded = !this.isTagsExpanded;
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedTags = [];
    this.selectedCategory = null;
    this.showDrafts = false;
    this.sortBy = 'newest';
    this.applyFilters();
  }

  // Сортировка по категории
  sortByCategory(category: string | null): void {
    this.selectedCategory = category;
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
    const colors = ['#4CAF50', '#2196F3', '#FF9800', '#9C27B0', '#E91E63'];
    const index = this.categories.findIndex(c => c.name === categoryName);
    return index !== -1 ? colors[index % colors.length] : colors[0];
  }

  // Методы для работы с диалогом создания поста
  openCreatePostDialog(): void {
    this.isCreatePostDialogOpen = true;
  }

  closeCreatePostDialog(): void {
    this.isCreatePostDialogOpen = false;
  }

  // Обновленный метод для обработки созданного поста
  onPostCreated(newPostData: NewPostData): void {
    console.log('Получены данные нового поста:', newPostData);

    // Генерируем уникальный ID для нового поста
    const newPostId = (this.allPosts.length + 1).toString();
    const currentUserId = 'user-' + Math.random().toString(36).substr(2, 8);

    // Создаем новый пост с данными из диалога
    const newPost: PostModel = {
      id: newPostId,
      title: newPostData.title,
      content: newPostData.content,
      authorId: currentUserId,
      category: newPostData.categoryName,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      isPublished: true,
      tagNames: [...newPostData.tagNames]
    };

    // Добавляем пост в начало списка
    this.allPosts.unshift(newPost);

    // Добавляем новые теги в общий список, если их там нет
    newPostData.tagNames.forEach(tagName => {
      const tagExists = this.allTags.some(tag => tag.name === tagName);
      if (!tagExists) {
        const newTagId = (this.allTags.length + 1).toString();
        this.allTags.push({ id: newTagId, name: tagName });
      }
    });

    // Обновляем отфильтрованные теги
    this.applyTagSearch();

    // Применяем фильтры
    this.applyFilters();

    // Закрываем диалог
    this.closeCreatePostDialog();

    // Показываем уведомление или выполняем другие действия
    console.log('Пост успешно добавлен:', newPost);
  }
}
