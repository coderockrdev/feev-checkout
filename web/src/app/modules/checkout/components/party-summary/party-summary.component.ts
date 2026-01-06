import { Component, input } from "@angular/core";
import { AvatarComponent } from "@app/shared/components/avatar/avatar.component";

@Component({
  selector: "app-party-summary",
  imports: [AvatarComponent],
  templateUrl: "./party-summary.component.html",
  styleUrl: "./party-summary.component.scss",
})
export class PartySummaryComponent {
  readonly name = input.required<string>();
  readonly document = input.required<string>();
  readonly source = input<string>();
  readonly isLoading = input<boolean>(false);
}
