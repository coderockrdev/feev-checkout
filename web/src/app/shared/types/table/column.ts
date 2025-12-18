export interface Column<T, K extends keyof T> {
  label?: string;
  key: K;
  className?: string;
  variant?: "standard" | "highlight";
  render?: (value: T[K], row: T, index: number) => string | number;
}

export type ColumnsFor<T> = {
  [K in keyof T]: Column<T, K>;
}[keyof T][];
