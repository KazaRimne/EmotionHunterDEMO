using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Attacksana : MonoBehaviour
{
    private AnimeSANA _sana;
    public static Attacksana Instance { get; private set; }
    public GameObject SwordsanaPrefab; // 劍的預製體

    public int Count = 0;
    public float STimer = 0f;
    public float STimerR = 0f;
    private float noClickTimer = 0f;
    private float attackCooldown = 0.2f;
    private float lastAttackInputTime = -10f;
    public float AttackBufferTime = 0.15f;
    public bool isAttacking = false;
    public bool four = false;
    public bool UP = false;
    public bool Countend = false;
    public bool Swupuse = false;
    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        four = false;
        UP = false;
        Swupuse = false;
        Count = 0;
        Countend = false;
        _sana = GetComponent<AnimeSANA>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Movesana.Instance.Die|| ESCSTOP.Instance.isPaused) return;

        attackCooldown += Time.deltaTime;
        noClickTimer += Time.deltaTime;
        if (STimerR > 0)
        {
            STimerR -= Time.deltaTime; // 讓冷卻時間慢慢減少
        }
        if (isAttacking == true)
        {
            float atktrueTimer = 0f;
            atktrueTimer += Time.deltaTime;
            if (atktrueTimer > 5f)
            {
                isAttacking = false;
            }
        }
        if (Input.GetKey(KeyCode.S) && Movesana.Instance.isGrounded
            && Datasana.Instance.Stamina >= 60)
        {
            four = false;
            Swupuse = true;
            STimer += Time.deltaTime;
            bool isCurrentlySwupTwo = _sana.stateMachine.CurrentState is SwupTwo;
            if (!isCurrentlySwupTwo && STimerR <= 0f)
            {
                _sana.stateMachine.ChangeState(new Swupone(_sana));
            }
            if (Input.GetMouseButtonDown(0) && STimer <= 1f && (Swupuse))
            {
                STimer = 0f;
                STimer += Time.deltaTime;
                STimerR = 2.0f;
                Datasana.Instance.Stamina -= 60;

                noClickTimer = 0f;
                attackCooldown = 0f;
                UP = true;
                _sana.stateMachine.ChangeState(new SwupTwo(_sana));
                RuntimeManager.PlayOneShot("event:/SanaatkUP", transform.position);
                SwordFlashMOVE();
                Count = 0;
                SpawnSword();
                StartCoroutine(AttackFlow(0.5f));
            }
        }
        else
        {
            Swupuse = false;
            STimer = 0f;
            if (UP)
            {
                StartCoroutine(UPIEnum(0.1f));
            }
        }
        if (Datasana.Instance.Stamina >= 30 && (!Movesana.Instance.wallLeft)
            && (!Movesana.Instance.wallRight) && (!Swupuse))
        {
            if (noClickTimer >= 1f)
            {
                Countend = false;
                four = false;
                Count = 0;
                isAttacking = false;
            }
            if (Input.GetMouseButtonDown(0)) // 0 = 左鍵
            {
                
               
                if (isAttacking && attackCooldown < GetCurrentActionTime()) return;
             
                lastAttackInputTime = Time.time;
                  ATKsana();
                    noClickTimer = 0f; 
                    
                
            }

           
        }

        
        float GetCurrentActionTime()
        {
            if (Count == 0)
            {
                return 0.5f; // 第一刀時間
            }
            else if (Count == 4)
            {
                return 0.333f; // 第一刀時間
            }
            else
            {
                return 0.5f;
            }
        }
        

        void SwordFlashMOVE()
        {
            
                if (Swupuse)
                {
                    Swup.Instance.PlaySwup();
                }
                else if (Count == 0)
                {
                    Flash0.Instance.PlayFlash();
                    Flash01.Instance.PlayFlash();
                }
                else if (Count == 1)
                {
                    Flash1.Instance.PlayFlash();
                    Flash11.Instance.PlayFlash();
                }
                else if (Count == 2)
                {
                    StartCoroutine(CrossATK());
                }
                else if (Count == 4)
                {
                    Flash4 .Instance.PlayFlash();
                    Flash41.Instance.PlayFlash();
                }

        }


        void ATKsana()
        {
            StartCoroutine(Movesana.Instance.ATKmove());
            if (Count == 0 && Time.time - lastAttackInputTime
                        <= AttackBufferTime && attackCooldown >= 0.175f)
            {
                four = false;
                Datasana.Instance.Stamina -= 30;
                noClickTimer = 0f;
                attackCooldown = 0f;
                _sana.stateMachine.ChangeState(new ATK(_sana));
                Countend = false;
                RuntimeManager.PlayOneShot("event:/Sanaatk0", transform.position);
                SwordFlashMOVE();
                Count++;
                SpawnSword();
                StartCoroutine(AttackFlow(0.3f));

            }
            else if (Count == 1 && Time.time - lastAttackInputTime
                <= AttackBufferTime && attackCooldown >= 0.25f)
            {
                Datasana.Instance.Stamina -= 30;
                noClickTimer = 0f;
                attackCooldown = 0f;
                _sana.stateMachine.ChangeState(new ATK(_sana));
                RuntimeManager.PlayOneShot("event:/Sanaatk1", transform.position);
                SwordFlashMOVE();
                Count++;
                SpawnSword();
                StartCoroutine(AttackFlow(0.25f));

            }
            else if (Count == 2 && Time.time - lastAttackInputTime <= AttackBufferTime
                && attackCooldown >= 0.2f)
            {
                Datasana.Instance.Stamina -= 30;
                noClickTimer = 0f;
                attackCooldown = 0f;
                _sana.stateMachine.ChangeState(new ATK(_sana));
                RuntimeManager.PlayOneShot("event:/Sanaatk2", transform.position);
                SwordFlashMOVE();
                Count++;
                SpawnSword();
                StartCoroutine(AttackFlow(0.3f));
            }
            //else if (Count == 3 && Time.time - lastAttackInputTime <= AttackBufferTime
            //    && attackCooldown >= 0.15f)
            //{
            //    Datasana.Instance.Stamina -= 20;
            //    noClickTimer = 0f;
            //    attackCooldown = 0f;
            //    _sana.stateMachine.ChangeState(new ATK(_sana));
            //    SwordFlashMOVE();
            //    Count++;
            //    SpawnSword();
            //    StartCoroutine(AttackFlow(0.15f));
            //}

            else if (Count == 4 && Time.time - lastAttackInputTime <= AttackBufferTime
                && attackCooldown >= 0.3f && Datasana.Instance.Stamina >= 40) // 0 = 左鍵
            {
                Datasana.Instance.Stamina -= 30;

                noClickTimer = 0f;
                attackCooldown = 0f;
                four = true;
                Countend = true;
                _sana.stateMachine.ChangeState(new ATK(_sana));
                RuntimeManager.PlayOneShot("event:/Sanaatk3", transform.position);
                SwordFlashMOVE();
                Count = 0;
                SpawnSword();
                StartCoroutine(AttackFlow(0.25f));

            }
        }
    }
    void SpawnSword()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 localPos;
        bool isFacingRight = Movesana.Instance.Fright; // 抓取角色面朝方向

        if (Mouseca.Instance.mouseright)
        {
            localPos = isFacingRight ? new Vector3(0.2f, 0f, 0f) : new Vector3(-0.2f, 0f, 0f);
        }
        else
        {
            localPos = isFacingRight ? new Vector3(-0.2f, 0f, 0f) : new Vector3(0.2f, 0f, 0f);
        }
        GameObject newSword = Instantiate(SwordsanaPrefab, spawnPosition, Quaternion.identity);
        newSword.transform.SetParent(transform);
        newSword.transform.localPosition = localPos;

        Vector3 s = newSword.transform.localScale;
        s.x = Mathf.Abs(s.x);
        newSword.transform.localScale = s;

        Destroy(newSword, 0.3f);
    }
    IEnumerator CrossATK()
    {
        Flash2.Instance.PlayFlash();
        Flash21.Instance.PlayFlash();
        Count++;
        yield return new WaitForSeconds(0.2f);
        SpawnSword();
        Flash3.Instance.PlayFlash();
        Flash31.Instance.PlayFlash();
    }
   public IEnumerator AttackFlow(float waitTime)
    {
        isAttacking = true;
        yield return new WaitForSeconds(waitTime);
        isAttacking = false; // 動作結束，解開鎖定
    }
    IEnumerator UPIEnum(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        UP = false; // 動作結束，解開鎖定
    }

    void Awake()
    {
        Instance = this;
    }
}
