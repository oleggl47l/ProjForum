import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {TuiButton, TuiError, TuiLink, TuiNotification, TuiTextfield, TuiTitle} from '@taiga-ui/core';
import { TuiFieldErrorPipe } from '@taiga-ui/kit';
import { TuiCardLarge, TuiForm, TuiHeader } from '@taiga-ui/layout';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, lastValueFrom, tap } from 'rxjs';
import { loginUser$Json } from '../../../api/fn/auth/login-user-json';
import { ApiConfiguration } from '../../../api/api-configuration';
import { LoginResultDto } from '../../../api/models/login-result-dto';
import { StrictHttpResponse } from '../../../api/strict-http-response';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
    TuiCardLarge,
    TuiError,
    TuiFieldErrorPipe,
    TuiForm,
    TuiHeader,
    TuiLink,
    TuiTextfield,
    TuiTitle,
    TuiNotification,
  ],
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginFormComponent {
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfiguration);

  // Сигналы для управления состоянием
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly form = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  async onSubmit(): Promise<void> {
    if (this.form.valid) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      // Добавляем логирование для отладки
      console.log('API rootUrl:', this.apiConfig.rootUrl);

      // Подготовка данных в формате, ожидаемом API
      const loginData = {
        userName: this.form.value.username || '',
        password: this.form.value.password || '',
      };

      console.log('Sending login data:', loginData);

      try {
        // Используем сгенерированную функцию loginUser$Json
        const observable = loginUser$Json(this.http, this.apiConfig.rootUrl, {
          body: loginData,
        }).pipe(
          tap((httpResponse: StrictHttpResponse<LoginResultDto>) => {
            const result = httpResponse.body;
            console.log('Login response:', result);
            console.log('Status:', httpResponse.status);

            // Проверяем успешность входа
            if (result?.result?.success && result?.token) {
              // Сохраняем токен в localStorage
              localStorage.setItem('accessToken', result.token);
              if (result.refreshToken) {
                localStorage.setItem('refreshToken', result.refreshToken);
              }

              console.log('Login successful! Token saved.');

              // Переходим на главную страницу
              this.router.navigate(['/']).then(success => {
                if (!success) {
                  console.error('Navigation to home page failed');
                }
              });
            } else {
              const errorMsg =
                result?.result?.message || 'Login failed (unknown reason)';
              console.error('Login failed:', errorMsg);
              this.errorMessage.set(errorMsg);
            }
          }),
          catchError((error: HttpErrorResponse) => {
            console.error('Login HTTP error:', error);
            console.error('Error details:', error.error);
            console.error('Error status:', error.status);

            let errorMsg =
              'Login failed. Please check your credentials and try again.';

            // Проверяем детали ошибки из ответа сервера
            if (error.error?.detail) {
              errorMsg = `Server error: ${error.error.detail}`;
            } else if (error.error?.message) {
              errorMsg = error.error.message;
            } else if (error.status === 400) {
              errorMsg = 'Invalid username or password.';
            } else if (error.status === 401) {
              errorMsg = 'Invalid credentials.';
            } else if (error.status === 403) {
              errorMsg = 'Access denied. Your account may be blocked.';
            } else if (error.status === 404) {
              errorMsg = 'User not found.';
            } else if (error.status >= 500) {
              errorMsg = 'Server error. Please try again later.';
            } else if (error.message) {
              errorMsg = error.message;
            }

            this.errorMessage.set(errorMsg);
            throw error; // Пробрасываем ошибку дальше
          })
        );

        // Используем lastValueFrom
        await lastValueFrom(observable);
      } catch (error: unknown) {
        console.error('Unexpected login error:', error);
        // Сообщение уже установлено в catchError
      } finally {
        this.isLoading.set(false);
      }
    } else {
      this.form.markAllAsTouched();
      this.errorMessage.set('Please fill in all required fields');
    }
  }

  navigateToRegister(): void {
    this.router
      .navigate(['/register'])
      .then(success => {
        if (!success) {
          console.error('Navigation to register page failed');
        }
      })
      .catch(error => {
        console.error('Navigation error:', error);
      });
  }
}
