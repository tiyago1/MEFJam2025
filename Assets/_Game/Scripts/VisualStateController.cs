using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts
{
    
    public class VisualStateController : MonoBehaviour
    {
        [SerializeField] private List<Sprite> sprites;
        [SerializeField] protected SpriteRenderer view;
        [SerializeField] private int initialState;
        
        [ReadOnly] private int activeState;

        private void Awake()
        {
            SetVisualState(initialState);
        }

        [Button]
        protected void SetVisualState(int state)
        {
            activeState = state;
            view.sprite = sprites[(int)activeState];
        }

        [Button]
        public void Previous()
        {
            SetVisualState(activeState - 1);
        }

        [Button]
        public void Next()
        {
            SetVisualState(activeState + 1);
        }
    }
}