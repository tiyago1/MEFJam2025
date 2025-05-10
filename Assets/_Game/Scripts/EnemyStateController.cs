using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts
{
    public enum EnemyVisualType
    {
        NormalBack,
        NormalFront,
        PunkFront,
        PunkBack
    }

    public class EnemyStateController : VisualStateController
    {
        [SerializeField, ReadOnly] private EnemyVisualType activeStateType;

        [Button]
        public void SetVisualState(EnemyVisualType state)
        {
            activeStateType = state;
            SetVisualState((int)activeStateType);
        }
    }
}