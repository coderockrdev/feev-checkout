import { FormControl, FormGroup } from "@angular/forms";
import * as z from "zod";

import { zodValidator } from "@shared/adapters/zod-validator.adapter";

export interface ZodControlConfig<T extends z.ZodTypeAny> {
  schema: T;
  defaultValue: z.infer<T>;
  nonNullable?: boolean;
}

type ZodFormConfig = Record<string, ZodControlConfig<z.ZodTypeAny>>;

type InferFormGroup<T extends ZodFormConfig> = {
  [K in keyof T]: FormControl<z.infer<T[K]["schema"]>>;
};

export function zodForm<T extends ZodFormConfig>(config: T): FormGroup<InferFormGroup<T>> {
  const controls = {} as InferFormGroup<T>;

  for (const key in config) {
    const { schema, defaultValue, nonNullable } = config[key];

    controls[key] = new FormControl(defaultValue, {
      nonNullable: nonNullable ?? true,
      validators: [zodValidator(schema)],
    }) as InferFormGroup<T>[typeof key];
  }

  return new FormGroup(controls);
}
