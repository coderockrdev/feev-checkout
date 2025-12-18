import { httpResource } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { RequestFactory } from "@shared/types/api/request-factory";
import { environment } from "@environments/environment";

const { apiUrl } = environment;

@Injectable({
  providedIn: "root",
})
export class ApiService {
  get<T>(requestFactory: RequestFactory) {
    return httpResource<T>(() => {
      const { path, params } = requestFactory();

      return {
        url: `${apiUrl}/${path}`,
        params,
        method: "GET",
      };
    });
  }
}
