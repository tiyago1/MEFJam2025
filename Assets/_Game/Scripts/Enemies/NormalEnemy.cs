using UnityEngine;
using Zenject;

namespace _Game.Scripts.Enemies
{
    public class NormalEnemy : Enemy, IDamageable
    {
        [SerializeField] private EnemyChase chaser;
        [SerializeField] private EnemyVisualStateController view;
        [SerializeField] private Collider collider;
        
        [Inject] private PlayerMovementController player;

        public override void Initialize()
        {
            base.Initialize();
            chaser.Chase(player.transform);
        }

        public void TakeDamage(int damage, GameObject owner)
        {
            Destroy(owner);
            view.IsPunked = true;
            view.RefreshVisualState();
            chaser.Stop();
            collider.enabled = false;
        }
    }
}