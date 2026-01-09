import { ComponentFixture, TestBed } from "@angular/core/testing";

import { RadioGroupComponent } from "./radio-group.component";

describe("RadioGroupComponent", () => {
  let component: RadioGroupComponent<string>;
  let fixture: ComponentFixture<RadioGroupComponent<string>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RadioGroupComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RadioGroupComponent<string>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
