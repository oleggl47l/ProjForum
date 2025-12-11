import { AsyncPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TuiButton, TuiError, TuiLink, TuiTextfield, TuiTitle } from '@taiga-ui/core';
import { TuiFieldErrorPipe } from '@taiga-ui/kit';
import { TuiCardLarge, TuiForm, TuiHeader } from '@taiga-ui/layout';

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

  protected readonly form = new FormGroup({
    username: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
  });

  onSubmit(): void {
    if (this.form.valid) {
      console.log('Registration form submitted:', this.form.value);
      // Здесь будет логика отправки данных регистрации
    } else {
      this.form.markAllAsTouched();
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
