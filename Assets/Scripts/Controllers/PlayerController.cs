using UnityEngine;

namespace Netologia.Tactical
{
    /// <summary>
    /// Блокирует управление на время визуализации хода и визуализирует ход.
    /// Пока — каркас: включает/выключает ввод через флаг.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public bool InputEnabled { get; private set; } = true;

        public void LockInput() => InputEnabled = false;
        public void UnlockInput() => InputEnabled = true;
    }
}