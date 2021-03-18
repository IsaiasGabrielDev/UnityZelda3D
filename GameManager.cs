using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public enum enemyState
{
    IDLE, ALERT, PATROL, FURY, FOLLOW, DIED
}

public enum GameState
{
    GAMEPLAY, DIE
}
public class GameManager : MonoBehaviour
{
   
   [Header ("infoPlayer")]
    public GameState gameState;
    public Transform player; 
    
    
    [Header("UI")]
    public Text txtgem;
    private int gems;
    
    [Header ("Slime IA")]
    public float slimedistancetoAttack = 2.3f;
    public float slimedistancetoAttackBOSS = 4f;
    public Transform[] slimeWaypoints;
    public Transform[] slimeWaypointsBoss;
    public float slideWaitTime = 0.5f; //tempo em IDLE  
    public float slimeAttackDelay = 1.5f;  
    public float slimeLookAtSpeed = 1f;
    
    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainPaticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOvertime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("Drop item")]
    public GameObject gemPrefab;
    public int percDrop = 50; //percentual de drop de diamante do SLime

    
    // Start is called before the first frame update
    void Start()
    {
        rainModule = rainPaticle.emission;
        txtgem.text = "x" + gems.ToString();
    }
    
    public void OnoffRain(bool isRain)
    {
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("PostBManager", isRain);
        StartCoroutine("RainManager", isRain);
    }
    IEnumerator RainManager(bool isRain)
    {
        switch(isRain)
        {
            case true: //aumenta a chuva

            for(float r = rainModule.rateOverTime.constant; r < rainRateOvertime; r += rainIncrement)
            {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }
            rainModule.rateOverTime = rainRateOvertime;

            break;

            case false: //diminui a chuva
             for(float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement)
            {
                rainModule.rateOverTime = r;
                yield return new WaitForSeconds(rainIncrementDelay);
            }
            rainModule.rateOverTime = 0;

            break;
        }
        
    }

    IEnumerator PostBManager(bool isRain)
    {
        switch(isRain)
        {
            case true:
                
                for(float w = postB.weight; w < 1; w += 1 *Time.deltaTime )
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight=1;
            
                break;

            case false:
                 
                 for(float w = postB.weight; w > 0; w -= 1 *Time.deltaTime )
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                
                postB.weight = 0;

            break;
        }
    }  

    public void ChangeGameState(GameState newState)
    {
        gameState = newState;
    }

    public void SetGems(int amount)
    {
        gems += amount;
        txtgem.text = "x" + gems.ToString();
        
    }

    public bool Perc(int p)
    {
        int temp = Random.Range(0,100);
        bool retorno = temp <= p ? true : false;
        return retorno;
    }
}
