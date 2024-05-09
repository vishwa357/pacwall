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
        [SerializeField] Transform powerUp;
        [SerializeField] GameUI gameUI;
        [SerializeField] [Range(1, 5)] int powerUpTime;
        [SerializeField] [Range(0, 5)] int powerUpOffsetTime;
        List<Ghost> ghosts = new List<Ghost>();
        Vector2Int powerUpPos = new Vector2Int(-1, -1);
        bool isPlaying = false;
        float powerupHideTime = -1, powerUpShowTime = -1;

        void Start() {
            Vector3[] corners = new Vector3[4];
            gridRef.GetWorldCorners(corners);
            grid.BuildGrid(14, corners[0], corners[2]);
            player.Init(grid);
            grid.UpdatePlayerPos(player.pos);
            ghostPrefab.transform.localScale = grid.scale;
            powerUp.localScale = grid.scale;
            ghosts.Add(CreateGhost());
            player.onMove += OnPlayerMove;
            gameUI.onRestart += OnGameRestart;
            isPlaying = true;
        }

        void Update() {
            if(isPlaying) {
                if(Time.timeSinceLevelLoad > powerupHideTime) {
                    PowerUpHide();
                    powerUpShowTime = Time.timeSinceLevelLoad + powerUpTime;
                    powerupHideTime = powerUpShowTime + 1;
                }
                else if(Time.timeSinceLevelLoad > powerUpShowTime) {
                    CreatePowerUp();
                    powerupHideTime = Time.timeSinceLevelLoad + powerUpOffsetTime;
                    powerUpShowTime = powerupHideTime + 1;
                }
            }
        }

        void OnPlayerMove(Vector2Int pos) {
            grid.UpdatePlayerPos(pos);
            if(grid.CheckPos(pos, MazeGrid.BlockItem.Ghost))
                OnGhostHitPlayer();
            else if(grid.CheckPos(pos, MazeGrid.BlockItem.PowerUp))
                OnPowerPickup();
        }

        void OnGhostHitPlayer() {
            isPlaying = false;
            gameUI.Activate();
            Destroy(player);
            foreach(var g in ghosts)
                Destroy(g);
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

        void OnPowerPickup() {
            PowerUpHide();
            powerUpShowTime = Time.timeSinceLevelLoad + powerUpTime;
            powerupHideTime = powerUpShowTime + 1;
            for(int i=0; i<ghosts.Count; i++)
                ghosts[i].SlowDown();
        }

        void PowerUpHide() {
            if(powerUpPos.x >= 0) {
                grid.RemovePowerUpPos(powerUpPos);
                powerUp.gameObject.SetActive(false);
            }
        }

        Vector2Int CreatePowerUp() {
            int x = grid.size.x, y = grid.size.y;
            Vector2Int pos = new Vector2Int(Random.Range(0, x), Random.Range(0, y));
            while(grid.CheckPos(pos, MazeGrid.BlockItem.Player | MazeGrid.BlockItem.Ghost))
                pos = new Vector2Int(Random.Range(0, x), Random.Range(0, y));
            grid.AddPowerUpPos(pos);
            powerUp.position = grid.GetPos(pos);
            powerUp.gameObject.SetActive(true);
            powerUpPos = pos;
            return pos;
        }

        public void OnGameRestart() {
            SceneManager.LoadScene(0);
        }
    }
}