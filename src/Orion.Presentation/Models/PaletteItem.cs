using CommunityToolkit.Mvvm.ComponentModel;
using Orion.Application.Commands;

namespace Orion.Presentation.Models;

/// <summary>Elemento de la Command Palette: un comando más su estado de favorito.</summary>
public sealed partial class PaletteItem(CommandInfo info, bool isFavorite) : ObservableObject
{
    public CommandInfo Info { get; } = info;

    public string Name => Info.Name;

    public string Description => Info.Description;

    public string Category => Info.Category.ToString();

    public bool RequiresParameters => Info.Parameters.Any(p => p.IsRequired);

    [ObservableProperty]
    private bool _isFavorite = isFavorite;

    public string FavoriteGlyph => IsFavorite ? char.ConvertFromUtf32(0xE735) : char.ConvertFromUtf32(0xE734);

    partial void OnIsFavoriteChanged(bool value) => OnPropertyChanged(nameof(FavoriteGlyph));
}
