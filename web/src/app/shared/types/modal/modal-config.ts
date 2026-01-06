import { ModalAction } from "./modal-action";
import { ModalType } from "./modal-type";

export interface ModalConfig {
  title: string;
  message: string | string[];
  type: ModalType;
  actions?: ModalAction[];
}
