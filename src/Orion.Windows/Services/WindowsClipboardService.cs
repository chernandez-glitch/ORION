using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsClipboardService(IKeyboardService keyboard, ILogger<WindowsClipboardService> logger) : IClipboardService
{
    public Result<string> GetText()
    {
        if (!NativeMethods.OpenClipboard(IntPtr.Zero))
        {
            return Result.Failure<string>(Error.Failure("Clipboard.Open", "No se pudo abrir el portapapeles."));
        }

        try
        {
            if (!NativeMethods.IsClipboardFormatAvailable(NativeMethods.CF_UNICODETEXT))
            {
                return Result.Success(string.Empty);
            }

            var handle = NativeMethods.GetClipboardData(NativeMethods.CF_UNICODETEXT);
            var pointer = NativeMethods.GlobalLock(handle);
            if (pointer == IntPtr.Zero)
            {
                return Result.Success(string.Empty);
            }

            try
            {
                return Result.Success(Marshal.PtrToStringUni(pointer) ?? string.Empty);
            }
            finally
            {
                NativeMethods.GlobalUnlock(handle);
            }
        }
        finally
        {
            NativeMethods.CloseClipboard();
        }
    }

    public Result SetText(string text)
    {
        if (!NativeMethods.OpenClipboard(IntPtr.Zero))
        {
            return Result.Failure(Error.Failure("Clipboard.Open", "No se pudo abrir el portapapeles."));
        }

        try
        {
            NativeMethods.EmptyClipboard();
            var bytes = Encoding.Unicode.GetBytes(text + '\0');
            var hGlobal = NativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE, (UIntPtr)bytes.Length);
            var target = NativeMethods.GlobalLock(hGlobal);
            Marshal.Copy(bytes, 0, target, bytes.Length);
            NativeMethods.GlobalUnlock(hGlobal);

            NativeMethods.SetClipboardData(NativeMethods.CF_UNICODETEXT, hGlobal);
            logger.LogInformation("Portapapeles: texto copiado ({Length} chars).", text.Length);
            return Result.Success();
        }
        finally
        {
            NativeMethods.CloseClipboard();
        }
    }

    public Result Paste() => keyboard.SendCombination("Ctrl+V");

    public Result<IReadOnlyList<string>> GetFiles()
    {
        if (!NativeMethods.OpenClipboard(IntPtr.Zero))
        {
            return Result.Failure<IReadOnlyList<string>>(Error.Failure("Clipboard.Open", "No se pudo abrir el portapapeles."));
        }

        try
        {
            if (!NativeMethods.IsClipboardFormatAvailable(NativeMethods.CF_HDROP))
            {
                return Result.Success<IReadOnlyList<string>>([]);
            }

            var hDrop = NativeMethods.GetClipboardData(NativeMethods.CF_HDROP);
            var count = NativeMethods.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
            var files = new List<string>((int)count);

            for (uint i = 0; i < count; i++)
            {
                var length = NativeMethods.DragQueryFile(hDrop, i, null, 0);
                var buffer = new StringBuilder((int)length + 1);
                NativeMethods.DragQueryFile(hDrop, i, buffer, length + 1);
                files.Add(buffer.ToString());
            }

            return Result.Success<IReadOnlyList<string>>(files);
        }
        finally
        {
            NativeMethods.CloseClipboard();
        }
    }

    public Result SetFiles(IEnumerable<string> paths)
    {
        var list = paths.ToArray();
        if (list.Length == 0)
        {
            return Result.Failure(Error.Validation("Clipboard.NoFiles", "No se indicaron archivos."));
        }

        var fileList = string.Join('\0', list) + "\0\0";
        var fileBytes = Encoding.Unicode.GetBytes(fileList);
        var headerSize = Marshal.SizeOf<DropFiles>();
        var total = headerSize + fileBytes.Length;

        var hGlobal = NativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE, (UIntPtr)total);
        var pointer = NativeMethods.GlobalLock(hGlobal);

        var header = new DropFiles { PFiles = (uint)headerSize, FWide = 1 };
        Marshal.StructureToPtr(header, pointer, false);
        Marshal.Copy(fileBytes, 0, pointer + headerSize, fileBytes.Length);
        NativeMethods.GlobalUnlock(hGlobal);

        if (!NativeMethods.OpenClipboard(IntPtr.Zero))
        {
            NativeMethods.GlobalFree(hGlobal);
            return Result.Failure(Error.Failure("Clipboard.Open", "No se pudo abrir el portapapeles."));
        }

        try
        {
            NativeMethods.EmptyClipboard();
            NativeMethods.SetClipboardData(NativeMethods.CF_HDROP, hGlobal);
            logger.LogInformation("Portapapeles: {Count} archivo(s) copiado(s).", list.Length);
            return Result.Success();
        }
        finally
        {
            NativeMethods.CloseClipboard();
        }
    }
}
