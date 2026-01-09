import * as z from "zod";

import { PaymentStatus } from "../enums/payment-status";

export const PaymentStatusSchema = z.enum(PaymentStatus);
