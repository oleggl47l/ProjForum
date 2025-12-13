import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TuiButton, TuiError, TuiLink, TuiTextfield, TuiTitle } from '@taiga-ui/core';
import { TuiFieldErrorPipe } from '@taiga-ui/kit';
import { TuiCardLarge, TuiForm, TuiHeader } from '@taiga-ui/layout';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, lastValueFrom, of, tap } from 'rxjs';
import { registerUser$Json } from '../../../api/fn/auth/register-user-json';
import { ApiConfiguration } from '../../../api/api-configuration';
import { OperationResultDto } from '../../../api/models/operation-result-dto';
import { StrictHttpResponse } from '../../../api/strict-http-response';

@Component({
  selector: 'app-register-form',
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
  ],
  templateUrl: './register-form.component.html',
  styleUrls: ['./register-form.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterFormComponent {
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfiguration);

  // Сигналы для управления состоянием
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);

  protected readonly form = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.minLength(3)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  async onSubmit(): Promise<void> {
    if (this.form.valid) {
      this.isLoading.set(true);
      this.errorMessage.set(null);
      this.successMessage.set(null);

      // Подготовка данных в формате, ожидаемом API
      const registerData = {
        userName: this.form.value.username || '',
        email: this.form.value.email || '',
        password: this.form.value.password || '',
        roles: ['User'] // Роль по умолчанию
      };

      console.log('Sending registration data:', registerData);

      try {
        // Используем сгенерированную функцию registerUser$Json вместо Plain
        const observable = registerUser$Json(
          this.http,
          this.apiConfig.rootUrl,
          { body: registerData }
        ).pipe(
          tap((httpResponse: StrictHttpResponse<OperationResultDto>) => {
            const result = httpResponse.body;
            console.log('Registration response:', result);
            console.log('Success value:', result?.success);
            console.log('Message:', result?.message);

            // Проверяем success как boolean
            if (result?.success === true) {
              this.successMessage.set('Registration successful! You can now log in.');
              this.form.reset();

              // Автоматический переход на страницу логина через 2 секунды
              setTimeout(() => {
                this.navigateToLogin();
              }, 2000);
            } else {
              // Если success = false или undefined
              const errorMsg = result?.message || 'Registration failed (unknown reason)';
              console.error('Registration failed:', errorMsg);
              this.errorMessage.set(errorMsg);
            }
          }),
          catchError((error: HttpErrorResponse) => {
            console.error('Registration HTTP error:', error);

            let errorMsg = 'Registration failed. Please try again.';
            if (error.status === 400) {
              errorMsg = 'Invalid registration data. Please check your input.';
            } else if (error.status === 409) {
              errorMsg = 'Username or email already exists.';
            } else if (error.status && error.status >= 500) {
              errorMsg = 'Server error. Please try again later.';
            } else if (error.message) {
              errorMsg = error.message;
            }

            this.errorMessage.set(errorMsg);
            return of(null);
          })
        );

        // Используем lastValueFrom вместо deprecated toPromise()
        await lastValueFrom(observable);

      } catch (error: unknown) {
        console.error('Unexpected error:', error);
        this.errorMessage.set('An unexpected error occurred. Please try again.');
      } finally {
        this.isLoading.set(false);
      }
    } else {
      this.form.markAllAsTouched();

      // Показать сообщения об ошибках валидации
      if (this.form.get('username')?.errors?.['required']) {
        this.errorMessage.set('Username is required');
      } else if (this.form.get('username')?.errors?.['minlength']) {
        this.errorMessage.set('Username must be at least 3 characters long');
      } else if (this.form.get('email')?.errors?.['required']) {
        this.errorMessage.set('Email is required');
      } else if (this.form.get('email')?.errors?.['email']) {
        this.errorMessage.set('Please enter a valid email address');
      } else if (this.form.get('password')?.errors?.['required']) {
        this.errorMessage.set('Password is required');
      } else if (this.form.get('password')?.errors?.['minlength']) {
        this.errorMessage.set('Password must be at least 6 characters long');
      }
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']).then(success => {
      if (!success) {
        console.error('Navigation to login page failed');
      }
    }).catch(error => {
      console.error('Navigation error:', error);
    });
  }
}
