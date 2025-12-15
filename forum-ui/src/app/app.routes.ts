import { Routes } from '@angular/router';
import { LoginFormComponent } from './core/components/login-form/login-form.component';
import { RegisterFormComponent } from './core/components/register-form/register-form.component';
import { ProfilePageComponent } from './core/components/profile-page/profile-page.component';
import {HomePageComponent} from './core/components/home-page/home-page.component';
import { inject } from '@angular/core';
import { Router } from '@angular/router';


// TODO: для логина нужно будет добавить еще guard canActivate персональный и
// для общего path тоже нужно будет добавить canActivate для предотварщения
// некорректной работы с роутами после логина и по его истечении и использования без аутентификации
// p.s. пример закомменченный для использования оставилd
// также что-то типа примеров:
// import { inject } from '@angular/core';
// import { Router, UrlTree } from '@angular/router';
// import { AuthService } from '@core/services/auth/auth.service';
// export const loginGuard = (): boolean | UrlTree => {
//   const authService = inject(AuthService);
//   const router = inject(Router);
//   if (authService.isLoggedIn()) {
//     return router.createUrlTree(['/home']);
//   }
//   return true;
// };
// import { inject } from '@angular/core';
// import {
//   ActivatedRouteSnapshot,
//   Router,
//   RouterStateSnapshot,
//   UrlTree,
// } from '@angular/router';
// import { AuthService } from '@core/services/auth/auth.service';
// export const authGuard = (
//   _route: ActivatedRouteSnapshot,
//   state: RouterStateSnapshot
// ): boolean | UrlTree => {
//   const authService = inject(AuthService);
//   const router = inject(Router);
//   if (authService.isLoggedIn()) {
//     return true;
//   }
//   const attemptedUrl = state.url || router.url || '/';
//   if (attemptedUrl.startsWith('/login')) {
//     return router.createUrlTree(['/login']);
//   }
//   return router.createUrlTree(['/login'], {
//     queryParams: { returnUrl: attemptedUrl },
//   });
// };

// TODO: уже сюда тоже напишу, для форм нужно будет также сделать гуард canDeactivate с вызовом формы подтверждения действия.
// работать будет примерно так: например, идет создание статьи, вы ввели какие-то значения в форму,
// а потом случайно закрываете - гуард перехватывает и выводит диалоговое окно с просьбой подтверждения
// что-то типо примера:
// import { CanDeactivateFn } from '@angular/router';
// import { CanComponentDeactivate } from '../interfaces/can-component-deactivate.interface';
// import { inject } from '@angular/core';
// import { DialogService } from '@shared/services/dialog.service';
// export const canDeactivateGuard: CanDeactivateFn<CanComponentDeactivate> = (
//   component: CanComponentDeactivate
// ) => {
//   if (component.canDeactivate()) {
//     return true;
//   }
//   const dialog = inject(DialogService);
//   return dialog.openConfirmDialog({
//     content: 'there are unsaved changes, proceed?',
//     yes: 'yes',
//     no: 'no',
//   });
// };

const authGuard = () => {
  const token = localStorage.getItem('accessToken');
  const router = inject(Router);

  if (!token) {
    return router.createUrlTree(['/login']);
  }
  return true;
};

export const routes: Routes = [

  { path: 'home', component: HomePageComponent },
  { path: 'login', component: LoginFormComponent },
  { path: 'register', component: RegisterFormComponent },
  { path: 'profile', component: ProfilePageComponent, canActivate: [authGuard] },

  {
    path: '',
    loadComponent: () =>
      import('./core/components/home-page/home-page.component').then(m => m.HomePageComponent)
  },

  {
    // пример
    // { path: 'login', component: LoginPageComponent, canActivate: [loginGuard] },
    path: '',
    loadComponent: () =>
      import('./core/components/layout/layout.component').then(
        m => m.LayoutComponent
      ),
    // пример
    // canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full',
      },
      {
        path: 'home',
        loadComponent: () =>
          import('./core/components/home-page/home-page.component').then(m => m.HomePageComponent),
      },
    ],
  },
];
