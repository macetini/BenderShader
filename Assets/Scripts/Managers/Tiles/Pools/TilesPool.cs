using UnityEngine;
using System.Collections.Generic;
using Managers.Tiles.Items;

namespace Managers.Tiles.Pools
{
    /// <summary>
    /// Pool system that recycles the tile items.
    /// </summary>
    public class TilesPool : MonoBehaviour
    {        
        private List<Tile>[] pool;

        /// <summary>
        /// Initializes the pooling system. Best called in it's owner's <b>Start()</b> method.
        /// </summary>
        /// <param name="tiles">List that contains all tile type prefabs that will be pooled.</param>
        /// <param name="initSize">Number of pre-instantiated pool items. Used so that the first pooled item is not null.</param>
        public void Init(Tile[] tiles, int initSize, GameObject tilesContainer)
        {            
            int numTypes = tiles.Length;

            pool = new List<Tile>[numTypes];

            for (int i = 0; i < numTypes; i++)
            {
                pool[i] = new List<Tile>(initSize);

                for (int j = 0; j < initSize; j++)
                {
                    Tile tile = Instantiate(tiles[i], tilesContainer.transform);

                    tile.gameObject.SetActive(false);

                    pool[i].Add(tile);
                }
            }
        }

        /// <summary>
        /// Recycles the tile in the pool.
        /// </summary>
        /// <param name="type">Type of the tile to be recycled.</param>
        /// <returns>Recycled tile.</returns>
        public Tile GetTile(int type)
        {
            for (int i = 0; i < this.pool[type].Count; i++)
            {
                Tile tile = pool[type][i];

                GameObject tileGO = tile.gameObject;

                if (tileGO.activeInHierarchy) continue;               
                tileGO.SetActive(true);

                return tile;
            }

            throw new System.Exception("ERROR::TilesPool::GetTile:: System can't pool the tile of the type: " + type);
        }

        /// <summary>
        /// Deactivates the tile pooled tile.
        /// </summary>
        /// <param name="tile"></param>
        public void ReleaseTile(GameObject tile)
        {
            tile.SetActive(false);
        }
    }
}