using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.Tiles.Items
{
    /// <summary>
    /// Tile class describes item behavior.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        /// <summary>
        /// Cube size of a tile item.
        /// </summary>
        public float tileSize;

        /// <summary>
        /// Color value of a item's Gizmo visualizer.
        /// </summary>
        public Color gizmoColor = Color.cyan;

        private void OnDrawGizmos()
        {
            Vector3 gizmoPos = transform.position - new Vector3(0f, 0f, tileSize * 0.5f);
            Vector3 gizmoSize = new Vector3(tileSize, tileSize, tileSize);

            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(gizmoPos, gizmoSize);
        }
    }
}
