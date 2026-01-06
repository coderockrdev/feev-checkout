import { Injectable, signal } from "@angular/core";
import { Theme } from "@shared/types/theme/theme";

@Injectable({
  providedIn: "root",
})
export class ThemeService {
  private readonly _theme = signal<Theme | null>(null);

  readonly theme = this._theme.asReadonly();

  setTheme(theme: Theme): void {
    this._theme.set(theme);

    document.title = `${theme.name} Checkout`;

    this.applyFavicon(theme);
    this.applyCssVariables(theme);
  }

  private applyFavicon(theme: Theme): void {
    const link: HTMLLinkElement | null = document.querySelector("link[rel~='icon']");

    if (link) {
      link.href = theme.assets.favicon;
    }
  }

  private applyCssVariables(theme: Theme): void {
    const root = document.documentElement;

    Object.entries(theme.colors).forEach(([key, value]) => {
      root.style.setProperty(`--${key}`, value);
    });
  }
}
