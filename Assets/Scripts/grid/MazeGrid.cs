namespace pacwall.grid
{
    using UnityEngine;

    public class MazeGrid : MonoBehaviour
    {
        [SerializeField] SpriteRenderer bg;
        [SerializeField] Transform debug0, debug1;

        [SerializeField] public Vector2Int size;
        [SerializeField] public Vector2 scale;
        Vector2 zero;
        // [SerializeField] public 

        public void BuildGrid(int wn, Vector2 bl, Vector2 tr)
        {
            debug0.position = bl;
            debug1.position = tr;

            Vector2 vSize = tr-bl;
            int hn = (int)(wn * (vSize.y/vSize.x));
            size = new Vector2Int(wn, hn);
            Vector2 offset = new Vector2(vSize.x/wn, vSize.y/hn);
            bg.transform.position = (bl + tr)/2;
            bg.transform.localScale = offset;
            bg.size = size;
            zero = bl + offset/2;

            scale = offset;
        }

        public Vector2 GetPos(Vector2Int pos) {
            return new Vector2(scale.x * pos.x, scale.y * pos.y) + zero;
        }
    }
}