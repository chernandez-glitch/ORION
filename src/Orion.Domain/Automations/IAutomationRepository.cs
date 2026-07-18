namespace Orion.Domain.Automations;

public interface IAutomationRepository
{
    Task<AutomationDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AutomationDefinition>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(AutomationDefinition automation, CancellationToken cancellationToken = default);

    void Update(AutomationDefinition automation);

    void Remove(AutomationDefinition automation);
}
