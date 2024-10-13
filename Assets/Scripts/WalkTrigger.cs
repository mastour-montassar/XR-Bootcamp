using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WalkTrigger : MonoBehaviour
{
 public CanvasGroup canvasGroup  ;
 private void Start()
 {
  canvasGroup.alpha = 0;
 }

 private void OnTriggerEnter(Collider other)
 {
  
  canvasGroup.alpha=1;
 }
}
