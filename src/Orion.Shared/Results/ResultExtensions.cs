namespace Orion.Shared.Results;

/// <summary>
/// Composición funcional sobre <see cref="Result"/> para encadenar operaciones
/// sin ramas <c>if</c> repetidas.
/// </summary>
public static class ResultExtensions
{
    /// <summary>Ejecuta <paramref name="next"/> solo si el resultado previo fue exitoso.</summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> next) =>
        result.IsSuccess ? next(result.Value) : Result.Failure<TOut>(result.Error);

    /// <summary>Transforma el valor de un resultado exitoso.</summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map) =>
        result.IsSuccess ? Result.Success(map(result.Value)) : Result.Failure<TOut>(result.Error);

    /// <summary>Colapsa un resultado a un único valor cubriendo ambos caminos.</summary>
    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

    /// <summary>Efecto colateral sobre el error, sin alterar el resultado.</summary>
    public static Result<TValue> TapError<TValue>(this Result<TValue> result, Action<Error> onError)
    {
        if (result.IsFailure)
        {
            onError(result.Error);
        }

        return result;
    }
}
