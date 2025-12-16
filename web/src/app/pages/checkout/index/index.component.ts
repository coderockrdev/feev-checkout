import { Component, inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: "app-index",
  imports: [],
  templateUrl: "./index.component.html",
  styleUrl: "./index.component.scss",
})
export class IndexComponent implements OnInit {
  route = inject(ActivatedRoute);
  router = inject(Router);

  ngOnInit() {
    const transaction = this.route.snapshot.paramMap.get("transaction");

    if (!transaction) {
      this.router.navigate(["/404"]);
      return;
    }
  }
}
