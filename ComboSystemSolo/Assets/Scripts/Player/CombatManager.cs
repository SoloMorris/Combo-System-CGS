using UnityEngine;

namespace Player {
    public class CombatManager : MonoSingleton<CombatManager> {
        [SerializeField] private GameObject enemyPrefab;

        public void SpawnEnemy() {
            Instantiate(enemyPrefab);
        }
    }
}