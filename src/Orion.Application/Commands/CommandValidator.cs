namespace Orion.Application.Commands;

/// <summary>Validador por defecto: exige que estén presentes los parámetros obligatorios.</summary>
public sealed class CommandValidator : ICommandValidator
{
    public CommandResult? Validate(CommandInfo command, ICommandContext context)
    {
        var missing = command.Parameters
            .Where(p => p.IsRequired && !context.HasParameter(p.Name))
            .Select(p => p.Name)
            .ToArray();

        return missing.Length == 0
            ? null
            : CommandResult.Failed($"Faltan parámetros obligatorios: {string.Join(", ", missing)}.");
    }
}
