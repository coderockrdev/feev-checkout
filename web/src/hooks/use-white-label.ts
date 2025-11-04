import { useEffect } from 'react';
import { FeevTheme } from '@/themes/feev';
import { SimonettiTheme } from '@/themes/simonetti';
import { Theme } from '@/themes/theme';
import { toAbsoluteUrl } from '@/lib/helpers';

const themes: Record<string, Theme> = {
  'feev.checkout.local': FeevTheme,
  'simonetti.checkout.local': SimonettiTheme,
};

export function useWhiteLabel() {
  const hostname = window.location.hostname;
  const theme = themes[hostname] || FeevTheme;

  useEffect(() => {
    const title: HTMLTitleElement | null = document.querySelector('title');

    if (title) {
      title.text = `${theme.name} Checkout`;
    }

    const link: HTMLLinkElement | null =
      document.querySelector("link[rel~='icon']");

    if (link) {
      link.href = toAbsoluteUrl(theme.assets.favicon);
    }

    const root = document.documentElement;

    Object.entries(theme.colors).forEach(([key, value]) => {
      root.style.setProperty(`--${key}`, value);
    });
  }, [theme]);

  return theme;
}
