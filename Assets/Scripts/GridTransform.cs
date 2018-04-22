using UnityEngine;

namespace Assets.Scripts
{
    public class GridTransform : MonoBehaviour
    {
        public Vector2 Position { get { return PlacementGrid.Instance.WorldToGrid(transform.position); } }
        public Vector2 Size;

        void Start()
        {
        }
        
        void Update()
        {
        }
    }
}
