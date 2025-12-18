import { CommonModule } from "@angular/common";
import { Component, input } from "@angular/core";

import { Column, ColumnsFor } from "@shared/types/table/column";

@Component({
  selector: "app-table",
  imports: [CommonModule],
  templateUrl: "./table.component.html",
  styleUrl: "./table.component.scss",
})
export class TableComponent<T> {
  readonly columns = input.required<ColumnsFor<T>>();
  readonly data = input.required<T[]>();

  getColClassNames(column: Column<T, keyof T>) {
    const isHighlight = column?.variant === "highlight";

    return {
      "text-gray-900": isHighlight,
      "text-gray-800": !isHighlight,
      "font-medium": isHighlight,
    };
  }
}
