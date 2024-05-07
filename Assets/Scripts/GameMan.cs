namespace pacwall
{
    using pacwall.grid;
    using UnityEngine;

    public class GameMan : MonoBehaviour
    {
        [SerializeField] RectTransform gridRef;
        [SerializeField] MazeGrid grid;
        [SerializeField] Player player;

        void Start()
        {
            Vector3[] corners = new Vector3[4];
            gridRef.GetWorldCorners(corners);
            grid.BuildGrid(22, corners[0], corners[2]);
            player.Init(grid);
        }
    }
}