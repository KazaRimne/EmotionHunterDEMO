using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Animestand;

public class Movesana : MonoBehaviour
{
    public static Movesana Instance { get; private set; }
    public bool Fright;
    public bool Die = false;
    public bool Up;
    public bool Down;
    public float moveX;
    public bool isDamaged = false;

    
    public float moveSpeed = 6.2f;
    public float stopSpeed = 1f;
    public float jumpForce = 10f;
    public float jumpStami = 20f;

    public float shiftForce = 20f;
    public float shiftStami = 55f;
    public float shiftCD;

    public float jumpCooldown = 0.5f; // 跳躍冷卻時間
    public float jumpBufferTime = 0.15f; 
    public float rayDistance = 0.2f;    // 向下偵測的長度
    public float footWidth = 0.3f;      
    public float yOffset = 0.1f;
    public float plushXOffset = 0.1f;

    public float lowGravity = 1f;
    public float highGravity = 4f;
    public float originalDrag = 3f;
    public float lowGravityDrag = 2f;
    public float lowGravitytime = 0.25f;
    public float Coyotetime = 0.2f;

    public PhysicsMaterial2D originMaterial;
    public PhysicsMaterial2D zeroFrictionMaterial;
    public CapsuleCollider2D bodyCollider;

    public bool isGrounded;
    public bool shifted;
    public Transform groundCheck;      // 用來檢查角色腳下
    public LayerMask Ground;
    public int jumpCount;

    private float jumpBufferCounter;
    private Rigidbody2D rb;
    private float originalGravity;
    private Coroutine _routine;
    private float wallTopY;
    private Collider2D _collider;

    public bool wallLeft;   // 左牆
    public bool wallRight;  // 右牆
    private AnimeSANA _sana;
    private bool isCurrentlyATK;
    private bool climbJump;
    private float climbtimer =0f;
    private float Coyotetimer = 0f;
    private bool Attackmove = false;

