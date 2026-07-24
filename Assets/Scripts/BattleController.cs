using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Luminia
{
    public sealed class BattleController : MonoBehaviour
    {
        private sealed class Unit
        {
            public string Name;
            public int Tier;
            public int Health;
            public int MaxHealth;
            public int Attack;
            public bool Enemy;
            public Text Label;
            public Image Portrait;
        }

        private readonly List<Unit> heroes = new List<Unit>();
        private readonly List<Unit> enemies = new List<Unit>();
        private readonly string[] heroNames = { "수호기사", "숲의 궁수", "견습 치유사", "화염술사" };
        private readonly Color[] heroColors =
        {
            new Color32(74, 112, 146, 255), new Color32(61, 130, 78, 255),
            new Color32(185, 166, 93, 255), new Color32(158, 66, 45, 255)
        };

        private GameController game;
        private RectTransform root;
        private RectTransform heroArea;
        private RectTransform enemyArea;
        private Text status;
        private Text goldLabel;
        private Button summonButton;
        private Button mergeButton;
        private Button fightButton;
        private int gold = 20;
        private bool fighting;

        public void Build(GameController controller, RectTransform parent, string stageName, int stage)
        {
            game = controller;
            root = parent;
            UiFactory.Label(root, "Stage", stageName + "  ·  STAGE " + stage, 30, TextAnchor.MiddleCenter,
                new Color32(232, 210, 151, 255), new Vector2(0.25f, 0.92f), new Vector2(0.75f, 0.985f));
            status = UiFactory.Label(root, "Status", "준비 단계 · 영웅을 소환하고 같은 영웅을 합성하세요.", 20,
                TextAnchor.MiddleCenter, Color.white, new Vector2(0.2f, 0.85f), new Vector2(0.8f, 0.91f));

            enemyArea = UiFactory.Panel(root, "Enemy Field", new Color32(62, 35, 42, 255),
                new Vector2(0.12f, 0.51f), new Vector2(0.88f, 0.84f));
            heroArea = UiFactory.Panel(root, "Hero Field", new Color32(29, 49, 55, 255),
                new Vector2(0.12f, 0.17f), new Vector2(0.88f, 0.49f));
            UiFactory.Label(enemyArea, "Caption", "마족 선봉대", 18, TextAnchor.UpperLeft,
                new Color32(240, 157, 157, 255), new Vector2(0, 0.82f), new Vector2(1, 1));
            UiFactory.Label(heroArea, "Caption", "혼합 기사단", 18, TextAnchor.UpperLeft,
                new Color32(154, 213, 220, 255), new Vector2(0, 0.82f), new Vector2(1, 1));

            goldLabel = UiFactory.Label(root, "Gold", "골드 20", 24, TextAnchor.MiddleLeft,
                new Color32(245, 206, 92, 255), new Vector2(0.12f, 0.08f), new Vector2(0.27f, 0.15f));
            summonButton = UiFactory.Button(root, "Summon", "영웅 소환  5G", Summon,
                new Color32(57, 94, 128, 255), new Vector2(0.29f, 0.075f), new Vector2(0.45f, 0.15f));
            mergeButton = UiFactory.Button(root, "Merge", "같은 영웅 합성", Merge,
                new Color32(101, 77, 128, 255), new Vector2(0.47f, 0.075f), new Vector2(0.63f, 0.15f));
            fightButton = UiFactory.Button(root, "Fight", "전투 시작", StartFight,
                new Color32(151, 67, 43, 255), new Vector2(0.65f, 0.075f), new Vector2(0.82f, 0.15f));
            UiFactory.Button(root, "Back", "지도", game.ShowWorldMap, new Color32(58, 65, 75, 255),
                new Vector2(0.015f, 0.015f), new Vector2(0.09f, 0.07f));
            UiFactory.Button(root, "Logs", "오류 로그", GameLog.OpenLogFolder, new Color32(58, 65, 75, 255),
                new Vector2(0.90f, 0.015f), new Vector2(0.985f, 0.07f));

            AddEnemy("마족 보병", 45, 7, new Color32(120, 51, 70, 255));
            AddEnemy("마족 사수", 32, 9, new Color32(100, 45, 104, 255));
            SummonFree(0);
            SummonFree(1);
            Refresh();
            Debug.Log($"Battle loaded: {stageName}, stage {stage}");
        }

        private void Summon()
        {
            if (fighting || gold < 5 || heroes.Count >= 8) return;
            gold -= 5;
            SummonFree(Random.Range(0, heroNames.Length));
            status.text = "새 영웅이 합류했습니다.";
            Refresh();
        }

        private void SummonFree(int kind)
        {
            var maxHealth = kind == 0 ? 55 : kind == 2 ? 38 : 42;
            var attack = kind == 1 ? 11 : kind == 3 ? 13 : kind == 2 ? 6 : 8;
            heroes.Add(new Unit
            {
                Name = heroNames[kind], Tier = 1, Health = maxHealth, MaxHealth = maxHealth,
                Attack = attack, Enemy = false, Portrait = CreateUnitCard(heroArea, heroNames[kind], heroColors[kind], false)
            });
        }

        private void AddEnemy(string name, int health, int attack, Color color)
        {
            enemies.Add(new Unit
            {
                Name = name, Tier = 1, Health = health, MaxHealth = health, Attack = attack, Enemy = true,
                Portrait = CreateUnitCard(enemyArea, name, color, true)
            });
        }

        private Image CreateUnitCard(RectTransform area, string unitName, Color color, bool enemy)
        {
            var card = UiFactory.Panel(area, unitName, color, new Vector2(0, 0.1f), new Vector2(0.1f, 0.76f));
            var image = card.GetComponent<Image>();
            var pixelArt = UiFactory.Panel(card, "Pixel Art", Color.white,
                new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.94f)).GetComponent<Image>();
            pixelArt.sprite = CreatePixelUnit(enemy);
            pixelArt.preserveAspect = true;
            UiFactory.Label(card, "Info", unitName, 15, TextAnchor.LowerCenter, Color.white,
                new Vector2(0, 0), new Vector2(1, 0.4f));
            image.gameObject.name = unitName;
            return image;
        }

        private static Sprite CreatePixelUnit(bool enemy)
        {
            const int size = 16;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = enemy ? "Demon Pixel" : "Hero Pixel"
            };
            var clear = new Color32(0, 0, 0, 0);
            var outline = new Color32(28, 24, 31, 255);
            var body = enemy ? new Color32(91, 29, 55, 255) : new Color32(202, 214, 220, 255);
            var accent = enemy ? new Color32(235, 66, 75, 255) : new Color32(64, 137, 184, 255);
            var pixels = new Color32[size * size];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;

            void Set(int x, int y, Color32 value)
            {
                if (x >= 0 && x < size && y >= 0 && y < size) pixels[y * size + x] = value;
            }

            for (var y = 4; y <= 11; y++)
            for (var x = 5; x <= 10; x++) Set(x, y, body);
            for (var x = 6; x <= 9; x++) Set(x, 12, body);
            Set(5, 12, outline); Set(10, 12, outline);
            Set(5, 10, outline); Set(10, 10, outline);
            Set(4, 8, outline); Set(11, 8, outline);
            Set(6, 9, accent); Set(9, 9, accent);
            Set(6, 3, outline); Set(9, 3, outline);
            Set(5, 2, outline); Set(10, 2, outline);
            Set(6, 1, enemy ? accent : outline); Set(9, 1, enemy ? accent : outline);
            Set(5, 4, outline); Set(10, 4, outline);
            Set(4, 5, accent); Set(11, 5, accent);
            Set(5, 0, enemy ? accent : clear); Set(10, 0, enemy ? accent : clear);

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
        }

        private void Merge()
        {
            if (fighting) return;
            for (var i = 0; i < heroes.Count; i++)
            {
                for (var j = i + 1; j < heroes.Count; j++)
                {
                    if (heroes[i].Name != heroes[j].Name || heroes[i].Tier != heroes[j].Tier) continue;
                    var first = heroes[i];
                    var second = heroes[j];
                    first.Tier++;
                    first.MaxHealth += 24;
                    first.Health = first.MaxHealth;
                    first.Attack += 6;
                    Destroy(second.Portrait.gameObject);
                    heroes.RemoveAt(j);
                    status.text = first.Name + "이(가) " + first.Tier + "성으로 합성되었습니다!";
                    Refresh();
                    return;
                }
            }
            status.text = "합성할 같은 등급의 영웅이 없습니다.";
        }

        private void StartFight()
        {
            if (fighting || heroes.Count == 0) return;
            fighting = true;
            summonButton.interactable = false;
            mergeButton.interactable = false;
            fightButton.interactable = false;
            status.text = "자동 전투 진행 중...";
            StartCoroutine(FightRoutine());
        }

        private IEnumerator FightRoutine()
        {
            while (Living(heroes) > 0 && Living(enemies) > 0)
            {
                yield return new WaitForSeconds(0.65f);
                AttackRound(heroes, enemies);
                if (Living(enemies) > 0) AttackRound(enemies, heroes);
                Refresh();
            }

            if (Living(heroes) > 0)
            {
                status.text = "승리! 인간 저항군으로 향하는 길을 확보했습니다.";
                gold += 10;
                Debug.Log("Battle result: Victory");
            }
            else
            {
                status.text = "패배 · 지도로 돌아가 다시 준비하세요.";
                Debug.Log("Battle result: Defeat");
            }
            Refresh();
        }

        private static int Living(List<Unit> units)
        {
            var count = 0;
            foreach (var unit in units) if (unit.Health > 0) count++;
            return count;
        }

        private static void AttackRound(List<Unit> attackers, List<Unit> defenders)
        {
            foreach (var attacker in attackers)
            {
                if (attacker.Health <= 0) continue;
                Unit target = null;
                foreach (var defender in defenders)
                {
                    if (defender.Health > 0) { target = defender; break; }
                }
                if (target == null) return;
                target.Health = Mathf.Max(0, target.Health - attacker.Attack);
            }
        }

        private void Refresh()
        {
            goldLabel.text = "골드 " + gold;
            summonButton.interactable = !fighting && gold >= 5 && heroes.Count < 8;
            mergeButton.interactable = !fighting;
            PositionUnits(heroes);
            PositionUnits(enemies);
        }

        private static void PositionUnits(List<Unit> units)
        {
            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.Portrait == null) continue;
                var rect = unit.Portrait.rectTransform;
                var width = 0.105f;
                var gap = 0.012f;
                var start = 0.5f - (units.Count * width + (units.Count - 1) * gap) * 0.5f;
                rect.anchorMin = new Vector2(start + i * (width + gap), 0.1f);
                rect.anchorMax = new Vector2(start + i * (width + gap) + width, 0.76f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.color = unit.Health > 0 ? rect.color : new Color32(45, 45, 45, 180);
                var label = rect.transform.Find("Info").GetComponent<Text>();
                label.text = unit.Name + "  " + new string('★', unit.Tier) + "\nHP " + unit.Health + "/" + unit.MaxHealth;
            }
        }
    }
}
