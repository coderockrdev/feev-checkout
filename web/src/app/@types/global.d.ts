declare global {
  interface KTComponent {
    init(): void;
  }

  interface KTToast extends KTComponent {
    show(options: KTToastOptions): void;
    hide(idOrInstance: string | KTToast): void;
    clearAll(): void;
  }

  interface KTToastOptions {
    message: string;
    variant?:
      | "info"
      | "success"
      | "error"
      | "warning"
      | "primary"
      | "secondary"
      | "destructive"
      | "mono";
    size?: "sm" | "md" | "lg";
    duration?: nuumber;
    progress?: boolean;
    important?: boolean;
    position?:
      | "top-end"
      | "top-center"
      | "top-start"
      | "bottom-end"
      | "bottom-center"
      | "bottom-start";
  }

  interface KTModalComponent extends KTComponent {
    getInstance(element: HTMLElement): KTModal;
  }

  interface KTModal {
    show();
    hide();
    toggle();
    getElement(): HTMLElement;
    dispose();
  }

  interface Window {
    KTModal: KTModalComponent;
    KTSelect: KTComponent;
    KTToast: KTToast;
  }
}

export {};
