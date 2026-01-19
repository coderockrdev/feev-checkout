import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Clipboard } from "@angular/cdk/clipboard";

import { ToastService } from "@shared/services/toast/toast.service";
import { ModalService } from "@shared/services/modal/modal.service";
import { TransactionStore } from "@modules/checkout/stores/transaction/transaction.store";

import { PixFinishPanelComponent } from "./pix-finish-panel.component";

class TransactionStoreMock {}

class ToastServiceMock {}

class ClipboardMock {}

class ModalServiceMock {}

describe("PixFinishPanelComponent", () => {
  let component: PixFinishPanelComponent;
  let fixture: ComponentFixture<PixFinishPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PixFinishPanelComponent],
      providers: [
        { provide: TransactionStore, useClass: TransactionStoreMock },
        { provide: ToastService, useClass: ToastServiceMock },
        { provide: Clipboard, useClass: ClipboardMock },
        { provide: ModalService, useClass: ModalServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PixFinishPanelComponent);
    component = fixture.componentInstance;

    TestBed.inject(TransactionStore);
    TestBed.inject(ToastService);
    TestBed.inject(Clipboard);
    TestBed.inject(ModalService);

    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
