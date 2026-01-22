using UnityEngine;

namespace Player {
    public class CombatManager : MonoSingleton<CombatManager> {
        [SerializeField] private GameObject enemyPrefab;

        public void SpawnEnemy() {
            Instantiate(enemyPrefab);
        }

        /// <summary>
        /// "Frame-authored" combat values are converted using this FPS.
        /// Commonly 60 for fighting games.
        /// </summary>
        public const float COMBAT_FPS = 60f;

        /// <summary>
        /// Default input buffer window (seconds).
        /// </summary>
        public const float INPUT_BUFFER_SECONDS = 0.5f;

        /// <summary>
        /// Default special-move input buffer window (seconds).
        /// </summary>
        public const float SPECIAL_MOVE_BUFFER_SECONDS = 0.75f;

        public static float FramesToSeconds(int frames) {
            if (frames <= 0) return 0f;
            return frames / COMBAT_FPS;
        }

        /// <summary>
        /// Quantizes a seconds-authored value to the nearest combat frame and returns seconds.
        /// Use this to preserve "frame feel" while keeping time-based execution stable.
        /// </summary>
        public static float QuantizeSecondsToFrames(float seconds) {
            if (seconds <= 0f) return 0f;
            var frames = Mathf.RoundToInt(seconds * COMBAT_FPS);
            return Mathf.Max(0, frames) / COMBAT_FPS;
        }
    }
}