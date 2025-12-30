import * as React from "react"

import { cn } from "@/lib/utils"

type InputTransform = "uppercase" | "digits"

type InputProps = React.ComponentProps<"input"> & {
  transform?: InputTransform
}

const MAX_NUMBER_INPUT_CHARS = 20

function countDigits(s: string) {
  let count = 0
  for (let i = 0; i < s.length; i++) {
    const code = s.charCodeAt(i)
    if (code >= 48 && code <= 57) count++
  }
  return count
}

function transformValue(value: string, transform: InputTransform) {
  if (transform === "uppercase") return value.toUpperCase()
  return value.replace(/\D+/g, "")
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className, type, transform, onChange, ...props }, ref) => {
    return (
      <input
        ref={ref}
        type={type}
        data-slot="input"
        className={cn(
          "file:text-foreground placeholder:text-muted-foreground selection:bg-primary selection:text-primary-foreground dark:bg-input/30 border-input h-9 w-full min-w-0 rounded-md border bg-transparent px-3 py-1 text-base shadow-xs transition-[color,box-shadow] outline-none file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm",
          "focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]",
          "aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive",
          className
        )}
        onChange={(e) => {
          if (type === "number") {
            const prev = e.currentTarget.value
            const next = prev.slice(0, MAX_NUMBER_INPUT_CHARS)

            if (prev !== next) {
              const start = e.currentTarget.selectionStart
              const end = e.currentTarget.selectionEnd

              e.currentTarget.value = next

              if (start != null && end != null) {
                e.currentTarget.setSelectionRange(
                  Math.min(start, next.length),
                  Math.min(end, next.length)
                )
              }
            }
          }

          if (transform) {
            const prev = e.currentTarget.value
            const next = transformValue(prev, transform)

            if (prev !== next) {
              const start = e.currentTarget.selectionStart
              const end = e.currentTarget.selectionEnd

              e.currentTarget.value = next

              if (start != null && end != null) {
                if (transform === "digits") {
                  const nextStart = countDigits(prev.slice(0, start))
                  const nextEnd = countDigits(prev.slice(0, end))
                  e.currentTarget.setSelectionRange(nextStart, nextEnd)
                } else {
                  e.currentTarget.setSelectionRange(start, end)
                }
              }
            }
          }

          onChange?.(e)
        }}
        {...props}
      />
    )
  }
)
Input.displayName = "Input"

export { Input }
