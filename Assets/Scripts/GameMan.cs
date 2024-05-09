namespace pacwall
{
    using System.Collections.Generic;
    using pacwall.grid;
    using pacwall.player;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GameMan : MonoBehaviour {
        [SerializeField] RectTransform gridRef;
        [SerializeField] MazeGrid grid;
        [SerializeField] Player player;
        [SerializeField] Ghost ghostPrefab;
        [SerializeField] GameUI gameUI;
        List<Ghost> ghosts = new List<Ghost>();

        void Start() {
            Vector3[] corners = new Vector3[4];
            gridRef.GetWorldCorners(corners);
            grid.BuildGrid(14, corners[0], corners[2]);
            player.Init(grid);
            grid.UpdatePlayerPos(player.pos);
            ghostPrefab.transform.localScale = grid.scale;
            ghosts.Add(CreateGhost());
            player.onMove += OnPlayerMove;
            gameUI.onRestart += OnGameRestart;
        }

        void OnPlayerMove(Vector2Int pos) {
            grid.UpdatePlayerPos(pos);
        }

        void OnGhostHitPlayer() {
            Destroy(player);
            foreach(var g in ghosts)
                Destroy(g);
            gameUI.Activate();
        }

        Ghost CreateGhost() {
            var item = Instantiate(ghostPrefab, ghostPrefab.transform.parent);
            item.Init(grid);
            item.onPlayerHit += OnGhostHitPlayer;
            Vector2Int p = Vector2Int.zero;
            var lpos = player.pos;
            var size = grid.size;
            while(Vector2Int.Distance(p, lpos) < 6) {
                p.x = Random.Range(0, size.x);
                p.y = Random.Range(0, size.y);
            }
            item.pos = p;
            grid.UpdateGhostPos(p, new Vector2Int(-1, -1));
            item.transform.position = grid.GetPos(p);
            item.onMove += grid.UpdateGhostPos;
            return item;
        }

        public void OnGameRestart() {
            SceneManager.LoadScene(0);
        }
    }
}