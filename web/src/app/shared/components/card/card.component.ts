import { Component, input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { SectionComponent } from "./section/section.component";

@Component({
  selector: "app-card",
  imports: [CommonModule, SectionComponent],
  templateUrl: "./card.component.html",
  styleUrl: "./card.component.scss",
  host: {
    class: "",
  },
})
export class CardComponent {
  heading = input.required<string>();
  contentNoPadding = input<boolean>(false);
}
