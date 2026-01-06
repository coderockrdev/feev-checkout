import { ButtonVariant } from "../button/button-variant";
import { IconName } from "../icon-name";

export interface ModalAction {
  label: string;
  icon?: IconName;
  variant?: ButtonVariant;
  onClick?: () => void;
}
