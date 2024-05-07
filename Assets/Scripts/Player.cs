using Unity.VisualScripting;

namespace pacwall
{
    using pacwall.grid;
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        MazeGrid grid;

        [SerializeField] Vector2Int mid;

        public void Init(MazeGrid grid)
        {
            this.grid = grid;
            transform.localScale = grid.scale;
            mid = grid.size/2;
            transform.localPosition = grid.GetPos(Vector2Int.zero);
        }

        void Update()
        {
            
        }
    }
}