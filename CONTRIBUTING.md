# Guía de contribución

## Antes de empezar

1. Lee [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) y [docs/PATTERNS.md](docs/PATTERNS.md).
2. Instala el tooling ([docs/INSTALL.md](docs/INSTALL.md)).
3. `dotnet build ORION.slnx` y `dotnet test` deben pasar en verde.

## Reglas no negociables

- **Result<T> en toda la capa de aplicación.** No lances excepciones para flujo de negocio.
- **Respeta la dirección de dependencias.** Domain no conoce Infrastructure ni WinUI.
- **DI por interfaces.** Nunca inyectes `IServiceProvider`.
- **Un módulo = un `AddOrionXxx()`.** Expón el registro desde el propio módulo.
- **CRLF** en todos los archivos (`.gitattributes`).
- **Sin código muerto ni comentado.** Bórralo; git guarda la historia.
- **Comentarios solo para el "por qué".**

## Dónde va cada cosa

| Tipo | Carpeta |
|------|---------|
| Entidad de dominio | `src/Orion.Domain/<Feature>/` |
| Interface de repo | `src/Orion.Domain/<Feature>/I<Feature>Repository.cs` |
| Caso de uso / servicio | `src/Orion.Application/<Feature>/` |
| Comando | `src/Orion.Application/Commands/BuiltIn/` |
| Configuración EF + repo | `src/Orion.Infrastructure/Persistence/` |
| Página / ViewModel | `src/Orion.Presentation/Views` · `/ViewModels` |
| Test | `tests/Orion.Tests/<Capa>/` |

## Estilo

- `Nullable` y `ImplicitUsings` habilitados; C# `latest`.
- Un tipo por archivo (salvo records auxiliares muy pequeños).
- Nombres de dominio en español; API/técnico según convención .NET.

## Flujo de trabajo

1. Crea una rama a partir de `master`.
2. Implementa con tests.
3. `dotnet build` y `dotnet test` en verde, **0 warnings**.
4. Commit descriptivo (imperativo). Abre PR.

## Commits

Mensajes claros en imperativo. Ejemplo:

```
feat(automation): agregar adaptador Windows para IProcessAutomation
```
