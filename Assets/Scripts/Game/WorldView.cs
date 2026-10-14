using System.Collections.Generic;
using UnityEngine;

namespace Statecraft.Game
{
    /// <summary>
    /// Placeholder map: one primitive cylinder per nation on a ring. Color shows contentment
    /// (#D94436 suffering, #E0C341 neutral, #4CAF50 content); height shows unrest. Reads sim
    /// state only. Norm's Blender art replaces it.
    /// </summary>
    public sealed class WorldView : MonoBehaviour
    {
        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] SimulationHost host;
        [SerializeField] Material markerMaterial;
        [SerializeField] float ringRadius = 5f;
        [SerializeField] float markerWidth = 2.2f;

        [Tooltip("Contentment is multiplied by this before coloring, so small differences stay visible.")]
        [SerializeField] float contentmentColorGain = 2.5f;

        readonly List<Transform> markers = new List<Transform>();
        readonly List<Renderer> renderers = new List<Renderer>();
        MaterialPropertyBlock block;
        Color suffering;
        Color neutral;
        Color content;

        void Awake()
        {
            block = new MaterialPropertyBlock();
            ColorUtility.TryParseHtmlString("#D94436", out suffering);
            ColorUtility.TryParseHtmlString("#E0C341", out neutral);
            ColorUtility.TryParseHtmlString("#4CAF50", out content);
        }

        void OnEnable()
        {
            if (host != null) host.Ticked += Refresh;
        }

        void OnDisable()
        {
            if (host != null) host.Ticked -= Refresh;
        }

        void Start() => Refresh();

        void Refresh()
        {
            if (host == null || host.Simulation == null) return;

            var nations = host.Simulation.World.Nations;
            if (markers.Count != nations.Count) BuildMarkers(nations.Count);

            for (int i = 0; i < nations.Count; i++)
            {
                var society = nations[i].Society;
                float height = 0.3f + 3f * (float)society.Unrest;
                var marker = markers[i];
                marker.localScale = new Vector3(markerWidth, height * 0.5f, markerWidth); // a Unity cylinder is 2 units tall
                marker.localPosition = new Vector3(marker.localPosition.x, height * 0.5f, marker.localPosition.z);

                float k = Mathf.Clamp((float)society.Contentment * contentmentColorGain, -1f, 1f);
                block.SetColor(BaseColorId, k < 0 ? Color.Lerp(neutral, suffering, -k) : Color.Lerp(neutral, content, k));
                renderers[i].SetPropertyBlock(block);
            }
        }

        void BuildMarkers(int count)
        {
            foreach (var marker in markers)
                if (marker != null) Destroy(marker.gameObject);
            markers.Clear();
            renderers.Clear();

            for (int i = 0; i < count; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                go.name = "NationMarker" + i;
                Destroy(go.GetComponent<Collider>());
                go.transform.SetParent(transform, false);
                float angle = Mathf.PI * 2f * i / count + Mathf.PI * 0.5f;
                go.transform.localPosition = new Vector3(Mathf.Cos(angle) * ringRadius, 0f, Mathf.Sin(angle) * ringRadius);

                var markerRenderer = go.GetComponent<Renderer>();
                if (markerMaterial != null) markerRenderer.sharedMaterial = markerMaterial;
                markers.Add(go.transform);
                renderers.Add(markerRenderer);
            }
        }
    }
}
