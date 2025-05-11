namespace _Game.Scripts
{
    using Pathfinding;
    using UnityEngine;

    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(CharacterController))] 
    public class EnemyChase : MonoBehaviour
    {
        public Transform target; 
        public float updateRate = 0.5f;
        public float speed = 3f;
        public float nextWaypointDistance = 0.5f;

        private Seeker seeker;
        private CharacterController controller;
        private Path path;
        private int currentWaypoint = 0;
        private bool reachedEndOfPath = false;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            controller = GetComponent<CharacterController>();

        }

        void UpdatePath()
        {
            
            if (seeker.IsDone() && target != null)
            {
                seeker.StartPath(transform.position, target.position, OnPathComplete);
            }
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        void Update()
        {
            if (path == null || target == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            controller.Move(direction * speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }

        public void Chase(Transform target) 
        {
            this.target = target;
            InvokeRepeating(nameof(UpdatePath), .1f, updateRate);
        }

        public void Stop()
        {
            this.target = null;
        }
    }
}