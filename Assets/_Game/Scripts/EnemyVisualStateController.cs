using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public enum EnemyVisualType
    {
        NormalBack,
        NormalFront,
        PunkFront,
        PunkBack,
        Shocked
    }

    public class EnemyVisualStateController : VisualStateController
    {
        [SerializeField, ReadOnly] private EnemyVisualType activeStateType;

        [Inject] private PlayerMovementController playerMovementController;
        [SerializeField] private LayerMask afterLayer;
        [SerializeField] private LayerMask beforeLayer;
        [SerializeField] private Collider collider;
        [SerializeField] private Collider characterController;

        public bool IsLeft;
        public bool IsFront;
        public bool IsPunked;
        public bool IsShocked;
        private Sequence _moveSequence;

        public override void Initialize()
        {
            base.Initialize();
            float value = 6;

            PreDeffance().Forget();

            _moveSequence = DOTween.Sequence();
            _moveSequence.Append(view.transform.DOLocalRotate(new Vector3(0, 0, -value), .3f));
            _moveSequence.Append(view.transform.DOLocalRotate(new Vector3(0, 0, value), .3f));
            _moveSequence.Append(view.transform.DOLocalRotate(new Vector3(0, 0, -value), .3f));
            _moveSequence.SetLoops(-1, LoopType.Yoyo);
        }

        private async UniTask PreDeffance()
        {
            characterController.excludeLayers = beforeLayer;
            collider.enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(.2));
            Sequence sequence = DOTween.Sequence();
            sequence.Append(view.DOFade(.5f, .2f));
            sequence.Append(view.DOFade(1f, .2f));
            sequence.SetLoops(2, LoopType.Yoyo);
     
            await sequence.AsyncWaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(.01f), cancellationToken: cancellationTokenSource.Token);

            characterController.excludeLayers = afterLayer;
            collider.enabled = true;
        }

        [Button]
        public void SetVisualState(EnemyVisualType state)
        {
            activeStateType = state;
            SetVisualState((int)activeStateType);
        }

        private void Update()
        {
            var position = this.transform.position - playerMovementController.transform.position;
            SetLookXDirection(position.x < .5f);
            SetLookYDirection(position.z > .5f);
        }

        public void SetLookXDirection(bool isLeft)
        {
            IsLeft = isLeft;
            view.flipX = IsLeft;
        }

        public void SetLookYDirection(bool isFront)
        {
            IsFront = isFront;
            RefreshVisualState();
        }

        public void RefreshVisualState()
        {
            if (IsPunked)
            {
                SetVisualState(IsFront ? EnemyVisualType.PunkFront : EnemyVisualType.PunkBack);
                return;
            }
            else
            {
                SetVisualState(IsFront ? EnemyVisualType.NormalFront : EnemyVisualType.NormalBack);
            }
            
            if (IsShocked)
            {
                SetVisualState(IsFront ? EnemyVisualType.Shocked : EnemyVisualType.NormalBack);
            }
            else
            {
                SetVisualState(IsFront ? EnemyVisualType.NormalFront : EnemyVisualType.NormalBack);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _moveSequence.Kill();
            IsShocked = false;
            IsPunked = false;
        }

        public void SetShocked()
        {
            IsShocked = true;
        }
    }
}