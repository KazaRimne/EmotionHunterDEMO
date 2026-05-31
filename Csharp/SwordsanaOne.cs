using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class SwordsanaOne : MonoBehaviour
{

    private float ATK;
    void Awake()
    {
      
    }

    void Start()
    {
        ATK = Datasana.Instance.ATK;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IHP Script = collision.GetComponent<IHP>();
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (Script != null)
            {
                if (Attacksana.Instance.UP)
                {
                    Cameracol.Instance.TriggerShake(0.15f, 0.2f, 1f);
                    float minDamage = ATK * 1.9f;
                    float maxDamage = ATK * 2.1f;
                    int damage = Mathf.RoundToInt(Random.Range(minDamage, maxDamage));
                    Script.TakeDamage(damage);
                    Busteffectsp();
                    ATKEff();
                    BALLEff();
                    DUSTEff();
                    RuntimeManager.PlayOneShot("event:/RBvelo0", transform.position);
                    StartCoroutine(DoHitStop(enemyRb, 0.08f));
                }
                else
                {
                    Cameracol.Instance.TriggerShake(0.08f, 0.14f, 1.5f);
                    if (Attacksana.Instance.Countend)
                    {
                        float minDamage = ATK * 1.0f;
                        float maxDamage = ATK * 1.5f;
                        int damage = Mathf.RoundToInt(Random.Range(minDamage, maxDamage));
                        Script.TakeDamage(damage);
                        RuntimeManager.PlayOneShot("event:/RBvelo0", transform.position);
                    }
                    else if (Attacksana.Instance.Count == 4|| Attacksana.Instance.Count == 3)
                    {
                        float minDamage = ATK * 0.2f;
                        float maxDamage = ATK * 0.7f;
                        int damage = Mathf.RoundToInt(Random.Range(minDamage, maxDamage));
                        Script.TakeDamage(damage);
                    }
                    else
                    {
                        float minDamage = ATK * 0.9f;
                        float maxDamage = ATK * 1.1f;
                        int damage = Mathf.RoundToInt(Random.Range(minDamage, maxDamage));
                        Script.TakeDamage(damage); 
                    }
                    Busteffectsp();
                    ATKEff();
                    DUSTEff();
                    if (Attacksana.Instance.Countend)
                    {
                        BALLEff();
                    }
                    StartCoroutine(DoHitStop(enemyRb, 0.08f));
                }
                RuntimeManager.PlayOneShot("event:/Hit_0", transform.position);
            }
            else
            {
                Debug.Log("Script == null檢查: MonoBehaviour, IHP");
            }
        }

        
    }
    private IEnumerator DoHitStop(Rigidbody2D rb, float duration)
    {
        if (rb == null) yield break;

        Vector2 originalVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        //if (anim != null) anim.speed = 0;

        // 等待期間，物件可能被 Destroy
        yield return new WaitForSeconds(duration);

        // 回復前再次檢查！！這一步最重要
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = originalVelocity;
            //if (anim != null) anim.speed = 1;
        }
    }

    void Busteffectsp()
    {

        bool isFacingRight = Movesana.Instance.Fright;
        float offsetX = isFacingRight ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0, 0);

        GameObject newSword = Objectpool.Instance.BustPool.Get();
        newSword.transform.position = spawnPos;
        newSword.transform.rotation = Quaternion.identity;

        // --- 關鍵修改：操作新生成的這一個，而不是 Instance ---
        Busteffect effectScript = newSword.GetComponent<Busteffect>();

        // 3. 強制斷開父子關係 (確保它在 World Space)
        newSword.transform.SetParent(null);

        // 4. 確保縮放正確
        newSword.transform.localScale = Vector3.one * 8f;
        // 5. 銷毀
        newSword.GetComponent<EffectDelayReturn>().StartCountdown(0.2f);
    }
    void ATKEff()
    {
        bool isFacingRight = Movesana.Instance.Fright;
        float offsetX = isFacingRight ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0, 0);

        // 1. 從池子撈出來（這時候物件池內部會觸發 SetActive(true)）
        GameObject newSword = Objectpool.Instance.AtkPool.Get();
        newSword.transform.position = spawnPos;
        newSword.transform.rotation = Quaternion.identity;

        newSword.GetComponent<EffectDelayReturn>().StartCountdown(0.5f);
    }
    void BALLEff()
    {

        bool isFacingRight = Movesana.Instance.Fright;
        float offsetX = isFacingRight ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0, 0);

        GameObject newSword = Objectpool.Instance.BallPool.Get();
        newSword.transform.position = spawnPos;
        newSword.transform.rotation = Quaternion.identity;

        // 叫特效自己倒數 2 秒後回歸池子
        newSword.GetComponent<EffectDelayReturn>().StartCountdown(2f);
    }
    void DUSTEff()
    {

        bool isFacingRight = Movesana.Instance.Fright;
        Vector3 spawnPos = transform.position + new Vector3(0, -0.5f, 0);

        GameObject newSword = Objectpool.Instance.DustPool.Get();
        newSword.transform.position = spawnPos;

        if (isFacingRight)
        {
            newSword.transform.rotation = Quaternion.Euler(-20, 90, -90);
            newSword.transform.localScale = new Vector3(newSword.transform.localScale.x, newSword.transform.localScale.y, 0.2f);
        }
        else
        {
            newSword.transform.rotation = Quaternion.Euler(20, 90, -90);
            newSword.transform.localScale = new Vector3(newSword.transform.localScale.x, newSword.transform.localScale.y, -0.2f);
        }
        newSword.GetComponent<EffectDelayReturn>().StartCountdown(0.5f);
    
   
    }
    public class EffectPoolDelayReturn : MonoBehaviour
    {
        public IObjectPool<GameObject> targetPool;

        public void StartReturnCountdown(float delay)
        {
            StopAllCoroutines();
            StartCoroutine(DelayReturn(delay));
        }

        private IEnumerator DelayReturn(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (targetPool != null)
            {
                targetPool.Release(gameObject); // 安全回收
            }
        }
    }
}
