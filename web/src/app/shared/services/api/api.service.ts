import { httpResource, HttpResourceOptions } from "@angular/common/http";
import { EnvironmentInjector, inject, Injectable, runInInjectionContext } from "@angular/core";

import { RequestFactory } from "@shared/types/api/request-factory";
import { environment } from "@environments/environment";

const { apiUrl } = environment;

@Injectable({
  providedIn: "root",
})
export class ApiService {
  private injector = inject(EnvironmentInjector);

  get<TResult>(requestFactory: RequestFactory, options: HttpResourceOptions<TResult, unknown>) {
    return httpResource<TResult>(() => {
      const { path, params } = requestFactory();

      return {
        url: `${apiUrl}/${path}`,
        params,
        method: "GET",
      };
    }, options);
  }

  post<TResult, TBody>(
    requestFactory: RequestFactory,
    body: TBody,
    options?: HttpResourceOptions<TResult, unknown>,
  ) {
    return runInInjectionContext(this.injector, () =>
      httpResource<TResult>(() => {
        const { path, params } = requestFactory();

        return {
          url: `${apiUrl}/${path}`,
          params,
          body,
          method: "POST",
        };
      }, options),
    );
  }
}
