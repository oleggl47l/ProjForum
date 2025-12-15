import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import {TuiButton, TuiLoader} from '@taiga-ui/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, lastValueFrom } from 'rxjs';
import { logoutUser } from '../../../api/fn/auth/logout-user';
import { ApiConfiguration } from '../../../api/api-configuration';
import { UserDto } from '../../../api/models/user-dto';

interface Post {
  id: string;
  title: string;
  content: string;
  date: string;
  category?: string;
  tags?: string[];
  isPublished?: boolean;
}

interface PostApiResponse {
  id: string;
  title: string | null;
  content: string | null;
  authorId: string;
  category: string | null;
  createdAt: string;
  updatedAt: string;
  isPublished: boolean;
  tagNames: string[] | null;
}

interface CurrentUserResponse {
  result: {
    success: boolean;
    message?: string;
  };
  currentUser: {
    id: string;
    userName: string | null;
    roles: string[] | null;
  };
}

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [TuiButton, TuiLoader],
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePageComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfiguration);

  // Сигналы для управления состоянием
  protected readonly activeItemIndex = signal(0);
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly userData = signal<UserDto | null>(null);
  protected readonly posts = signal<Post[]>([]);

  ngOnInit(): void {
    console.log(
      'ProfilePageComponent initialized - URL:',
      window.location.href
    );
    console.log('Token exists:', !!localStorage.getItem('accessToken'));

    // Загружаем данные пользователя
    this.loadUserData().catch(error => {
      console.error('Failed to load user data:', error);
      this.errorMessage.set('Не удалось загрузить данные пользователя');
      this.isLoading.set(false);
    });
  }

  // Переход на главную страницу
  protected async goHome(): Promise<void> {
    await this.router.navigate(['/home']);
  }

  // Получение данных текущего пользователя
  private async loadUserData(): Promise<void> {
    // Проверяем авторизацию
    const token = localStorage.getItem('accessToken');

    console.log('Token from localStorage:', token);
    console.log('Token type:', typeof token);

    if (!token) {
      await this.router.navigate(['/login']);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      // 1. Получаем данные текущего пользователя через новый эндпоинт
      const currentUserData = await lastValueFrom(
        this.http
          .get<CurrentUserResponse>(
            `${this.apiConfig.rootUrl}/api/identity/v1/Auth/getCurrentUser`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          )
          .pipe(
            catchError((error: HttpErrorResponse) => {
              console.error('Error getting current user:', error);
              if (error.status === 401) {
                this.errorMessage.set(
                  'Требуется авторизация. Пожалуйста, войдите снова.'
                );
                // Очищаем токен и перенаправляем на логин
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                setTimeout(async () => {
                  await this.router.navigate(['/login']);
                }, 1000);
              } else {
                this.errorMessage.set('Ошибка получения данных пользователя');
              }
              throw error;
            })
          )
      );

      if (!currentUserData?.result?.success || !currentUserData.currentUser) {
        throw new Error('Не удалось получить данные пользователя');
      }

      const currentUser = currentUserData.currentUser;
      const userId = currentUser.id;

      if (!userId) {
        throw new Error('ID пользователя не найден');
      }

      // 2. Получаем полную информацию о пользователе по его ID
      const userResponse = await lastValueFrom(
        this.http
          .get<UserDto>(
            `${this.apiConfig.rootUrl}/api/identity/Users/${userId}`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          )
          .pipe(
            catchError((error: HttpErrorResponse) => {
              console.error('Error getting user details:', error);
              if (error.status === 401) {
                this.errorMessage.set(
                  'Требуется авторизация для получения детальной информации'
                );
              } else {
                this.errorMessage.set('Ошибка получения детальной информации');
              }
              throw error;
            })
          )
      );

      this.userData.set(userResponse);

      // 3. Получаем посты пользователя из форумного сервиса
      await this.loadUserPosts(userId, token);
    } catch (error: unknown) {
      console.error('Error loading user data:', error);
      if (error instanceof Error) {
        this.errorMessage.set(error.message);
      } else {
        this.errorMessage.set('Неизвестная ошибка при загрузке данных');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  // Загрузка постов пользователя
  private async loadUserPosts(userId: string, token: string): Promise<void> {
    try {
      // Получаем все посты
      const postsResponse = await lastValueFrom(
        this.http
          .get<PostApiResponse[]>(
            `${this.apiConfig.rootUrl}/api/forum/v1/Post/GetPosts`,
            {
              headers: { Authorization: `Bearer ${token}` },
            }
          )
          .pipe(
            catchError((error: HttpErrorResponse) => {
              console.error('Error getting posts:', error);
              if (error.status === 401) {
                this.errorMessage.set(
                  'Требуется авторизация для загрузки постов'
                );
              } else {
                this.errorMessage.set('Ошибка загрузки постов');
              }
              throw error;
            })
          )
      );

      // Фильтруем посты текущего пользователя
      const userPosts = postsResponse
        .filter(post => post.authorId === userId)
        .map(post => ({
          id: post.id,
          title: post.title || 'Без названия',
          content: post.content || '',
          date: post.createdAt
            ? new Date(post.createdAt).toLocaleDateString('ru-RU')
            : 'Дата не указана',
          category: post.category || undefined,
          tags: post.tagNames || undefined,
          isPublished: post.isPublished,
        }));

      this.posts.set(userPosts);
    } catch (error: unknown) {
      console.error('Error loading posts:', error);
      // Не прерываем загрузку профиля, если не удалось загрузить посты
      this.posts.set([]);
    }
  }

  // Переключение вкладок
  protected onActiveItemIndexChange(index: number): void {
    this.activeItemIndex.set(index);
  }

  // Выход из аккаунта
  protected async onLogout(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const observable = logoutUser(this.http, this.apiConfig.rootUrl, {}).pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Logout error:', error);

          let errorMsg = 'Ошибка при выходе. Попробуйте еще раз.';
          if (error.status === 401) {
            errorMsg = 'Вы уже вышли из системы.';
          } else if (error.status >= 500) {
            errorMsg = 'Ошибка сервера. Попробуйте позже.';
          } else if (error.message) {
            errorMsg = error.message;
          }

          this.errorMessage.set(errorMsg);
          throw error;
        })
      );

      await lastValueFrom(observable);

      // Очищаем данные авторизации
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');

      // Очищаем состояние компонента
      this.userData.set(null);
      this.posts.set([]);

      console.log('Logout successful');
      await this.router.navigate(['/login']);
    } catch (error: unknown) {
      console.error('Unexpected logout error:', error);
      // Даже если ошибка, очищаем локально и перенаправляем
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      this.userData.set(null);
      this.posts.set([]);
      await this.router.navigate(['/login']);
    } finally {
      this.isLoading.set(false);
    }
  }
}
