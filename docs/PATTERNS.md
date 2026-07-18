# Patrones y convenciones

## Result<T> — sin excepciones para flujo de negocio

Toda la capa de aplicación devuelve `Result` / `Result<T>` (`Orion.Shared.Results`).
Los errores de negocio se modelan como `Error` (código estable + mensaje + tipo),
nunca como excepciones.

```csharp
public async Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken ct)
{
    if (request.FirstArgument is not { } target)
        return Result.Failure<CommandOutcome>(CommandErrors.MissingArgument("aplicación"));

    var launched = await _process.LaunchAsync(target, null, ct);
    return launched.IsSuccess
        ? Result.Success(CommandOutcome.Ok($"Abriendo '{target}'."))
        : Result.Failure<CommandOutcome>(launched.Error);
}
```

Composición funcional disponible: `Map`, `Bind`, `Match`, `TapError`.

## Puertos y adaptadores

La aplicación depende de **interfaces** (puertos). Los adaptadores concretos se
inyectan en el composition root. Ejemplos: `ISystemMetrics` (puerto en Application,
adaptador en Infrastructure), `IProcessAutomation` (puerto en Automation, adaptador
no-op hoy, adaptador Windows real en Fase 1).

## Inyección de dependencias

Cada módulo expone un método de extensión `AddOrionXxx(this IServiceCollection)`.
El composition root (`Orion.Presentation.CompositionRoot`) los ensambla. Nunca se
inyecta `IServiceProvider` en constructores: se inyectan interfaces.

## Módulos auto-descriptivos

Cada módulo implementa `IModuleStatusProvider` para reportar su estado al
dashboard. Añadir un módulo no obliga a tocar el dashboard.

## Motor de comandos

`ICommand` + `CommandDescriptor`. Los comandos se **descubren automáticamente**
por reflexión (`AddOrionApplication`) y se indexan por nombre y alias.

## MVVM (Presentation)

ViewModels con `CommunityToolkit.Mvvm` (`ObservableObject`, `[ObservableProperty]`,
`[RelayCommand]`). Las páginas resuelven su ViewModel desde `App.Services`.

## Estilo de código

- `Nullable` habilitado, `ImplicitUsings` habilitado, C# `latest`.
- CRLF en todo el repo (`.gitattributes`).
- Comentarios solo cuando el "por qué" no es obvio.
- Nombres de dominio en español; términos técnicos según convención .NET.
