using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // player yang akan diikuti kamera
    public float smootSpeed; // kecepatan kamera mengikuti player
    public Vector3 offset; // jarak offset antara kamera dan player

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }
        Vector3 camPos = player.position + offset; // posisi kamera berdasarkan posisi player dan offset
        Vector3 smoothMove = Vector3.Lerp(
            transform.position,
            camPos,
            smootSpeed);
        transform.position = smoothMove;
    }
}