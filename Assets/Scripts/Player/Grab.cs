using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _grabRange = 2f;
    [SerializeField] private float _snapSpeed = 40f;
    [SerializeField] private GameObject _cardPosition; 
    [SerializeField] private GameObject _cardReader; 
    [SerializeField] private GameObject _card;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody _grabbedObject;
    private bool _grabPressed = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_grabbedObject)
        {
            _grabbedObject.velocity = (_holdPosition.position - _grabbedObject.transform.position) * _snapSpeed;
        }
    }

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
            }
            
        }
    }

    private void DropGrabbedObject()
    {
        _grabbedObject.transform.parent = null;
        _grabbedObject = null;
    }

    private void OnThrow()
    {
        KeyCard key = _card.GetComponent<KeyCard>();
        CardReader cardReader = _cardReader.GetComponent<CardReader>();
        if (DoorTrigger.intTriger &&  _grabbedObject!=null && key.accessLevel == cardReader.accessLevel )
        {
            _card.SetActive(false);
            _cardPosition.SetActive(true); 
         DoorInteractor.OpenDoor(); 
        }
        else
        {
            Debug.Log("Throwing card as usual.");
        }
    }


}
