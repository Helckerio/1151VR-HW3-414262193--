using UnityEngine;

public class RouletteController : MonoBehaviour
{
    public float maxSpeed = 800f;
    public float acceleration = 400f;
    public float deceleration = 200f;
    private float currentSpeed = 0f;
    private int state = 0; // 0: 停止, 1: 加速, 2: 減速
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 按下空白鍵開始加速
        if (Input.GetKeyDown(KeyCode.Space) && state == 0)
        {
            state = 1;
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        // 放開空白鍵開始減速
        else if (Input.GetKeyUp(KeyCode.Space) && state == 1)
        {
            state = 2;
        }

        // 狀態速度計算
        if (state == 1)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
        else if (state == 2)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
                state = 0; // 完全停止
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }

        // 執行旋轉 (Z軸)
        transform.Rotate(0, 0, -currentSpeed * Time.deltaTime);
    }
}