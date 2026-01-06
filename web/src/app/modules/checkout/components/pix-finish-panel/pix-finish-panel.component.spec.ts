import { ComponentFixture, TestBed } from "@angular/core/testing";

import { PixFinishPanelComponent } from "./pix-finish-panel.component";

describe("PixFinishPanelComponent", () => {
  let component: PixFinishPanelComponent;
  let fixture: ComponentFixture<PixFinishPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PixFinishPanelComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PixFinishPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
