export const isFuture = (date: string | Date) => {
  if (date instanceof Date) {
    return Date.now() < date.getTime();
  }

  const time = new Date(date).getTime();

  if (isNaN(time)) {
    throw new Error("Invalid date");
  }

  return Date.now() < time;
};
