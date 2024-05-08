namespace pacwall.grid
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEditor.Callbacks;
    using UnityEngine;
    using UnityEngine.Rendering;

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
                    Wall = 8;
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

        public Vector2 GetPos(Vector2Int pos) {
            return new Vector2(scale.x * pos.x, scale.y * pos.y) + zero;
        }

        [SerializeField][TextArea(20, 20)] string dTxt;

        public void AddPlayerPos(Vector2Int pos) {
            poss[lpos.x, lpos.y] = poss[lpos.x, lpos.y] ^ BlockItem.Player;
            poss[pos.x, pos.y] = poss[pos.x, pos.y] | BlockItem.Player;
            AddTmpWall(lpos);

            if(CheckWall(pos)) {
                AddTmpWall(pos);
                TmpWall2Wall();
            }

            lpos = pos;

            StringBuilder sb = new StringBuilder(size.x*size.y + size.x);
            for(int i=0; i<size.y; i++) {
                for(int j=0; j<size.x; j++)
                    sb.Append(" " + (poss[j, size.y-i-1] & BlockItem.TmpWall).ToString());
                sb.Append("\n");
            }
            dTxt = sb.ToString();
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

        void TmpWall2Wall() {
            for(int i = 0; i < tmpWalls.Count; i++) {
                Vector2Int v = tmpWalls[i].pos;
                if((poss[v.x, v.y] & BlockItem.Wall) == 0) {
                    var w = Instantiate(wallPrefab, wallPrefab.parent);
                    w.position = tmpWalls[i].transform.position;
                    walls.Add(w);
                }
                poss[v.x, v.y] = poss[v.x, v.y] | BlockItem.Wall;
                Destroy(tmpWalls[i].gameObject);
            }
            tmpWalls.Clear();
        }
    }
}