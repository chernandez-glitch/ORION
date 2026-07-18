using Orion.Shared.Modules;

namespace Orion.AI;

internal sealed class AIStatusProvider : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus() =>
        new("IA", ModuleStatus.Disabled, "Contratos definidos. Proveedores (OpenAI/Claude/Gemini/Ollama/Azure) en Fase 2.");
}
