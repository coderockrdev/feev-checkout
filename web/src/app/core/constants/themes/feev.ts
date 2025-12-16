import { Theme } from "../../models/theme";

export const FeevTheme: Theme = {
  name: "Feev",
  assets: {
    logo: "assets/media/themes/feev/logo.svg",
    logoMini: "assets/media/themes/feev/logo-mini.svg",
    favicon: "assets/media/themes/feev/logo-mini.svg",
    badges: [
      { name: "Checkout Seguro", src: "assets/media/badges/safe-checkout.webp" },
      { name: "Clearsale", src: "assets/media/badges/clearsale.webp" },
    ],
  },
  colors: {
    primary: "oklch(75.51% 0.15 165.36)",
  },
};
