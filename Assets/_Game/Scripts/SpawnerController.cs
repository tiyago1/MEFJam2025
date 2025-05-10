using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Zenject;
using Random = UnityEngine.Random;

namespace _Game.Scripts
{
    public enum EnemyType
    {
        Normal,
        Policeman,
        Grandma
    }
    
    public class SpawnerController : MonoBehaviour
    {
        [SerializeField] private float spawnRate;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private int initialSpawnCount;
        [SerializeField] private int maxSpawnCount;
        [SerializeField] private List<EnemyData> data;
        
        private Dictionary<EnemyType, int> _enemiesRate;
        [Inject] private DiContainer _container;

        private void Awake()
        {
            _enemiesRate = new Dictionary<EnemyType, int>()
            {
                { EnemyType.Normal, 80 },
                { EnemyType.Policeman, 10 },
                { EnemyType.Grandma, 10 }
            };
            StartSpawning().Forget();
        }

        [Button]
        public async UniTask StartSpawning()
        {
            EnemyType key = EnemyType.Normal;
            int selectedEnemy = 0;
            for (int i = 0; i < initialSpawnCount; i++)
            {
                for (int j = 0; j < data.Count; j++)
                {
                    var ratio = Random.Range(0, data[j].Ratio);
                    if (ratio > data[j].Ratio)
                    {
                        selectedEnemy = j;
                        break;
                    }
                }

                SpawnEnemy(selectedEnemy);
                await UniTask.Delay(TimeSpan.FromSeconds(spawnRate));
            }
        }
        

        [Button]
        public void SpawnEnemy(int index)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var enemy = _container.InstantiatePrefabForComponent<Enemy>(data[index].Prefab,spawnPoint.transform.position,quaternion.identity, this.transform);
            enemy.Initialize();
        }
    }

    [System.Serializable]
    public class EnemyData
    {
        public EnemyType Type;
        public int Ratio;
        public Enemy Prefab;
    }
}