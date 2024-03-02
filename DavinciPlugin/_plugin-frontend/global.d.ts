import TrinityApp from "trinity-types/TrinityApp";

declare global {
  interface Window {
    trinityApp: typeof TrinityApp;
  }
}

export {};
