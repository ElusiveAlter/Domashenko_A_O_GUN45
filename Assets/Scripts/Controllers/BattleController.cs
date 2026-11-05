using UnityEngine;
using Netologia.Tactical.Commands;

namespace Netologia.Tactical
{
    /// <summary>
    /// Обрабатывает весь инпут игрока.
    /// Пока только часть с мышью; клавиатура (Cancel/Confirm) подключим следом.
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [SerializeField]
        private Battlefield _battlefield;

        private CellSelectCommand _cellSelect;

        private void Awake()
        {
            _cellSelect = new CellSelectCommand(_battlefield);
        }

        private void OnEnable() => _battlefield.CellClicked += OnCellClicked;
        private void OnDisable() => _battlefield.CellClicked -= OnCellClicked;

        private void OnCellClicked(Cell cell) => _cellSelect.Interact(cell);
    }
}