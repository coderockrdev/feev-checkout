import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class ToastService {
  show(options: KTToastOptions) {
    window.KTToast.show(options);
  }
}
