using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Vector3 = System.Numerics.Vector3;

namespace _Game.Scripts.Enemies
{
    public class NormalEnemy : Enemy, IDamageable
    {
        [SerializeField] private EnemyChase chaser;
        [SerializeField] private EnemyVisualStateController view;
        [SerializeField] private Collider collider;
        
        [Inject] private PlayerMovementController player;
        [Inject] private SignalBus signalBus;

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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            
            if (other.gameObject.CompareTag("Player"))   
            {
                Debug.Log("x"); 
                signalBus.Fire<GameSignals.OnPlayerDamageTaken>();
                chaser.Stop();
                collider.enabled = false;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(view.view.DOColor(Color.red, .5f));
                // sequence.Append(view.view.DOColor(Color.yellow, .5f));
                sequence.Append(this.transform.DOScale(UnityEngine.Vector3.zero, 1).SetEase(Ease.OutBounce));
                // sequence.Append(transform.DOMoveY(-8, 0.5f));
                sequence.OnComplete(() =>
                {
                    Destroy(this.gameObject);
                });
                
                // this.transform.DOMoveY(8f, .5f);
                // this.transform.DOScale(UnityEngine.Vector3.zero, 1).SetEase(Ease.OutBounce).OnComplete(() =>
                // {
                //     Destroy(this.gameObject);
                // });
            }
        }
    }
}