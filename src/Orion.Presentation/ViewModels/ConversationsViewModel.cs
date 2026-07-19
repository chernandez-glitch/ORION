using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orion.Presentation.Models;

namespace Orion.Presentation.ViewModels;

/// <summary>Conversaciones con la IA. Datos simulados; la IA real llega en Fase 2.</summary>
public sealed partial class ConversationsViewModel : ObservableObject
{
    public ConversationsViewModel()
    {
        Conversations =
        [
            new ConversationItem("Preparar despliegue", "Revisemos los pasos para publicar la app…", "Hace 5 min"),
            new ConversationItem("Resumen del día", "Estos fueron tus comandos más usados…", "Hoy, 09:12"),
            new ConversationItem("Ideas de automatización", "Podríamos automatizar el backup nocturno…", "Ayer")
        ];
    }

    public ObservableCollection<ConversationItem> Conversations { get; }

    [ObservableProperty]
    private string _hint = "La conversación con IA se habilita en la Fase 2. Esta es una vista previa de la experiencia.";

    [RelayCommand]
    private void NewConversation() =>
        Hint = "La IA aún no está conectada. Podrás iniciar conversaciones reales en la Fase 2.";
}
