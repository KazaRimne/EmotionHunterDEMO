using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class Monstermove : MonoBehaviour, IFreezable
{
    public float moveSpeed = 2f;          // 移動速度
    public float minChangeTime = 3f;      // 轉向最短間隔
    public float maxChangeTime = 10f;
    public float offset = 0.1f;
    public bool isGrounded;
    public bool ATKing = false;
    public bool Fright;
    
    public float direction = 1f;            // 1 = 右, -1 = 左
    public float ATKmoveSpeed = 1.8f;

    public float freeze;
    public float FreezeTime
    {
        get { return freeze; }
        set { freeze = value; }
    }
    public LayerMask Ground;
    public LayerMask Player;
    private bool Canmove = true;
    private Rigidbody2D rb;
    private float changeTime;             // 下次轉向時間倒數
    private float initialScaleX;
    private float turnCooldown = 0f;
    private Transform playerposition;
    public Monsteranime myAnime;
    private Monsterattack myAttack;
    public bool RfrontHasFloor = false;
    public bool LfrontHasFloor = false;
    public float wallTurnTimer = 0f;
    public float stuckTimer = 0f;
    public float  AtkXposition= 1.6f;
    GameObject Pplayer;
    private bool ATKed = false;
    bool Used = false;

    // Start is called before the first frame update
    void Start()
    {
        freeze = 0;
        Canmove = true;
        Fright = true;
        ATKing = false;
        
        Used = false;
        initialScaleX = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        ResetChangeTime();
        GameObject Pplayer = GameObject.FindGameObjectWithTag("Player");
        playerposition = Pplayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (freeze > 0f)
            {
                freeze -= Time.deltaTime;
                Canmove = false;
            }
            else 
            {
            freeze = 0f;
            Canmove = true;
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
            stuckTimer = 0f;
            wallTurnTimer = 0f;
        }
        if (!ATKing && direction == 0)
        {
            
            
            StartCoroutine(AttackFlow(3.5f));
        }
      
        if (Canmove)
        {
            LayerMask combinedMask = Ground | Player;
            float rayLengthplayer = 5f;
            Vector2 rayDirectionONE = new Vector2(direction, 0);
            Vector2 RedRayposition = new Vector2(rb.position.x, rb.position.y - 0.4f);
            RaycastHit2D hit = Physics2D.Raycast(RedRayposition, rayDirectionONE, 
            rayLengthplayer, combinedMask);
            Debug.DrawRay(RedRayposition, rayDirectionONE * rayLengthplayer, Color.red);

            float rayLength = 1f;
            Vector2 rayDirection = new Vector2(direction, 0);
            RaycastHit2D hiit = Physics2D.Raycast(rb.position,
                new Vector2(direction, 0), rayLength, Ground);

            // 在 Scene 視窗畫出綠線，方便你確認射線長度有沒有穿出身體
            Debug.DrawRay(rb.position, rayDirection * rayLength, Color.green);
            if (ATKing)
            {
                if (isGrounded)
                {
                GameObject Pplayer = GameObject.FindGameObjectWithTag("Player");
                playerposition = Pplayer.transform;
                Vector3 directionToPlayer = playerposition.transform.position - transform.position;
                float distanceX = Mathf.Abs(directionToPlayer.x);
                if (!RfrontHasFloor &&
                    playerposition.transform.position.x > transform.position.x)
                {
                    StartCoroutine(Attackdelay());
                    direction = 0.00001f;
                }
                else if (!LfrontHasFloor &&
                    playerposition.transform.position.x < transform.position.x)
                {

                        StartCoroutine(Attackdelay());
                        direction = -0.00001f;
                }

                else
                {
                    if (distanceX > (AtkXposition+0.05f)&& !myAttack.atking)
                    {
                            StopAllCoroutines();
                            if (!myAttack.atking)
                        {
                            if (myAnime != null)
                            {
                                myAnime._playOnce = false;
                                myAnime.SetAnimation(myAnime.MoveATK, myAnime.FASTicks);

                            }
                               
                        }
                            
                        direction = (directionToPlayer.x > AtkXposition) ? ATKmoveSpeed : -ATKmoveSpeed;
                            ATKed = false;
                        }
                        else
                        {
                            if (playerposition.transform.position.x >= transform.position.x)
                            {

                                StartCoroutine(Attackdelay());
                                direction = 0.00001f;
                            }
                            else
                            {
                                StartCoroutine(Attackdelay());
                                direction = -0.00001f;
                            }
                            
                        }
                   
                    if ((Pplayer.transform.position - transform.position).sqrMagnitude > 576f)
                    {
                        if (direction > 0)
                        {
                            direction = 1f;
                        }
                        else
                        {
                            direction = -1f;
                        }
                        ATKing = false;
                        if (myAttack != null) myAttack.RUNed = true;
                    }
                }
                }
                else if (!isGrounded&&myAnime != null)
                {
                    myAnime.SetAnimation(myAnime.frames, myAnime.SLTicks);
                }
            }
            else
            {
                if (myAnime != null)
                {
                    myAnime._playOnce = false;
                    myAnime.SetAnimation(myAnime.frames, myAnime.SLTicks);
                }
               
                if (changeTime <= 0f)
                {
                  
                    direction = 1;
                    direction *= -1;
                    ResetChangeTime();
                }
            }
            if (hit.collider != null)
            {
                // 檢查撞到的「第一個東西」是誰
                if (((1 << hit.collider.gameObject.layer) & Player) != 0)
                {
                    ATKing = true;
                }
            }
            if (wallTurnTimer > 0f)
            {
                wallTurnTimer -= Time.deltaTime;
            }

            if (hiit.collider != null && ((1 << hiit.collider.gameObject.layer) & Ground) != 0)
            {
                // 【A. 只要射線一直戳到牆，stuckTimer 就開始瘋狂累加時間】
                stuckTimer += Time.deltaTime;

                // 【B. 保險機制：如果卡牆連續超過 3 秒，無視一切條件強制轉向】
                if (stuckTimer >= 3.0f)
                {
                    direction *= -1f;    // 強制反轉
                    if (direction > 0)
                    {
                        direction = 1f;
                    }
                    else
                    {
                        direction = -1f;
                    }
                    stuckTimer = 0f;     // 歸零計時器
                    ATKing = false;      // 強制解除攻擊
                    ResetChangeTime();
                    rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
                }
                else
                {
                    // 【C. 正常撞牆判定：牆在右怪往右，或牆在左怪往左（正面撞牆才轉）】
                    float wallRelativeX = hiit.point.x - transform.position.x;
                    if ((wallRelativeX > 0f && direction > 0f) || (wallRelativeX < 0f && direction < 0f))
                    {
                        direction *= -1f;
                        if (direction > 0)
                        {
                            direction = 1f;
                        }
                        else
                        {
                            direction = -1f;
                        }
                        stuckTimer = 0f; // 正常轉向成功也要記得歸零計時器
                        ATKing = false;
                        ResetChangeTime();
                        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
                    }
                }
            }
            else
            {
                // 【D. 安全鎖：只要射線一離開牆面（沒撞牆了），卡牆計時器立刻歸零】
                stuckTimer = 0f;
            }
            if (hiit.collider == null&& hit.collider == null)
            {
                changeTime -= Time.deltaTime;
                if (changeTime <= 0f)
                {
                    direction *= -1;
                    ResetChangeTime();
                }
            }
        }

        if (direction <= 0f)
        {
            Fright = false;
            Vector3 s = transform.localScale;
            s.x = +initialScaleX; // 直接用初始值乘以方向
            transform.localScale = s;
        }
        else
        {
            Fright = true;
            Vector3 s = transform.localScale;
            s.x = -initialScaleX; // 直接用初始值乘以方向
            transform.localScale = s;
        }

    }
    void ATKistrue()
    {
       
    }
    void ResetChangeTime()
    {
        changeTime = UnityEngine.Random.Range(minChangeTime, maxChangeTime);
    }
    IEnumerator AttackFlow(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!Used) 
        {
            Used = true;
            if (Fright) { direction = -1; }
            else { direction = 1; }
        }
        yield return new WaitForSeconds(2f);
        Used = false;
    }
    IEnumerator Attackdelay()
    {
        if (ATKed)
        {
            ATKed = false;
            yield break;
        }
        float waitTime;
        waitTime = UnityEngine.Random.Range(0.15f, 0.3f);
        ATKed = true;
        yield return new WaitForSeconds(waitTime);

        if (myAnime != null&& ATKed)
        {
            myAnime._playOnce = true;
            myAnime.SetAnimation(myAnime.framesATK, myAnime.AtkTicks);
        }
        if (myAttack != null && ATKed)
        {
            myAttack.EnemyATK();
        }
        ATKed = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = false;
        RfrontHasFloor = false;
        LfrontHasFloor = false;
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 地板判定
                if (contact.normal.y > 0.8f)
                {
                    isGrounded = true;
                }
            }
        }
        // 1. 先跑完所有碰撞點，確認目前地板狀態
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                float relativeX = contact.point.x - transform.position.x;
                if (relativeX > offset) RfrontHasFloor = true;
                else if (relativeX < -offset) LfrontHasFloor = true;
            }
        }

        // 2. 如果正在冷卻，更新完地板狀態就結束，不執行轉向動作
        if (turnCooldown > 0)
        {
            turnCooldown -= Time.deltaTime;
            return;
        }

        // 3. 根據地板狀態決定是否轉身
        if (isGrounded)
        {
            // 往右走但右邊沒路
            if (!RfrontHasFloor && direction > 0f)
            {
                if (ATKing) { direction = 0.00001f; } // 攻擊中則停住
                else
                {
                    direction = -1f; // 巡邏中則轉身
                    turnCooldown = 0.3f;
                    ResetChangeTime();
                }
            }
            // 往左走但左邊沒路
            else if (!LfrontHasFloor && direction < 0f)
            {
                if (ATKing) { direction = -0.00001f; }
                else
                {
                    direction = 1f;
                    turnCooldown = 0.3f;
                    ResetChangeTime();
                }
            }
        }
    }

    void Awake()
    {
        myAttack = GetComponent<Monsterattack>();
        myAnime = GetComponent<Monsteranime>();
       
    }
}
