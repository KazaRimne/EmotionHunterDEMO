using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Mouseca : MonoBehaviour
{
    public static Mouseca Instance { get; private set; }
    public bool mouseright = true;
    // 讓其他檔案讀取的變數
    public float currentAngle;
    // 如果你也想讓別人知道發射方向向量，可以多加這個
    public Quaternion aimRotation;
    // 四元數 (Quaternion) 
    public bool isManualAim;
    public Vector2 shootDirection;
    public Vector2 targetPos;
    private Transform player;
    public GameObject TargEnemy;
    public GameObject SanalockPrefab;
    private GameObject lastTargEnemy;

    public GameObject Sanalockcurrent;
    GameObject Pplayer;
    // Start is called before the first frame update
    void Start()
    {
        mouseright = true;
        GameObject Pplayer = GameObject.FindGameObjectWithTag("Player");
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
        Vector3 mouseInput = Input.mousePosition;
        mouseInput.z = 10f;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePos2D = Camera.main.ScreenToWorldPoint(mouseInput);
        if (mousePos2D.x >= player.position.x)
        {
            mouseright = true;
        }
        else
        {
            mouseright = false;
        }
        if (TargEnemy == null)
        {
            lastTargEnemy = null;
            if (Sanalockcurrent != null)
            {
                Destroy(Sanalockcurrent);
            }
        }
        if (Input.GetMouseButtonDown(1)) // 1 是右鍵
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                GameObject clickedEnemy = hit.collider.gameObject;
                    if (TargEnemy == clickedEnemy)
                    {
                        // 點到的是同一個敵人 → 取消鎖定
                        TargEnemy = null;
                    }
                    else
                    {
                        TargEnemy = clickedEnemy;
                    }
                
            }
        }
        isManualAim = Input.GetMouseButton(1); // 檢查右鍵是否「正被按著」
        if (!isManualAim&& TargEnemy == null)
        {
            targetPos = Vector2.zero;
        }
        if (isManualAim)
        {
            targetPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        }
        else if (TargEnemy != null)
        {
            if (TargEnemy.name.Contains("boss"))
            {
                targetPos = new Vector3(TargEnemy.transform.position.x, 
                    TargEnemy.transform.position.y - 3f, TargEnemy.transform.position.z);
            }
            else
            {
                targetPos = TargEnemy.transform.position;
            }
            mouseright = (targetPos.x >= player.position.x);
            Sanalock();
            if ((TargEnemy.transform.position - player.position).sqrMagnitude > 576f)
            {
                TargEnemy = null;
                Debug.Log("距離太遠，解除鎖定");
            }
        }
        else
        {
            return;
        }
        Vector2 direction = targetPos - (Vector2)transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            // 1. 計算角度並存入 public 變數
            currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 2. 將角度轉為四元數並存起來 (子彈會用到)
            // 如果你的子彈圖片是面朝上的，記得在這裡 angle - 90f
            aimRotation = Quaternion.Euler(0, 0, currentAngle);
            // 2. (選配) 同步存入方向向量，這對 Rigidbody2D 移動很有幫助
            shootDirection = direction.normalized;

        }

    }
    void Sanalock()
    {
        // 檢查目標是否為空，避免報錯
        if (TargEnemy == null) return;

        // --- 邏輯 A：如果換了目標，就摧毀並重生成 ---
        if (TargEnemy != lastTargEnemy)
        {
            if (Sanalockcurrent != null)
            {
                Destroy(Sanalockcurrent);
            }

            Vector3 spawnPosition = TargEnemy.transform.position;
            Sanalockcurrent = Instantiate(SanalockPrefab, spawnPosition, Quaternion.identity);
            if (TargEnemy.name.Contains("SAKURA"))
            {
                var rotateScript = Sanalockcurrent.GetComponent<LockRota>();
                rotateScript.Setup(TargEnemy.transform, new Vector3(-0.4f, -1.5f, 0f));
                //Renderer[] allRenderers = Sanalockcurrent.GetComponentsInChildren<Renderer>();

                //// 使用 foreach 迴圈，將抓到的每一個 Renderer 都關閉
                //foreach (Renderer r in allRenderers)
                //{
                //    r.enabled = false;
                //}
            }
            if (TargEnemy.name.Contains("boss"))
            {
                var rotateScript = Sanalockcurrent.GetComponent<LockRota>();
                rotateScript.Setup(TargEnemy.transform, new Vector3(-0f, -3f, 0f));
            }

            // 更新最後紀錄的目標
            lastTargEnemy = TargEnemy;
        }

        // --- 邏輯 B：每一幀都更新位置，確保跟隨目標移動 ---
        // 只有在物件存在時才移動，避免在 Destroy 的那一瞬間出錯
        if (Sanalockcurrent != null)
        {
            Sanalockcurrent.transform.position = TargEnemy.transform.position;
        }
    }
        void Awake()
    {
        Instance = this;
    }
}
