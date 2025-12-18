import { inject, Injectable } from "@angular/core";
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer } from "@angular/platform-browser";

import { ICON_NAMES } from "@shared/constants/icons/icon-names";

@Injectable({
  providedIn: "root",
})
export class IconRegistryService {
  readonly registry = inject(MatIconRegistry);
  readonly sanitizer = inject(DomSanitizer);

  constructor() {
    ICON_NAMES.forEach((icon) =>
      this.registry.addSvgIcon(
        icon,
        this.sanitizer.bypassSecurityTrustResourceUrl(`/assets/icons/${icon}.svg`),
      ),
    );
  }
}
