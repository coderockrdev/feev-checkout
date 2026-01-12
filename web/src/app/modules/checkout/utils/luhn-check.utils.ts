import { isDevMode } from "@angular/core";

const LOOKUP = [0, 2, 4, 6, 8, 1, 3, 5, 7, 9];
const TESTING_CARD = "0000000000000001";

export const luhnCheck = (input: string): boolean => {
  if (isDevMode() && input === TESTING_CARD) return true;

  const digits = input.replace(/\D/g, "");

  if (!digits) return false;

  let sum = 0;
  let shouldDouble = false;

  for (let i = digits.length - 1; i >= 0; i--) {
    const digit = Number(digits[i]);
    sum += shouldDouble ? LOOKUP[digit] : digit;
    shouldDouble = !shouldDouble;
  }

  return sum % 10 === 0;
};
