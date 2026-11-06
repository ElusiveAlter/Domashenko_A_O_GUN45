using UnityEngine;

namespace Netologia.Tactical.Commands
{
    /// <summary>
    /// Команда выбора и хода: в Select выбирает фишку текущего игрока,
    /// в Move выполняет ход по подсвеченной клетке или перевыбор фишки
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
                    TryMoveOrReselect(cell);
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

        private void TryMoveOrReselect(Cell cell)
        {
            var selected = _battlefield.SelectedCell;

            // Клик по подсвеченной клетке хода — выполняем ход
            if (selected != null && _battlefield.TryGetMove(cell, out var victim))
            {
                var attack = victim != null ? $" с атакой на ({victim.X}, {victim.Z})" : string.Empty;
                Debug.Log($"[Command] Ход ({selected.X}, {selected.Z}) -> ({cell.X}, {cell.Z}){attack}");
                _player.ExecuteMove(_battlefield, selected, cell, victim);
                return;
            }

            // Повторный клик по выбранной фишке — сброс выбора
            if (cell == selected)
            {
                ResetSelection();
                return;
            }

            // Клик по своей другой фишке — переключение выбора
            if (cell.Unit != null && cell.Unit.Color == _player.CurrentTurn)
                TrySelect(cell);
            // Клик по пустой недоступной клетке — без действия
        }

        private void ResetSelection()
        {
            _battlefield.ResetSelection();
            _player.Status = GameStatus.Select;
            Debug.Log("[Command] Выбор сброшен");
        }
    }
}