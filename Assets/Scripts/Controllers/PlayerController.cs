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

        /// <summary>Идёт цепочка обязательных атак — выйти из состояния нельзя </summary>
        public bool IsAttackChain { get; private set; }

        private void Awake()
        {
            // Первый ход выбирается случайно
            CurrentTurn = Random.value < 0.5f ? UnitColor.Black : UnitColor.White;
            Debug.Log($"[Game] Первый ход: {CurrentTurn}");
        }

        public void LockInput() => InputEnabled = false;
        public void UnlockInput() => InputEnabled = true;

        /// <summary>
        /// Выполняет ход: блокирует ввод, применяет перемещение и решает, что дальше —
        /// продолжение атаки той же фишкой (п. 7 ТЗ) или переход хода с аналитикой
        /// </summary>
        public void ExecuteMove(Battlefield battlefield, Cell from, Cell to, Cell victim)
        {
            LockInput();
            battlefield.MoveUnit(from, to, victim);
            UnlockInput();

            // После взятия той же фишкой возможна ещё атака — продолжаем обязательно
            if (victim != null && battlefield.HasAttacksFrom(to))
            {
                IsAttackChain = true;
                battlefield.TrySelectUnit(to, CurrentTurn);
                Status = GameStatus.Move;
                Debug.Log("[Game] Цепочка атак: продолжайте взятие этой фишкой");
                return;
            }

            IsAttackChain = false;
            SwitchTurn();
            // Аналитика всех фишек нового игрока в начале его хода
            battlefield.ComputeMandatoryAttacks(CurrentTurn);
        }

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == UnitColor.White ? UnitColor.Black : UnitColor.White;
            Status = GameStatus.Select;
            Debug.Log($"[Game] Ход переходит: {CurrentTurn}");
        }
    }
}