import * as z from "zod";

import { PaymentRuleSchema } from "../schemas/payment-rule.schema";

export type PaymentRule = z.infer<typeof PaymentRuleSchema>;
