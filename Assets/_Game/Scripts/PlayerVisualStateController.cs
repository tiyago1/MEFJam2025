using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts
{
    public enum PlayerVisualStateType
    {
        Front,
        Back,
    }

    public class PlayerVisualStateController : VisualStateController
    {
        [SerializeField, ReadOnly] private PlayerVisualStateType activeStateType;
        [SerializeField] private Transform hairContainer;

        public bool IsLeft;
        public bool IsFront;

        private void Update()
        {
            var position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            SetLookXDirection(position.x < .5f);
            SetLookYDirection(position.y > .5f);
        }

        [Button]
        public void SetVisualState(PlayerVisualStateType state)
        {
            activeStateType = state;
            SetVisualState((int)activeStateType);
        }

        public void SetLookXDirection(bool isLeft)
        {
            IsLeft = isLeft;
            view.flipX = !IsFront ? IsLeft : !IsLeft;
            hairContainer.transform.eulerAngles = new Vector3(0, view.flipX ? 0 : -180,114.969f);
        }

        public void SetLookYDirection(bool isFront)
        {
            IsFront = isFront;
            SetVisualState(IsFront ? PlayerVisualStateType.Front : PlayerVisualStateType.Back);
        }

        public void Attack()
        {
            hairContainer.gameObject.SetActive(false);
        }

        public void AttackReset()
        {
            hairContainer.gameObject.SetActive(true);
        }
    }
}