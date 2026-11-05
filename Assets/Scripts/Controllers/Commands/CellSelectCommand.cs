using UnityEngine;

namespace Netologia.Tactical.Commands
{
    /// <summary>
    /// Команда выбора клетки: обрабатывает клик и регистрирует действие.
    /// Пока — журнал в консоль, дальше на её месте появится выбор фишки.
    /// </summary>
    public class CellSelectCommand : IGameplayCommand
    {
        private readonly Battlefield _battlefield;

        public CellSelectCommand(Battlefield battlefield)
        {
            _battlefield = battlefield;
        }

        public void Interact(Cell cell)
        {
            Debug.Log($"[Command] Выбрана клетка ({cell.X}, {cell.Z})");
        }
    }
}