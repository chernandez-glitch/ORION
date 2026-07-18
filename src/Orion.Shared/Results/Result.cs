namespace Orion.Shared.Results;

/// <summary>
/// Resultado de una operación que puede tener éxito o fallar sin lanzar
/// excepciones. Es el tipo de retorno estándar en toda la capa de aplicación.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Un resultado exitoso no puede llevar un error.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Un resultado fallido requiere un error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>Crea un resultado a partir de un valor posiblemente nulo.</summary>
    public static Result<TValue> Create<TValue>(TValue? value, Error ifNull) =>
        value is not null ? Success(value) : Failure<TValue>(ifNull);
}

/// <summary>
/// Resultado que transporta un valor cuando la operación tiene éxito.
/// </summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Valor producido por la operación. Acceder a él en un resultado fallido
    /// es un error de programación y lanza una excepción.
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido.");

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}
