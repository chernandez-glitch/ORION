using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsKeyboardService(ILogger<WindowsKeyboardService> logger) : IKeyboardService
{
    private static readonly Dictionary<string, ushort> Keys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ctrl"] = 0x11, ["control"] = 0x11, ["shift"] = 0x10, ["alt"] = 0x12, ["menu"] = 0x12,
        ["win"] = 0x5B, ["windows"] = 0x5B, ["enter"] = 0x0D, ["return"] = 0x0D,
        ["esc"] = 0x1B, ["escape"] = 0x1B, ["tab"] = 0x09, ["delete"] = 0x2E, ["del"] = 0x2E,
        ["space"] = 0x20, ["backspace"] = 0x08, ["up"] = 0x26, ["down"] = 0x28, ["left"] = 0x25,
        ["right"] = 0x27, ["home"] = 0x24, ["end"] = 0x23,
        ["f1"] = 0x70, ["f2"] = 0x71, ["f3"] = 0x72, ["f4"] = 0x73, ["f5"] = 0x74, ["f6"] = 0x75,
        ["f7"] = 0x76, ["f8"] = 0x77, ["f9"] = 0x78, ["f10"] = 0x79, ["f11"] = 0x7A, ["f12"] = 0x7B
    };

    public Result SendText(string text)
    {
        if (text is null)
        {
            return Result.Failure(Error.Validation("Keyboard.Null", "El texto no puede ser nulo."));
        }

        var inputs = new List<Input>(text.Length * 2);
        foreach (var ch in text)
        {
            inputs.Add(Unicode(ch, up: false));
            inputs.Add(Unicode(ch, up: true));
        }

        return Send(inputs, $"escribir {text.Length} caracteres");
    }

    public Result SendKey(string key)
    {
        if (!TryResolve(key, out var vk))
        {
            return Result.Failure(Error.Validation("Keyboard.UnknownKey", $"Tecla no reconocida: '{key}'."));
        }

        return Send([KeyDown(vk), KeyUp(vk)], $"tecla {key}");
    }

    public Result SendCombination(string combination)
    {
        var tokens = combination.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            return Result.Failure(Error.Validation("Keyboard.EmptyCombination", "Combinación vacía."));
        }

        var codes = new ushort[tokens.Length];
        for (var i = 0; i < tokens.Length; i++)
        {
            if (!TryResolve(tokens[i], out codes[i]))
            {
                return Result.Failure(Error.Validation("Keyboard.UnknownKey", $"Tecla no reconocida: '{tokens[i]}'."));
            }
        }

        var inputs = new List<Input>();
        foreach (var code in codes)
        {
            inputs.Add(KeyDown(code));
        }

        for (var i = codes.Length - 1; i >= 0; i--)
        {
            inputs.Add(KeyUp(codes[i]));
        }

        return Send(inputs, $"combinación {combination}");
    }

    private Result Send(IReadOnlyList<Input> inputs, string action)
    {
        var array = inputs.ToArray();
        var sent = NativeMethods.SendInput((uint)array.Length, array, Marshal.SizeOf<Input>());
        if (sent == 0)
        {
            return Result.Failure(Error.Failure("Keyboard.SendFailed", "SendInput no envió ninguna entrada."));
        }

        logger.LogInformation("Teclado: {Action}.", action);
        return Result.Success();
    }

    private static bool TryResolve(string token, out ushort vk)
    {
        if (Keys.TryGetValue(token, out vk))
        {
            return true;
        }

        if (token.Length == 1 && char.IsLetterOrDigit(token[0]))
        {
            vk = char.ToUpperInvariant(token[0]);
            return true;
        }

        vk = 0;
        return false;
    }

    private static Input KeyDown(ushort vk) => new()
    {
        Type = NativeMethods.INPUT_KEYBOARD,
        U = new InputUnion { Keyboard = new KeyboardInput { VirtualKey = vk } }
    };

    private static Input KeyUp(ushort vk) => new()
    {
        Type = NativeMethods.INPUT_KEYBOARD,
        U = new InputUnion { Keyboard = new KeyboardInput { VirtualKey = vk, Flags = NativeMethods.KEYEVENTF_KEYUP } }
    };

    private static Input Unicode(char ch, bool up) => new()
    {
        Type = NativeMethods.INPUT_KEYBOARD,
        U = new InputUnion
        {
            Keyboard = new KeyboardInput
            {
                ScanCode = ch,
                Flags = NativeMethods.KEYEVENTF_UNICODE | (up ? NativeMethods.KEYEVENTF_KEYUP : 0)
            }
        }
    };
}
