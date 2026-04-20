using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("Camera Settings")]
    public Transform player; // player yang akan diikuti kamera
    public float smootSpeed; // kecepatan kamera mengikuti player
    public Vector3 offset; // jarak offset antara kamera dan player


    [Header("Max Camera XY")]
    public float minX; // batas minimum posisi x kamera
    public float maxX; // batas maksimum posisi x kamera
    public float minY; // batas minimum posisi y kamera
    public float maxY; // batas maksimum posisi y kamera
    
    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 targetPos = player.position + offset; // posisi kamera berdasarkan posisi player dan offset

        float clampedX = Mathf.Clamp(player.position.x, minX, maxX); // membatasi posisi x kamera
        float clampedY = Mathf.Clamp(player.position.y, minY, maxY); // membatasi posisi y kamera

        Vector3 camPos = new Vector3(clampedX, clampedY, player.position.z) + offset; // posisi kamera berdasarkan posisi player dan offset
        Vector3 smoothMove = Vector3.Lerp(
            transform.position,
            camPos,
            smootSpeed);
        transform.position = smoothMove;
    }
}