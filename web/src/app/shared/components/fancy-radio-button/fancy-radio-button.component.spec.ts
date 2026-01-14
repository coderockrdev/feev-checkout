import { ComponentFixture, TestBed } from "@angular/core/testing";

import { FancyRadioButtonComponent } from "./fancy-radio-button.component";

describe("FancyRadioButtonComponent", () => {
  let component: FancyRadioButtonComponent<unknown>;
  let fixture: ComponentFixture<FancyRadioButtonComponent<unknown>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FancyRadioButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(FancyRadioButtonComponent);

    fixture.componentRef.setInput("id", "123");
    fixture.componentRef.setInput("name", "radio-field");
    fixture.componentRef.setInput("label", "Label");
    fixture.componentRef.setInput("value", 1);
    fixture.componentRef.setInput("icon", "check");
    fixture.componentRef.setInput("selected", false);

    component = fixture.componentInstance;
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
