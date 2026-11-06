using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Netologia.Tactical
{
    /// <summary>
    /// Рестарт уровня: при удержании TAB появляется шкала и заполняется,
    /// при заполнении — перезапуск сцены (критерий «Рестарт интерфейс»)
    /// </summary>
    public class RestartController : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset _inputActions;
        [SerializeField]
        private GameObject _restartBar;
        [SerializeField]
        private UnityEngine.UI.Image _fillImage;

        // Время удержания до полного заполнения шкалы
        [SerializeField]
        private float _holdDuration = 1.5f;

        private InputAction _restartAction;
        private float _progress;

        private void Awake()
        {
            _restartAction = _inputActions.FindActionMap("Gameplay").FindAction("Restart");
        }

        private void OnEnable()
        {
            _restartAction.Enable();
            _progress = 0f;
            if (_fillImage != null)
                _fillImage.fillAmount = 0f;
            if (_restartBar != null)
                _restartBar.SetActive(false);
        }

        private void OnDisable()
        {
            _restartAction.Disable();
        }

        private void Update()
        {
            if (_restartAction == null || _restartBar == null || _fillImage == null) return;

            if (_restartAction.IsPressed())
            {
                if (!_restartBar.activeSelf) _restartBar.SetActive(true);

                _progress += Time.deltaTime / _holdDuration;
                if (_progress >= 1f)
                {
                    _progress = 0f;
                    Debug.Log("[Game] Рестарт сцены по удержанию TAB");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
            else if (_progress > 0f)
            {
                // Отпустили TAB — шкала прячется и сбрасывается
                _progress = 0f;
                _restartBar.SetActive(false);
            }

            _fillImage.fillAmount = _progress;
        }
    }
}