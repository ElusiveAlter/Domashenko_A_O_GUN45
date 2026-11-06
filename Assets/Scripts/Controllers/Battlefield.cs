using System;
using System.Collections.Generic;
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
        private Material _moveMaterial;
        [SerializeField]
        private Material _unitWhiteMaterial;
        [SerializeField]
        private Material _unitBlackMaterial;
        [SerializeField]
        private Material _kingWhiteMaterial;
        [SerializeField]
        private Material _kingBlackMaterial;
        [SerializeField]
        private int _size = 8;

        // Высота, на которую фишка ставится на клетку (клетка 0.2 высотой, верх на 0.1)
        private const float UnitOffset = 0.3f;

        // Диагональные смещения по X (вперёд/назад задаётся направлением фишки)
        private static readonly int[] Diagonals = { -1, 1 };

        private Cell[,] _cells;
        private Cell _selected;
        // Доступные ходы выбранной фишки: клетка назначения -> клетка жертвы (null без атаки)
        private readonly Dictionary<Cell, Cell> _currentMoves = new();
        private readonly List<Cell> _moveCells = new();

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
        /// Выбирает фишку текущего игрока, стоящую на клетке, и подсвечивает её ходы.
        /// Возвращает false, если фишки нет или она чужая (статус и ход проверяет команда)
        /// </summary>
        public bool TrySelectUnit(Cell cell, UnitColor player)
        {
            if (cell.Unit == null || cell.Unit.Color != player) return false;
            if (_selected == cell) return false;
            ResetSelection();
            _selected = cell;
            cell.MeshRenderer.sharedMaterial = _selectedMaterial;
            HighlightMoves(cell);
            return true;
        }

        private void HighlightMoves(Cell cell)
        {
            _currentMoves.Clear();
            _moveCells.Clear();

            var unit = cell.Unit;
            if (unit == null) return;

            if (unit.IsKing)
            {
                // Дамка ходит на любое расстояние по всем четырём диагоналям
                foreach (var dx in Diagonals)
                    foreach (var dz in Diagonals)
                        ScanKingLine(unit.Color, cell.X, cell.Z, dx, dz);
            }
            else
            {
                var x = cell.X;
                var z = cell.Z;
                // Чёрные ходят в сторону роста Z, белые — в сторону уменьшения
                var forward = unit.Color == UnitColor.Black ? 1 : -1;

                foreach (var dx in Diagonals)
                {
                    // Простое перемещение вперёд на пустую клетку
                    TryAddSimple(x + dx, z + forward);
                    // Атака вперёд: чужая фишка перед, пустая за ней
                    TryAddAttack(unit.Color, x + dx, z + forward, x + 2 * dx, z + 2 * forward);
                    // Атака назад — единственный разрешённый ход назад
                    TryAddAttack(unit.Color, x + dx, z - forward, x + 2 * dx, z - 2 * forward);
                }
            }

            foreach (var destination in _currentMoves.Keys)
            {
                destination.MeshRenderer.sharedMaterial = _moveMaterial;
                _moveCells.Add(destination);
            }
        }

        // Проходит по диагонали до упора: пустые клетки — ходы,
        // первая встреченная фишка заканчивает линию (свою — блокирует, чужую — даёт атаку)
        private void ScanKingLine(UnitColor mover, int x, int z, int dx, int dz)
        {
            var cx = x + dx;
            var cz = z + dz;
            while (TryGetCell(cx, cz, out var cell))
            {
                if (cell.Unit == null)
                {
                    _currentMoves[cell] = null;
                }
                else
                {
                    // Чужая фишка — атака с посадкой сразу за ней
                    if (cell.Unit.Color != mover)
                        TryAddAttack(mover, cx, cz, cx + dx, cz + dz);
                    return; // первая фишка заканчивает линию в любом случае
                }
                cx += dx;
                cz += dz;
            }
        }

        private void TryAddSimple(int x, int z)
        {
            if (TryGetCell(x, z, out var cell) && cell.Unit == null)
                _currentMoves[cell] = null;
        }

        private void TryAddAttack(UnitColor mover, int midX, int midZ, int dstX, int dstZ)
        {
            // Клетка с чужой фишкой должна существовать
            if (!TryGetCell(midX, midZ, out var mid) || mid.Unit == null) return;
            // Своя фишка блокирует всё направление
            if (mid.Unit.Color == mover) return;
            // Клетка за ней должна существовать и быть пустой, иначе направление блокировано
            if (!TryGetCell(dstX, dstZ, out var dst) || dst.Unit != null) return;
            _currentMoves[dst] = mid;
        }

        private bool TryGetCell(int x, int z, out Cell cell)
        {
            cell = null;
            if (x < 0 || z < 0 || x >= _size || z >= _size) return false;
            cell = _cells[x, z];
            return true;
        }

        /// <summary>
        /// Проверяет, доступен ли ход в клетку назначения, и возвращает жертву атаки (если есть)
        /// </summary>
        public bool TryGetMove(Cell to, out Cell victim)
        {
            victim = null;
            return _selected != null && _currentMoves.TryGetValue(to, out victim);
        }

        /// <summary>Выполняет ход: перемещает фишку, убирает жертву атаки, снимает подсветку</summary>
        public void MoveUnit(Cell from, Cell to, Cell victim)
        {
            ResetSelection();

            var unit = from.Unit;
            from.Unit = null;

            if (victim != null)
            {
                var victimUnit = victim.Unit;
                victim.Unit = null;
                Destroy(victimUnit.gameObject);
            }

            unit.transform.localPosition = to.transform.localPosition + Vector3.up * UnitOffset;
            unit.SetCell(to);

            // Достижение противоположного конца — превращение в дамку
            if (!unit.IsKing && (unit.Color == UnitColor.Black ? to.Z == _size - 1 : to.Z == 0))
            {
                unit.Promote();
                unit.MeshRenderer.sharedMaterial = unit.Color == UnitColor.White ? _kingWhiteMaterial : _kingBlackMaterial;
                Debug.Log($"[Game] Фишка ({to.X}, {to.Z}) стала дамкой!");
            }
        }

        public void ResetSelection()
        {
            ClearMoveHighlights();
            if (_selected == null) return;
            _selected.MeshRenderer.sharedMaterial = _selected.IsDark ? _darkMaterial : _lightMaterial;
            _selected = null;
        }

        private void ClearMoveHighlights()
        {
            foreach (var cell in _moveCells)
                cell.MeshRenderer.sharedMaterial = cell.IsDark ? _darkMaterial : _lightMaterial;
            _moveCells.Clear();
            _currentMoves.Clear();
        }

        private void OnCellEnter(Cell cell)
        {
            if (cell == _selected) return;
            cell.MeshRenderer.sharedMaterial = _hoverMaterial;
        }

        private void OnCellExit(Cell cell)
        {
            if (cell == _selected) return;
            // Клетку хода после ухода курсора возвращаем в голубой, обычные — в шахматный цвет
            if (_moveCells.Contains(cell))
                cell.MeshRenderer.sharedMaterial = _moveMaterial;
            else
                cell.MeshRenderer.sharedMaterial = cell.IsDark ? _darkMaterial : _lightMaterial;
        }

        private void OnCellClick(Cell cell) => CellClicked?.Invoke(cell);

        // Наведение на фишку подсвечивает клетку, на которой она стоит
        private void OnUnitEnter(Unit unit) => OnCellEnter(unit.Cell);
        private void OnUnitExit(Unit unit) => OnCellExit(unit.Cell);
        private void OnUnitClick(Unit unit) => OnCellClick(unit.Cell);
    }
}