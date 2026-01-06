declare global {
  interface KTComponent {
    init: () => void;
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
  }
}

export {};
