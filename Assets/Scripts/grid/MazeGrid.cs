namespace pacwall.grid
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class MazeGrid : MonoBehaviour {
        [SerializeField] SpriteRenderer bg;
        [SerializeField] Transform wallPrefab;
        [SerializeField] TmpWallItem tmpWallPrefab;
        [SerializeField] Transform debug0, debug1;

        [SerializeField] public Vector2Int size;
        [SerializeField] public Vector2 scale;

        Vector2 zero;
        List<TmpWallItem> tmpWalls = new List<TmpWallItem>();
        List<Transform> walls = new List<Transform>();
        int[,] poss;
        Vector2Int lpos;    // last player pos

        public static class BlockItem {
            public const int None = 1,
                    Player = 2,
                    TmpWall = 4,
                    Wall = 8,
                    Ghost = 16,
                    PowerUp = 32;
        }

        public void BuildGrid(int wn, Vector2 bl, Vector2 tr) {
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

            poss = new int[wn, hn];
            scale = offset;
            tmpWallPrefab.transform.localScale = offset;
            wallPrefab.localScale = offset;
        }

        public bool CheckPos(Vector2Int pos, int itemBits) {
            return (poss[pos.x, pos.y] & itemBits) > 0;
        }

        public Vector2 GetPos(Vector2Int pos) {
            return new Vector2(scale.x * pos.x, scale.y * pos.y) + zero;
        }

        public Vector2Int GetPlayerPos() {
            return lpos;
        }

        bool CheckGhost(Vector2Int pos, ref HashSet<int> set) {
            if(pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y)
                return false;
            if(set.Contains(pos.x*64 + pos.y))
                return false;
            if((poss[pos.x, pos.y] & BlockItem.Ghost) > 0)
                return true;
            if((poss[pos.x, pos.y] & BlockItem.Wall) > 0)
                return false;
            set.Add(pos.x*64 + pos.y);  // assuming grid size is less than 64
            return CheckGhost(new Vector2Int(pos.x-1, pos.y), ref set)
                || CheckGhost(new Vector2Int(pos.x-1, pos.y+1), ref set)
                || CheckGhost(new Vector2Int(pos.x, pos.y+1), ref set)
                || CheckGhost(new Vector2Int(pos.x+1, pos.y+1), ref set)
                || CheckGhost(new Vector2Int(pos.x+1, pos.y), ref set)
                || CheckGhost(new Vector2Int(pos.x+1, pos.y-1), ref set)
                || CheckGhost(new Vector2Int(pos.x, pos.y-1), ref set)
                || CheckGhost(new Vector2Int(pos.x-1, pos.y-1), ref set);
        }

        public void UpdateGhostPos(Vector2Int pos, Vector2Int oldPos) {
            if(oldPos.x >= 0)
                poss[oldPos.x, oldPos.y] = poss[oldPos.x, oldPos.y] ^ BlockItem.Ghost;
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.Ghost;
        }

        [SerializeField][TextArea(20, 20)] string dTxt;

        public void UpdatePlayerPos(Vector2Int pos) {
            poss[lpos.x, lpos.y] = poss[lpos.x, lpos.y] ^ BlockItem.Player;
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.Player;
            AddTmpWall(lpos);

            if(CheckWall(pos)) {
                AddTmpWall(pos);
                TmpWall2Wall();
                HashSet<int> set = new HashSet<int>();
                var t = GetRight(pos, lpos);
                var b = CheckGhost(t, ref set);
                if(!b)
                    FloodFillWithWall(t);
                set.Clear();
                t = GetLeft(pos, lpos);
                b = CheckGhost(t, ref set);
                if(!b)
                    FloodFillWithWall(t);
            }

            lpos = pos;

            StringBuilder sb = new StringBuilder(size.x*size.y + size.x);
            for(int i=0; i<size.y; i++) {
                for(int j=0; j<size.x; j++)
                    sb.Append(" " + (poss[j, size.y-i-1] & BlockItem.TmpWall).ToString("##"));
                sb.Append("\n");
            }
            dTxt = sb.ToString();
        }

        public void AddPowerUpPos(Vector2Int pos) {
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.PowerUp;
        }

        public void RemovePowerUpPos(Vector2Int pos) {
            poss[pos.x, pos.y] = poss[pos.x, pos.y] & (BlockItem.PowerUp ^ int.MaxValue);
        }

        void AddTmpWall(Vector2Int pos) {
            if(pos.x < 0)
                return;
            if((poss[pos.x, pos.y] & (BlockItem.TmpWall | BlockItem.Wall)) > 0)
                return;

            var t = Instantiate(tmpWallPrefab, tmpWallPrefab.transform.parent);
            t.pos = pos;
            t.transform.position = GetPos(pos);
            tmpWalls.Add(t);
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.TmpWall;
        }

        bool CheckWall(Vector2Int pos) {
            Vector2Int wpos = pos;
            if(pos.x == 0 || pos.x == size.x-1 || pos.y == 0 || pos.y == size.y-1)
                return true;
            if((poss[pos.x, pos.y] & BlockItem.Wall) > 0)
                return true;
            if((poss[pos.x-1, pos.y] & BlockItem.Wall) > 0 && pos.x-1 != lpos.x && pos.y != lpos.y)
                return true;
            if((poss[pos.x-1, pos.y-1] & BlockItem.Wall) > 0 && pos.x-1 != lpos.x && pos.y-1 != lpos.y)
                return true;
            if((poss[pos.x, pos.y-1] & BlockItem.Wall) > 0 && pos.x != lpos.x && pos.y-1 != lpos.y)
                return true;
            if((poss[pos.x+1, pos.y-1] & BlockItem.Wall) > 0 && pos.x+1 != lpos.x && pos.y-1 != lpos.y)
                return true;
            if((poss[pos.x+1, pos.y] & BlockItem.Wall) > 0 && pos.x+1 != lpos.x && pos.y != lpos.y)
                return true;
            if((poss[pos.x+1, pos.y+1] & BlockItem.Wall) > 0 && pos.x+1 != lpos.x && pos.y+1 != lpos.y)
                return true;
            if((poss[pos.x, pos.y+1] & BlockItem.Wall) > 0 && pos.x != lpos.x && pos.y+1 != lpos.y)
                return true;
            if((poss[pos.x-1, pos.y+1] & BlockItem.Wall) > 0 && pos.x-1 != lpos.x && pos.y+1 != lpos.y)
                return true;

            return false;
        }

        Vector2Int GetLeft(Vector2Int pos, Vector2Int prev) {
            if(pos.x == prev.x)
                return new Vector2Int(pos.x - (pos.y - prev.y), pos.y);
            else
                return new Vector2Int(pos.x, pos.y + (pos.x - prev.x));
        }

        Vector2Int GetRight(Vector2Int pos, Vector2Int prev) {
            if(pos.x == prev.x)
                return new Vector2Int(pos.x + (pos.y - prev.y), pos.y);
            else
                return new Vector2Int(pos.x, pos.y - (pos.x - prev.x));
        }

        void FloodFillWithWall(Vector2Int pos) {
            if(pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y
                    || (poss[pos.x, pos.y] & BlockItem.Wall) > 0)
                return;
            Transform w = Instantiate(wallPrefab, wallPrefab.parent);
            w.position = GetPos(pos);
            walls.Add(w);
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.Wall;
            FloodFillWithWall(new Vector2Int(pos.x-1, pos.y));
            FloodFillWithWall(new Vector2Int(pos.x-1, pos.y+1));
            FloodFillWithWall(new Vector2Int(pos.x, pos.y+1));
            FloodFillWithWall(new Vector2Int(pos.x+1, pos.y+1));
            FloodFillWithWall(new Vector2Int(pos.x+1, pos.y));
            FloodFillWithWall(new Vector2Int(pos.x+1, pos.y-1));
            FloodFillWithWall(new Vector2Int(pos.x, pos.y-1));
            FloodFillWithWall(new Vector2Int(pos.x-1, pos.y-1));
        }

        void TmpWall2Wall() {
            for(int i = 0; i < tmpWalls.Count; i++) {
                Vector2Int v = tmpWalls[i].pos;
                if((poss[v.x, v.y] & BlockItem.Wall) == 0) {
                    var w = Instantiate(wallPrefab, wallPrefab.parent);
                    w.position = tmpWalls[i].transform.position;
                    walls.Add(w);
                }
                if((poss[v.x, v.y] & BlockItem.TmpWall) > 0)
                    poss[v.x, v.y] = poss[v.x, v.y] ^ BlockItem.TmpWall;
                poss[v.x, v.y] = poss[v.x, v.y] | BlockItem.Wall;
                Destroy(tmpWalls[i].gameObject);
            }
            tmpWalls.Clear();
        }
    }
}