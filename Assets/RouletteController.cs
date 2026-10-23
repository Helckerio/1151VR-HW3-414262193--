using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RouletteController : MonoBehaviour
{
    [Header("把 .mp3 或 .wav 音檔直接拖進下面這個格子！")]
    public AudioClip soundFile;

    public float maxSpeed = 800f;
    public float acceleration = 500f;
    public float deceleration = 200f;

    private float currentSpeed = 0f;
    private int state = 0; // 0: 停止, 1: 自動加速中, 2: 自動減速中
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    void Update()
    {
        bool isTriggered = false;

        // 同時支援：電腦空白鍵、滑鼠左鍵、以及「手機螢幕手指觸控」！
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isTriggered = true;
        }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isTriggered = true;
        }
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            isTriggered = true;
        }
#else
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            isTriggered = true;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isTriggered = true;
        }
#endif

        // 當觸發時，開始加速並播放音效
        if (isTriggered && state == 0)
        {
            state = 1;
            if (soundFile != null)
            {
                audioSource.clip = soundFile;
                audioSource.Play();
            }
            else if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        // 狀態 1：自動加速到最高速
        if (state == 1)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
                state = 2; // 加速到頂後，自動進入減速狀態
            }
        }
        // 狀態 2：自動減速直到完全停止
        else if (state == 2)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed <= 0f)
            {
                currentSpeed = 0f;
                state = 0; // 完全停止
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        // 執行旋轉 (Z軸)
        transform.Rotate(0f, 0f, -currentSpeed * Time.deltaTime);
    }
}