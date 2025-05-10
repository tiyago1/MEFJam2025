using System;
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
        PunkBack
    }

    public class EnemyVisualStateController : VisualStateController
    {
        [SerializeField, ReadOnly] private EnemyVisualType activeStateType;

        [Inject] private PlayerMovementController playerMovementController;

        public bool IsLeft;
        public bool IsFront;
        public bool IsPunked;

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
            }
            else
            {
                SetVisualState(IsFront ? EnemyVisualType.NormalFront : EnemyVisualType.NormalBack);
            }
        }
    }
}