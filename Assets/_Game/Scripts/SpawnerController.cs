using System;
using System.Collections.Generic;
using System.Threading;
using _Game.Scripts.Enemies;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
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

    public class SpawnerController : MonoBehaviour, IDisposable
    {
        [SerializeField] private float spawnRate;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private int initialSpawnCount;
        [SerializeField] private int maxSpawnCount;
        [SerializeField] private List<EnemyData> data;
        [SerializeField] private List<Enemy> enemies;

        [Inject] private DiContainer _container;
        [Inject] private NormalEnemy.Pool _normalPool;
        [Inject] private GrandmaEnemy.Pool _grandmaPool;

        private CancellationTokenSource cancellationTokenSource;

        private void Awake()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartSpawning().Forget();
            MainSpawning().Forget();
        }

        [Button]
        private int Test()
        {
            int weightSum = 0;
            for (int i = 0; i < data.Count; i++)
            {
                weightSum += data[i].Ratio;
            }

            int index = 0;
            int lastIndex = data.Count - 1;
            while (index < lastIndex)
            {
                // Do a probability check with a likelihood of weights[index] / weightSum.
                if (Random.Range(0, weightSum) < data[index].Ratio)
                {
                    return index;
                }

                // Remove the last item from the sum of total untested weights and try again.
                weightSum -= data[index++].Ratio;
            }

            return index;
        }

        [Button]
        public async UniTask StartSpawning()
        {
            EnemyType key = EnemyType.Normal;
            int selectedEnemy = 0;

            for (int i = 0; i < initialSpawnCount; i++)
            {
                SpawnEnemy(Test());
                await UniTask.Delay(TimeSpan.FromSeconds(spawnRate), cancellationToken: cancellationTokenSource.Token);
            }
        }

        public async UniTask MainSpawning()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(spawnRate), cancellationToken: cancellationTokenSource.Token);
                SpawnEnemy(Test());
            }
        }

        [Button]
        public void SpawnEnemy(int index)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            // var enemy = _container.InstantiatePrefabForComponent<Enemy>(data[index].Prefab,
            //     spawnPoint.transform.position,
            //     quaternion.identity,
            //     this.transform);
            Enemy enemy = null;
            switch (data[index].Type)
            {
                case EnemyType.Normal:
                    enemy = _normalPool.Spawn();
                    enemy.transform.position = spawnPoint.transform.position;
                    this.transform.gameObject.SetActive(true);

                    enemy.Initialize();

                    break;
                case EnemyType.Policeman:
                    break;
                case EnemyType.Grandma:
                    enemy = _grandmaPool.Spawn();
                    enemy.transform.position = spawnPoint.transform.position;

                    break;
            }
            
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
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