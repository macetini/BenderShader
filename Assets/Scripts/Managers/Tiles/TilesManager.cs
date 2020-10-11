using UnityEngine;
using System.Collections.Generic;
using Managers.Tiles.Pools;
using Managers.Tiles.Items;

namespace Managers.Tiles
{
    /// <summary>
    /// <b>TilesManager</b> defines and updates tiles behavior. It references all the tile items and manages pooling and movement speed.
    /// </summary>
    public class TilesManager : MonoBehaviour
    {
        [Tooltip("Maximal number of tiles visible at one time.")]
        public int maxTiles = 6;

        [Tooltip("Maximal possible speed that tile road moves at.")]
        [Range(0f, 50f)]
        public float maxSpeed = 10f;

        [Tooltip("Seed rate of change towards maximal speed.")]
        public float velocity = .05f;

        [Tooltip("Seed value used for random generation of tile objects.")]
        public int seed = 32;

        [Tooltip("Manager's pooling system object.")]
        public TilesPool tilesPool;

        [Tooltip("Collection of Types of Tiles")]
        public Tile[] tiles;

        public GameObject tilesContainer;

        private System.Random random;
        
        private float currentSpeed = 0f;

        /// <summary>
        /// Current active tiles list, all items in this list are visible and in front of player.
        /// </summary>
        private List<GameObject> activeTiles = new List<GameObject>();

        void Awake()
        {
            random = new System.Random(seed);            
        }

        /// <summary>
        /// Initializes the manager system. Best called in owners <b>Start()</b> method.
        /// </summary>
        public void Init()
        {
            tilesPool.Init(tiles, maxTiles, tilesContainer);

            InitTiles();       
        }

        /// <summary>
        /// Initializes the maximum visible number of tiles.
        /// </summary>
        private void InitTiles()
        {
            for (int i = 0; i < maxTiles; i++)
            {
                AddTile(0);
            }
        }        

        /// <summary>
        /// Updates the tiles speed and positions.
        /// </summary>
        public void UpdateTiles()
        {
            IncreaseSpeed(velocity);
            UpdateTilePositions();
        }        
        
        /// <summary>
        /// Only increases speed if it is lower that the maximal speed.
        /// </summary>
        /// <param name="velocity">The value of speed increase.</param>
        protected void IncreaseSpeed(float velocity)
        {
            currentSpeed += velocity;

            if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
        }

        /// <summary>
        /// Translates the tile position based on a rate of change. In a case the position is behind the main camera,
        /// then deactivates (recycles) the tile.
        /// </summary>
        protected void UpdateTilePositions()
        {
            for (int i = activeTiles.Count - 1; i >= 0; i--)
            {
                GameObject tile = activeTiles[i];

                tile.transform.Translate(0f, 0f, -currentSpeed * Time.deltaTime);

                if (tile.transform.position.z < Camera.main.transform.position.z)
                {
                    activeTiles.RemoveAt(i);

                    tilesPool.ReleaseTile(tile);

                    int type = random.Next(0, tiles.Length);

                    AddTile(type);
                }
            }
        }

        /// <summary>
        /// Activates and translates the tile behind the last active one.
        /// </summary>
        /// <param name="index">The index of tile to be activated.</param>
        protected void AddTile(int index)
        {
            Tile tile = tilesPool.GetTile(index);

            GameObject tileGO = tile.gameObject;            
            tileGO.transform.position = transform.position;
            tileGO.transform.rotation = transform.rotation;

            float zPos = activeTiles.Count == 0 ? 0f : activeTiles[activeTiles.Count - 1].transform.position.z + tile.tileSize;
            tile.transform.Translate(0f, 0f, zPos);

            activeTiles.Add(tileGO);
        }        
    }
}