using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour
{   private GameManager _GameManager; //importando uma função de outro script
    private Animator anim; //variavel do animator
    public int HP = 3; //HP da IA
    public ParticleSystem hitfx; //váriavel de efeito do hit
    public GameObject Retirardacena; //retirar o Slime da Cena
    public bool isDied; // Se ele está morto
    public  enemyState state; //estado da IA
   
    //public const float patrolWaitTime = 9f; //tempo de PATROL
    //IA
    private NavMeshAgent agent; //localização atual do agent
    private Vector3 destination; //variavel para setar um destino
    private int idWaypoint; //Sortear um destination para IA
    private bool isWalk; //bool está andando?
    private bool isAlert; //está alerta?
    private bool isPlayerVisible; //Player está visivel?
    private bool isAttack; //está atacando?

    // Start is called before the first frame update
    void Start() //Inicializa todas as funções ao inicio
    {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ChangeState(state);
    }

    
    void Update() //Atualiza a cada frame
    {
        
        if(agent.desiredVelocity.magnitude >= 0.1)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        anim.SetBool("isWalk", isWalk);   
        anim.SetBool("isAlert", isAlert);

        
    
    }
    private void LateUpdate() 
    {
    StateManager();
        
    }
        
    

IEnumerator Died() //corrotina de morte.
{
    
    destination = transform.position;
    agent.destination = destination; //trava o andamento da IA.
    yield return new WaitForSeconds(2f);

    if(_GameManager.Perc(_GameManager.percDrop)) //pega o percentual de chance de drop.
    {
        Instantiate(_GameManager.gemPrefab, transform.position, _GameManager.gemPrefab.transform.rotation); //Summona um cristal no local de morte.
    }
    Retirardacena.SetActive(false); //retira o Slime da cena. 


    
}

private void OnTriggerEnter(Collider other) 
{
 if(other.gameObject.tag == "Player") //Muda o estado da IA de Idle para Alerta caso colida com o inimigo.
{
    isPlayerVisible = true;
    if(state == enemyState.IDLE || state == enemyState.PATROL)
    {
    ChangeState(enemyState.ALERT);        
    }        
}
    if(other.gameObject.tag == "Enemy") //Muda o estado da IA se ela colidir no campo de visão com outra.
{
    
    if(state == enemyState.PATROL)
    {
    ChangeState(enemyState.IDLE);        
    }        
}


}
private void OnTriggerExit(Collider other)  //caso saia da colisão seta a variavel de está visivel para falso.
{
    if(other.gameObject.tag == "Player")
{
    isPlayerVisible = false;   
}  
}
    
    #region Meus Métodos
    void GetHit(int amount) //Metodo de tomar dano.
    {
        if(isDied == true){return;}
        HP -= amount;
        if(HP>0){
            
        ChangeState(enemyState.FURY);
        anim.SetTrigger("GetHit"); 
        hitfx.Emit(50);
        
        }
        else
        {
            ChangeState(enemyState.DIED); // se HP for igual a zero seta como o status como morto.
            anim.SetTrigger("Die"); //inicia animaçao de morte
        
            
            StartCoroutine("Died"); //inicia corrotina de morte.



        }
        
    }

    void StateManager() //Gerenciador de status do bot status atual.
    {
        switch(state){
            
           
            case enemyState.ALERT:
            LookAt();
            break;
            
            
            case enemyState.FOLLOW:
            destination = _GameManager.player.position;
            agent.destination = destination;
            LookAt();
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                if(_GameManager.gameState == GameState.GAMEPLAY)
                {
                Attackslime();
                }
            } 
            break;

            case enemyState.FURY:
            LookAt();
            destination = _GameManager.player.position;
            agent.stoppingDistance = _GameManager.slimedistancetoAttack;
            agent.destination = destination;
        
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                if(_GameManager.gameState == GameState.GAMEPLAY)
                {
                Attackslime();
                }
            }

            break;

            case enemyState.PATROL:
            break;
    
        }

    }
    void ChangeState(enemyState newState) ////Gerenciador de status da IA, muda o status dela.
    {
        StopAllCoroutines(); // ENCERRA TODAS A CORROTINAS
        
        isAlert = false;
        state = newState;
        switch(newState){
             
            case enemyState.IDLE:
            
            agent.stoppingDistance = 0;
            destination = transform.position;
            agent.destination = destination;

            StartCoroutine("IDLE");

            break;

            case enemyState.ALERT:
            agent.stoppingDistance = 0;
            destination = transform.position;
            agent.destination = destination;
            isAlert = true;
            StartCoroutine("ALERT");
            
            break;

            

            case enemyState.FOLLOW:
            
            agent.stoppingDistance = _GameManager.slimedistancetoAttack;


            break;

            case enemyState.FURY:
            
            break;

            case enemyState.PATROL:
            agent.stoppingDistance = 0;
            idWaypoint = Random.Range(0,_GameManager.slimeWaypoints.Length);
            destination = _GameManager.slimeWaypoints[idWaypoint].position;
            agent.destination = destination;
            


            StartCoroutine("PATROL");
            break;

            case enemyState.DIED:
            destination = transform.position;
            agent.destination = destination;
            break;
    
        }
        

    }
    IEnumerator IDLE(){
        yield return new WaitForSeconds(_GameManager.slideWaitTime);
        Staystill(50); //50% porcentagem de chance de se manter em IDLE
       
    }

    IEnumerator PATROL()
    {
        yield return new WaitUntil( () => agent.remainingDistance<=0);
        Staystill(30); //30% de chance de ficar parado e 70% de entrar em patrulha
    }
    IEnumerator ALERT()
        {
            yield return new WaitForSeconds(_GameManager.slideWaitTime);
            if(isPlayerVisible == true)
            {
                ChangeState(enemyState.FOLLOW);
            }
            else
            {
            Staystill(25); //10% DE CHANCE
            }
                
            
        }

    IEnumerator ATTACKDELLAY() //Delay de atack
    {
        yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
        isAttack = false;

    }    
    
    
void Staystill(int yes) //sorteio de chance.
{
    if(Rand() <= yes)
    {
        ChangeState(enemyState.IDLE);
    }
    else //caso no
    {
        ChangeState(enemyState.PATROL);
    }
}

    int Rand()
    {
       int rand = Random.Range(0,100); //0...99
        return rand;
    }
    void Attackslime(){
        if(isAttack == false && isPlayerVisible == true)
        {
        
        isAttack = true;
        anim.SetTrigger("Attack");
        }
    }

    void AttackisDone() //quando a animação de attack acabar, starta a corrotina de delay.
{
   
    StartCoroutine("ATTACKDELLAY");
}
void LookAt() //Função de ficar olhando para o player.
{
    
    Vector3 Lookdirection =  (_GameManager.player.position - transform.position).normalized;
    Quaternion LookRotation = Quaternion.LookRotation(Lookdirection);
    transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);
}



    #endregion
}
