using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.AI;
using Orion.Configuration;
using Orion.Configuration.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>
/// ViewModel de la pantalla de configuración. Refleja y edita
/// <see cref="OrionSettings"/> y lo persiste vía <see cref="IConfigurationService"/>.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configuration;

    public SettingsViewModel(IConfigurationService configuration)
    {
        _configuration = configuration;
        AvailableProviders = Enum.GetNames<AIProviderKind>();

        var settings = configuration.Current;
        _assistantName = settings.Assistant.Name;
        _activationWord = settings.Assistant.ActivationWord;
        _language = settings.Assistant.Language;
        _aiProvider = settings.AI.Provider;
        _aiModel = settings.AI.Model;
        _microphone = settings.Voice.Microphone;
        _speaker = settings.Voice.Speaker;
        _themeIndex = (int)settings.Appearance.Theme;
    }

    public IReadOnlyList<string> AvailableProviders { get; }

    public IReadOnlyList<string> AvailableThemes { get; } = ["Sistema", "Claro", "Oscuro"];

    [ObservableProperty]
    private string _assistantName;

    [ObservableProperty]
    private string _activationWord;

    [ObservableProperty]
    private string _language;

    [ObservableProperty]
    private string _aiProvider;

    [ObservableProperty]
    private string _aiModel;

    [ObservableProperty]
    private string _microphone;

    [ObservableProperty]
    private string _speaker;

    [ObservableProperty]
    private int _themeIndex;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [RelayCommand]
    private async Task SaveAsync()
    {
        var settings = _configuration.Current;
        settings.Assistant.Name = AssistantName;
        settings.Assistant.ActivationWord = ActivationWord;
        settings.Assistant.Language = Language;
        settings.AI.Provider = AiProvider;
        settings.AI.Model = AiModel;
        settings.Voice.Microphone = Microphone;
        settings.Voice.Speaker = Speaker;
        settings.Appearance.Theme = (ThemePreference)ThemeIndex;

        var result = await _configuration.SaveAsync(settings).ConfigureAwait(true);
        if (result.IsSuccess)
        {
            (Microsoft.UI.Xaml.Application.Current as App)?.ApplyTheme(settings.Appearance.Theme);
            StatusMessage = "Configuración guardada.";
        }
        else
        {
            StatusMessage = result.Error.Message;
        }
    }
}
