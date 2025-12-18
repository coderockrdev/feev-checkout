import { ComponentFixture, TestBed } from "@angular/core/testing";

import { SelectComponent } from "./select.component";

describe("SelectComponent", () => {
  let component: SelectComponent<number>;
  let fixture: ComponentFixture<SelectComponent<number>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SelectComponent<number>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
