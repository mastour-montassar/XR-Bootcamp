using UnityEngine;
using System.Collections.Generic;

public class PuzzlePiece : MonoBehaviour
{
    public List<Transform> validPositions; // Predefined valid positions for this piece
    
    private Dictionary<Transform, bool> positionStatus; // Tracks whether a position is occupied

    private void Awake()
    {
        // Initialize the dictionary
        positionStatus = new Dictionary<Transform, bool>();
        foreach (Transform position in validPositions)
        {
            positionStatus[position] = false; // All positions start as unoccupied
        }
    }

    // Check if a position is occupied
    public bool IsPositionOccupied(Transform position)
    {
        return positionStatus.ContainsKey(position) && positionStatus[position];
    }

    // Mark a position as occupied
    public void OccupyPosition(Transform position)
    {
        if (positionStatus.ContainsKey(position))
        {
            positionStatus[position] = true;
        }
    }

    // Mark a position as unoccupied
    public void ReleasePosition(Transform position)
    {
        if (positionStatus.ContainsKey(position))
        {
            positionStatus[position] = false;
        }
    }

    public bool IsPlacedCorrectly(Transform bigCubeTransform)
    {
        // Iterate through all the valid positions for this piece
        foreach (Transform validPosition in validPositions)
        {
            // Convert the valid position to local space in relation to the big cube
            Vector3 validLocalPosition = bigCubeTransform.InverseTransformPoint(validPosition.position);
        
            // Convert the current piece's world position to local space in relation to the big cube
            Vector3 pieceLocalPosition = bigCubeTransform.InverseTransformPoint(transform.position);

            // Compare the local piece position with the valid position in local space
            if (Vector3.Distance(pieceLocalPosition, validLocalPosition) < 0.2f)
            {
                return true;  // The piece is placed correctly within the threshold
            }
        }

        return false;  // The piece is not placed correctly
    }


}