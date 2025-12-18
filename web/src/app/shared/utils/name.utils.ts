/**
 * Extract name initials
 * e.g. John Doe = JD
 */
export const getNameInitials = (name = "") => {
  const initials = name
    .trim()
    .split(/\s+/)
    .map((piece) => piece.charAt(0).toUpperCase());
  const size = initials.length;

  return size === 1 ? initials[0] : initials[0] + initials[size - 1];
};
