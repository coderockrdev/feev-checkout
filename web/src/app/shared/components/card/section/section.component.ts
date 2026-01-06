import { Component, input } from "@angular/core";

@Component({
  selector: "app-card-section",
  imports: [],
  templateUrl: "./section.component.html",
  styleUrl: "./section.component.scss",
})
export class SectionComponent {
  readonly noPadding = input<boolean>(false);
}
