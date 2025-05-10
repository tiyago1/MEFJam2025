using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyType type;
        [SerializeField] private LookAt lookAt;
        
        [Inject(Id = "IsoCamera")] private Transform characterLookAt;
        
        [Button]
        public virtual void Initialize()
        {
            lookAt.Initialize(characterLookAt);
        }
    }
}