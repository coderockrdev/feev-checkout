import { ThemeAssets } from "./theme-assets";
import { ThemeColors } from "./theme-colors";

export interface Theme {
  name: string;
  assets: ThemeAssets;
  colors: ThemeColors;
}
