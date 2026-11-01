---
paths:
  - "src/**/Errors/**"
  - "src/**/Program.cs"
  - "src/SharedKernel/**"
---

# Errors: domain error types

- Errors are subclasses of `Error` (`NotFound`, `Conflict`, `InvalidValue`, `InvalidParameters`) from `SharedKernel.Aggregates`, placed in the aggregate assembly, usually `<Aggregate>/Errors/`. Example: `TariffNotFound : NotFound` in `Tariff.Aggregates`.
- A new error type must live in an assembly whose `AssemblyReference` is already passed to the API's `Program.cs`. At startup `Program.cs` finds all `Error` subclasses in that assembly and passes them to `ErrorJsonTypeInfoModifier` (polymorphic JSON) and `ServiceSwaggerGenOptions` (swagger document `<ApiName>-errors`). Otherwise the new type is missing from both.
- Domain errors reach the client as ProblemDetails (`Hellang.Middleware.ProblemDetails`).
- Check after adding a type: the `<api>-errors` swagger document contains its schema.
