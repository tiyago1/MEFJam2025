using UnityEngine;

namespace _Game.Scripts
{
    public interface IDamageable
    {
        void TakeDamage(int damage, GameObject owner);
    }
}