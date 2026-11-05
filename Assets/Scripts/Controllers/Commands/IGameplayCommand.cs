using Netologia.Tactical;

namespace Netologia.Tactical.Commands
{
    /// <summary>
    /// Обработка взаимодействия игрока с игровым полем
    /// </summary>
    public interface IGameplayCommand
    {
        void Interact(Cell cell);
    }
}