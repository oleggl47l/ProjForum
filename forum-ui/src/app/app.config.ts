import { provideEventPlugins } from '@taiga-ui/event-plugins';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import {provideHttpClient} from '@angular/common/http';

import { routes } from './app.routes';
import {provideApiConfiguration} from './api/api-configuration';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideEventPlugins(),
    provideHttpClient(),
    { provide: provideApiConfiguration, useValue: { rootUrl: 'http://localhost:5000' } },
  ],
};
