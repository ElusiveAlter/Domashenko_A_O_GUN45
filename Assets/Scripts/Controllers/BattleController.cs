using UnityEngine;
using UnityEngine.InputSystem;
using Netologia.Tactical.Commands;

namespace Netologia.Tactical
{
    /// <summary>
    /// Обрабатывает весь инпут игрока с клавиатуры и с мыши.
    /// Мышь приходит событиями от Battlefield, клавиатура — через InputActionAsset.
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

        private void Awake()
        {
            _cellSelect = new CellSelectCommand(_battlefield);

            var map = _inputActions.FindActionMap("Gameplay");
            _cancelAction = map.FindAction("Cancel");
            _confirmAction = map.FindAction("Confirm");
        }

        private void OnEnable()
        {
            _battlefield.CellClicked += OnCellClicked;
            _cancelAction.performed += OnCancel;
            _confirmAction.performed += OnConfirm;
            _cancelAction.Enable();
            _confirmAction.Enable();
        }

        private void OnDisable()
        {
            _battlefield.CellClicked -= OnCellClicked;
            _cancelAction.performed -= OnCancel;
            _confirmAction.performed -= OnConfirm;
            _cancelAction.Disable();
            _confirmAction.Disable();
        }

        private void OnCellClicked(Cell cell)
        {
            // Пока ввод разрешён — клик уходит в команду
            if (_player.InputEnabled)
                _cellSelect.Interact(cell);
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (_player.InputEnabled)
                Debug.Log("[Input] Cancel");
        }

        private void OnConfirm(InputAction.CallbackContext context)
        {
            if (_player.InputEnabled)
                Debug.Log("[Input] Confirm");
        }
    }
}