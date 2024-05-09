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
        [SerializeField] [Range(1, 4)] int ghostCount;
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
            for(int i=0; i<ghostCount; i++)
                ghosts.Add(CreateGhost());
            player.onMove += OnPlayerMove;
            gameUI.onRestart += OnGameRestart;
            grid.onProgress += OnProgress;
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
            Debug.Log("update player pos: " + pos);
            grid.UpdatePlayerPos(pos);
            if(grid.CheckPos(pos, MazeGrid.BlockItem.Ghost))
                OnGhostHitPlayer();
            else if(grid.CheckPos(pos, MazeGrid.BlockItem.PowerUp))
                OnPowerPickup();
        }

        void OnGhostHitPlayer() {
            Debug.Log("OnGhostHitPlayer");
            isPlaying = false;
            gameUI.Show("Fail !!");
            Destroy(player);
            foreach(var g in ghosts)
                g.Stop();
        }

        void OnProgress(int progress) {
            gameUI.UpdateProgres(progress*10/8);
            if(progress >= 80) {
                gameUI.Show("Pass !!");
                Destroy(player);
                foreach(var g in ghosts)
                    g.Stop();
            }
        }

        Ghost CreateGhost() {
            var item = Instantiate(ghostPrefab, ghostPrefab.transform.parent);
            item.gameObject.SetActive(true);
            item.onPlayerHit += OnGhostHitPlayer;
            Vector2Int p = Vector2Int.zero;
            var lpos = player.pos;
            var size = grid.size;
            while(Vector2Int.Distance(p, lpos) < 10 || grid.CheckPos(p, MazeGrid.BlockItem.Ghost)) {
                p.x = Random.Range(0, size.x);
                p.y = Random.Range(0, size.y);
            }
            item.onMove += grid.UpdateGhostPos;
            grid.UpdateGhostPos(p, new Vector2Int(-1, -1));
            item.pos = p;
            item.Init(grid);
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