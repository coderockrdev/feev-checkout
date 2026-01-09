import { Routes } from "@angular/router";

// Core
import { AppLayoutComponent } from "@shared/layouts/app-layout/app-layout.component";
import { NotFoundPageComponent } from "@shared/pages/not-found-page/not-found-page.component";

// Transaction
import { IndexPageComponent as TransactionIndexPageComponent } from "@modules/checkout/pages/transaction/index-page/index-page.component";
import { TransactionStore } from "@modules/checkout/stores/transaction/transaction.store";

export const routes: Routes = [
  {
    path: "",
    component: AppLayoutComponent,
    children: [
      {
        path: "checkout/:transaction",
        children: [{ path: "", component: TransactionIndexPageComponent }],
        providers: [TransactionStore],
      },
      {
        path: "**",
        component: NotFoundPageComponent,
      },
    ],
  },
];
