using UnityEngine;

public class AutoMoveRight : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // In log mỗi frame
        Debug.Log("AutoMoveRight.Update chạy, time=" + Time.time);

        // Kéo nhân vật sang phải
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
