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
        private int _size = 8;

        private Cell[,] _cells;

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
                    cell.MeshRenderer.sharedMaterial = isDark ? _darkMaterial : _lightMaterial;
                    _cells[x, z] = cell;
                }
        }
    }
}