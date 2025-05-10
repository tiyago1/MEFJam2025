using DG.Tweening;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    private void Update()
    {
        this.transform.DOLookAt(target.position, .2f, AxisConstraint.Y);
    }
}
