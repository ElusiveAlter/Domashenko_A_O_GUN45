using UnityEngine;
using TMPro;

namespace Netologia.Tactical
{
    /// <summary>
    /// Панель хода: показывает сторону, чей сейчас ход (критерий «Панель хода»)
    /// </summary>
    public class TurnPanelController : MonoBehaviour
    {
        [SerializeField]
        private PlayerController _player;
        [SerializeField]
        private TMP_Text _label;

        private void OnEnable()
        {
            if (_player != null)
            {
                _player.TurnChanged += OnTurnChanged;
                OnTurnChanged(_player.CurrentTurn);
            }
        }

        private void Start()
        {
            // Страховка: если OnEnable панели выполнился раньше Awake игрока,
            // событие ещё не приходило — показываем актуальный ход из Start
            if (_player != null)
                OnTurnChanged(_player.CurrentTurn);
        }

        private void OnDisable()
        {
            if (_player != null)
                _player.TurnChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(UnitColor turn)
        {
            if (_label == null) return;
            _label.text = turn == UnitColor.White ? "Ход: белые" : "Ход: чёрные";
            _label.color = turn == UnitColor.White ? Color.white : Color.black;
        }
    }
}