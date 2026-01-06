import { Component, computed, input } from "@angular/core";

import { getNameInitials } from "@shared/utils/name.utils";

@Component({
  selector: "app-avatar",
  imports: [],
  templateUrl: "./avatar.component.html",
  styleUrl: "./avatar.component.scss",
})
export class AvatarComponent {
  readonly name = input<string>();
  readonly source = input<string>();
  readonly isLoading = input<boolean>(false);

  readonly initials = computed(() => getNameInitials(this.name()));
}
