using UnityEngine;

public class RobotRagdollController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;

    private void Start()
    {
        SetRagdollState(false); 
    }

    public void TriggerRagdoll()
    {
        SetRagdollState(true); 
    }

    private void SetRagdollState(bool activateRagdoll)
    {
        _animator.enabled = !activateRagdoll; 
        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic = !activateRagdoll; 
        }
    }
    
}