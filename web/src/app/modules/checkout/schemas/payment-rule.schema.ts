import * as z from "zod";

import { DateOnlySchema } from "@app/shared/schemas/date";

import { InstallmentSchema } from "./installment.schema";
import { PaymentMethodSchema } from "./payment-method.schema";

export const PaymentRuleSchema = z.object({
  method: PaymentMethodSchema,
  installments: z.array(InstallmentSchema),
  firstInstallment: DateOnlySchema.nullable(),
  interest: z.int().positive().nullable(),
  lateFee: z.int().positive().nullable(),
});
