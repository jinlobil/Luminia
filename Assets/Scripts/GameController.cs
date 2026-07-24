using UnityEngine;
using UnityEngine.UI;

namespace Luminia
{
    public sealed class GameController : MonoBehaviour
    {
        private Canvas canvas;
        private RectTransform screen;

        private readonly Color midnight = new Color32(8, 14, 28, 255);
        private readonly Color parchment = new Color32(224, 205, 155, 255);
        private readonly Color gold = new Color32(194, 145, 55, 255);

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            canvas = UiFactory.CreateCanvas(transform);
            ShowTitle();
            Debug.Log("Luminia prototype started successfully.");
        }

        private void SetScreen(string name)
        {
            UiFactory.Clear(canvas.transform);
            screen = UiFactory.Panel(canvas.transform, name, midnight, Vector2.zero, Vector2.one);
        }

        private void AddUtilityButtons()
        {
            UiFactory.Button(screen, "Logs", "오류 로그 폴더", GameLog.OpenLogFolder,
                new Color32(56, 66, 82, 255), new Vector2(0.01f, 0.015f), new Vector2(0.14f, 0.07f));
            UiFactory.Button(screen, "Quit", "종료", Application.Quit,
                new Color32(90, 45, 45, 255), new Vector2(0.91f, 0.015f), new Vector2(0.985f, 0.07f));
        }

        public void ShowTitle()
        {
            SetScreen("Title Screen");
            var glow = UiFactory.Panel(screen, "Castle Glow", new Color32(25, 36, 58, 255),
                new Vector2(0.17f, 0.15f), new Vector2(0.83f, 0.88f));
            UiFactory.Label(glow, "Crest", "♜", 110, TextAnchor.MiddleCenter, gold,
                new Vector2(0, 0.58f), new Vector2(1, 0.88f));
            UiFactory.Label(glow, "Title", "L U M I N I A", 62, TextAnchor.MiddleCenter, parchment,
                new Vector2(0, 0.43f), new Vector2(1, 0.63f));
            UiFactory.Label(glow, "Subtitle", "무너진 순환 왕정", 28, TextAnchor.MiddleCenter,
                new Color32(173, 180, 192, 255), new Vector2(0, 0.34f), new Vector2(1, 0.45f));
            UiFactory.Button(glow, "Start", "프롤로그 시작", ShowPrologue, new Color32(137, 79, 38, 255),
                new Vector2(0.31f, 0.14f), new Vector2(0.69f, 0.26f));
            AddUtilityButtons();
        }

        private void ShowPrologue()
        {
            SetScreen("Prologue");
            UiFactory.Label(screen, "Chapter", "프롤로그  ·  왕관이 멈춘 날", 34, TextAnchor.MiddleCenter,
                gold, new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.93f));
            var story = "수백 년 동안 왕위는 인간에서 엘프로, 엘프에서 자이언트로,\n" +
                        "자이언트에서 마족으로 이어지며 대륙의 평화를 지켜 왔다.\n\n" +
                        "그러나 인간 왕은 대관식 직후 암살당했다.\n" +
                        "인간은 재선출을, 엘프는 정당한 계승을 주장했다.\n" +
                        "혼합 기사단이 분열된 순간, 마족은 종합지구를 점령했다.\n\n" +
                        "『순환 왕정은 끝났다. 이제 힘 있는 자가 지배한다.』\n\n" +
                        "혼합 기사단의 생존자인 당신은 흩어진 영웅을 모아\n" +
                        "빼앗긴 종합지구를 되찾아야 한다.";
            UiFactory.Label(screen, "Story", story, 25, TextAnchor.MiddleCenter, parchment,
                new Vector2(0.13f, 0.23f), new Vector2(0.87f, 0.81f));
            UiFactory.Button(screen, "Continue", "대륙 지도", ShowWorldMap, new Color32(61, 96, 75, 255),
                new Vector2(0.39f, 0.1f), new Vector2(0.61f, 0.19f));
            AddUtilityButtons();
        }

        public void ShowWorldMap()
        {
            SetScreen("World Map");
            UiFactory.Label(screen, "Header", "에테리아 대륙  ·  해방 작전", 32, TextAnchor.MiddleCenter,
                parchment, new Vector2(0.2f, 0.91f), new Vector2(0.8f, 0.98f));
            Region(new Vector2(0.08f, 0.51f), new Vector2(0.44f, 0.88f), new Color32(94, 72, 50, 255),
                "자이언트 고원", "우리는 우리 땅을 지킨다", null);
            Region(new Vector2(0.56f, 0.51f), new Vector2(0.92f, 0.88f), new Color32(45, 91, 68, 255),
                "엘프 숲", "왕위는 우리 차례였다", null);
            Region(new Vector2(0.56f, 0.12f), new Vector2(0.92f, 0.48f), new Color32(101, 103, 54, 255),
                "인간령", "왕의 원수를 갚아라", () => StartBattle("인간령 외곽", 1));
            Region(new Vector2(0.08f, 0.12f), new Vector2(0.44f, 0.48f), new Color32(83, 35, 47, 255),
                "마족 점령지", "힘 있는 자가 지배한다", null);
            var capital = UiFactory.Button(screen, "Capital", "종합지구\n마족 점령 · 최종 목표", null,
                new Color32(52, 48, 72, 255), new Vector2(0.39f, 0.37f), new Vector2(0.61f, 0.63f));
            capital.interactable = false;
            UiFactory.Label(screen, "Hint", "현재 임무: 인간령 외곽의 저항군을 구출하세요.", 20,
                TextAnchor.MiddleCenter, new Color32(220, 191, 99, 255), new Vector2(0.25f, 0.06f), new Vector2(0.75f, 0.11f));
            AddUtilityButtons();
        }

        private void Region(Vector2 min, Vector2 max, Color color, string title, string motto, UnityEngine.Events.UnityAction action)
        {
            var button = UiFactory.Button(screen, title, title + "\n<size=16>“" + motto + "”</size>",
                action == null ? null : new System.Action(() => action()), color, min, max);
            if (action == null)
            {
                button.interactable = false;
            }
        }

        private void StartBattle(string stageName, int stage)
        {
            SetScreen("Battle");
            var battle = screen.gameObject.AddComponent<BattleController>();
            battle.Build(this, screen, stageName, stage);
        }
    }
}
