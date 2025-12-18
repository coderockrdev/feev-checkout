import * as z from "zod";

import { PaymentMethodSchema } from "@modules/checkout/schemas/payment-method.schema";

export type PaymentMethod = z.infer<typeof PaymentMethodSchema>;
