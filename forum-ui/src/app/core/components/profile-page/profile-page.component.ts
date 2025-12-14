import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  TuiButton,
  TuiLoader,
} from '@taiga-ui/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, lastValueFrom } from 'rxjs';
import { logoutUser } from '../../../api/fn/auth/logout-user';
import { ApiConfiguration } from '../../../api/api-configuration';
import { UserDto } from '../../../api/models/user-dto';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [
    TuiButton,
    TuiLoader,
  ],
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

  // Заглушка постов
  protected readonly posts = signal([
    {
      id: 1,
      title: 'Мой первый пост',
      content: 'Содержание первого поста...',
      date: '2024-12-10',
    },
    {
      id: 2,
      title: 'Второй пост',
      content: 'Содержание второго поста...',
      date: '2024-12-11',
    },
  ]);

  ngOnInit(): void {
    // Убираем await, так как это не асинхронный метод
    this.loadUserData();
  }

  // Получение данных пользователя
  private loadUserData(): void {
    // Проверяем, авторизован ли пользователь
    const token = localStorage.getItem('accessToken');
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      // В реальном приложении здесь нужно получить userId из токена
      // Для демо используем заглушку - нужно будет доработать
      // Пока что просто проверяем авторизацию
      this.errorMessage.set(
        'Для получения данных пользователя нужен userId. Пока используем заглушку.'
      );

      // Заглушка данных
      this.userData.set({
        id: '00000000-0000-0000-0000-000000000000',
        userName: 'TestUser',
        email: 'test@example.com',
        active: true,
        accessFailedCount: 0,
        roles: ['User'],
      } as UserDto);
    } catch (error: unknown) {
      console.error('Error loading user data:', error);
      this.errorMessage.set('Ошибка загрузки данных пользователя');
    } finally {
      this.isLoading.set(false);
    }
  }

  // Переключение вкладок
  protected onActiveItemIndexChange(index: number): void {
    this.activeItemIndex.set(index);
  }

  // Выход из аккаунта
  async onLogout(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      // Используем сгенерированную функцию logoutUser
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

      // Очищаем токен из localStorage
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');

      console.log('Logout successful');
      this.router.navigate(['/login']);
    } catch (error: unknown) {
      console.error('Unexpected logout error:', error);
      // Даже если ошибка, очищаем локально и перенаправляем
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      this.router.navigate(['/login']);
    } finally {
      this.isLoading.set(false);
    }
  }
}
