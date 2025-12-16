import { Injectable } from "@angular/core";

interface KTComponent {
  init: () => void;
}

// Declare all Metronic and KTUI components
declare let KTToggle: KTComponent;
declare let KTDrawer: KTComponent;
declare let KTMenu: KTComponent;
declare let KTScrollable: KTComponent;
declare let KTSticky: KTComponent;
declare let KTReparent: KTComponent;
declare let KTDropdown: KTComponent;
declare let KTModal: KTComponent;
declare let KTCollapse: KTComponent;
declare let KTDismiss: KTComponent;
declare let KTTabs: KTComponent;
declare let KTAccordion: KTComponent;
declare let KTScrollspy: KTComponent;
declare let KTScrollto: KTComponent;
declare let KTTooltip: KTComponent;
declare let KTStepper: KTComponent;
declare let KTThemeSwitch: KTComponent;
declare let KTImageInput: KTComponent;
declare let KTTogglePassword: KTComponent;
declare let KTDataTable: KTComponent;
declare let KTDatepicker: KTComponent;
declare let KTSelect: KTComponent;
declare let KTToast: KTComponent;

@Injectable({
  providedIn: "root",
})
export class MetronicInitService {
  constructor() {}

  init() {
    this.initToggles();
    this.initScrollables();
    this.initDrawers();
    this.initMenus();
    this.initSticky();
    this.initReparent();
    this.initDropdowns();
    this.initModals();
    this.initCollapse();
    this.initDismiss();
    this.initTabs();
    this.initAccordions();
    this.initScrollspy();
    this.initScrollto();
    this.initTooltips();
    this.initSteppers();
    this.initThemeSwitch();
    this.initImageInput();
    this.initTogglePassword();
    this.initDatatables();
    this.initDatepicker();
    this.initSelect();
    this.initToast();
  }

  initDrawers() {
    if ("KTDrawer" in window) KTDrawer.init();
  }

  initMenus() {
    if ("KTMenu" in window) KTMenu.init();
  }

  initDropdowns() {
    if ("KTDropdown" in window) KTDropdown.init();
  }

  initSticky() {
    if ("KTSticky" in window) KTSticky.init();
  }

  initReparent() {
    if ("KTReparent" in window) KTReparent.init();
  }

  initScrollables() {
    if ("KTScrollable" in window) KTScrollable.init();
  }

  initToggles() {
    if ("KTToggle" in window) KTToggle.init();
  }

  initModals() {
    if ("KTModal" in window) KTModal.init();
  }

  initCollapse() {
    if ("KTCollapse" in window) KTCollapse.init();
  }

  initDismiss() {
    if ("KTDismiss" in window) KTDismiss.init();
  }

  initTabs() {
    if ("KTTabs" in window) KTTabs.init();
  }

  initAccordions() {
    if ("KTAccordion" in window) KTAccordion.init();
  }

  initScrollspy() {
    if ("KTScrollspy" in window) KTScrollspy.init();
  }

  initScrollto() {
    if ("KTScrollto" in window) KTScrollto.init();
  }

  initTooltips() {
    if ("KTTooltip" in window) KTTooltip.init();
  }

  initSteppers() {
    if ("KTStepper" in window) KTStepper.init();
  }

  initThemeSwitch() {
    if ("KTThemeSwitch" in window) KTThemeSwitch.init();
  }

  initImageInput() {
    if ("KTImageInput" in window) KTImageInput.init();
  }

  initTogglePassword() {
    if ("KTTogglePassword" in window) KTTogglePassword.init();
  }

  initDatatables() {
    if ("KTDataTable" in window) KTDataTable.init();
  }

  initDatepicker() {
    if ("KTDatepicker" in window) KTDatepicker.init();
  }

  initSelect() {
    if ("KTSelect" in window) KTSelect.init();
  }

  initToast() {
    if ("KTToast" in window) KTToast.init();
  }
}
