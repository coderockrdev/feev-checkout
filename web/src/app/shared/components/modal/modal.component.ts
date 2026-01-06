import { AfterViewInit, Component, ElementRef, signal, ViewChild } from "@angular/core";

import { KtModalDirective } from "@shared/directives/kt-modal.directive";
import { ModalConfig } from "@shared/types/modal/modal-config";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { ModalAction } from "@shared/types/modal/modal-action";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-modal",
  imports: [CommonModule, KtModalDirective, IconFrameComponent, IconComponent],
  templateUrl: "./modal.component.html",
  styleUrl: "./modal.component.scss",
})
export class ModalComponent implements AfterViewInit {
  @ViewChild("modalRoot", { static: true }) private modalRoot!: ElementRef<HTMLDivElement>;

  protected readonly id = `modal-${Math.random()}`;

  private _instance: Nullable<KTModal> = null;
  private _config = signal<Nullable<ModalConfig>>(null);

  protected readonly config = this._config.asReadonly();
  protected messages: string[] = [];
  protected actions: ModalAction[] = [];

  ngAfterViewInit(): void {
    this._instance = window.KTModal.getInstance(this.modalRoot.nativeElement);
  }

  public open(config: ModalConfig) {
    this._config.set(config);
    this.messages = this.normalizeMessage(config.message);
    this.actions = config?.actions ?? [];
    this._instance?.show();
  }

  public close() {
    this._config.set(null);
    this.messages = [];
    this._instance?.hide();
  }

  private normalizeMessage(message: string | string[]): string[] {
    return Array.isArray(message) ? message : [message];
  }

  protected getActionVariantClass(action: ModalAction) {
    return !action.variant ? "" : `kt-btn-${action.variant}`;
  }
}
