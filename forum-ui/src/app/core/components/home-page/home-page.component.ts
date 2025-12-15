import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { catchError, lastValueFrom } from 'rxjs';

// Taiga UI Core для standalone
import { TuiButton, TuiLoader } from '@taiga-ui/core';
// Taiga UI Kit для standalone
import { TuiAvatar, TuiBadge, TuiPagination } from '@taiga-ui/kit';

// Импортируем сгенерированные API модели
import { ApiConfiguration } from '../../../api/api-configuration';
import { PostModel } from '../../../api/models/post-model';
import { CategoryModel } from '../../../api/models/category-model';
import { TagModel } from '../../../api/models/tag-model';
import { CurrentUserDto } from '../../../api/models/current-user-dto';
import { OperationResultDto } from '../../../api/models/operation-result-dto';

// Импортируем компонент диалога
import { CreatePostDialogComponent, NewPostData } from '../../../features/post/create-post-dialog/create-post-dialog.component';

// Интерфейс для ответа текущего пользователя (как в profile-page)
interface CurrentUserResponse {
  result: OperationResultDto;
  currentUser: CurrentUserDto;
}

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    TuiButton,
    TuiLoader,
    TuiAvatar,
    TuiBadge,
    TuiPagination,
    CreatePostDialogComponent
  ],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePageComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfiguration);

  // Сигналы для состояния
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly currentUser = signal<CurrentUserDto | null>(null);

  // Сигналы для данных
  protected readonly posts = signal<PostModel[]>([]);
  protected readonly allPosts = signal<PostModel[]>([]);
  protected readonly categories = signal<CategoryModel[]>([]);
  protected readonly allTags = signal<TagModel[]>([]);
  protected readonly filteredTags = signal<TagModel[]>([]);

  // Сигналы для фильтров
  protected readonly searchQuery = signal('');
  protected readonly tagSearchQuery = signal('');
  protected readonly selectedTags = signal<string[]>([]);
  protected readonly selectedCategory = signal<string | null>(null);
  protected readonly showDrafts = signal(false);
  protected readonly sortBy = signal<'newest' | 'oldest'>('newest');
  protected readonly currentPage = signal(0);
  protected readonly itemsPerPage = 10;
  protected readonly isTagsExpanded = signal(false);

  // Диалог создания поста
  protected readonly isCreatePostDialogOpen = signal(false);

  // Метод для получения заголовков с токеном
  private getAuthHeaders(): HttpHeaders | null {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      return null;
    }
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  ngOnInit(): void {
    console.log('HomePageComponent initialized');
    console.log('Token exists:', !!localStorage.getItem('accessToken'));
    console.log('Token value:', localStorage.getItem('accessToken')?.substring(0, 20) + '...');

    this.loadInitialData().catch(error => {
      console.error('Failed to load initial data:', error);
      this.errorMessage.set('Не удалось загрузить данные');
      this.isLoading.set(false);
    });
  }

  protected async loadInitialData(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      // 1. Загружаем текущего пользователя
      await this.loadCurrentUser();

      // 2. Загружаем все данные параллельно
      await Promise.all([
        this.loadPosts(),
        this.loadCategories(),
        this.loadTags()
      ]);

      console.log('Initial data loaded successfully');
    } catch (error: unknown) {
      console.error('Error loading initial data:', error);
      if (error instanceof Error) {
        this.errorMessage.set(error.message);
      } else {
        this.errorMessage.set('Ошибка при загрузке данных');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  private async loadCurrentUser(): Promise<void> {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      console.log('No token found, user not logged in');
      return;
    }

    console.log('Loading current user with token length:', token.length);

    try {
      // Используем прямой запрос с заголовком, как в profile-page
      const response = await lastValueFrom(
        this.http.get<CurrentUserResponse>(
          `${this.apiConfig.rootUrl}/api/identity/v1/Auth/getCurrentUser`,
          {
            headers: { Authorization: `Bearer ${token}` }
          }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error getting current user:', error);
            console.error('Error status:', error.status);
            console.error('Error message:', error.message);
            console.error('Error headers:', error.headers);

            if (error.status === 401) {
              console.log('User not authenticated - token might be invalid or expired');
              // Если токен невалидный, очищаем его
              localStorage.removeItem('accessToken');
              localStorage.removeItem('refreshToken');
              this.errorMessage.set('Сессия истекла. Пожалуйста, войдите снова.');
            } else if (error.status === 403) {
              console.log('Forbidden - insufficient permissions');
              this.errorMessage.set('Недостаточно прав для выполнения операции');
            }
            throw error;
          })
        )
      );

      if (response?.result?.success && response.currentUser) {
        this.currentUser.set(response.currentUser);
        console.log('Current user loaded:', response.currentUser);
      } else {
        console.log('Failed to load current user - response:', response);
      }
    } catch (error: unknown) {
      // Игнорируем ошибку, если пользователь не авторизован
      console.log('User might not be logged in or token expired');
      if (error instanceof HttpErrorResponse && error.status !== 401) {
        console.error('Error loading current user:', error);
        this.errorMessage.set('Ошибка загрузки данных пользователя');
      }
    }
  }

  private async loadPosts(): Promise<void> {
    console.log('Loading posts...');
    const token = localStorage.getItem('accessToken');

    try {
      let headers = {};
      if (token) {
        headers = { Authorization: `Bearer ${token}` };
      }

      // Используем прямой запрос с заголовком
      const response = await lastValueFrom(
        this.http.get<PostModel[]>(
          `${this.apiConfig.rootUrl}/api/forum/v1/Post/GetPosts`,
          { headers }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error loading posts:', error);
            console.error('Error status:', error.status);

            if (error.status === 401) {
              console.log('Posts require authentication, but token is missing or invalid');
              // Не устанавливаем ошибку, так как посты могут требовать авторизации
              return [];
            } else if (error.status === 403) {
              this.errorMessage.set('Недостаточно прав для просмотра постов');
            } else {
              this.errorMessage.set('Ошибка загрузки постов');
            }
            throw error;
          })
        )
      );

      const posts = response || [];
      console.log(`Loaded ${posts.length} posts`);

      if (posts.length > 0) {
        console.log('First post sample:', {
          id: posts[0].id,
          title: posts[0].title,
          authorId: posts[0].authorId,
          category: posts[0].category
        });
      }

      this.allPosts.set(posts);
      this.posts.set(posts); // Изначально показываем все посты

    } catch (error: unknown) {
      console.error('Failed to load posts:', error);
      // Устанавливаем пустой массив в случае ошибки
      this.allPosts.set([]);
      this.posts.set([]);
    }
  }

  private async loadCategories(): Promise<void> {
    console.log('Loading categories...');
    const token = localStorage.getItem('accessToken');

    try {
      let headers = {};
      if (token) {
        headers = { Authorization: `Bearer ${token}` };
      }

      const response = await lastValueFrom(
        this.http.get<CategoryModel[]>(
          `${this.apiConfig.rootUrl}/api/forum/v1/Category/GetCategories`,
          { headers }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error loading categories:', error);
            if (error.status === 401) {
              console.log('Categories require authentication');
              // Возвращаем пустой массив для категорий
              return [];
            }
            throw error;
          })
        )
      );

      const categories = response || [];
      console.log(`Loaded ${categories.length} categories`);
      this.categories.set(categories);

    } catch (error) {
      console.error('Failed to load categories, using empty array:', error);
      this.categories.set([]);
    }
  }

  private async loadTags(): Promise<void> {
    console.log('Loading tags...');
    const token = localStorage.getItem('accessToken');

    try {
      let headers = {};
      if (token) {
        headers = { Authorization: `Bearer ${token}` };
      }

      const response = await lastValueFrom(
        this.http.get<TagModel[]>(
          `${this.apiConfig.rootUrl}/api/forum/v1/Tag/GetTags`,
          { headers }
        ).pipe(
          catchError((error: HttpErrorResponse) => {
            console.error('Error loading tags:', error);
            if (error.status === 401) {
              console.log('Tags require authentication');
              // Возвращаем пустой массив для тегов
              return [];
            }
            throw error;
          })
        )
      );

      const tags = response || [];
      console.log(`Loaded ${tags.length} tags`);
      this.allTags.set(tags);
      this.filteredTags.set(tags);

    } catch (error) {
      console.error('Failed to load tags, using empty array:', error);
      this.allTags.set([]);
      this.filteredTags.set([]);
    }
  }

  // Методы для фильтрации (остаются без изменений)
  protected applyFilters(): void {
    console.log('Applying filters...');
    this.isLoading.set(true);

    let filtered = [...this.allPosts()];

    // Фильтрация по поисковому запросу
    const searchQuery = this.searchQuery().toLowerCase().trim();
    if (searchQuery) {
      filtered = filtered.filter(post =>
        (post.title?.toLowerCase().includes(searchQuery)) ||
        (post.content?.toLowerCase().includes(searchQuery)) ||
        (post.category?.toLowerCase().includes(searchQuery))
      );
      console.log(`Filtered by search query "${searchQuery}": ${filtered.length} posts`);
    }

    // Фильтрация по тегам
    const selectedTags = this.selectedTags();
    if (selectedTags.length > 0) {
      filtered = filtered.filter(post =>
        post.tagNames?.some(tag => selectedTags.includes(tag))
      );
      console.log(`Filtered by tags ${selectedTags.join(', ')}: ${filtered.length} posts`);
    }

    // Фильтрация по категории
    const selectedCategory = this.selectedCategory();
    if (selectedCategory) {
      filtered = filtered.filter(post => post.category === selectedCategory);
      console.log(`Filtered by category "${selectedCategory}": ${filtered.length} posts`);
    }

    // Скрыть черновики, если не выбрано иное
    if (!this.showDrafts()) {
      filtered = filtered.filter(post => post.isPublished);
    }

    // Сортировка с проверкой на существование createdAt
    filtered.sort((a, b) => {
      const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;
      const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;

      switch (this.sortBy()) {
        case 'newest':
          return dateB - dateA;
        case 'oldest':
          return dateA - dateB;
        default:
          return 0;
      }
    });

    this.posts.set(filtered);
    this.currentPage.set(0);
    this.isLoading.set(false);
  }

  protected applyTagSearch(): void {
    const query = this.tagSearchQuery().toLowerCase().trim();
    if (query) {
      const filtered = this.allTags().filter(tag =>
        tag.name?.toLowerCase().includes(query)
      );
      this.filteredTags.set(filtered);
    } else {
      this.filteredTags.set([...this.allTags()]);
    }
  }

  protected clearTagSearch(): void {
    this.tagSearchQuery.set('');
    this.applyTagSearch();
  }

  protected toggleTagFilter(tagName: string): void {
    const tags = [...this.selectedTags()];
    const index = tags.indexOf(tagName);

    if (index > -1) {
      tags.splice(index, 1);
    } else {
      tags.push(tagName);
    }

    this.selectedTags.set(tags);
    this.applyFilters();
  }

  protected isTagSelected(tagName: string): boolean {
    return this.selectedTags().includes(tagName);
  }

  protected getTagAppearance(tagName: string): string {
    return this.isTagSelected(tagName) ? 'primary' : 'secondary';
  }

  protected toggleTagsExpanded(): void {
    this.isTagsExpanded.set(!this.isTagsExpanded());
  }

  protected clearFilters(): void {
    console.log('Clearing all filters');
    this.searchQuery.set('');
    this.selectedTags.set([]);
    this.selectedCategory.set(null);
    this.tagSearchQuery.set('');
    this.showDrafts.set(false);
    this.sortBy.set('newest');
    this.posts.set([...this.allPosts()]);
    this.currentPage.set(0);
  }

  protected clearSearchQuery(): void {
    this.searchQuery.set('');
    this.applyFilters();
  }

  protected sortByCategory(category: string | null): void {
    console.log('Sorting by category:', category);
    this.selectedCategory.set(category);
    this.applyFilters();
  }

  protected get filteredPosts(): PostModel[] {
    const start = this.currentPage() * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.posts().slice(start, end);
  }

  protected get totalPages(): number {
    return Math.ceil(this.posts().length / this.itemsPerPage);
  }

  protected formatDate(dateString: string | undefined | null): string {
    if (!dateString) {
      return 'Дата не указана';
    }

    try {
      return new Date(dateString).toLocaleDateString('ru-RU', {
        day: 'numeric',
        month: 'short',
        year: 'numeric'
      });
    } catch (error) {
      console.error('Error formatting date:', error);
      return 'Дата не указана';
    }
  }

  protected getPostExcerpt(content: string | null | undefined): string {
    if (!content) return '';
    const plainText = content.replace(/<[^>]*>/g, '');
    return plainText.length > 100 ? plainText.substring(0, 100) + '...' : plainText;
  }

  // Методы для работы с диалогом создания поста
  protected openCreatePostDialog(): void {
    this.isCreatePostDialogOpen.set(true);
  }

  protected closeCreatePostDialog(): void {
    this.isCreatePostDialogOpen.set(false);
  }

  protected onPostCreated(newPostData: NewPostData): void {
    console.log('New post created:', newPostData);
    // TODO: Реализовать обновление списка постов после создания нового
    // Пока просто закрываем диалог
    this.closeCreatePostDialog();
  }

  // Геттер для currentPage для работы с пагинацией
  protected get currentPageValue(): number {
    return this.currentPage();
  }

  protected set currentPageValue(value: number) {
    this.currentPage.set(value);
  }
}
