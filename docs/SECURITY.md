# Seguridad

ORION puede llegar a controlar el equipo por completo. La seguridad es un pilar
de diseño desde el primer día.

## Principios

1. **Confirmación de acciones destructivas.** Apagar, reiniciar, eliminar archivos
   o ejecutar shell deben requerir confirmación explícita del usuario. Los
   comandos de energía aplican un margen de cancelación de 15 s.
2. **Fail-safe por defecto.** En Fase 0 la automatización real está deshabilitada:
   el adaptador `PhaseZeroAutomation` rechaza toda acción. Nada se ejecuta sobre el
   sistema por accidente.
3. **Sin secretos en el repositorio.** Claves de IA/servicios se guardan fuera de
   control de versiones (variables de entorno / almacén seguro). `settings.json`
   **no** contiene credenciales.
4. **Aislamiento de plugins.** Los plugins se cargan desde un directorio conocido;
   los fallos se registran y se omiten. Solo instala plugins de fuentes de confianza.
5. **Registro auditable.** Cada comando queda en el historial (memoria) y en los
   logs de Serilog (`%AppData%\OrionAI\logs`).

## Dependencias

Se fijan versiones seguras de paquetes transitivos para evitar advisories conocidos
(ver `Directory.Packages.props`, sección "Pins de seguridad"). Revisa `NU1903` en
cada actualización de EF Core / SQLite.

## Actualizaciones

El auto-update (Fase 5) descargará paquetes **firmados**; la verificación de firma
es obligatoria antes de aplicar cualquier actualización.

## Reporte de vulnerabilidades

Uso interno de Grupo Platino: reporta cualquier hallazgo al equipo de desarrollo.
