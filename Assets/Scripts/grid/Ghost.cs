namespace pacwall.grid
{
    using System;
    using System.Collections;
    using UnityEngine;
    using static Unity.Mathematics.math;

    /// <summary>
    /// Controls Ghost NPC movement and event fires.
    /// </summary>
    public class Ghost : MonoBehaviour {
        [SerializeField] [Range(2, 8)] int speed;
        [SerializeField] [Range(0, 4)] int slowSpeed = 1;
        [SerializeField] [Range(1, 5)] int slowDuration = 3;

        /// <summary>
        /// Current pos on grid
        /// </summary>
        public Vector2Int pos;
        /// <summary>
        /// Fired when ghost touches the player or temp wall
        /// </summary>
        public event Action onPlayerHit;
        /// <summary>
        /// Fired on every movement (except when ghost hits player or temp wall)
        /// </summary>
        public event Action<Vector2Int, Vector2Int> onMove;

        [SerializeField] float timeOffset;
        float frameOffset;
        float totalFrames = 4;
        float frameCounter = 0;
        float nextUpdate = -1;
        MazeGrid grid;
        Vector2 nextPos, lastPos;
        bool isPlaying = false;
        Vector2Int lastPosInt = -Vector2Int.one;
        
        /// <summary>
        /// Initialize the ghost
        /// </summary>
        /// <param name="grid">MazeGrid reference</param>
        public void Init(MazeGrid grid) {
            totalFrames = 32/speed;
            slowSpeed = slowSpeed == 0 ? 2 : slowSpeed;
            this.grid = grid;
            timeOffset = 1/(float)speed;
            frameOffset = timeOffset/totalFrames;
            transform.position = lastPos = nextPos = grid.GetPos(pos);
            isPlaying = true;
            StartCoroutine(MoveCoroutine());
        }

        /// <summary>
        /// Slow down the ghost
        /// </summary>
        public void SlowDown() {
            StopCoroutine(RecoverSpeed());
            totalFrames = 32/speed;
            timeOffset = 1/(float)slowSpeed;
            totalFrames = 32/slowSpeed;
            frameOffset = timeOffset/totalFrames;
            StartCoroutine(RecoverSpeed());
        }

        /// <summary>
        /// Stop chasing player
        /// </summary>
        public void Stop() {
            totalFrames *= 4;
            frameCounter *= 4;
            StopAllCoroutines();
            isPlaying = false;
        }

        void Update() {
            if(frameCounter <= totalFrames && Time.timeSinceLevelLoad > nextUpdate) {
                transform.position = Vector2.Lerp(lastPos, nextPos, frameCounter++/(float)totalFrames);
                nextUpdate = Time.timeSinceLevelLoad + frameOffset;
            }
        }

        IEnumerator RecoverSpeed() {
            yield return new WaitForSeconds(slowDuration);
            timeOffset = 1/(float)speed;
            totalFrames = 32/speed;
        }

        void OnDestroy() {
            StopAllCoroutines();
        }

        IEnumerator MoveCoroutine() {
            while(true) {
                MoveTowardsPlayer();
                yield return new WaitForSeconds(timeOffset);
            }
        }

        void MoveTowardsPlayer() {
            // Player's position is the destination. Get next pos block in that direction (newpx and newpy)
            Vector2Int dst =  grid.GetPlayerPos();
            Vector2Int diff = dst - pos;
            Vector2Int newpx = diff.x == 0 ? pos : new (pos.x + (diff.x > 0 ? 1 : -1), pos.y);
            Vector2Int newpy = diff.y == 0 ? pos : new (pos.x, pos.y + (diff.y > 0 ? 1 : -1));
            Vector2Int newp;
            // There's a vertical next block and horizontal next block in player's direction. Choose based on
            // 1. which one is not covered in wall or outside grid
            // 2. which one is longer distance (if there is not wall/grid edge on either side and player is at distance x:4, y:8, this logic will prefer movement in y direction)
            if(abs(diff.x) > abs(diff.y) && !grid.CheckPos(newpx, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.Ghost))
                newp = newpx;
            else if(newpy != pos && !grid.CheckPos(newpy, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.Ghost))
                newp = newpy;
            else if(lastPosInt.x >= 0 && !grid.CheckPos(lastPosInt, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.Ghost))
                newp = lastPosInt;
            else    // there's wall/grid edge in both directions, don't move
                return;
            lastPosInt = pos;
            pos = newp;
            lastPos = nextPos;
            transform.position = lastPos;
            nextPos = grid.GetPos(pos);
            frameCounter = 1;
            if(grid.CheckPos(pos, MazeGrid.BlockItem.Player | MazeGrid.BlockItem.TmpWall))
                onPlayerHit?.Invoke();
            else
                onMove?.Invoke(pos, lastPosInt);
        }
    }
}