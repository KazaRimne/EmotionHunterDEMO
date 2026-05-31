using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class ATK_flower : MonoBehaviour, IHP

{
    [Header("Anime")]

    public Sprite[] frames;
    public Sprite[] framesATK;

    public GameObject HPbar;
    public GameObject bulletPrefab;


    public float MaxHP;
    public float MonsterHP;
    public float MonsterATK;
    public bool Fright;
    public float rotateSpeed = 10f;
    public bool Canmove = true;
    public LayerMask Ground;
    public LayerMask Player;
    public int defaultTick = 6;
    public float PlaySEtime = 0.3333333f;
    public int[] Ticks = { 6,24, 24};
    public int[] StandTicks = { 48 };

    public bool ATKAnime = false;

    private const float SEC_PER_FRAME = 1f / 24f;
    private SpriteRenderer _spriteRenderer;
    private float bulletSpeed;
    private float Xdistance;
    private float Ydistance;
    private float Cosdistance;
    private float Cosangle;
    private  float targetAngle ;

    private Sprite[] _currentActiveFrames;
    private float _timer;
    private int[] _currentTicks;
    private int _currentFrame;

    GameObject ThisObject;

    private bool isGrounded;
    private Rigidbody2D rb;
    private Transform player;
    private GameObject HPbarInstance;
    GameObject Pplayer;
    private SpriteRenderer FlowerRenderer;
    private Transform FlowerTransform;
    private bool isAttacking = false;
    Vector3 scale;
    public float EnemyHP => MonsterHP;
    void Start()
    {
        if (Movesana.Instance.Die) return;
        Canmove = true;
        Fright = false;
        targetAngle = 0f;
        rb = GetComponent<Rigidbody2D>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (Onedata.Instance != null)
        {
            StartCoroutine(StartCD(0.5f));
        }
        else
        {
            Debug.LogError("Onedata == null");
        }
        ThisObject = this.gameObject;
        scale = ThisObject.transform.localScale;

        GameObject Pplayer = GameObject.FindGameObjectWithTag("Player");
        FlowerTransform = ThisObject.transform.Find("Flower");
        if (FlowerTransform != null)
        {
            FlowerRenderer = FlowerTransform.GetComponent<SpriteRenderer>();

            // 初始狀態：先隱藏起來
            FlowerRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("找不到名為 Flower 的子物件！");
        }


        if (Pplayer != null)
        {
            player = Pplayer.transform;
        }
        else
        {
            Debug.LogError("找不到 Tag: Player 的物件！");
        }
    }

    // Update is called once per frame
    void Update()
    {
       
            Anime();
            if (Canmove&& isGrounded)
        {
            LayerMask combinedMask = Ground | Player;
            Xdistance = player.position.x - transform.position.x;
            Ydistance = player.position.y - transform.position.y;
            Vector3 direction = player.position - transform.position;

            Cosdistance = Vector3.Distance(transform.position, player.position);
            if (Cosdistance > 0.001f)
            {
                Cosangle = Xdistance / Cosdistance;
            }
            if (Cosdistance <= 12f)
            {
                float rayLength = 12f;
                Vector2 rayDirection = new Vector2(0, 0);
                RaycastHit2D hiit = Physics2D.Raycast(rb.position,
                    new Vector2(Xdistance, Ydistance), 12f, combinedMask);

                // 在 Scene 視窗畫出綠線，方便你確認射線長度有沒有穿出身體
                Debug.DrawRay(rb.position, rayDirection * rayLength, Color.red);
                if (hiit.collider != null)
                {
                    if (((1 << hiit.collider.gameObject.layer) & Ground) != 0)
                    {
                        FlowerRenderer.enabled = false;
                        return;
                    }
                    else if (((1 << hiit.collider.gameObject.layer) & Player) != 0)
                    {


                        if (Cosangle >= 0)
                        {
                            Fright = true;
                        }
                        else
                        { Fright = false; }

                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


                        if (Fright)
                        {
                            targetAngle = angle;
                        }
                        else
                        {
                            targetAngle = -(angle - 180f);
                        }

                        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

                        // 2. 使用 RotateTowards 慢慢轉過去 (當前, 目標, 速度)
                        FlowerTransform.localRotation = Quaternion.RotateTowards(
                            FlowerTransform.localRotation,
                            targetRot,
                            rotateSpeed * 20f * Time.deltaTime
                        );
                        if (Ydistance >= 0)
                        {
                            if (Mathf.Abs(Cosangle) >= 0.1736f)
                            {
                                Tane();
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(Cosangle) >= 0.6428f)
                            {
                                Tane();
                            }
                        }


                        if (Xdistance >= 0)
                        {
                            Fright = true;
                            scale.x = Mathf.Abs(scale.x);
                            transform.localScale = scale;
                        }
                        else
                        {
                            Fright = false;
                            scale.x = -Mathf.Abs(scale.x);
                            transform.localScale = scale;
                        }

                    }
                }
            }

            else
            {
                FlowerRenderer.enabled = false;
            }

        }
    }
    
    public void TakeDamage(float damage)
    {
        SpawnHPBar();
        MonsterHP -= Mathf.CeilToInt(damage); 
        if (MonsterHP <= 0)
        {
            Die();
        }
    }
    public void SpawnHPBar()
    {
        if (HPbarInstance != null) return;

        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Vector3 spawnPos = new Vector3(col.bounds.center.x, col.bounds.max.y + 0.5f, transform.position.z);

        // 1. 生成
        HPbarInstance = Instantiate(HPbar, spawnPos, Quaternion.identity);

        // 2. 獲取腳本
        EnemyHPbar script = HPbarInstance.GetComponent<EnemyHPbar>();
        if (script != null)
        {
            // 直接從發射端把數值塞進去
            float initialHP = Onedata.Instance.FlowerHP;
            script.maxHP = initialHP;
            // 額外保險：如果 currentHP 也是 0，一併初始化它
            script.currentHP = initialHP;
        }

        // 3. 設定縮放與父子關係
        HPbarInstance.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        HPbarInstance.transform.SetParent(this.transform, true);
    }
    private void Tane()
    {
        if (this == null || gameObject == null) return;
        if (isAttacking || bulletPrefab == null) return;
        
        RuntimeManager.PlayOneShot("event:/Enemyshot0", transform.position);
        StartCoroutine(PlaySE());
        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float visualOffset = -90f;
        Quaternion visualRotation = Quaternion.Euler(0, 0, angle + visualOffset);

        // 1. 改從主角身上的物件池拿取子彈，並移至發射位置與角度
        GameObject bullet = Objectpool.Instance.TanePool.Get();
        if (bullet == null)
        {
            Debug.Log($"[ObjectPool 錯誤] 物件池吐出了一個已經被銷毀的子彈空殼！");
            return;
        }

        // 再次確認 FlowerTransform 沒事
        if (FlowerTransform == null)
        {
            Debug.Log($"[Flower 錯誤] 當前這朵花的 FlowerTransform 是 Null！可能在 Update 期間花被拆了。");
            return;
        }
        bullet.transform.position = FlowerTransform.position;
        bullet.transform.rotation = visualRotation;

        // 2. 獲取 Collider 並忽略與花的碰撞
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D flowerCol = GetComponent<Collider2D>(); // 獲取花自己的 Collider

        if (bulletCol != null && flowerCol != null)
        {
            // 核心代碼：讓這兩者不會互撞
            Physics2D.IgnoreCollision(bulletCol, flowerCol);
        }

        Tane bulletScript = bullet.GetComponent<Tane>();
        if (bulletScript != null)
        {
            bulletScript.Fright = this.Fright;
            bulletScript.MonsterATK = this.MonsterATK;
        }

        // 3. 給予物理速度（進入前重置物理速度，防止繼承舊速度）
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Vector3 fireDirection = (player.position - FlowerTransform.position).normalized;
            bulletSpeed = UnityEngine.Random.Range(
                Onedata.Instance.Tanestartspeed, Onedata.Instance.Tanespeed);
            rb.velocity = fireDirection * bulletSpeed;
        }

        StartCoroutine(TaneCD());
    }

    private void Die()
    {
        StopAllCoroutines();
        this.enabled = false;
        Destroy(gameObject);
    }

    private void Anime()
    {
        if (_currentActiveFrames == null || _currentActiveFrames.Length == 0) return;

        _timer += Time.deltaTime;

        int ticksForThisFrame = (_currentTicks != null && _currentFrame < _currentTicks.Length)
                                ? _currentTicks[_currentFrame]
                                : defaultTick;

        float targetTime = ticksForThisFrame * SEC_PER_FRAME;

        if (_timer >= targetTime)
        {
            _timer -= targetTime; // 使用 -= 而不是重置為 0，動畫會更平滑
            _currentFrame++;

            // 如果是攻擊動畫且播到了最後一幀，不要循環（可選）
            if (_currentActiveFrames == framesATK && _currentFrame >= _currentActiveFrames.Length)
            {
                _currentFrame = _currentActiveFrames.Length - 1; // 停在最後一格
            }
            else
            {
                _currentFrame %= _currentActiveFrames.Length;
            }

            _spriteRenderer.sprite = _currentActiveFrames[_currentFrame];
        }
    }
    public void SetAnimation(Sprite[] newFrames, int[] ticks)
    {
        if (_currentActiveFrames == newFrames) return;

        _currentActiveFrames = newFrames;
        _currentTicks = ticks;
        _currentFrame = 0;
        _timer = 0;

        if (newFrames != null && newFrames.Length > 0)
            _spriteRenderer.sprite = newFrames[0];
    }

    IEnumerator TaneCD()
    {
        isAttacking = true;

        // 1. 取得隨機冷卻時間
        float randomDelay = UnityEngine.Random.Range(
            Onedata.Instance.Tanestartcd, Onedata.Instance.Tanecd);

        // 2. 計算動畫播放需要的總時間
        // 假設 framesATK 有 3 幀，Ticks 分別是 6, 24, 12 (總共 42 Ticks)
        // 總秒數 = 42 * (1/24f) = 1.75 秒
        float animeDuration = 0f;
        foreach (int t in Ticks) { animeDuration += t * SEC_PER_FRAME; }

        // 3. 播放攻擊動畫
        SetAnimation(framesATK, Ticks);
        float remainingWait = AtkAnimeDuration;
        StartCoroutine(FlowerRendtrue(remainingWait));

        // 4. 等待動畫播完 (或者等待 delay，看哪個長)
        yield return new WaitForSeconds(animeDuration);

        // 5. 動畫播完後切換回靜止動畫
        SetAnimation(frames, StandTicks);
        
        // 6. 如果冷卻時間比動畫長，補足剩下的等待時間
        float remainingCD = randomDelay - animeDuration;
        if (remainingCD > 0)
        {
            yield return new WaitForSeconds(remainingCD);
        }

        isAttacking = false;
    }
    public float AtkAnimeDuration
    {
        get
        {
            if (Ticks == null || Ticks.Length == 0) return 0f;

            float totalTicks = -7.2f;
            foreach (int t in Ticks)
            {
                totalTicks += t;
            }
            return totalTicks / 24f;
        }
    }
    private IEnumerator FlowerRendtrue(float duration)
    {
        yield return new WaitForSeconds(AtkAnimeDuration);
        FlowerRenderer.enabled = true;
        yield return new WaitForSeconds(0.25f);
        FlowerRenderer.enabled = false;
    }
    private IEnumerator StartCD(float duration)
    {
        yield return new WaitForSeconds(duration);
        MonsterHP = Onedata.Instance.FlowerHP;
        MaxHP = Onedata.Instance.FlowerHP;
        MonsterATK = Onedata.Instance.FlowerATK;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = false;
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
    }
    private IEnumerator PlaySE()
    {
        yield return new WaitForSeconds(PlaySEtime);
    }
}
