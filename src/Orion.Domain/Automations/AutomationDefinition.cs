using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Automations;

/// <summary>
/// Definición de una automatización guardada por el usuario (un flujo con nombre
/// que ORION podrá ejecutar en fases futuras). En esta fase solo se almacena;
/// la ejecución real llega con el módulo de automatización.
/// </summary>
public sealed class AutomationDefinition : AuditableEntity
{
    private AutomationDefinition(Guid id, Guid userId, string name, string description, string trigger, bool isEnabled, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        UserId = userId;
        Name = name;
        Description = description;
        Trigger = trigger;
        IsEnabled = isEnabled;
    }

    public Guid UserId { get; private init; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    /// <summary>Frase/patrón que dispara la automatización (p. ej. "buenos días").</summary>
    public string Trigger { get; private set; }

    public bool IsEnabled { get; private set; }

    public static AutomationDefinition Create(Guid userId, string name, string description, string trigger, DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(name);
        Guard.AgainstNullOrWhiteSpace(trigger);

        return new AutomationDefinition(Guid.CreateVersion7(), userId, name.Trim(), description ?? string.Empty, trigger.Trim(), true, nowUtc);
    }

    public void Enable(DateTime nowUtc)
    {
        IsEnabled = true;
        Touch(nowUtc);
    }

    public void Disable(DateTime nowUtc)
    {
        IsEnabled = false;
        Touch(nowUtc);
    }

    public void Update(string name, string description, string trigger, DateTime nowUtc)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name).Trim();
        Description = description ?? string.Empty;
        Trigger = Guard.AgainstNullOrWhiteSpace(trigger).Trim();
        Touch(nowUtc);
    }
}
