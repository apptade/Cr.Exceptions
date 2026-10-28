# Intro

ASP.NET Core integration for `Cr.Exceptions` to automatically map exceptions into RFC-compliant Problem Details with customizable HTTP status codes and log levels.

---

## Installation

### Default

```csharp
builder.Services.AddCrExceptionHandling();
app.UseExceptionHandler();
```

### Custom

```csharp
builder.Services
    .AddCrExceptionHandler()
    .AddCrStatusCodeMapping(builder =>
    {
        // Configure
    })
    .AddCrLogLevelMapping(builder =>
    {
        // Configure
    });

app.UseExceptionHandler();
```

## Exception Handling

### Flow

When a `CrException` is thrown:

```csharp
public sealed class KeycloakInvalidGrantException : ValidationException
{
    private static readonly ImmutableArray<CrError> _errors
        = [new("IdentityInvalidGrant", "The authorization grant is invalid.")];

    public KeycloakInvalidGrantException() : base(_errors) { }
}
```

The client receives a next response:

```json
{
    "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
    "title": "Bad Request",
    "status": 400,
    "errors": [
        {
            "code": "IdentityInvalidGrant",
            "message": "The authorization grant is invalid."
        }
    ],
    "traceId": "6dd0bbb69ae26c77dd09d915915b1958"
}
```

> [!NOTE]
> `traceId` populates from `Activity.Current?.TraceId` with a fallback to `HttpContext.TraceIdentifier`.

If an exception is not derived from `CrException`, or for any unknown exception, the handler returns the following fallback response:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "errors": [
    {
      "code": "InternalError",
      "message": "An unexpected internal error occurred."
    }
  ],
  "traceId": "ff3e0d35e6f0fa022a6de34583ea9e81"
}
```

### Logging

The handler emits structured logs for exceptions and handling events. See [Log Event IDs](CrExceptionHandlerLogExtensions.cs).

---

## HTTP Status Code Mapping

You can customize how `CrException` types map to HTTP status codes.

> [!NOTE]
> The mapping process traverses the exception type hierarchy.

### Registration

```csharp
builder.Services.AddCrStatusCodeMapping(builder =>
{
    builder.AddDefaultMappings();

    builder.Map<MyCustomException>(StatusCodes.Status499ClientClosedRequest);
});
```

See [Default Mappings](Mapping/StatusCodeMapBuilderExtensions.cs).

---

## Log Level Mapping

You can customize how `CrException` types map to a `LogLevel`.

> [!NOTE]
> The mapping process traverses the exception type hierarchy.

### Registration

```csharp
builder.Services.AddCrLogLevelMapping(builder =>
{
    builder.AddDefaultMappings();

    builder.Map<MyCustomException>(LogLevel.Warning);
});
```

See [Default Mappings](Mapping/LogLevelMapBuilderExtensions.cs).