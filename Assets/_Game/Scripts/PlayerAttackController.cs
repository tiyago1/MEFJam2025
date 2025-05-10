using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private int maxBulletCount = 4;
        [SerializeField] private int current = 4;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private List<GameObject> passiveObjects;
        [SerializeField] private List<GameObject> activeObjects;
        [SerializeField] private PlayerVisualStateController visual;
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private List<Transform> bulletObjects;

        [Inject] private DiContainer _container;

        private bool _isReloading;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsEnoughBullets() && !_isReloading)
            {
                Attack();
                DecreaseBullet();
            }
        }

        [Button]
        private void DecreaseBullet()
        {
            if (current > 0)
            {
                current--;
                bulletObjects[current].transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBounce);
            }

            if (!IsEnoughBullets())
            {
                _isReloading = true;
                FillAllBullets().Forget();
            }
        }

        [Button]
        private void IncreaseBullet()
        {
            if (current < maxBulletCount)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(bulletObjects[current].transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce));
                sequence.Append(bulletObjects[current].transform.DOPunchScale(Vector3.one, .5f).SetEase(Ease.OutBounce));
                current++; 
            }
        }

        private async UniTask FillAllBullets()
        {
            for (int i = 0; i < maxBulletCount; i++)
            {
                IncreaseBullet();
                await UniTask.Delay(TimeSpan.FromSeconds(.2f));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _isReloading = false;
        }
        
        private bool IsEnoughBullets()
        {
            return current > 0 && current <= maxBulletCount;
        }

        private void Attack()
        {
            visual.Attack();
            SetState(true);

            activeObjects[0].SetActive(visual.IsFront);
            activeObjects[1].SetActive(!visual.IsFront);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                Vector3 direction = (hit.point - this.transform.position).normalized;

                Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);

                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                activeObjects[2].transform.DORotateQuaternion(targetRotation, .08f).SetEase(Ease.OutQuad);

                var bullet = _container.InstantiatePrefabForComponent<Bullet>(bulletPrefab, this.transform.position,
                    this.transform.rotation, null);
                bullet.Force(direction, hit.point);
            }

            DisableAttackState().Forget();
        }

        private async UniTask DisableAttackState()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(.2f));
            SetState(false);
            visual.AttackReset();
        }

        private void SetState(bool state)
        {
            foreach (var obj in passiveObjects)
            {
                obj.SetActive(!state);
            }

            foreach (var obj in activeObjects)
            {
                obj.SetActive(state);
            }
        }
    }
}