using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTrigger : MonoBehaviour
{
 private void OnTriggerEnter(Collider other)
 {
  Debug.Log("Player Enter Trigger Volume");
 }
}
