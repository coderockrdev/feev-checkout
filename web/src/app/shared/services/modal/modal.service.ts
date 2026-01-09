import { Injectable } from "@angular/core";

import { ModalConfig } from "@shared/types/modal/modal-config";
import { ModalComponent } from "@shared/components/modal/modal.component";

@Injectable({
  providedIn: "root",
})
export class ModalService {
  private modal: Nullable<ModalComponent> = null;

  register(modal: ModalComponent) {
    this.modal = modal;
  }

  unregister(modal: ModalComponent) {
    if (this.modal === modal) {
      this.modal = null;
    }
  }

  public open(config: ModalConfig) {
    this.modal?.open(config);
  }

  public close() {
    this.modal?.close();
  }
}
