using Netologia.Tactical;

namespace Netologia.Tactical.Commands
{
    /// <summary>
    /// Обрабатывает одно взаимодействие игрока с полем: выбор фишки или подтверждение хода по указанной клетке
    /// </summary>
    public interface IGameplayCommand
    {
        void Interact(Cell cell);
    }
}