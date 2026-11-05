using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Tactical
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        public event Action<Cell> PointerEnter;
        public event Action<Cell> PointerExit;
        public event Action<Cell> Clicked;

        public int X { get; private set; }
        public int Z { get; private set; }
        public bool IsDark { get; private set; }

        public MeshRenderer MeshRenderer => _meshRenderer;

        public void Initialize(int x, int z, bool isDark)
        {
            X = x;
            Z = z;
            IsDark = isDark;
        }

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke(this);
        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);
    }
}