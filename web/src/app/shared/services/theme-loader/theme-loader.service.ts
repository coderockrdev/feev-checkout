import { Injectable } from "@angular/core";
import { THEME_TENTANTS, THEMES } from "@shared/constants/themes/themes";

@Injectable({
  providedIn: "root",
})
export class ThemeLoaderService {
  loadTheme(tenant: string) {
    if (this.isKnownTenant(tenant)) {
      return THEMES[tenant];
    }

    return THEMES["default"];
  }

  private isKnownTenant(tenant: string): boolean {
    return THEME_TENTANTS.includes(tenant);
  }
}
