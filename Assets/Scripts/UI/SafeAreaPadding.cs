using UnityEngine;
using UnityEngine.UIElements;

namespace Statecraft.UI
{
    /// <summary>
    /// Pads a full-screen UI Toolkit element so content avoids notches and rounded corners
    /// (Screen.safeArea). Recomputes only when the screen, safe area or panel size changes.
    /// </summary>
    public sealed class SafeAreaPadding
    {
        Rect lastSafeArea;
        Vector2Int lastScreen;
        Vector2 lastPanelSize;

        public void Apply(VisualElement element)
        {
            if (element == null || element.panel == null) return;

            Rect safe = Screen.safeArea;
            var screen = new Vector2Int(Screen.width, Screen.height);
            Vector2 panelSize = element.panel.visualTree.layout.size;
            if (screen.x <= 0 || screen.y <= 0 || float.IsNaN(panelSize.x) || panelSize.x <= 0) return;
            if (safe == lastSafeArea && screen == lastScreen && panelSize == lastPanelSize) return;

            lastSafeArea = safe;
            lastScreen = screen;
            lastPanelSize = panelSize;

            float scaleX = panelSize.x / screen.x;
            float scaleY = panelSize.y / screen.y;
            element.style.paddingLeft = safe.xMin * scaleX;
            element.style.paddingRight = (screen.x - safe.xMax) * scaleX;
            element.style.paddingTop = (screen.y - safe.yMax) * scaleY; // Screen.safeArea's origin is bottom-left
            element.style.paddingBottom = safe.yMin * scaleY;
        }
    }
}
