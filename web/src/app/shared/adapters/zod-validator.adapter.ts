import * as z from "zod";
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export const zodValidator =
  <T>(schema: z.ZodSchema<T>): ValidatorFn =>
  (control: AbstractControl): ValidationErrors | null => {
    const result = schema.safeParse(control.value);

    if (result.success) {
      return null;
    }

    return {
      zod: z.flattenError(result.error).formErrors,
    };
  };
