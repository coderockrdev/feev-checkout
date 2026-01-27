import { Component } from "@angular/core";
import { MessageCardComponent } from "@app/shared/components/message-card/message-card.component";

@Component({
  selector: "app-not-found-page",
  imports: [MessageCardComponent],
  templateUrl: "./not-found-page.component.html",
  styleUrl: "./not-found-page.component.scss",
})
export class NotFoundPageComponent {}
