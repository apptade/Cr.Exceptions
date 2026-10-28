# Intro

A high-performance .NET library to map external error codes into typed exceptions and translate infrastructure failures into domain exceptions.

---

## Exception Factory

**Use Case:** Materialize string-based error codes from external APIs into strongly-typed exceptions.

```json
{
  "error": "invalid_grant"
}
```

### Creating

```csharp
public sealed class KeycloakInvalidGrantException : ValidationException
{
    private static readonly ImmutableArray<CrError> _errors
        = [new("IdentityInvalidGrant", "The authorization grant is invalid.")];

    public KeycloakInvalidGrantException() : base(_errors) { }
}
```

### Registration

```csharp
var builder = new ExceptionFactoryBuilder()
    .Map("invalid_grant", static () => new KeycloakInvalidGrantException());

var factory = builder.Build();
```

### Receiving

```csharp
throw factory.Create(errorCode); // Throws KeyNotFoundException if no mapping exists.
```

**Or:**

```csharp
if (factory.TryCreate(errorCode, out var exception)) throw exception;
```

---

## Exception Translator

**Use Case:** Catch infrastructure failures and translate them into clean domain exceptions.

> [!NOTE]
> The mapping process traverses the exception type hierarchy.

### Creating

```csharp
public sealed class UserInvalidCredentialsException : ValidationException
{
    private static readonly ImmutableArray<CrError> _errors = [new("UserInvalidCredentials")];

    public UserInvalidCredentialsException(Exception? innerException = null) : base(_errors, innerException) { }
}
```

### Registration

```csharp
var builder = new ExceptionTranslatorBuilder()
    .Map<KeycloakInvalidGrantException>(static ex => new UserInvalidCredentialsException(ex));

var translator = builder.Build();
```

### Receiving

```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    throw translator.Translate(ex); // Throws KeyNotFoundException if no mapping exists.
}
```

**Or:**

```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    if (translator.TryTranslate(ex, out var translatedException))
    {
        throw translatedException;
    }

    throw;
}
```

---

## Core

**Base:** All custom exceptions inherit from the `CrException` class. 
It carries an `ImmutableArray` of structured error details (zero or more `CrError` instances):

```csharp
record class CrError(string Code, string? Message = null);
```

---

## Exception Categories

Inherit from semantic categories depending on the architectural context:

See the [Categories](Categories/) folder for all available built-in exceptions.