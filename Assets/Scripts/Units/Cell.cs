using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Tactical
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;
        [SerializeField]
        private Material _highlightMaterial;

        private Material _defaultMaterial;

        public MeshRenderer MeshRenderer => _meshRenderer;

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Запоминаем текущий материал клетки перед подсветкой
            _defaultMaterial = _meshRenderer.sharedMaterial;
            _meshRenderer.sharedMaterial = _highlightMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _meshRenderer.sharedMaterial = _defaultMaterial;
        }
    }
}