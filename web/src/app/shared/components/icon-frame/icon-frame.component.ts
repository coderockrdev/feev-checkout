import { Component, computed, input } from "@angular/core";

@Component({
  selector: "app-icon-frame",
  imports: [],
  templateUrl: "./icon-frame.component.html",
  styleUrl: "./icon-frame.component.scss",
  host: {
    class: "inline-flex",
  },
})
export class IconFrameComponent {
  size = input<number>(44);

  protected height = computed(() => this.size() + 3);
}
