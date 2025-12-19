import * as z from "zod";

import { RawDateOnlySchema } from "@app/shared/schemas/date";

import { InstallmentSchema } from "./installment.schema";
import { PaymentMethodSchema } from "./payment-method.schema";

export const PaymentRuleSchema = z.object({
  method: PaymentMethodSchema,
  installments: z.array(InstallmentSchema),
  firstInstallment: RawDateOnlySchema.nullable(),
  interest: z.int().min(0).nullable(),
  lateFee: z.int().min(0).nullable(),
});
