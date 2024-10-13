using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFalling : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private float duration = 3.0f;
    private float time = 0f;
    private bool pressed = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
           pressed = true; 
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
            pressed = false;
            time = 0f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            time += Time.deltaTime;


        if (time >= duration)
        {
          cube.SetActive(true);  
        }
        
        }
    }
}
