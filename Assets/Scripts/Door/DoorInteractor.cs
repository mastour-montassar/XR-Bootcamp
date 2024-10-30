using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractor : MonoBehaviour
{
    [SerializeField] private Animator[] _animator;
    [SerializeField] private float openDuration = 4f;
    [SerializeField] private GameObject[] _cards;        // Array for cards
    [SerializeField] private GameObject[] _cardPositions; 
    [SerializeField] private GameObject[] _cardReaders;
    public static bool done =false;
    public static int currentCard=0; 
    private float Distancecheck = 4f;
    
    
    
    private bool isOpen = false;
    private float timer = 0f;
    private int counter = 0;
    
    public void Update()
    {
        if (done && !isOpen ) OpenDoor();
        if (isOpen)
        {
            timer += Time.deltaTime;
            if (timer >= openDuration)
            {
                CloseDoor();
            }
        }
    }

    private void CloseDoor()
    {
        _animator[counter].SetTrigger("CloseDoor");
        isOpen = false;
        timer = 0f;
        
    }

    public void OpenDoor()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            KeyCard key = _cards[i].GetComponent<KeyCard>();
            CardReader cardReader = _cardReaders[i].GetComponent<CardReader>(); 
            
            float distance = Vector3.Distance(key.transform.position, cardReader.transform.position);
            // Check access level and whether it matches card reader
            if (currentCard == key.accessLevel && key.accessLevel == cardReader.accessLevel && distance <= Distancecheck)
            {
                _cards[i].SetActive(false);
                _cardPositions[i].SetActive(true);


                _animator[i].SetTrigger("OpenDoor");
                counter = i;
                isOpen = true;
                timer = 0f;
                done = false;
                
                return; // Exit the function after opening the appropriate door
            }
        }
        
            Debug.Log("You don't have access to this card");
            done = false;
        

    }
    
    

}
