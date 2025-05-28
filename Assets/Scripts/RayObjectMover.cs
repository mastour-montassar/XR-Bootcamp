using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RayObjectMover : MonoBehaviour
{
    public XRRayInteractor rayInteractor;                   // XR Ray Interactor for selection
    public InputAction moveInAction;                        // Input for moving the object closer
    public InputAction moveOutAction;                       // Input for moving the object farther
    public InputAction rotateXAction;                       // Input for rotating around the X-axis
    public InputAction rotateYAction;                       // Input for rotating around the Y-axis
    public InputAction selectAction;                        // Input for selecting the object
    public InputAction rotateBigCubeAction;                 // Input for rotating the big cube
    public Animator bigCubeAnimator;
    public Transform bigCubeTransform;                      // The transform of the big cube
    public Collider bigCubeCollider;                        // Transparent big cube collider
    public Material completedMaterial;                      // Material to apply when the puzzle is complete
    public float snapThreshold = 0.2f;                      // Maximum distance to snap to a predefined position

    private Transform selectedObject;                       // Currently selected object
    private float currentDistance = 1.0f;                   // Default distance from ray origin
    private float moveSpeed = 1.0f;                         // Speed for rotating the object
    private PuzzlePiece selectedPieceScript;                // The script on the selected object

    private int grabLayer;                                  // Layer for objects that can be grabbed

    private void Start()
    {
        grabLayer = LayerMask.NameToLayer("Grab");

        if (grabLayer == -1)
        {
            Debug.LogError("Layer 'Grab' is not defined. Please create it in the Layer settings.");
        }
    }

    private void OnEnable()
    {
        selectAction.Enable();
        moveInAction.Enable();
        moveOutAction.Enable();
        rotateXAction.Enable();
        rotateYAction.Enable();
        rotateBigCubeAction.Enable();
    }

    private void OnDisable()
    {
        selectAction.Disable();
        moveInAction.Disable();
        moveOutAction.Disable();
        rotateXAction.Disable();
        rotateYAction.Disable();
        rotateBigCubeAction.Disable();
    }

    void Update()
    {
        if (selectAction.WasPressedThisFrame())
        {
            SelectObject();
        }

        if (selectAction.IsPressed() && selectedObject != null)
        {
            HandleObjectControl();
        }

        if (selectAction.WasReleasedThisFrame() && selectedObject != null)
        {
            DeselectObject();
        }

        // Rotate the big cube when the ray detects it and the button is pressed
        if (rotateBigCubeAction.WasPressedThisFrame())
        {
            if (IsBigCubeDetected())
            {
                RotateBigCube();
            }
        }

        // Check if all puzzle pieces are correctly placed
        if (IsBigCubeFull())
        {
            TriggerBigCubeFullAnimation();
            ChangeBigCubeMaterial();
        }
    }
    
    private void TriggerBigCubeFullAnimation()
    {
        if (bigCubeAnimator != null)
        {
            // Trigger the animation using a trigger parameter in the Animator
            bigCubeAnimator.SetTrigger("OpenDoor");
        }
        else
        {
            Debug.LogWarning("Animator not assigned.");
        }
    }


    private void SelectObject()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject.layer == grabLayer)
            {
                selectedObject = hit.collider.transform;

                selectedPieceScript = selectedObject.GetComponent<PuzzlePiece>();

                if (selectedPieceScript != null)
                {
                    Debug.Log("Selected a puzzle piece: " + selectedObject.name);
                }

                currentDistance = Vector3.Distance(rayInteractor.transform.position, hit.point);

                // Disable physics for precise control
                Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
            else
            {
                Debug.Log("Object not on 'Grab' layer or not valid for manipulation.");
            }
        }
    }

    private void HandleObjectControl()
    {
        if (selectedObject == null) return;

        // Movement and rotation control
        if (moveInAction.IsPressed())
        {
            currentDistance -= moveSpeed * Time.deltaTime;
        }
        if (moveOutAction.IsPressed())
        {
            currentDistance += moveSpeed * Time.deltaTime;
        }

        currentDistance = Mathf.Clamp(currentDistance, 0.5f, 5.0f);

        Vector3 rayOrigin = rayInteractor.transform.position;
        Vector3 rayDirection = rayInteractor.transform.forward;
        selectedObject.position = rayOrigin + rayDirection * currentDistance;

        if (rotateXAction.WasPressedThisFrame())
        {
            selectedObject.Rotate(90f, 0f, 0f, Space.Self);
        }
        if (rotateYAction.WasPressedThisFrame())
        {
            selectedObject.Rotate(0f, 90f, 0f, Space.Self);
        }
    }

    private void DeselectObject()
    {
        if (selectedObject == null || selectedPieceScript == null) return;

        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();

        if (bigCubeCollider.bounds.Contains(selectedObject.position))
        {
            Transform nearestPosition = GetNearestValidPosition(selectedObject.position, selectedPieceScript.validPositions);

            if (nearestPosition != null 
                && Vector3.Distance(selectedObject.position, nearestPosition.position) <= snapThreshold 
                && !selectedPieceScript.IsPositionOccupied(nearestPosition))
            {
                selectedObject.position = nearestPosition.position;
                selectedObject.rotation = Quaternion.identity;

                selectedObject.SetParent(bigCubeTransform);

                selectedPieceScript.OccupyPosition(nearestPosition);

                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
            else
            {
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
            }
        }
        else
        {
            if (selectedObject.parent == bigCubeTransform)
            {
                selectedObject.SetParent(null);
                Transform previouslyOccupiedPosition = GetNearestValidPosition(selectedObject.position, selectedPieceScript.validPositions);
                if (previouslyOccupiedPosition != null)
                {
                    selectedPieceScript.ReleasePosition(previouslyOccupiedPosition);
                }
            }

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        selectedObject = null;
        selectedPieceScript = null;
    }

    private Transform GetNearestValidPosition(Vector3 position, List<Transform> validPositions)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var predefined in validPositions)
        {
            float distance = Vector3.Distance(position, predefined.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = predefined;
            }
        }

        return nearest;
    }

    private void RotateBigCube()
    {
        if (bigCubeTransform == null)
        {
            Debug.LogError("Big cube transform not assigned.");
            return;
        }

        bigCubeTransform.Rotate(0, 90, 0, Space.World);

        foreach (Transform child in bigCubeTransform)
        {
            child.localRotation = Quaternion.identity;
        }

        Debug.Log("Rotated the big cube and all contained objects by 90 degrees around the Y-axis.");
    }

    private bool IsBigCubeDetected()
    {
        LayerMask bigCubeLayer = LayerMask.GetMask("BigCube");

        if (Physics.Raycast(rayInteractor.transform.position, rayInteractor.transform.forward, out RaycastHit hit, Mathf.Infinity, bigCubeLayer))
        {
            Debug.Log("Big Cube detected via direct raycast!");
            return true;
        }

        Debug.Log("Big Cube not detected via direct raycast.");
        return false;
    }

    // Check if all puzzle pieces are in their correct positions
    private bool IsBigCubeFull()
    {
        // Check if the big cube has any children (puzzle pieces)
        if (bigCubeTransform.childCount<= 5)
        {
            return false;  // No pieces, so it's not full
        }

        // If there are children, check if all puzzle pieces are placed correctly
        PuzzlePiece[] puzzlePieces = bigCubeTransform.GetComponentsInChildren<PuzzlePiece>();
        foreach (PuzzlePiece piece in puzzlePieces)
        {
            // Pass the big cube's transform to validate the piece's position in local space
            if (!piece.IsPlacedCorrectly(bigCubeTransform))
            {
                return false;  // If any piece is not placed correctly, return false
            }
        }

        return true;  // All pieces are placed correctly
    }



    // Change the material of the big cube when all pieces are placed correctly
    private void ChangeBigCubeMaterial()
    {
        Renderer bigCubeRenderer = bigCubeTransform.GetComponent<Renderer>();
        if (bigCubeRenderer != null)
        {
            bigCubeRenderer.material = completedMaterial;
            Debug.Log("The puzzle is complete! Big cube material changed.");
        }
    }
}
