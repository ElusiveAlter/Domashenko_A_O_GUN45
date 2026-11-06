using UnityEngine;

namespace Netologia.Tactical
{
    /// <summary>
    /// Блокирует управление на время визуализации хода, хранит статус игры
    /// и очередь хода, выполняет ход
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public bool InputEnabled { get; private set; } = true;
        public GameStatus Status { get; set; } = GameStatus.Select;
        public UnitColor CurrentTurn { get; private set; }

        private void Awake()
        {
            // Первый ход выбирается случайно
            CurrentTurn = Random.value < 0.5f ? UnitColor.Black : UnitColor.White;
            Debug.Log($"[Game] Первый ход: {CurrentTurn}");
        }

        public void LockInput() => InputEnabled = false;
        public void UnlockInput() => InputEnabled = true;

        /// <summary>
        /// Выполняет ход: блокирует ввод, применяет перемещение, переключает очередь.
        /// Плавная визуализация будет добавлена на этом же месте — каркас уже готов
        /// </summary>
        public void ExecuteMove(Battlefield battlefield, Cell from, Cell to, Cell victim)
        {
            LockInput();
            battlefield.MoveUnit(from, to, victim);
            UnlockInput();
            SwitchTurn();
        }

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == UnitColor.White ? UnitColor.Black : UnitColor.White;
            Status = GameStatus.Select;
            Debug.Log($"[Game] Ход переходит: {CurrentTurn}");
        }
    }
}