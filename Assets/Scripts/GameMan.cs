namespace pacwall
{
    using pacwall.grid;
    using pacwall.player;
    using UnityEngine;

    public class GameMan : MonoBehaviour {
        [SerializeField] RectTransform gridRef;
        [SerializeField] MazeGrid grid;
        [SerializeField] Player player;
        [SerializeField] [Range(8, 20)] int ghostSpeed;

        void Start() {
            Vector3[] corners = new Vector3[4];
            gridRef.GetWorldCorners(corners);
            grid.BuildGrid(14, corners[0], corners[2]);
            player.Init(grid);
            grid.UpdatePlayerPos(player.pos);
            var g = grid.AddGhost();
            g.Init(grid, ghostSpeed);
            g.onPlayerHit += OnGhostHitPlayer;
            player.onMove += OnPlayerMove;
        }

        void OnPlayerMove(Vector2Int pos) {
            grid.UpdatePlayerPos(pos);
        }

        void OnGhostHitPlayer() {
            Debug.Log("game over");
        }
    }
}