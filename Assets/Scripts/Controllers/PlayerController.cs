using UnityEngine;

namespace Netologia.Tactical
{
    /// <summary>
    /// Блокирует управление на время визуализации хода, хранит статус игры
    /// и очередь хода
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public bool InputEnabled { get; private set; } = true;
        public GameStatus Status { get; set; } = GameStatus.Select;
        public UnitColor CurrentTurn { get; private set; }

        private void Awake()
        {
            // Первый ход выбирается случайно
            CurrentTurn = Random.value < 0.5f ? UnitColor.White : UnitColor.Black;
            Debug.Log($"[Game] Первый ход: {CurrentTurn}");
        }

        public void LockInput() => InputEnabled = false;
        public void UnlockInput() => InputEnabled = true;

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == UnitColor.White ? UnitColor.Black : UnitColor.White;
            Status = GameStatus.Select;
        }
    }
}