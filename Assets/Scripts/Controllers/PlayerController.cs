using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        /// <summary>Идёт цепочка обязательных атак — выйти из состояния нельзя</summary>
        public bool IsAttackChain { get; private set; }

        /// <summary>Событие смены очереди хода — для панели хода и прочего UI</summary>
        public event Action<UnitColor> TurnChanged;

        private void Awake()
        {
            // Первый ход выбирается случайно
            CurrentTurn = Random.value < 0.5f ? UnitColor.Black : UnitColor.White;
            Debug.Log($"[Game] Первый ход: {CurrentTurn}");
            TurnChanged?.Invoke(CurrentTurn);
        }

        public void LockInput() => InputEnabled = false;
        public void UnlockInput() => InputEnabled = true;

        /// <summary>
        /// Выполняет ход: блокирует ввод, применяет перемещение и решает, что дальше —
        /// продолжение атаки той же фишкой или переход хода с аналитикой
        /// </summary>
        public void ExecuteMove(Battlefield battlefield, Cell from, Cell to, Cell victim)
        {
            // Блокируем ввод на время визуализации хода.
            // Сейчас ход применяется мгновенно, поэтому блок снимается в конце метода;
            // при добавлении анимации UnlockInput нужно будет вызывать по её завершении
            LockInput();

            battlefield.MoveUnit(from, to, victim);

            if (victim != null && battlefield.HasAttacksFrom(to))
            {
                IsAttackChain = true;
                // Только взятия: простые ходы в цепочке атак запрещены
                battlefield.TrySelectUnit(to, CurrentTurn, true);
                Status = GameStatus.Move;
                UnlockInput();
                Debug.Log("[Game] Цепочка атак: продолжайте взятие этой фишкой");
                return;
            }

            IsAttackChain = false;
            SwitchTurn();
            // Аналитика всех фишек нового игрока в начале его хода
            battlefield.ComputeMandatoryAttacks(CurrentTurn);
            UnlockInput();
        }

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == UnitColor.White ? UnitColor.Black : UnitColor.White;
            Status = GameStatus.Select;
            Debug.Log($"[Game] Ход переходит: {CurrentTurn}");
            TurnChanged?.Invoke(CurrentTurn);
        }
    }
}