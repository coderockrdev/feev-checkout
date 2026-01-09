import * as z from "zod";

import { PaymentMethod } from "@modules/checkout/enums/payment-method";

import { PaymentStatusSchema } from "./payment-status.schema";

export const CreditCardPaymentAttemptSchema = z.object({
  extraData: z.preprocess(
    (payload: Record<string, unknown>) => ({
      proofOfSale: payload["proofOfSale"] ?? payload["ProofOfSale"],
      acquirerTransactionId: payload["acquirerTransactionId"] ?? payload["AcquirerTransactionId"],
      authorizationCode: payload["authorizationCode"] ?? payload["AuthorizationCode"],
      softDescriptor: payload["softDescriptor"] ?? payload["SoftDescriptor"],
      sentOrderId: payload["sentOrderId"] ?? payload["SentOrderId"],
      paymentId: payload["paymentId"] ?? payload["PaymentId"],
      capturedDate: payload["capturedDate"] ?? payload["CapturedDate"],
      provider: payload["provider"] ?? payload["Provider"],
      reasonCode: payload["reasonCode"] ?? payload["ReasonCode"],
      reasonMessage: payload["reasonMessage"] ?? payload["ReasonMessage"],
      status: payload["status"] ?? payload["Status"],
      providerReturnCode: payload["providerReturnCode"] ?? payload["ProviderReturnCode"],
      providerReturnMessage: payload["providerReturnMessage"] ?? payload["ProviderReturnMessage"],
      paymentAccountReference:
        payload["paymentAccountReference"] ?? payload["PaymentAccountReference"],
    }),
    z.object({
      proofOfSale: z.string(),
      acquirerTransactionId: z.string(),
      authorizationCode: z.string(),
      softDescriptor: z.string(),
      sentOrderId: z.string(),
      paymentId: z.string(),
      capturedDate: z.string(),
      provider: z.string(),
      reasonCode: z.int(),
      reasonMessage: z.string(),
      status: z.int(),
      providerReturnCode: z.string(),
      providerReturnMessage: z.string(),
      paymentAccountReference: z.string(),
    }),
  ),
  status: PaymentStatusSchema.nullable().optional().default(null),
  method: z.literal(PaymentMethod.CreditCard),
});
