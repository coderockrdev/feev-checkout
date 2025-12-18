import { HttpParams } from "@angular/common/http";

export type RequestFactory = () => {
  path: string;
  params?: HttpParams;
};
