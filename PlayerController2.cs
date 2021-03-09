using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController2 : MonoBehaviour
{
    private GameManager _GameManager;
    private CharacterController controller;
    private Animator anim;
    private bool isWalk;
    private Rigidbody rb;
    
    [Header("Config Player")]
    public float movementSpeed = 2f; //valor de speed no Header
    private Vector3 direction; //Vector de direção para a movimentação
    public int HP; //HP do personagem
    [Header("Attack Config")] //menu de Config de Attack
    public ParticleSystem fxAttack; //Particula de Attack
    public Transform hitBox; //tamanho da hitbox do Attack
    [Range(0.2f, 1f)] //valor maximo e minimo do hitbox no Header
    public float hitRange = 0.5f;// valor pré-estabelecido
    public LayerMask hitMask; //layer que permite dar o hit em colisão
    public Collider[] hitInfo; // informa em que o Player colidiu com o Attack
    private bool isAttack; //Interruptor de Attack
    private float horizontal; //variavel de movimentação 
    private float vertical; //variavel de movimentação 
    public int amountDmg; //Dano causado do Player
    private const float GRAVIDADE = -9.81f; //Gravidade manual
    public Vector3 velocity; //velocidade Manual

    

    // Start is called before the first frame update
    void Start() //inciando funções
    {
        _GameManager = FindObjectOfType(typeof(GameManager))as GameManager;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody> ();
        

    }
    void Update() //Função que é atualiza a cada frame
    {
        if(_GameManager.gameState == GameState.GAMEPLAY)
        {
            Inputs();
            
        }
        
        MoveCharacter();
        UpdadeAnimator();

        
    }


#region Meus Métodos

void Inputs(){ //Inputs
horizontal = Input.GetAxis("Horizontal");
vertical = Input.GetAxis("Vertical");

        if(Input.GetButtonDown("Fire1") && isAttack ==false)
        {
            Attack();
        }
        
}
void MoveCharacter() //Movimentação do personagem
{
        direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            isWalk = true;
        }
        else
        { //(direction.magnitude <= 0.1f)
            isWalk = false;
        }

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f; //Esse valor assegura que o player vá descer nas elevações do terreno.
            }
        }
        else
        {
            velocity.y += GRAVIDADE * Time.deltaTime;
        }
        controller.Move((direction + velocity) * movementSpeed * Time.deltaTime);
}
void UpdadeAnimator() //atualizar a animação de Walk
{
    anim.SetBool("isWalk", isWalk);
}

void Attack() //Função de Attack
{   
    isAttack = true;
    anim.SetTrigger("Attack");
    Invoke("FxAttackdelay", 0.4f);
    hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);
    
    foreach(Collider c in hitInfo)
    {
    c.gameObject.SendMessage("GetHit", amountDmg, SendMessageOptions.DontRequireReceiver);
    }
} 

void FxAttackdelay() //Delay de Attack
{
 fxAttack.Emit(1);
}    

void AttackisDone() //Seta o interruptor de Attack como falso, mostrando que o ele pode atacar novamente.
{
    isAttack = false;
}

void GetHit(int amount) //Função de tomar dano e morte do jogador
{
    HP -= amount;
    
    if(HP>0){
        anim.SetTrigger("Hit");
        
    }
    else
    {   
       velocity.y += GRAVIDADE * Time.deltaTime;
        _GameManager.ChangeGameState(GameState.DIE);

        
        anim.SetTrigger("Die");
    }
}

    private void OnTriggerEnter(Collider other) //Função que detecta colisão no objeto que estiver setado na Tag TakeDamage, o colisor do objeto colide com o do Player.
{
 if(other.gameObject.tag =="TakeDamage")
 {
     GetHit(1);
 }   
}


#endregion
      private void OnDrawGizmosSelected() //Função para mostrar as linha de HitBox.
{ 
          if(hitBox != null){
          Gizmos.color = Color.red;
          Gizmos.DrawWireSphere(hitBox.position, hitRange);
      }
}  
}

