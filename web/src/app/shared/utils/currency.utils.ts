/**
 * Format integer values into currency
 * e.g. 10000 = R$ 100,00
 */
export const integerToCurrency = (value: number): string =>
  (value / 100).toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
