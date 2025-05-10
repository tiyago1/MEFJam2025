using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class PlayerAttackController : MonoBehaviour
    {
        [SerializeField] private int maxBulletCount = 3;
        [SerializeField] private int current = 0;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private List<GameObject> passiveObjects;
        [SerializeField] private List<GameObject> activeObjects;
        [SerializeField] private PlayerVisualStateController visual;
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask groundLayer;

        [Inject] private DiContainer _container;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetState(true);

                activeObjects[0].SetActive(visual.IsFront);
                activeObjects[1].SetActive(!visual.IsFront);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
                {
                    Vector3 direction = (hit.point - this.transform.position).normalized;
                    
                    Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);
                    
                    Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                    activeObjects[2].transform.DORotateQuaternion(targetRotation, .05f).SetEase(Ease.OutQuad);
                    
                    var bullet = _container.InstantiatePrefabForComponent<Bullet>(bulletPrefab, this.transform.position,
                        this.transform.rotation, null);
                    bullet.Force(direction,hit.point);
                }
                

                DisableAttackState().Forget();
            }
        }

        private async UniTask DisableAttackState()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(.2f));
            SetState(false);
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