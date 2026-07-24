using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Luminia
{
    public static class UiFactory
    {
        private static Font cachedFont;

        public static Font Font
        {
            get
            {
                if (cachedFont == null)
                {
                    var fontNames = new[] { "Malgun Gothic", "맑은 고딕", "Arial Unicode MS", "Arial" };
                    foreach (var fontName in fontNames)
                    {
                        cachedFont = Font.CreateDynamicFontFromOSFont(fontName, 20);
                        if (cachedFont != null)
                        {
                            break;
                        }
                    }
                    if (cachedFont == null)
                    {
                        cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    }
                }
                return cachedFont;
            }
        }

        public static Canvas CreateCanvas(Transform parent)
        {
            var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.matchWidthOrHeight = 0.5f;

            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var eventObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventObject.GetComponent<EventSystem>().sendNavigationEvents = false;
            }
            return canvas;
        }

        public static RectTransform Panel(Transform parent, string name, Color color, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return rect;
        }

        public static Text Label(Transform parent, string name, string value, int size, TextAnchor anchor,
            Color color, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = new Vector2(8, 4);
            rect.offsetMax = new Vector2(-8, -4);
            var text = go.GetComponent<Text>();
            text.font = Font;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = color;
            text.text = value;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Button Button(Transform parent, string name, string value, Action onClick,
            Color color, Vector2 min, Vector2 max)
        {
            var rect = Panel(parent, name, color, min, max);
            var button = rect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.25f);
            button.colors = colors;
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }
            Label(rect, "Text", value, 22, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one);
            return button;
        }

        public static void Clear(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
