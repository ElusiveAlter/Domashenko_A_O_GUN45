using UnityEngine;

namespace Netologia.Tactical.Commands
{
    /// <summary>
    /// Команда выбора: в Select выбирает фишку текущего игрока,
    /// в Move позволяет перевыбрать свою фишку или сбросить выбор
    /// </summary>
    public class CellSelectCommand : IGameplayCommand
    {
        private readonly Battlefield _battlefield;
        private readonly PlayerController _player;

        public CellSelectCommand(Battlefield battlefield, PlayerController player)
        {
            _battlefield = battlefield;
            _player = player;
        }

        public void Interact(Cell cell)
        {
            if (cell == null) return;
            switch (_player.Status)
            {
                case GameStatus.Select:
                    TrySelect(cell);
                    break;
                case GameStatus.Move:
                    TryReselect(cell);
                    break;
            }
        }

        private void TrySelect(Cell cell)
        {
            if (_battlefield.TrySelectUnit(cell, _player.CurrentTurn))
            {
                _player.Status = GameStatus.Move;
                Debug.Log($"[Command] Выбрана фишка ({cell.X}, {cell.Z}) команды {_player.CurrentTurn}");
            }
            else if (cell.Unit != null)
            {
                Debug.Log($"[Command] Нельзя выбрать фишку соперника ({cell.X}, {cell.Z})");
            }
        }

        private void TryReselect(Cell cell)
        {
            // Повторный клик по выбранной фишке — сброс выбора
            if (cell == _battlefield.SelectedCell)
            {
                ResetSelection();
                return;
            }
            // Клик по своей другой фишке — переключение выбора
            if (cell.Unit != null && cell.Unit.Color == _player.CurrentTurn)
                TrySelect(cell);
            // Клик по пустой клетке — пока без действия, ходы появятся следующим шагом
        }

        private void ResetSelection()
        {
            _battlefield.ResetSelection();
            _player.Status = GameStatus.Select;
            Debug.Log("[Command] Выбор сброшен");
        }
    }
}