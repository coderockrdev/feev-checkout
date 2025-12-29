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
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
