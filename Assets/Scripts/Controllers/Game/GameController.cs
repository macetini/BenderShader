using UnityEngine;
using System.Collections;
using Managers.Tiles;

namespace Controllers.Game
{
    /// <summary>
    /// Central Game object that defines game setting.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Tooltip("Tile manager that is continuously updated by the main game loop.")]     
        public TilesManager tilesManager;        

        void Start()
        {
            tilesManager.Init();
        }

        void Update()
        {
            tilesManager.UpdateTiles();
        }
    }
}