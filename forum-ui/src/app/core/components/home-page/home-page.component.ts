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

// Комментарии (модели из сгенерированного api/models)
import { CommentModel } from '../../../api/models/comment-model';
import { CreateCommentCommand } from '../../../api/models/create-comment-command';
import { UpdateCommentCommand } from '../../../api/models/update-comment-command';
import { DeleteCommentCommand } from '../../../api/models/delete-comment-command';
import { UserDto } from '../../../api/models/user-dto';

// Импортируем компонент диалога
import { CreatePostDialogComponent, NewPostData } from '../../../features/post/create-post-dialog/create-post-dialog.component';

// Интерфейс для ответа текущего пользователя (как в profile-page)
interface CurrentUserResponse {
  result: OperationResultDto;
  currentUser: CurrentUserDto;
}

type StringMap<T> = Record<string, T>;

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

  // =========================
  // Комментарии (НОВОЕ)
  // =========================
  protected readonly commentsExpandedByPostId = signal<StringMap<boolean>>({});
  protected readonly commentsLoadingByPostId = signal<StringMap<boolean>>({});
  protected readonly commentsErrorByPostId = signal<StringMap<string | null>>({});
  protected readonly commentsByPostId = signal<StringMap<CommentModel[]>>({});

  // Черновик нового комментария для каждого поста
  protected readonly newCommentContentByPostId = signal<StringMap<string>>({});

  // Редактирование
  protected readonly editingCommentId = signal<string | null>(null);
  protected readonly editingCommentContent = signal<string>('');

  // Кеш логинов по authorId (Identity)
  protected readonly userNameById = signal<StringMap<string>>({});

  // ВАЖНО: для шаблона (нельзя new Date() в html)
  protected get nowIso(): string {
    return new Date().toISOString();
  }

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

  private getHttpOptionsWithAuth(): { headers?: HttpHeaders } {
    const headers = this.getAuthHeaders();
    return headers ? { headers } : {};
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
          category: posts[0].category,
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

  // НОВОЕ: дата+время (для комментариев)
  protected formatDateTime(dateString: string | undefined | null): string {
    if (!dateString) return 'Время не указано';
    try {
      return new Date(dateString).toLocaleString('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
    } catch (e) {
      console.error('Error formatting datetime:', e);
      return 'Время не указано';
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

  // =========================
  // Комментарии: UI helpers (НОВОЕ)
  // =========================
  protected isCommentsExpanded(postId: string | null | undefined): boolean {
    if (!postId) return false;
    return this.commentsExpandedByPostId()[postId] ?? false;
  }

  protected isCommentsLoading(postId: string | null | undefined): boolean {
    if (!postId) return false;
    return this.commentsLoadingByPostId()[postId] ?? false;
  }

  protected getCommentsError(postId: string | null | undefined): string | null {
    if (!postId) return null;
    return this.commentsErrorByPostId()[postId] ?? null;
  }

  protected getCommentsForPost(postId: string | null | undefined): CommentModel[] {
    if (!postId) return [];
    return this.commentsByPostId()[postId] ?? [];
  }

  protected getCommentsCount(postId: string | null | undefined): number {
    if (!postId) return 0;
    const cached = this.commentsByPostId()[postId];
    return cached ? cached.length : 0;
  }

  protected getNewCommentContent(postId: string | null | undefined): string {
    if (!postId) return '';
    return this.newCommentContentByPostId()[postId] ?? '';
  }

  protected setNewCommentContent(postId: string, value: string): void {
    if (!postId) return;
    const map = { ...this.newCommentContentByPostId() };
    map[postId] = value;
    this.newCommentContentByPostId.set(map);
  }

  protected canManageComment(comment: CommentModel): boolean {
    const me = this.currentUser();
    if (!me?.id) return false;
    return comment.authorId === me.id;
  }

  protected isEditing(commentId: string | null | undefined): boolean {
    if (!commentId) return false;
    return this.editingCommentId() === commentId;
  }

  protected startEditComment(comment: CommentModel, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (!comment.id) return;
    if (!this.canManageComment(comment)) return;

    this.editingCommentId.set(comment.id);
    this.editingCommentContent.set(comment.content ?? '');
    console.log('[comments] startEditComment', { commentId: comment.id });
  }

  protected cancelEditComment(event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    console.log('[comments] cancelEditComment', { commentId: this.editingCommentId() });
    this.editingCommentId.set(null);
    this.editingCommentContent.set('');
  }

  protected async toggleCommentsForPost(postId: string | null | undefined, event: Event): Promise<void> {
    event.preventDefault();
    event.stopPropagation();

    if (!postId) return;

    const expanded = this.isCommentsExpanded(postId);
    const map = { ...this.commentsExpandedByPostId() };
    map[postId] = !expanded;
    this.commentsExpandedByPostId.set(map);

    console.log('[comments] toggleCommentsForPost', { postId, expanded: !expanded });

    if (!expanded) {
      // раскрыли -> загружаем
      await this.loadCommentsByPost(postId);
    }
  }

  // =========================
  // Комментарии: API calls (НОВОЕ)
  // =========================
  private setPostBool(mapSignal: ReturnType<typeof signal<StringMap<boolean>>>, postId: string, value: boolean): void {
    const map = { ...mapSignal() };
    map[postId] = value;
    mapSignal.set(map);
  }

  private setPostError(postId: string, value: string | null): void {
    const map = { ...this.commentsErrorByPostId() };
    map[postId] = value;
    this.commentsErrorByPostId.set(map);
  }

  private async loadCommentsByPost(postId: string): Promise<void> {
    console.log('[comments] loadCommentsByPost start', { postId });

    this.setPostBool(this.commentsLoadingByPostId, postId, true);
    this.setPostError(postId, null);

    try {
      const url = `${this.apiConfig.rootUrl}/api/forum/v1/Comment/GetCommentsByPost/${postId}/post`;
      const response = await lastValueFrom(
        this.http.get<CommentModel[]>(url, this.getHttpOptionsWithAuth())
      );

      const comments = response ?? [];
      console.log('[comments] loadCommentsByPost success', { postId, count: comments.length });

      const cache = { ...this.commentsByPostId() };
      cache[postId] = comments;
      this.commentsByPostId.set(cache);

      // Подгружаем логины авторов
      const authorIds = Array.from(
        new Set(comments.map(c => c.authorId).filter((x): x is string => !!x))
      );
      await this.preloadUserNames(authorIds);

    } catch (e) {
      console.error('[comments] loadCommentsByPost error', e);
      this.setPostError(postId, 'Не удалось загрузить комментарии');
    } finally {
      this.setPostBool(this.commentsLoadingByPostId, postId, false);
    }
  }

  protected async createComment(postId: string | null | undefined, event: Event): Promise<void> {
    event.preventDefault();
    event.stopPropagation();

    if (!postId) return;

    const me = this.currentUser();
    if (!me) {
      this.setPostError(postId, 'Чтобы комментировать — войдите в аккаунт');
      return;
    }

    const content = (this.getNewCommentContent(postId) ?? '').trim();
    if (!content) {
      this.setPostError(postId, 'Комментарий не может быть пустым');
      return;
    }

    const body: CreateCommentCommand = {
      postId,
      content,
    };

    console.log('[comments] createComment start', { postId, contentLength: content.length });

    try {
      const url = `${this.apiConfig.rootUrl}/api/forum/v1/Comment/CreateComment`;
      await lastValueFrom(this.http.post(url, body, this.getHttpOptionsWithAuth()));
      console.log('[comments] createComment success', { postId });

      // очистить поле
      this.setNewCommentContent(postId, '');

      // перезагрузить список
      await this.loadCommentsByPost(postId);
    } catch (e) {
      console.error('[comments] createComment error', e);
      this.setPostError(postId, 'Не удалось добавить комментарий');
    }
  }

  protected async saveEditedComment(comment: CommentModel, postId: string | null | undefined, event: Event): Promise<void> {
    event.preventDefault();
    event.stopPropagation();

    if (!postId) return;
    if (!comment.id) return;
    if (!this.canManageComment(comment)) return;

    const content = (this.editingCommentContent() ?? '').trim();
    if (!content) {
      this.setPostError(postId, 'Комментарий не может быть пустым');
      return;
    }

    const body: UpdateCommentCommand = {
      id: comment.id,
      content,
    };

    console.log('[comments] updateComment start', { commentId: comment.id, postId, contentLength: content.length });

    try {
      const url = `${this.apiConfig.rootUrl}/api/forum/v1/Comment/UpdateComment`;
      await lastValueFrom(this.http.patch(url, body, this.getHttpOptionsWithAuth()));
      console.log('[comments] updateComment success', { commentId: comment.id });

      this.editingCommentId.set(null);
      this.editingCommentContent.set('');

      await this.loadCommentsByPost(postId);
    } catch (e) {
      console.error('[comments] updateComment error', e);
      this.setPostError(postId, 'Не удалось обновить комментарий');
    }
  }

  protected async deleteComment(comment: CommentModel, postId: string | null | undefined, event: Event): Promise<void> {
    event.preventDefault();
    event.stopPropagation();

    if (!postId) return;
    if (!comment.id) return;
    if (!this.canManageComment(comment)) return;

    const ok = confirm('Удалить комментарий?');
    if (!ok) return;

    const body: DeleteCommentCommand = { id: comment.id };

    console.log('[comments] deleteComment start', { commentId: comment.id, postId });

    try {
      const url = `${this.apiConfig.rootUrl}/api/forum/v1/Comment/DeleteComment`;
      await lastValueFrom(
        this.http.request('delete', url, {
          ...this.getHttpOptionsWithAuth(),
          body,
        })
      );

      console.log('[comments] deleteComment success', { commentId: comment.id });
      await this.loadCommentsByPost(postId);
    } catch (e) {
      console.error('[comments] deleteComment error', e);
      this.setPostError(postId, 'Не удалось удалить комментарий');
    }
  }

  // =========================
  // Identity: логины авторов (НОВОЕ)
  // =========================
  protected getAuthorLogin(authorId: string | null | undefined): string {
    if (!authorId) return 'unknown';
    return this.userNameById()[authorId] ?? `Пользователь ${authorId.substring(0, 8)}`;
  }

  protected getInitials(text: string | null | undefined): string {
    const value = (text ?? '').trim();
    if (!value) return 'U';
    return value.substring(0, 2).toUpperCase();
  }

  private async preloadUserNames(userIds: string[]): Promise<void> {
    for (const id of userIds) {
      if (!id) continue;
      if (this.userNameById()[id]) continue;

      try {
        const url = `${this.apiConfig.rootUrl}/api/identity/Users/${id}`;
        const user = await lastValueFrom(this.http.get<UserDto>(url, this.getHttpOptionsWithAuth()));
        const login = user?.userName ?? `Пользователь ${id.substring(0, 8)}`;

        const map = { ...this.userNameById() };
        map[id] = login;
        this.userNameById.set(map);

        console.log('[identity] cached userName', { id, login });
      } catch (e) {
        console.error('[identity] getUserById error', { id, e });
        const map = { ...this.userNameById() };
        map[id] = `Пользователь ${id.substring(0, 8)}`;
        this.userNameById.set(map);
      }
    }
  }
}
