namespace pacwall
{
    using pacwall.grid;
    using pacwall.player;
    using UnityEngine;

    public class GameMan : MonoBehaviour {
        [SerializeField] RectTransform gridRef;
        [SerializeField] MazeGrid grid;
        [SerializeField] Player player;

        void Start() {
            Vector3[] corners = new Vector3[4];
            gridRef.GetWorldCorners(corners);
            grid.BuildGrid(14, corners[0], corners[2]);
            player.Init(grid);
            grid.AddPlayerPos(player.pos);
            grid.AddGhost();
            player.onPlayerMove += OnPlayerMove;
        }

        void OnPlayerMove(Vector2Int pos) {
            grid.AddPlayerPos(pos);
        }
    }
}