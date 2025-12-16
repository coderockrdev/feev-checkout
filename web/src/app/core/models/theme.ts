export interface Badge {
  name: string;
  src: string;
}

export interface ThemeAssets {
  logo: string;
  logoMini: string;
  favicon: string;
  badges: Badge[];
}

export interface ThemeColors {
  primary: string;
}

export interface Theme {
  name: string;
  assets: ThemeAssets;
  colors: ThemeColors;
}
