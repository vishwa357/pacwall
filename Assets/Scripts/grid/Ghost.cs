namespace pacwall.grid
{
    using System;
    using System.Collections;
    using UnityEngine;
    using static Unity.Mathematics.math;

    public class Ghost : MonoBehaviour {
        [SerializeField] [Range(5, 16)] int speed;
        [SerializeField] [Range(0, 4)] int slowSpeed = 1;
        [SerializeField] [Range(1, 5)] int slowDuration = 3;

        public Vector2Int pos;
        public event Action onPlayerHit;
        public event Action<Vector2Int, Vector2Int> onMove;

        [SerializeField] float timeOffset;
        float frameOffset;
        float totalFrames = 4;
        float frameCounter = 0;
        float nextUpdate = -1;
        MazeGrid grid;
        Vector2 nextPos, lastPos;
        bool isPlaying = false;
        
        public void Init(MazeGrid grid) {
            this.grid = grid;
            timeOffset = 1/(float)speed;
            frameOffset = timeOffset/totalFrames;
            transform.position = lastPos = nextPos = grid.GetPos(pos);
            isPlaying = true;
            StartCoroutine(MoveCoroutine());
        }

        public void SlowDown() {
            StopCoroutine(RecoverSpeed());
            timeOffset = slowSpeed == 0 ? 2 : 1/(float)slowSpeed;
            frameOffset = timeOffset/totalFrames;
            StartCoroutine(RecoverSpeed());
        }

        public void Stop() {
            StopAllCoroutines();
            isPlaying = false;
        }

        void Update() {
            if(isPlaying && Time.timeSinceLevelLoad > nextUpdate) {
                transform.position = Vector2.Lerp(lastPos, nextPos, frameCounter++/(float)totalFrames);
                nextUpdate = Time.timeSinceLevelLoad + frameOffset;
            }
        }

        IEnumerator RecoverSpeed() {
            yield return new WaitForSeconds(slowDuration);
            timeOffset = 1/(float)speed;
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
            Vector2Int dst =  grid.GetPlayerPos();
            Vector2Int diff = dst - pos;
            Vector2Int newpx = diff.x == 0 ? pos : new (pos.x + (diff.x > 0 ? 1 : -1), pos.y);
            Vector2Int newpy = diff.y == 0 ? pos : new (pos.x, pos.y + (diff.y > 0 ? 1 : -1));
            Vector2Int newp;
            if(abs(diff.x) > abs(diff.y) && !grid.CheckPos(newpx, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.Ghost))
                newp = newpx;
            else if(newpy != pos && !grid.CheckPos(newpy, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.Ghost))
                newp = newpy;
            else
                return;
            Vector2Int oldPos = pos;
            pos = newp;
            lastPos = nextPos;
            transform.position = lastPos;
            nextPos = grid.GetPos(pos);
            frameCounter = 1;
            if(grid.CheckPos(pos, MazeGrid.BlockItem.Player | MazeGrid.BlockItem.TmpWall))
                onPlayerHit?.Invoke();
            else
                onMove?.Invoke(pos, oldPos);
        }
    }
}