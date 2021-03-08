using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTriggers : MonoBehaviour
{
    public GameObject camb;
    private GameManager _GameManager;

    private void Start() 
    {
         _GameManager = FindObjectOfType(typeof(GameManager))as GameManager;
    }
    
    private void OnTriggerEnter(Collider other) 
        {
            switch(other.gameObject.tag){
                case "Camtrigger":
                    camb.SetActive(true);
                    break;

                case "Pickups":
                _GameManager.SetGems(1);
                Destroy(other.gameObject);
                break;                

            }

        }
        private void OnTriggerExit(Collider other) {
            switch(other.gameObject.tag){
                case "Camtrigger":
                    camb.SetActive(false);
                    break;
        }
        }

}
