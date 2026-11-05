using UnityEngine;

namespace Netologia.Tactical
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        public MeshRenderer MeshRenderer => _meshRenderer;
    }
}