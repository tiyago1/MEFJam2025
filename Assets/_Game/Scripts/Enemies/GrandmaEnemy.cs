using DG.Tweening;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Enemies
{
    public class GrandmaEnemy : Enemy, IDamageable
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
        }

        public void TakeDamage(int damage, GameObject owner)
        {
            Destroy(owner);
            view.IsPunked = true;
            view.RefreshVisualState();
            collider.enabled = false;

            transform.DOMoveY(8, .4f).SetDelay(.4f);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);

            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("x");
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
    }
}