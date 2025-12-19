import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

export const PaymentMethodSchema = z.enum(PaymentMethod);
