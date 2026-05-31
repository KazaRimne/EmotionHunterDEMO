using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cameracol : MonoBehaviour
{
    public static Cameracol Instance { get; set; }
    // Start is called before the first frame update
    public GameObject target;
    public bool center;
    public int currentTargetIndex = 0;
    public float smoothSpeed = 0.01f;        // 平滑速度
    private Vector3 _velocity = Vector3.zero;
    public float edgeXPercent = 0.3f;  // 左右邊界百分比
    public float edgeYPercent = 0.2f;
    public Vector3 offset = new Vector3(0, 0, -10);
    private Camera cam;
    private Vector3 targetPos;
    private float _shakeDuration = 0f;
    private float _shakeMagnitude = 0.5f;
    private float _dampingSpeed = 1.0f;

    // 用於 Noise 採樣的隨機起點
    private float _noiseSeed;
    void Start()
    {
        center = true;
        _noiseSeed = Random.value * 100f;
        cam = GetComponent<UnityEngine.Camera>();
        target = GameObject.FindWithTag("Player");
        if (cam == null)
        {
            Debug.LogError("Camera 取得失敗");
        }
    }
    void Update()
    {
        targetPos = target.transform.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            center = !center;
        }
        if (_shakeDuration > 0)
        {
            if (center)
            {

                // 使用 Perlin Noise 產生平滑且有方向感的位移
                // 乘以時間讓 Noise 隨時間流動
                float x = (Mathf.PerlinNoise(_noiseSeed, Time.time * 25f) - 0.5f) * 2f;
                float y = (Mathf.PerlinNoise(_noiseSeed + 1, Time.time * 25f) - 0.5f) * 2f;

                transform.position += new Vector3(x, y, 0) * _shakeMagnitude;

                _shakeDuration -= Time.deltaTime * _dampingSpeed;
            }
            else
            {
                _shakeDuration = 0f;
            }
        }
        else
        {
            _shakeDuration = 0f;
        }
    }
        void LateUpdate()

    {
        if (target == null || cam == null) return;
       
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = cam.orthographicSize * cam.aspect;

        // 2. 算出玩家相對於鏡頭中心的距離
        float diffX = targetPos.x - transform.position.x;
        float diffY = targetPos.y - transform.position.y;

        // 3. 算出臨界點 (例如畫面的 30% 處)
        float thresholdX = camHalfWidth * (1f - edgeXPercent);
        float thresholdY = camHalfHeight * (1f - edgeYPercent);

        // 4. 計算理想的目標位置 (這部分最關鍵：不要用 camPos 去加)
        Vector3 finalTargetPos = transform.position;

        if (center)
        {
            
            Vector3 currentTargetPos = target.transform.position;

            Vector3 targetPos2 = new Vector3(currentTargetPos.x, currentTargetPos.y+1f, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPos2, 0.5f);

            if ((transform.position - targetPos2).sqrMagnitude < 0.001f)
            {
                transform.position = targetPos2;
            }
        }
        else
        {// 如果玩家超過右邊界
            if (diffX > thresholdX)
                finalTargetPos.x = targetPos.x - thresholdX;
            // 如果玩家超過左邊界
            else if (diffX < -thresholdX)
                finalTargetPos.x = targetPos.x + thresholdX;

            // 如果玩家超過上邊界
            if (diffY > thresholdY)
                finalTargetPos.y = targetPos.y - thresholdY;
            // 如果玩家超過下邊界
            else if (diffY < -thresholdY)
                finalTargetPos.y = targetPos.y + thresholdY;

            finalTargetPos.z = offset.z;

            // 5. 執行平滑移動 (使用 SmoothDamp 徹底解決抖動)
            transform.position = Vector3.SmoothDamp(
                transform.position,
                finalTargetPos,
                ref _velocity,
                0.0000000001f
            );
        }
    }
    public void TriggerShake(float duration, float magnitude, float damping = 1.0f)
    {
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _dampingSpeed = damping;
    }
    void Awake()
    {
        Instance = this;
    }
}

