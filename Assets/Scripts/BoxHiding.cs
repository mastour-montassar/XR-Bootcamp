using UnityEngine;

public class BoxHiding : MonoBehaviour
{
    public GameObject player;
    public GameObject box;
    public Transform playerCamera;
    public GameObject boxViewOverlay;
    public float boxDistanceInFront = 1.5f;
    private bool isHiding = false;

    public void OnHide()
    {
        if (isHiding) return;
        
        isHiding = true;
        player.GetComponent<Renderer>().enabled = false;
        box.SetActive(false);
        boxViewOverlay.SetActive(true);
        SetBoxView();
    }

    public void OnUnHide()
    {
        if (!isHiding) return;

        isHiding = false;
        player.GetComponent<Renderer>().enabled = true;
        box.SetActive(true);
        boxViewOverlay.SetActive(false);
        MoveBoxInFrontOfPlayer();
        SetNormalView();
    }

    private void MoveBoxInFrontOfPlayer()
    {
        Vector3 forwardDirection = playerCamera.forward;
        Vector3 newPosition = playerCamera.position + forwardDirection * boxDistanceInFront;
        box.transform.position = newPosition;
        box.transform.rotation = playerCamera.rotation;
        box.SetActive(true);
    }
    
    private void SetBoxView()
    {
        boxViewOverlay.SetActive(true);
    }

    private void SetNormalView()
    {
        boxViewOverlay.SetActive(false);
    }

    public bool IsPlayerHiding()
    {
        return isHiding;
    }
}