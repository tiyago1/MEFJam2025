using UnityEngine;
using Zenject;

namespace _Game.Scripts.Enemies
{
    public class NormalEnemy : Enemy
    {
        [SerializeField] private EnemyChase chaser;
        [Inject] private PlayerMovementController player;
        
        public override void Initialize()
        {
            base.Initialize();
            chaser.Chase(player.transform);

        }
    }
}