using System;
using System.Collections.Generic;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts
{
    
    public class VisualStateController : MonoBehaviour, IDisposable
    {
        [SerializeField] private List<Sprite> sprites;
        [SerializeField] public SpriteRenderer view;
        [SerializeField] private int initialState;
        
        [ReadOnly] private int activeState;
        
        protected CancellationTokenSource cancellationTokenSource;

        public virtual void Initialize()
        {
            SetVisualState(initialState);
            cancellationTokenSource = new CancellationTokenSource();
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

        public virtual void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}