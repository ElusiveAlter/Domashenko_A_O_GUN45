using UnityEngine;

namespace Netologia.Tactical
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField]
        private Cell _cellPrefab;
        [SerializeField]
        private Material _lightMaterial;
        [SerializeField]
        private Material _darkMaterial;
        [SerializeField]
        private Material _hoverMaterial;
        [SerializeField]
        private Material _selectedMaterial;
        [SerializeField]
        private int _size = 8;

        private Cell[,] _cells;
        private Cell _selected;

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
                }
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

        private void OnCellClick(Cell cell)
        {
            if (_selected != null)
                _selected.MeshRenderer.sharedMaterial = _selected.IsDark ? _darkMaterial : _lightMaterial;
            _selected = cell;
            cell.MeshRenderer.sharedMaterial = _selectedMaterial;
        }
    }
}