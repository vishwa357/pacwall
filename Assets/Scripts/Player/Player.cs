namespace pacwall.player
{
    using System;
    using System.Collections;
    using pacwall.grid;
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        MazeGrid grid;

        [SerializeField] Vector2Int mid;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] [Range(8, 20)] int speed = 14;
        [SerializeField] SpriteRenderer sr;

        public event Action<Vector2Int> onPlayerMove;

        public enum MoveDirection {
            None,
            Left,
            Up,
            Right,
            Down
        }

        MoveDirection lastMove, nextMove;
        float moveOffset;
        [SerializeField] public Vector2Int pos;

        public void Init(MazeGrid grid) {
            this.grid = grid;
            transform.localScale = grid.scale;
            mid = grid.size/2;
            pos = Vector2Int.zero;
            transform.localPosition = grid.GetPos(pos);
            moveOffset = 1/(float)speed;

            playerInput.onLeft += OnLeft;
            playerInput.onUp += OnUp;
            playerInput.onRight += OnRright;
            playerInput.onDown += OnDown;

            StartCoroutine(MoveCoroutine());
        }

        IEnumerator MoveCoroutine() {
            while(true) {
                UpdatePos();
                yield return new WaitForSeconds(moveOffset);
            }
        }

        void UpdatePos() {
            switch(nextMove) {
                case MoveDirection.Left:
                    if(pos.x == 0)
                        return;
                    sr.transform.localEulerAngles = new Vector3(0, 0, 180);
                    pos.x--;
                    break;
                case MoveDirection.Up:
                    if(pos.y == grid.size.y-1)
                        return;
                    sr.transform.localEulerAngles = new Vector3(0, 0, 90);
                    pos.y++;
                    break;
                case MoveDirection.Right:
                    if(pos.x == grid.size.x-1)
                        return;
                    sr.transform.localEulerAngles = new Vector3(0, 0, 0);
                    pos.x++;
                    break;
                case MoveDirection.Down:
                    if(pos.y == 0)
                        return;
                    sr.transform.localEulerAngles = new Vector3(0, 0, -90);
                    pos.y--;
                    break;
                default:
                    return;
            }
            transform.localPosition = grid.GetPos(pos);
            lastMove = nextMove;
            nextMove = MoveDirection.None;
            onPlayerMove?.Invoke(pos);
        }

        void OnLeft() {
            nextMove = MoveDirection.Left;
        }

        void OnUp() {
            nextMove = MoveDirection.Up;
        }

        void OnRright() {
            nextMove = MoveDirection.Right;
        }

        void OnDown() {
            nextMove = MoveDirection.Down;
        }
    }
}