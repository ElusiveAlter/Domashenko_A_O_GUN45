using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Netologia.Tactical.Commands;

namespace Netologia.Tactical
{
    /// <summary>
    /// Обрабатывает весь инпут игрока с клавиатуры и с мыши
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [SerializeField]
        private Battlefield _battlefield;
        [SerializeField]
        private PlayerController _player;
        [SerializeField]
        private InputActionAsset _inputActions;

        private CellSelectCommand _cellSelect;
        private InputAction _cancelAction;
        private InputAction _confirmAction;

        // Клетка, над которой сейчас находится курсор (для Confirm по Space)
        private Cell _hoveredCell;

        private void Awake()
        {
            _cellSelect = new CellSelectCommand(_battlefield, _player);

            var map = _inputActions.FindActionMap("Gameplay");
            _cancelAction = map.FindAction("Cancel");
            _confirmAction = map.FindAction("Confirm");
        }

        private void Start()
        {
            // Первая аналитика обязательных атак в начале игры
            _battlefield.ComputeMandatoryAttacks(_player.CurrentTurn);
        }

        private void OnEnable()
        {
            _battlefield.CellClicked += OnCellClicked;
            _battlefield.CellHovered += OnCellHovered;
            _battlefield.CellUnhovered += OnCellUnhovered;

            _cancelAction.performed += OnCancel;
            _confirmAction.performed += OnConfirm;

            _cancelAction.Enable();
            _confirmAction.Enable();
        }

        private void OnDisable()
        {
            _battlefield.CellClicked -= OnCellClicked;
            _battlefield.CellHovered -= OnCellHovered;
            _battlefield.CellUnhovered -= OnCellUnhovered;

            _cancelAction.performed -= OnCancel;
            _confirmAction.performed -= OnConfirm;

            _cancelAction.Disable();
            _confirmAction.Disable();

            // При выключении контроллера курсор «висеть» над клеткой не должен
            _hoveredCell = null;
        }

        private void OnCellClicked(Cell cell)
        {
            TryInteract(cell);
        }

        private void OnCellHovered(Cell cell)
        {
            _hoveredCell = cell;
        }

        private void OnCellUnhovered(Cell cell)
        {
            // Снимаем наведение, только если ушла именно та клетка, что зарегистрирована
            if (_hoveredCell == cell)
                _hoveredCell = null;
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (!_player.InputEnabled) return;

            // Из цепочки атак выйти насильно нельзя
            if (_player.IsAttackChain)
            {
                Debug.Log("[Input] Cancel: цепочка атак не может быть прервана");
                return;
            }

            if (_player.Status == GameStatus.Move)
            {
                // Ограничение обязательной атаки не снимается
                _battlefield.ResetSelection();
                _player.Status = GameStatus.Select;
                Debug.Log("[Input] Cancel: сброс выбора фишки");
            }
        }

        private void OnConfirm(InputAction.CallbackContext context)
        {
            // Space повторяет действие ЛКМ по клетке под курсором.
            // Если под курсором нет клетки — действие просто не срабатывает
            TryInteract(_hoveredCell);
        }

        /// <summary>
        /// Общая точка входа для ЛКМ и Space: выбор фишки или подтверждение хода
        /// </summary>
        private void TryInteract(Cell cell)
        {
            if (cell == null)
                return;

            if (_player.InputEnabled)
                _cellSelect.Interact(cell);
        }
    }
}