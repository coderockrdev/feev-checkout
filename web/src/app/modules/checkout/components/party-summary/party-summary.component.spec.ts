import { ComponentFixture, TestBed } from "@angular/core/testing";

import { PartySummaryComponent } from "./party-summary.component";

describe("PartySummaryComponent", () => {
  let component: PartySummaryComponent;
  let fixture: ComponentFixture<PartySummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PartySummaryComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PartySummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
