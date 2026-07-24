using UnityEngine;

namespace Luminia
{
    public static class LuminiaBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void StartGame()
        {
            if (Object.FindAnyObjectByType<GameController>() != null)
            {
                return;
            }

            var root = new GameObject("Luminia Game");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<GameLog>();
            root.AddComponent<GameController>();
        }
    }
}
