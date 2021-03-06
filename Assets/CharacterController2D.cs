using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    //Animation-----------------------
    public Animator animator;
    private bool running = false; //digunakan untuk mengatasi bug salah arah pada saat menyerang sambil berlari.
    //--------------------------------

    //double jump
    private int doubleJump;
    public int doubleJumpAmount;

    //Attack resource------------------
    private bool lightAttack;//kondisi pengaktifan light attack
    private bool heavyAttack;//kondisi pengaktifan heavy attack
    private float timeAttack;//digunakan untuk mengimplementasikan attack rate
    private float buttonTimeA;//digunakan untuk implementasi hold key pada heavy attack bersama dengan buttonTimeZ
    private float buttonTimeZ;
    public float attackRate = .99f;//selang waktu minimum untuk tiap attack
    public Transform attackPos;//sebuah class Transform dari pusat attackRange
    public LayerMask whatIsEnemies;//merujuk pada layer enemies berada
    public float attackRange = 1f;//besar attackRange
    //Attack resource------------------

    //Block resource-----------------
    public bool isBlock; //kondisi block dijalankan
    public float blockDuration; //maksimal durasi block dalam satu aksi
    private float blockTime; //implementasi block duration
    private float blockTimeMemA; //berfungsi menyimpan selang waktu dengan block terakhir, bersama dengan blockTimeMemZ
    private float blockTimeMemZ;
    private bool musuhMenyerang; //sementara digunakan untuk menggantikan konsisi diserang oleh musuh
    //Block resource-----------------

    //Dash resource-----------------
    private bool rightDash;
    private bool leftDash;
    private bool downDash;
    public float dashDistance;
    private float lastclickKanan;
    private float lastclickKiri;
    private float lastclickBawah;
    //Dash resource-----------------

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = false;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void Update(){
        //Attack condition-----------------
        if(timeAttack <= 0){
            if(Input.GetButtonDown("Fire1")){
                buttonTimeA = Time.time;
            }
            else if(Input.GetButtonUp("Fire1")){
                buttonTimeZ = Time.time - buttonTimeA;
            }
            if(buttonTimeZ>0f){
                if(buttonTimeZ > 1f){ //1f adalah waktu seberapa lama holdkey diperlukan untuk heavy attack
                    heavyAttack = true;
                }
                else lightAttack = true;
            }
            buttonTimeZ = 0f;
        }
        else{
            timeAttack -= Time.deltaTime;
        }
        //Attack condition------------------

        Block();

        //dash condition-------------------
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            float selangWaktu = Time.time - lastclickKanan;
            if(selangWaktu <= .2f){
                rightDash = true;
            }
            lastclickKanan = Time.time;
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            float selangWaktu = Time.time - lastclickKiri;
            if(selangWaktu <= .2f){
                leftDash = true;
            }
            lastclickKiri = Time.time;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) && !m_Grounded){
            float selangWaktu = Time.time - lastclickBawah;
            if(selangWaktu <= .2f){
                downDash = true;
            }
            lastclickBawah = Time.time;
        }
        //dash condition-------------------

        //for double jumping 
        if (m_Grounded)
        {
            doubleJump = doubleJumpAmount;
        }
        //MAINTAIN JUMPFORCE 5<jumpforce<10
        if(Input.GetKeyDown(KeyCode.UpArrow) && doubleJump > 0)
        {
            m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
            doubleJump--; //biar ga infinite double jump
        }else if(Input.GetKeyDown(KeyCode.UpArrow) && doubleJump == 0 && m_Grounded == true){
            m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
        }


        //flip fix
        float m = Input.GetAxisRaw("Horizontal");
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("run")) running = true;
        if(m>0 && running){
            m_FacingRight = true;
            transform.rotation = Quaternion.Euler(0f,0f,0f);
        }
        else if(m<0 && running){
            m_FacingRight = false;
            transform.rotation = Quaternion.Euler(0f,-180f,0f);
        }

        //keadaan saat frame sebelumnya berada pada state "run" namun frame sekarang sudah tidak berada pada state "run"
        if(running && !animator.GetCurrentAnimatorStateInfo(0).IsName("run")){
            transform.Rotate(0f, 180f, 0f);
            running = false;
        }
    }
    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        Attack();
        StartCoroutine(Dash());
        DownwardDash();
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            // if (move > 0 && !m_FacingRight)
            // {
            //     // ... flip the player.
            //     Flip();
            // }
            // Otherwise if the input is moving the player left and the player is facing right...
            // else if (move < 0 && m_FacingRight)
            // {
            //     // ... flip the player.
            //     Flip();
            // }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    public void Attack(){
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        if(lightAttack){
            animator.SetTrigger("LightAttack");
            //block mini-stun section
            if(blockTimeMemZ<=.3f){
                float r = UnityEngine.Random.Range(1,100);//random class using UnityEngine library
                if(r%4 == 0){
                    for(int i = 0; i<enemiesToDamage.Length; i++){
                        //stun method on enemy class
                    }
                }
            }
            //block mini-stun section

            for(int i = 0; i<enemiesToDamage.Length; i++){
                // Debug.Log("lightAttack");
                //enemies take damage method for lightAttack
            }
            lightAttack = false;
        }
        else if(heavyAttack){
            for(int i = 0; i<enemiesToDamage.Length; i++){
                // Debug.Log("heavyAttack");
                //enemies take damage method for heavyAttack
            }
            heavyAttack = false;
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    void Block(){
        if(Input.GetKeyDown(KeyCode.B)){
            isBlock = true;
            blockTime = blockDuration;
        }
        else if(Input.GetKeyUp(KeyCode.B) || blockTime<=0){
            isBlock = false;
            blockTime = 0; 
        }

        if(blockTime>0) blockTime -= Time.deltaTime;
        blockTimeMemZ = Time.time - blockTimeMemA;

        if(Input.GetKeyDown(KeyCode.G)) musuhMenyerang = true;
        else if(Input.GetKeyUp(KeyCode.G)) musuhMenyerang = false;

        //block section
        if(isBlock && musuhMenyerang){
            isBlock = false;
            blockTimeMemA = Time.time;   
        }
    }

    IEnumerator Dash(){
        if(rightDash){
            m_Rigidbody2D.velocity = Vector2.right*dashDistance;
            rightDash = false;
            yield return new WaitForSeconds(0.05f);
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        else if(leftDash){
            m_Rigidbody2D.velocity = Vector2.left*dashDistance;
            leftDash = false;
            yield return new WaitForSeconds(0.05f);
            m_Rigidbody2D.velocity = Vector2.zero;
        }
    }

    void DownwardDash(){
        if(downDash){
            m_Rigidbody2D.velocity = Vector2.down*50f;
            downDash = false;
        }
    }    
}