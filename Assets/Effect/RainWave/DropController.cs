using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    //public Animator? Key = null; 
    // Start is called before the first frame updat

    public void OnBest()
    {
        //if (Key?.gameObject.GetComponent<DragController>() == null)
        //    Key?.SetTrigger("OnBest");
        
        /*
         * These paragraphs were once used to display a white divergent circle at the best time for pressing buttons
         * but later I found that the effect was not ideal, so I decided to delete them 
         */
    }

    public void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
