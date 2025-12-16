import { Theme } from "../../models/theme";

export const SimonettiTheme: Theme = {
  name: "Simonetti",
  assets: {
    logo: "assets/media/themes/simonetti/logo.svg",
    logoMini: "assets/media/themes/simonetti/logo-mini.svg",
    favicon: "assets/media/themes/simonetti/logo-mini.svg",
    badges: [
      { name: "Checkout Seguro", src: "assets/media/badges/safe-checkout.webp" },
      { name: "Clearsale", src: "assets/media/badges/clearsale.webp" },
      { name: "Confi", src: "assets/media/badges/confi.png" },
    ],
  },
  colors: {
    primary: "oklch(57.59% 0.23 27.37)",
  },
};
