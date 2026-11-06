using System;
using UnityEngine;

namespace Netologia.Tactical
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField]
        private Cell _cellPrefab;
        [SerializeField]
        private Unit _unitPrefab;
        [SerializeField]
        private Material _lightMaterial;
        [SerializeField]
        private Material _darkMaterial;
        [SerializeField]
        private Material _hoverMaterial;
        [SerializeField]
        private Material _selectedMaterial;
        [SerializeField]
        private Material _unitWhiteMaterial;
        [SerializeField]
        private Material _unitBlackMaterial;
        [SerializeField]
        private int _size = 8;

        // Высота, на которую фишка ставится на клетку (клетка 0.2 высотой, верх на 0.1)
        private const float UnitOffset = 0.3f;

        private Cell[,] _cells;
        private Cell _selected;

        /// <summary>Клетка с выбранной фишкой — нужна команде для сброса выбора</summary>
        public Cell SelectedCell => _selected;

        public event Action<Cell> CellClicked;

        private void Awake()
        {
            BuildBoard();
        }

        private void BuildBoard()
        {
            _cells = new Cell[_size, _size];
            for (var x = 0; x < _size; x++)
                for (var z = 0; z < _size; z++)
                {
                    var cell = Instantiate(_cellPrefab, transform);
                    cell.transform.localPosition = new Vector3(x, 0f, z);
                    var isDark = (x + z) % 2 == 0;
                    cell.Initialize(x, z, isDark);
                    cell.MeshRenderer.sharedMaterial = isDark ? _darkMaterial : _lightMaterial;
                    cell.PointerEnter += OnCellEnter;
                    cell.PointerExit += OnCellExit;
                    cell.Clicked += OnCellClick;
                    _cells[x, z] = cell;

                    // Шашки стоят только на тёмных клетках, по три ряда с каждой стороны
                    if (isDark && (z <= 2 || z >= 5))
                        SpawnUnit(cell, z <= 2 ? UnitColor.Black : UnitColor.White);
                }
        }

        private void SpawnUnit(Cell cell, UnitColor color)
        {
            var unit = Instantiate(_unitPrefab, transform);
            unit.transform.localPosition = cell.transform.localPosition + Vector3.up * UnitOffset;
            unit.Initialize(color, cell);
            unit.MeshRenderer.sharedMaterial = color == UnitColor.White ? _unitWhiteMaterial : _unitBlackMaterial;
            unit.PointerEnter += OnUnitEnter;
            unit.PointerExit += OnUnitExit;
            unit.Clicked += OnUnitClick;
        }

        /// <summary>
        /// Выбирает фишку текущего игрока, стоящую на клетке.
        /// Возвращает false, если фишки нет или она чужая (статус и ход проверяет команда)
        /// </summary>
        public bool TrySelectUnit(Cell cell, UnitColor player)
        {
            if (cell.Unit == null || cell.Unit.Color != player) return false;
            if (_selected == cell) return false;
            ResetSelection();
            _selected = cell;
            cell.MeshRenderer.sharedMaterial = _selectedMaterial;
            return true;
        }

        public void ResetSelection()
        {
            if (_selected == null) return;
            _selected.MeshRenderer.sharedMaterial = _selected.IsDark ? _darkMaterial : _lightMaterial;
            _selected = null;
        }

        private void OnCellEnter(Cell cell)
        {
            if (cell == _selected) return;
            cell.MeshRenderer.sharedMaterial = _hoverMaterial;
        }

        private void OnCellExit(Cell cell)
        {
            if (cell == _selected) return;
            cell.MeshRenderer.sharedMaterial = cell.IsDark ? _darkMaterial : _lightMaterial;
        }

        private void OnCellClick(Cell cell) => CellClicked?.Invoke(cell);

        // Наведение на фишку подсвечивает клетку, на которой она стоит
        private void OnUnitEnter(Unit unit) => OnCellEnter(unit.Cell);
        private void OnUnitExit(Unit unit) => OnCellExit(unit.Cell);
        private void OnUnitClick(Unit unit) => OnCellClick(unit.Cell);
    }
}