    void Start()
    {
        shifted = false;
        isDamaged = false;
        climbJump = false;
        isCurrentlyATK = false;
        Fright = true;
        Up = false;
        Down = false;
        rb = GetComponent<Rigidbody2D>();
        originalGravity = 2f;
        _sana = GetComponent<AnimeSANA>();
        _collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckGrounded();
        if (isDamaged || Die|| ESCSTOP.Instance.isPaused) return;

        isCurrentlyATK = _sana.stateMachine.CurrentState is ATK or RATK;
        if (shiftCD != 0)
        {
            shiftCD -= Time.deltaTime;
            
            if (shiftCD <= 0)
            {
                shiftCD = 0;
                shifted = false;
            }
        }
        if (climbtimer != 0)
        {
            climbtimer -= Time.deltaTime;

            if (climbtimer <= 0)
            {
                climbtimer = 0;
            }
        }
        if (isGrounded)
        {
            jumpCount = 0;
            rb.gravityScale = originalGravity;
            rb.drag = originalDrag;
            bodyCollider.sharedMaterial = originMaterial;
            if (!Attackmove)
            {
                moveX = 0f;
            }
        }
            if (CheckGrounded())
        {
            jumpCount = 0;
            rb.gravityScale = originalGravity;
            rb.drag = originalDrag;
            bodyCollider.sharedMaterial = originMaterial;
            if (!Attackmove)
            {
                moveX = 0f;
            }
        }
        else
        {
            Coyotetimer = Coyotetime;
            if (Coyotetimer != 0)
            {
                Coyotetimer -= Time.deltaTime;
                if (Coyotetimer <= 0)
                {
                    Coyotetimer = 0;
                }
            }
        }

        if (shifted) // 如果正在衝刺中
        {
            if (shiftCD >= 0.5)
            {
               
                if (!Fright)
                {
                    rb.velocity = new Vector2(-shiftForce * 1.1f, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(shiftForce, rb.velocity.y);
                }
            }
        }

        else if (Input.GetKey(KeyCode.D))
        {
            if (wallRight && Datasana.Instance.Stamina <= 20f)
            {
                moveX = 0;
            }
            
            else
            {

                if (wallRight)
                {
                    jumpCount = 1;
                }
                moveX = 1f;
                if (!isGrounded&&!wallLeft && !wallRight)
                {
                    // 在空中：慢慢滑行至 0
                    moveX = Mathf.MoveTowards(moveX, 0f, Time.deltaTime * stopSpeed);
                }
                rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (wallLeft && Datasana.Instance.Stamina <= 20f)
            {
                moveX = 0;
            }

            else
            {

                if (wallLeft)
                {
                    jumpCount = 1;
                }
                moveX = -1f;
                if (!isGrounded && !wallRight && !wallLeft)
                {
                    // 在空中：慢慢滑行至 0
                    moveX = Mathf.MoveTowards(moveX, 0f, Time.deltaTime * stopSpeed);
                }
                rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
            }
        }

        Vector3 scale = transform.localScale;
        if (Input.GetKey(KeyCode.LeftShift) && Datasana.Instance.Stamina >= shiftStami && !shifted)
        {
            if ((wallLeft) || (wallRight)) return;
            shiftCD = 0.55f;
            Datasana.Instance.Stamina -= shiftStami;
            shifted = true;
            RuntimeManager.PlayOneShot("event:/Shift0", transform.position);
        }

        if (!ESCSTOP.Instance.isPaused)
        {
            if (moveX > 0f)
            {
                Fright = true;
                scale.x = Mathf.Abs(scale.x);
            }
            else if (moveX < 0f)
            {
                Fright = false;
                scale.x = -Mathf.Abs(scale.x);
            }
        }

        transform.localScale = scale;
        if ((wallLeft && !isGrounded) || (wallRight && !isGrounded))
        {
            if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
                || (Input.GetKey(KeyCode.D)&&Input.GetKey(KeyCode.W) )
                && Datasana.Instance.Stamina > 20f)
            {
                if (Mathf.Abs(moveX) >= 0.5f && climbtimer == 0f)
                {
                    Up = true;
                    Down = false;
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.6f);

                }

            }
            else if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
                || (Input.GetKey(KeyCode.D) & Input.GetKey(KeyCode.S)))
            {
                if (Mathf.Abs(moveX) >= 0.5f && climbtimer == 0f)
                {
                    Up = false;
                    Down = true;
                    rb.velocity = new Vector2(rb.velocity.x, -jumpForce * 0.6f);
                }
            }

            else
            {
                Up = false;
                Down = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (jumpCount == 1 && (Mathf.Abs(moveX) > 0.1f) && Datasana.Instance.Stamina >= 15)
            {
                if (jumpCount >= 2)
                {
                    return;
                }
                jumpCount++;
                Datasana.Instance.Stamina -= 15;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                RuntimeManager.PlayOneShot("event:/Jump2", transform.position);
                StartCoroutine(TemporaryLowGravity());
                
            }
            else if (jumpCount == 0)
            {
                   
                    if (jumpCount >= 1)
                    {
                        return;
                    }
                    if ((!wallLeft) && (!wallRight))
                    {
                        if ( Datasana.Instance.Stamina >= jumpStami)
                        {
                            Datasana.Instance.Stamina -= jumpStami;
                            jumpBufferCounter = jumpBufferTime;
                            if (CheckGrounded()||Coyotetimer > 0f)
                                {
                                    Up = false;
                                    Down = false;
                                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                                    RuntimeManager.PlayOneShot("event:/Jump", transform.position);
                                    StartCoroutine(TemporaryLowGravity());
                            jumpCount = 1;
                        }
                        }
                    }

                    
            }
        }
        if (jumpBufferCounter > 0)
        {
            jumpCount = 1;
            Up = false;
            Down = false;
            jumpBufferCounter -= Time.deltaTime;
        }
        else if (isGrounded&& jumpBufferCounter <= 0)
        {
            jumpCount = 0;
            jumpBufferCounter = 0;
        }
        if (!isGrounded)
        {
            if (Input.GetKey(KeyCode.S))
            {
                rb.gravityScale = highGravity;

            }

            else if ((wallLeft) || (wallRight)&& !shifted)
            {
                jumpCount = 1;
                Coyotetimer = 0f;
                if (Datasana.Instance.Stamina <= 20f)
                {
                    climbtimer = 0.25f;
                    moveX = 0;
                    rb.drag = 0f;
                    rb.gravityScale = highGravity;
                    bodyCollider.sharedMaterial = zeroFrictionMaterial;
                    _sana.stateMachine.ChangeState(new Jump(_sana));
                }
                if (Mathf.Abs(moveX) > 0.5f&& climbtimer== 0f)
                {
                    float playerCenterY = _collider.bounds.center.y - 0.15f;
                    if (playerCenterY > wallTopY && !climbJump)
                    {
                        climbJump = true;
                        // 執行自動向上彈跳
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f); // 這裡用你原本的跳躍力

                    }
                    else
                    {
                        bodyCollider.sharedMaterial = originMaterial;
                        rb.gravityScale = originalGravity;
                        rb.drag = originalDrag;
                        if (moveX != 0)
                        {
                            // 正常爬牆
                            jumpCount = 0;
                          
                                _sana.stateMachine.ChangeState(new ClimbState(_sana));
                          
                        }

                        else
                        {
                          
                                _sana.stateMachine.ChangeState(new Jump(_sana));
                            }
                            
                        }
                }
                else
                {
                    _sana.stateMachine.ChangeState(new Jump(_sana));
                }
            }

            else if (isCurrentlyATK)
            {
                if (_routine != null)
                {
                    StopCoroutine(_routine);
                }
                if (shifted)
                { _routine = StartCoroutine(Routine(0.75f)); }
                else
                {
                    _routine = StartCoroutine(Routine(0.5f));
                }
            }
            else
            {
                if (!shifted)
                {
                    if (jumpCount == 2)
                    {
                        _sana.stateMachine.ChangeState(new JumpTwiceState(_sana));
                    }
                    else
                    {
                        _sana.stateMachine.ChangeState(new Jump(_sana));
                    }

                }

                else
                {
                    _sana.stateMachine.ChangeState(new Shift(_sana));
                    if (!isCurrentlyATK)
                    {
                        StartCoroutine(Routine(0.65f));
                    }
                       
                }
            }
        }
        else if (Mathf.Abs(moveX) > 0.1f)
        {
            if (isCurrentlyATK)
            {
                if (_routine != null)
                {
                    StopCoroutine(_routine);
                }
                if (shifted)
                { _routine = StartCoroutine(Routine(0.75f)); }
                else
                {
                    _routine = StartCoroutine(Routine(0.5f));
                }
                    
            }
                else if ((wallLeft) || (wallRight)&& !shifted)
                {
                    _sana.stateMachine.ChangeState(new ClimbState(_sana));
                }
                else
                {
                    _sana.stateMachine.ChangeState(new Move(_sana));
                }
            
           
        }
        else
        {
            bool isCurrentlySwupTwo = _sana.stateMachine.CurrentState is SwupTwo;
            rb.gravityScale = originalGravity;
            if (isCurrentlySwupTwo)
            {
                StartCoroutine(Routine(0.25f));

            }
            else if (isCurrentlyATK)
            {
                if (_routine != null)
                {
                    StopCoroutine(_routine);
                }
                // 重新開始一個完整的 delay
                _routine = StartCoroutine(Routine(0.55f));
            }
            else if (!shifted)
            {
                    _sana.stateMachine.ChangeState(new Stand(_sana));
            }
            else if(shifted&&!isCurrentlyATK)
            {
                _sana.stateMachine.ChangeState(new Shift(_sana));
                StartCoroutine(Routine(0.65f));
            }
        }

    }

    void FixedUpdate()
    {

    }
    IEnumerator TemporaryLowGravity()
    {
        rb.gravityScale = lowGravity;
        rb.drag = lowGravityDrag;
        yield return new WaitForSeconds(lowGravitytime); 
        rb.gravityScale = originalGravity;
        rb.drag = originalDrag;
    }
    public IEnumerator ATKmove()
    {
        if (jumpCount == 2)
        {
            jumpCount = 3;
        }
        if (shifted)
        {
            yield break;
        }
        Attackmove = true;
        if (Mouseca.Instance.mouseright)
        {
            moveX = 1f;
        }
        else
        {
            moveX = -1f;
        }
        rb.velocity = new Vector2(moveX*2, rb.velocity.y);
        yield return new WaitForSeconds(0.1f);
        Attackmove = false;
        moveX = 0f;
    }

    public IEnumerator Routine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _routine = null;
        if (!isGrounded)
        {
            _sana.stateMachine.ChangeState(new Jump(_sana));
        }
        else if (!shifted)
        {
            if (Mathf.Abs(moveX) > 0.1f)
            {
                _sana.stateMachine.ChangeState(new Move(_sana));
            }
            else
            { _sana.stateMachine.ChangeState(new Stand(_sana)); }
        }
        
    }

    public IEnumerator BeDamaged(float duration)
    {
        isDamaged = true;

        // 這裡可以加一點視覺反饋，比如閃紅光
        yield return new WaitForSeconds(duration);

        isDamaged = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = wallLeft = wallRight = false;
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 地板判定
                if (contact.normal.y > 0.8f)
                {
                    wallLeft = false;
                    wallRight = false;
                    if (moveX != 0)
                    {
                        climbJump = false;
                    }
                }
                if (contact.normal.x > 0.6f)
                { wallLeft = true; }

                if (contact.normal.x < -0.6f)
                { wallRight = true; }
                
                wallTopY = collision.collider.bounds.max.y;
            }
        }
    }
   public bool CheckGrounded()
    {
        if (Fright)
        {
            // 計算左腳與右腳的發射點
            Vector2 leftFoot = new Vector2(transform.position.x - footWidth - plushXOffset, transform.position.y - yOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + footWidth, transform.position.y - yOffset);

            // 只要其中一條射線射到地面，就算接地
            RaycastHit2D leftHit = Physics2D.Raycast(leftFoot, Vector2.down, rayDistance, Ground);
            RaycastHit2D rightHit = Physics2D.Raycast(rightFoot, Vector2.down, rayDistance, Ground);
            return (leftHit.collider != null || rightHit.collider != null);

        }
        else{
            Vector2 leftFoot = new Vector2(transform.position.x - footWidth , transform.position.y - yOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + footWidth + plushXOffset, transform.position.y - yOffset);

            // 只要其中一條射線射到地面，就算接地
            RaycastHit2D leftHit = Physics2D.Raycast(leftFoot, Vector2.down, rayDistance, Ground);
            RaycastHit2D rightHit = Physics2D.Raycast(rightFoot, Vector2.down, rayDistance, Ground);
            return (leftHit.collider != null || rightHit.collider != null);
        }
        
    }

    void OnDrawGizmos()
    {
        // 視覺化：讓你在 Scene 視窗直接看到這兩根「針」
        Gizmos.color = isGrounded ? Color.green : Color.red;
        if (Fright)
        {
            Vector2 leftFoot = new Vector2(transform.position.x - footWidth - plushXOffset, transform.position.y - yOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + footWidth, transform.position.y - yOffset);

            // 畫出左腳偵測線
            Gizmos.DrawLine(leftFoot, leftFoot + Vector2.down * rayDistance);
            // 畫出右腳偵測線
            Gizmos.DrawLine(rightFoot, rightFoot + Vector2.down * rayDistance);
        }

        else
        {
            Vector2 leftFoot = new Vector2(transform.position.x - footWidth , transform.position.y - yOffset);
            Vector2 rightFoot = new Vector2(transform.position.x + footWidth + plushXOffset, transform.position.y - yOffset);

            // 畫出左腳偵測線
            Gizmos.DrawLine(leftFoot, leftFoot + Vector2.down * rayDistance);
            // 畫出右腳偵測線
            Gizmos.DrawLine(rightFoot, rightFoot + Vector2.down * rayDistance);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = wallLeft = wallRight = false;
    }
    void Awake()
    {
        Instance = this;
    }
}
