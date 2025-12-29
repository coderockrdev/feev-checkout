import { environment } from "../../../../environments/environment";
import { Theme } from "../../types/theme/theme";

import { FeevTheme } from "./feev";
import { SimonettiTheme } from "./simonetti";

const { tenants } = environment;

export const THEME_TENTANTS = [tenants.feev, tenants.simonetti] as const;

export const THEMES: Record<string, Theme> = {
  [tenants.feev]: FeevTheme,
  [tenants.simonetti]: SimonettiTheme,
  default: FeevTheme,
};
