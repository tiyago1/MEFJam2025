using System;
using Pathfinding;
using UnityEngine;

namespace GamePlay.Components
{
    [RequireComponent(typeof(FunnelModifier))]
    public class PathfActor : AIPath
    {
        private Seeker _seeker;
        private Action _onComplete;
        
        protected override void Awake()
        {
            _seeker = this.GetComponent<Seeker>();
            canMove = false;
        }

        public void MovePoint(Vector3 point, Action callback)
        {
            destination = point;
            _onComplete = callback;
            canMove = true;
        }

        public void Stop()
        {
            canMove = false;
        }
        
        public override void OnTargetReached()
        {
            base.OnTargetReached();
            _onComplete?.Invoke();
            _onComplete = null;
            Stop();
        }

        public void SetSpeed(float speed)
        {
            maxSpeed = speed;
        }
    }
}