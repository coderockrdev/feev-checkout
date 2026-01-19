import { ComponentFixture, TestBed } from "@angular/core/testing";

import { AppLayoutComponent } from "./app-layout.component";

describe("AppLayoutComponent", () => {
  let component: AppLayoutComponent;
  let fixture: ComponentFixture<AppLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppLayoutComponent],
    }).compileComponents();

    window.KTModal = {
      init: jasmine.createSpy("init"),
      getInstance: jasmine.createSpy("getInstance").and.returnValue({}),
    };

    fixture = TestBed.createComponent(AppLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
