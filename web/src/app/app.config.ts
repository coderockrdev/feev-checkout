import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection,
} from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient } from "@angular/common/http";

import { routes } from "@app/app.routes";
import { ThemeService } from "@shared/services/theme/theme.service";
import { ThemeLoaderService } from "@shared/services/theme-loader/theme-loader.service";
import { IconRegistryService } from "@shared/services/icon-registry/icon-registry.service";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideAppInitializer(() => {
      const themeService = inject(ThemeService);
      const themeLoader = inject(ThemeLoaderService);
      inject(IconRegistryService);

      const tenant = window.location.hostname;

      themeService.setTheme(themeLoader.loadTheme(tenant));
    }),
  ],
};
