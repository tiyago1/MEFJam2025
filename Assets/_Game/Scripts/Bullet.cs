using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private float bulletForce;

        private void OnCollisionEnter(Collision other)
        {
            var enemy = other.gameObject.GetComponent<IDamageable>();

            if (enemy is not null)  
            {
                enemy.TakeDamage(1, this.gameObject);
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