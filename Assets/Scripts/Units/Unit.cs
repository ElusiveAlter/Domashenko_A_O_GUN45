using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Tactical
{
    /// <summary>
    /// Фишка на поле. Ловит наведение и клик,
    /// хранит ссылку на свою клетку и данные фишки: цвет команды и статус дамки
    /// </summary>
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        public event Action<Unit> PointerEnter;
        public event Action<Unit> PointerExit;
        public event Action<Unit> Clicked;

        public UnitColor Color { get; private set; }
        public Cell Cell { get; private set; }
        public bool IsKing { get; private set; }

        public MeshRenderer MeshRenderer => _meshRenderer;

        public void Initialize(UnitColor color, Cell cell)
        {
            Color = color;
            IsKing = false;
            SetCell(cell);
        }

        public void SetCell(Cell cell)
        {
            Cell = cell;
            cell.Unit = this;
        }

        /// <summary>Превращение в дамку при достижении противоположного края</summary>
        public void Promote() => IsKing = true;

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke(this);
        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);
    }
}