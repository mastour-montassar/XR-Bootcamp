using UnityEngine;

public class MineGrab : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;   
    [SerializeField] private Transform _holdPosition;     
    [SerializeField] private float _grabRange = 2f;       
    [SerializeField] private float _throwForce = 20f;    
    [SerializeField] private float _snapSpeed = 40f;     
    [SerializeField] private LayerMask _surfaceLayer;    

    private Rigidbody _grabbedObject;                 
    private bool _grabPressed = false;                   
    private bool _isThrown = false;                    

    void FixedUpdate()
    {
        if (_grabbedObject)
        {
            _grabbedObject.velocity = (_holdPosition.position - _grabbedObject.transform.position) * _snapSpeed;
        }
    }

    // Function to handle grabbing the mine
    private void OnGrab()
    {
        if (_grabPressed)
        {
            _grabPressed = false;
            
            Debug.Log("Grab Released");

            if (!_grabbedObject) return;

            DropGrabbedObject();
        }
        else
        {
            _grabPressed = true;
            
            Debug.Log("Grab Pressed");
            if (Physics.Raycast(_cameraPosition.position, _cameraPosition.forward, out RaycastHit hit, _grabRange))
            {
                if (!hit.transform.gameObject.CompareTag("Grabbable")) return; 

                _grabbedObject = hit.transform.GetComponent<Rigidbody>();
                _grabbedObject.transform.parent = _holdPosition;  
                _grabbedObject.isKinematic = true;  
            }
        }
    }

    // Function to handle throwing the mine
    private void OnThrow()
    {
        if (!_grabbedObject) return;
        
        _grabbedObject.isKinematic = false;
        _grabbedObject.transform.parent = null; 
        _grabbedObject.AddForce(_cameraPosition.forward * _throwForce, ForceMode.Impulse); 
        _isThrown = true; 
        
        // No longer holding the object
        _grabbedObject = null;
    }

   
    private void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown) return;

        if (_surfaceLayer == (_surfaceLayer | (1 << collision.gameObject.layer)))
        {
            StickToSurface(collision.contacts[0].point, collision.contacts[0].normal); 
        }
    }

    private void StickToSurface(Vector3 hitPoint, Vector3 hitNormal)
    {

        // Place the mine at the collision point and align it to the surface normal
        transform.position = hitPoint;
        transform.rotation = Quaternion.LookRotation(hitNormal);

        _isThrown = false; // Reset throw state after sticking
    }

    // Function to drop the grabbed object (without throwing)
    private void DropGrabbedObject()
    {
        if (!_grabbedObject) return;
        
        _grabbedObject.transform.parent = null;
        _grabbedObject.isKinematic = false; // Re-enable physics when dropped
        _grabbedObject = null;
    }
}
