import { Component } from "@angular/core";
import { CardComponent } from "@app/shared/components/card/card.component";
import { QRCodeComponent } from "angularx-qrcode";
import { InputComponent } from "@app/shared/components/input/input.component";
import { IconComponent } from "@app/shared/components/icon/icon.component";

@Component({
  selector: "app-pix-finish-panel",
  imports: [CardComponent, QRCodeComponent, InputComponent, IconComponent],
  templateUrl: "./pix-finish-panel.component.html",
  styleUrl: "./pix-finish-panel.component.scss",
})
export class PixFinishPanelComponent {
  protected code = "";
}
