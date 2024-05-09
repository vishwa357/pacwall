namespace pacwall.grid
{
    using System;
    using System.Collections;
    using UnityEngine;
    using static Unity.Mathematics.math;

    public class Ghost : MonoBehaviour {
        [SerializeField] [Range(8, 20)] int speed;
        [SerializeField] [Range(8, 20)] int slowSpeed = 1;
        [SerializeField] [Range(8, 20)] int slowDuration = 3;

        public Vector2Int pos;
        public event Action onPlayerHit;
        public event Action<Vector2Int, Vector2Int> onMove;

        [SerializeField] float timeOffset;
        MazeGrid grid;
        
        public void Init(MazeGrid grid) {
            this.grid = grid;
            timeOffset = 1/(float)speed;
            StartCoroutine(MoveCoroutine());
        }

        public void SlowDown() {
            timeOffset = slowSpeed == 0 ? 0 : 1/(float)slowSpeed;
            StartCoroutine(RecoverSpeed());
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
            Vector2Int oldPos = pos;
            Vector2Int newPos = pos;
            if(abs(diff.x) > abs(diff.y))
                newPos = new (pos.x + (diff.x > 0 ? 1 : -1), pos.y);
            else if(diff.y != 0)
                newPos = new (pos.x, pos.y + (diff.y > 0 ? 1 : -1));
            if(grid.CheckPos(newPos, MazeGrid.BlockItem.Wall | MazeGrid.BlockItem.TmpWall))
                return;
            pos = newPos;
            transform.position = grid.GetPos(pos);
            if(grid.CheckPos(newPos, MazeGrid.BlockItem.Player))
                onPlayerHit?.Invoke();
            else
                onMove?.Invoke(pos, oldPos);
        }
    }
}