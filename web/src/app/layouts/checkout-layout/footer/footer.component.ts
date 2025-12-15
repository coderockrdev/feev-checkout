import { Component, inject } from "@angular/core";
import { ThemeService } from "../../../core/services/theme.service";
import { FooterBadge } from "./interfaces/footer-badge";

@Component({
  selector: "[app-footer]",
  imports: [],
  templateUrl: "./footer.component.html",
  styleUrl: "./footer.component.scss",
})
export class FooterComponent {
  theme = inject(ThemeService).theme;

  badges: FooterBadge[] = [
    { name: "Checkout Seguro", src: "assets/media/badges/safe-checkout.png" },
    { name: "Clearsale", src: "assets/media/badges/clearsale.png" },
    { name: "Confi", src: "assets/media/badges/confi.png" },
  ];
}
