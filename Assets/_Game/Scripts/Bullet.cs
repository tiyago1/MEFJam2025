using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private float bulletForce;
        
        [Inject] private SignalBus signalBus;


        private void OnCollisionEnter(Collision other)
        {
            var enemy = other.gameObject.GetComponent<IDamageable>();

            if (enemy is not null)  
            {
                enemy.TakeDamage(1, this.gameObject);
                signalBus.Fire<GameSignals.OnPlayerDamageTaken>();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Force(Vector3 direction, Vector3 position)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            rigidbody.AddForce(direction * bulletForce, ForceMode.Impulse);
            this.transform.DOLookAt(position,.1f, AxisConstraint.Y);
        }
    }
}