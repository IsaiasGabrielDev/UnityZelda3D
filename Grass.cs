using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public ParticleSystem fxhit;
    
    private bool isCute;
    // Start is called before the first frame update
    void GetHit(int amount)
    {
        
        if(isCute ==false)
        {
            isCute = true;
            transform.localScale = new Vector3(1f , 1f, 1f);
            fxhit.Emit(10);            
       }
       StartCoroutine("CRESCERGRAMA");

    }
    IEnumerator CRESCERGRAMA()
    {
        yield return new WaitForSeconds(300f);
        fxhit.Emit(10);
        transform.localScale = new Vector3(3f , 3f, 3f);
        isCute = false;
    }

}
