using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.UV
{
    public class UVItem : MonoBehaviour
    {
        public float itemSize;        
        public List<MeshRenderer> renderers = new();

        [ContextMenu("Assign Renderers")]
        public void AssignRenderers()
        {
            renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
            if (renderers.Count == 0)
            {
                Debug.LogWarning($"UVItem on '{gameObject.name}' has no renderers assigned. Size remains 0.", this);
            }
        }

        // --- Item Size Calculation ---
        [ContextMenu("Calculate Item Size")]
        public void CalculateItemSize()
        {
            AssignRenderers();

            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Count; i++)
            {
                if (renderers[i] != null && renderers[i].enabled)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }
            }

            // Calculate the size in the item's local space (relative to the UVItem root)
            // Get the largest local dimension for the item size/spacing            
            Vector3 sizeInLocalSpace = transform.InverseTransformVector(combinedBounds.size);

            itemSize = Mathf.Max(Mathf.Abs(sizeInLocalSpace.x), Mathf.Abs(sizeInLocalSpace.y), Mathf.Abs(sizeInLocalSpace.z));
        }
    }
}