import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ModalService } from "@shared/services/modal/modal.service";
import { ModalConfig } from "@shared/types/modal/modal-config";

import { ModalComponent } from "./modal.component";

class ModalServiceMock {
  register = jasmine.createSpy("register");
  unregister = jasmine.createSpy("unregister");
}

describe("ModalComponent", () => {
  let component: ModalComponent;
  let fixture: ComponentFixture<ModalComponent>;
  let modalService: ModalServiceMock;

  const showSpy = jasmine.createSpy("show");

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModalComponent],
      providers: [{ provide: ModalService, useClass: ModalServiceMock }],
    }).compileComponents();

    window.KTModal = {
      init: jasmine.createSpy("init"),
      getInstance: jasmine.createSpy("getInstance").and.returnValue({
        show: showSpy,
      }),
    };

    fixture = TestBed.createComponent(ModalComponent);
    component = fixture.componentInstance;
    modalService = TestBed.inject(ModalService) as unknown as ModalServiceMock;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should register and initialize KTModal on view init", () => {
    fixture.detectChanges();

    expect(window.KTModal.getInstance).toHaveBeenCalled();
    expect(modalService.register).toHaveBeenCalledWith(component);
  });

  it("should unregister on destroy", () => {
    fixture.detectChanges();
    fixture.destroy();

    expect(modalService.unregister).toHaveBeenCalledWith(component);
  });

  it("should open modal with normalized messages and actions", () => {
    fixture.detectChanges();

    const config: ModalConfig = {
      title: "Modal Title",
      message: "Hello",
      type: "error",
      actions: [{ label: "OK" }],
    };

    component.open(config);

    expect(component["messages"]).toEqual(["Hello"]);
    expect(component["actions"]).toEqual(config.actions!);
    expect(window.KTModal.getInstance(fixture.nativeElement).show).toHaveBeenCalled();
  });
});
