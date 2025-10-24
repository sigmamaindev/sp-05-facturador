import { AlertCircleIcon } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "../ui/alert";

export default function AlertMessage({
  variant,
  message,
  description,
}: {
  variant: "default" | "destructive";
  message: string | undefined;
  description?: string;
}) {
  return (
    <Alert variant={variant}>
      <AlertCircleIcon />
      <AlertTitle>{message}</AlertTitle>
      {description ? <AlertDescription>{description}</AlertDescription> : null}
    </Alert>
  );
}
