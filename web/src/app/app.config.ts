import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection,
} from "@angular/core";
import { provideRouter } from "@angular/router";

import { routes } from "./app.routes";
import { ThemeService } from "./core/services/theme.service";
import { ThemeLoaderService } from "./core/services/theme-loader.service";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAppInitializer(() => {
      const themeService = inject(ThemeService);
      const themeLoader = inject(ThemeLoaderService);

      const tenant = window.location.hostname;

      themeService.setTheme(themeLoader.loadTheme(tenant));
    }),
  ],
};
