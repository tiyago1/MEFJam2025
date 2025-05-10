using DG.Tweening;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform target;
    private bool _isInitialized;
    public void Initialize(Transform target)
    {
        this.target = target;
        _isInitialized = true;
    }
    
    private void Update()
    {
        if (!_isInitialized)
            return;
        
        this.transform.DOLookAt(target.position, .2f, AxisConstraint.Y);
    }
}
