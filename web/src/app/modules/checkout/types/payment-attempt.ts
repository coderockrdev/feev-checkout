import * as z from "zod";

import { PaymentAttemptSchema } from "../schemas/payment-attempt.schema";

export type PaymentAttempt = z.infer<typeof PaymentAttemptSchema>;
