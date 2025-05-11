using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Enemies
{
    public class GrandmaEnemy : Enemy, IDamageable, IDisposable
    {
        [SerializeField] private EnemyVisualStateController view;
        [SerializeField] private Collider collider;
        [SerializeField] private RandomPathAI randomPathAI;

        [Inject] private PlayerMovementController player;
        [Inject] private SignalBus signalBus;
        
        private Pool _pool;

        public override void Initialize()
        {
            base.Initialize();
            view.Initialize();
            WaitAndActivate().Forget();
        }

        private async UniTask WaitAndActivate()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            randomPathAI.Activate();
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            randomPathAI.Dispose();
            Die();
        } 

        public void TakeDamage(int damage, GameObject owner)
        {
            Destroy(owner);
            Die();
        }

        private void Die()
        {
            randomPathAI.Cancel();
            view.SetShocked();
            view.RefreshVisualState();
            collider.enabled = false; 
            transform.DOMoveY(8, .2f).OnComplete(() =>
            {
                _pool.Despawn(this);
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);

            if (other.gameObject.CompareTag("Player"))
            {
                signalBus.Fire<GameSignals.OnPlayerDamageTaken>();
                collider.enabled = false;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(view.view.DOColor(Color.red, .5f));
                sequence.Append(this.transform.DOScale(UnityEngine.Vector3.zero, 1).SetEase(Ease.OutBounce));
                sequence.OnComplete(() => { _pool.Despawn(this); });
            }
        }


        public class Pool : MonoMemoryPool<GrandmaEnemy>
        {
            protected override void OnCreated(GrandmaEnemy item)
            {
                item.gameObject.name += $"_{NumTotal}";
                item._pool = this;
                base.OnCreated(item);
            }
        }

        public void Dispose()
        {
            view?.Dispose();
            _pool?.Dispose();
            randomPathAI.Dispose();
        }
    }
}