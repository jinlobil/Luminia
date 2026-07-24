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
            public Color BaseColor;
            public Image Portrait;
            public int AttackCount;
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
            CreateBattleGrid(enemyArea);
            CreateBattleGrid(heroArea);
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

        private static void CreateBattleGrid(RectTransform area)
        {
            const int columns = 8;
            const int rows = 2;
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    var min = new Vector2(column / (float)columns, row / (float)rows);
                    var max = new Vector2((column + 1) / (float)columns, (row + 1) / (float)rows);
                    var cell = UiFactory.Panel(area, "Grid " + column + "-" + row,
                        new Color32(255, 255, 255, 10), min, max);
                    cell.offsetMin = new Vector2(2, 2);
                    cell.offsetMax = new Vector2(-2, -2);
                    cell.GetComponent<Image>().raycastTarget = false;
                }
            }
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
                Attack = attack, Enemy = false, BaseColor = heroColors[kind], AttackCount = 0,
                Portrait = CreateUnitCard(heroArea, heroNames[kind], heroColors[kind], false)
            });
        }

        private void AddEnemy(string name, int health, int attack, Color color)
        {
            enemies.Add(new Unit
            {
                Name = name, Tier = 1, Health = health, MaxHealth = health, Attack = attack, Enemy = true,
                BaseColor = color, AttackCount = 0,
                Portrait = CreateUnitCard(enemyArea, name, color, true)
            });
        }

        private Image CreateUnitCard(RectTransform area, string unitName, Color color, bool enemy)
        {
            var card = UiFactory.Panel(area, unitName, color, new Vector2(0, 0.1f), new Vector2(0.1f, 0.76f));
            var image = card.GetComponent<Image>();
            var pixelArt = UiFactory.Panel(card, "Pixel Art", Color.white,
                new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.94f)).GetComponent<Image>();
            pixelArt.sprite = CreatePixelUnit(unitName, enemy);
            pixelArt.preserveAspect = true;
            UiFactory.Label(card, "Info", unitName, 15, TextAnchor.LowerCenter, Color.white,
                new Vector2(0, 0.06f), new Vector2(1, 0.36f));
            var healthBar = UiFactory.Panel(card, "HealthBar", new Color32(28, 31, 35, 255),
                new Vector2(0.08f, 0.01f), new Vector2(0.92f, 0.08f));
            UiFactory.Panel(healthBar, "Fill", new Color32(47, 218, 91, 255), Vector2.zero, Vector2.one);
            image.gameObject.name = unitName;
            return image;
        }

        private static Sprite CreatePixelUnit(string unitName, bool enemy)
        {
            const int size = 24;
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

            for (var y = 7; y <= 18; y++)
            for (var x = 8; x <= 15; x++) SetPixel(pixels, size, x, y, body);
            for (var x = 9; x <= 14; x++) SetPixel(pixels, size, x, 19, outline);
            for (var x = 9; x <= 14; x++) SetPixel(pixels, size, x, 20, body);
            SetPixel(pixels, size, 8, 17, outline); SetPixel(pixels, size, 15, 17, outline);
            SetPixel(pixels, size, 7, 13, outline); SetPixel(pixels, size, 16, 13, outline);
            SetPixel(pixels, size, 10, 12, accent); SetPixel(pixels, size, 13, 12, accent);
            for (var x = 9; x <= 14; x++) SetPixel(pixels, size, x, 6, outline);
            for (var x = 10; x <= 13; x++) SetPixel(pixels, size, x, 5, body);
            SetPixel(pixels, size, 9, 4, outline); SetPixel(pixels, size, 14, 4, outline);

            if (enemy)
            {
                SetPixel(pixels, size, 8, 3, accent); SetPixel(pixels, size, 15, 3, accent);
                SetPixel(pixels, size, 7, 2, accent); SetPixel(pixels, size, 16, 2, accent);
                for (var y = 9; y <= 18; y++) SetPixel(pixels, size, 18, y, outline);
                SetPixel(pixels, size, 19, 8, accent); SetPixel(pixels, size, 20, 7, accent);
            }
            else if (unitName.Contains("수호"))
            {
                for (var y = 10; y <= 18; y++)
                for (var x = 3; x <= 7; x++) SetPixel(pixels, size, x, y, accent);
                for (var y = 6; y <= 18; y++) SetPixel(pixels, size, 18, y, outline);
                SetPixel(pixels, size, 19, 5, outline);
            }
            else if (unitName.Contains("궁수"))
            {
                for (var y = 7; y <= 19; y++) SetPixel(pixels, size, 18, y, outline);
                SetPixel(pixels, size, 19, 8, accent); SetPixel(pixels, size, 20, 10, accent);
                SetPixel(pixels, size, 20, 16, accent); SetPixel(pixels, size, 19, 18, accent);
            }
            else if (unitName.Contains("치유"))
            {
                for (var y = 5; y <= 20; y++) SetPixel(pixels, size, 18, y, accent);
                for (var x = 16; x <= 20; x++) SetPixel(pixels, size, x, 7, accent);
            }
            else
            {
                for (var y = 7; y <= 20; y++) SetPixel(pixels, size, 18, y, outline);
                SetPixel(pixels, size, 17, 5, accent); SetPixel(pixels, size, 18, 4, accent);
                SetPixel(pixels, size, 19, 5, accent); SetPixel(pixels, size, 18, 6, accent);
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 16);
        }

        private static void SetPixel(Color32[] pixels, int size, int x, int y, Color32 value)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
            {
                pixels[y * size + x] = value;
            }
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
                yield return StartCoroutine(TeamTurn(heroes, enemies));
                if (Living(enemies) > 0)
                {
                    yield return StartCoroutine(TeamTurn(enemies, heroes));
                }
                Refresh();
                yield return new WaitForSeconds(0.25f);
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

        private IEnumerator TeamTurn(List<Unit> attackers, List<Unit> defenders)
        {
            foreach (var attacker in attackers)
            {
                if (attacker.Health <= 0) continue;

                if (!attacker.Enemy && attacker.Name.Contains("치유"))
                {
                    var wounded = MostWoundedHero();
                    if (wounded != null && wounded.Health < wounded.MaxHealth)
                    {
                        yield return StartCoroutine(HealAnimation(attacker, wounded));
                        continue;
                    }
                }

                var target = FirstLiving(defenders);
                if (target == null) yield break;
                yield return StartCoroutine(AttackAnimation(attacker, target, defenders));
                if (Living(defenders) == 0) yield break;
            }
        }

        private IEnumerator AttackAnimation(Unit attacker, Unit target, List<Unit> defenders)
        {
            attacker.AttackCount++;
            var usesSkill = attacker.AttackCount % 3 == 0;
            var isRanged = attacker.Enemy && attacker.Name.Contains("사수") ||
                           attacker.Name.Contains("궁수") || attacker.Name.Contains("화염");
            var damage = usesSkill ? Mathf.RoundToInt(attacker.Attack * 1.6f) : attacker.Attack;
            status.text = usesSkill ? attacker.Name + " 스킬 발동!" : attacker.Name + " 공격";

            if (isRanged)
            {
                yield return StartCoroutine(ProjectileAnimation(attacker, target,
                    attacker.Name.Contains("화염") ? new Color32(255, 92, 35, 255) : new Color32(235, 222, 145, 255),
                    usesSkill ? 22 : 12));
            }
            else
            {
                yield return StartCoroutine(LungeAnimation(attacker, target));
            }

            if (usesSkill && attacker.Name.Contains("화염"))
            {
                foreach (var defender in defenders)
                {
                    if (defender.Health <= 0) continue;
                    ApplyDamage(defender, damage);
                    StartCoroutine(FloatingNumber(defender, damage, new Color32(255, 128, 48, 255)));
                }
            }
            else
            {
                ApplyDamage(target, damage);
                StartCoroutine(FloatingNumber(target, damage, usesSkill ? Color.yellow : Color.white));
            }

            yield return StartCoroutine(HitFlash(target));
            Refresh();
        }

        private IEnumerator LungeAnimation(Unit attacker, Unit target)
        {
            var transform = attacker.Portrait.rectTransform;
            var start = transform.position;
            var destination = Vector3.Lerp(start, target.Portrait.rectTransform.position, 0.32f);
            yield return StartCoroutine(MoveWorld(transform, start, destination, 0.13f));
            yield return StartCoroutine(MoveWorld(transform, destination, start, 0.13f));
        }

        private IEnumerator ProjectileAnimation(Unit attacker, Unit target, Color color, float size)
        {
            var projectile = UiFactory.Panel(root, "Projectile", color, Vector2.zero, Vector2.zero);
            projectile.sizeDelta = new Vector2(size, size);
            projectile.position = attacker.Portrait.rectTransform.position;
            var start = projectile.position;
            var destination = target.Portrait.rectTransform.position;
            yield return StartCoroutine(MoveWorld(projectile, start, destination, 0.28f));
            Destroy(projectile.gameObject);
        }

        private static IEnumerator MoveWorld(RectTransform moving, Vector3 start, Vector3 destination, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                moving.position = Vector3.Lerp(start, destination, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }
            moving.position = destination;
        }

        private IEnumerator HitFlash(Unit target)
        {
            if (target.Portrait == null) yield break;
            target.Portrait.color = new Color32(255, 70, 70, 255);
            var transform = target.Portrait.rectTransform;
            var start = transform.localRotation;
            transform.localRotation = Quaternion.Euler(0, 0, -6);
            yield return new WaitForSeconds(0.10f);
            transform.localRotation = Quaternion.Euler(0, 0, 6);
            yield return new WaitForSeconds(0.10f);
            transform.localRotation = start;
        }

        private IEnumerator HealAnimation(Unit healer, Unit target)
        {
            healer.AttackCount++;
            var amount = healer.Attack + 8;
            status.text = healer.Name + "의 회복 마법!";
            yield return StartCoroutine(ProjectileAnimation(healer, target, new Color32(72, 255, 139, 255), 16));
            target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);
            StartCoroutine(FloatingNumber(target, amount, new Color32(72, 255, 139, 255), true));
            Refresh();
            yield return new WaitForSeconds(0.15f);
        }

        private IEnumerator FloatingNumber(Unit target, int amount, Color color, bool healing = false)
        {
            var text = UiFactory.Label(root, "Combat Number", (healing ? "+" : "-") + amount, 28,
                TextAnchor.MiddleCenter, color, Vector2.zero, Vector2.zero);
            text.rectTransform.sizeDelta = new Vector2(90, 42);
            text.rectTransform.position = target.Portrait.rectTransform.position + Vector3.up * 35;
            var start = text.rectTransform.position;
            var elapsed = 0f;
            while (elapsed < 0.65f)
            {
                elapsed += Time.deltaTime;
                text.rectTransform.position = start + Vector3.up * (elapsed * 55);
                text.color = new Color(color.r, color.g, color.b, 1f - elapsed / 0.65f);
                yield return null;
            }
            Destroy(text.gameObject);
        }

        private Unit MostWoundedHero()
        {
            Unit result = null;
            var lowestRatio = 1.01f;
            foreach (var hero in heroes)
            {
                if (hero.Health <= 0) continue;
                var ratio = (float)hero.Health / hero.MaxHealth;
                if (ratio < lowestRatio) { lowestRatio = ratio; result = hero; }
            }
            return result;
        }

        private static Unit FirstLiving(List<Unit> units)
        {
            foreach (var unit in units) if (unit.Health > 0) return unit;
            return null;
        }

        private static void ApplyDamage(Unit target, int damage)
        {
            target.Health = Mathf.Max(0, target.Health - damage);
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
                unit.Portrait.color = unit.Health > 0
                    ? unit.BaseColor
                    : new Color32(45, 45, 45, 180);
                var canvasGroup = unit.Portrait.GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = unit.Portrait.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = unit.Health > 0 ? 1f : 0.3f;
                var fill = rect.transform.Find("HealthBar/Fill") as RectTransform;
                if (fill != null)
                {
                    fill.anchorMax = new Vector2(Mathf.Clamp01((float)unit.Health / unit.MaxHealth), 1);
                    fill.offsetMax = Vector2.zero;
                }
                var label = rect.transform.Find("Info").GetComponent<Text>();
                label.text = unit.Name + "  " + new string('★', unit.Tier) + "\nHP " + unit.Health + "/" + unit.MaxHealth;
            }
        }
    }
}
