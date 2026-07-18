using System.Runtime.CompilerServices;

namespace Orion.Shared.Guards;

/// <summary>
/// Cláusulas de guarda para validar invariantes de argumentos. Se usan para
/// precondiciones que, de fallar, indican un error de programación (no un error
/// de negocio: para eso está <see cref="Results.Result"/>).
/// </summary>
public static class Guard
{
    public static T AgainstNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : class =>
        value ?? throw new ArgumentNullException(paramName);

    public static string AgainstNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El valor no puede ser nulo ni estar en blanco.", paramName);
        }

        return value;
    }

    public static Guid AgainstEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede ser Guid.Empty.", paramName);
        }

        return value;
    }

    public static int AgainstNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "El valor no puede ser negativo.");
        }

        return value;
    }
}
