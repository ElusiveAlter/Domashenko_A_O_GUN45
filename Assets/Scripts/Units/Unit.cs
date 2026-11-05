using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Tactical
{
    /// <summary>
    /// Фишка на поле. Умеет ловить наведение и клик,
    /// хранит ссылку на свою клетку и данные фишки (цвет команды; тип/статы добавим позже).
    /// </summary>
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<Unit> PointerEnter;
        public event Action<Unit> PointerExit;
        public event Action<Unit> Clicked;

        public UnitColor Color { get; private set; }
        public Cell Cell { get; private set; }

        public void Initialize(UnitColor color, Cell cell)
        {
            Color = color;
            SetCell(cell);
        }

        public void SetCell(Cell cell)
        {
            Cell = cell;
            cell.Unit = this;
        }

        public void OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke(this);
        public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);
    }
}