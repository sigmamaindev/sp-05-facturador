export type PersonLookupResult = {
  document: string;
  name: string;
  source: "customer" | "supplier" | "sri";
};
