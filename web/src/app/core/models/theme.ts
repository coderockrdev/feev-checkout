export interface ThemeAssets {
  logo: string;
  logoMini: string;
  favicon: string;
}

export interface ThemeColors {
  primary: string;
}

export interface Theme {
  name: string;
  assets: ThemeAssets;
  colors: ThemeColors;
}
