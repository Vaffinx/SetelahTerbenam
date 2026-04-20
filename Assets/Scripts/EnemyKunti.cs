using UnityEngine;
using System.Collections;

public class EnemyKunti : MonoBehaviour
{
    // ============================================================
    // ENUM: State Machine - semua state yang dimiliki Kuntilanak
    // ============================================================
    private enum KuntiState
    {
        Idling,      // Diam di tempat, menunggu
        Wandering,   // Terbang berkeliling dalam area batas
        FindPlayer,  // Mengejar player
        Search       // Mencari player di posisi terakhir
    }

    // ============================================================
    // PENGATURAN MOVEMENT (atur di Inspector)
    // ============================================================
    [Header("Movement")]
    [SerializeField] private float wanderSpeed = 2f;       // Kecepatan saat berkeliling
    [SerializeField] private float chaseSpeed = 4f;        // Kecepatan saat mengejar player
    [SerializeField] private float searchSpeed = 1.5f;     // Kecepatan saat mencari player
    [SerializeField] private float stunDuration = 3f;
    private bool IsStunned = false;

    // ============================================================
    // PENGATURAN DETEKSI PLAYER
    // ============================================================
    [Header("Player Detection")]
    [SerializeField] private float detectionRadius = 6f;   // Jarak deteksi player
    [SerializeField] private LayerMask playerLayer;        // Layer untuk mendeteksi player
    [SerializeField] private int hidingLayer = 7;          // Layer player saat bersembunyi

    // ============================================================
    // PENGATURAN WAKTU
    // ============================================================
    [Header("Timing")]
    [SerializeField] private float idleTimeMin = 1f;       // Waktu diam minimum
    [SerializeField] private float idleTimeMax = 3f;       // Waktu diam maksimum
    [SerializeField] private float searchDuration = 4f;    // Berapa lama mencari player

    // ============================================================
    // BATAS AREA WANDERING
    // ============================================================
    [Header("Wander Boundary")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 6f;

    // ============================================================
    // VARIABEL INTERNAL
    // ============================================================
    private KuntiState currentState = KuntiState.Idling;
    private Transform player;              // Referensi ke player
    private SpriteRenderer sr;             // Untuk flip sprite

    private Vector2 wanderTarget;          // Titik tujuan wandering
    private Vector2 lastKnownPlayerPos;    // Posisi terakhir player terlihat

    private float stateTimer = 0f;         // Timer untuk state saat ini
    private float arrivalThreshold = 0.2f; // Jarak minimum dianggap "sampai"

    // ============================================================
    // START
    // ============================================================
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Cari player di scene berdasarkan tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyKunti: Player dengan tag 'Player' tidak ditemukan!");
        }

        // Mulai dari state Idling
        EnterState(KuntiState.Idling);
    }

    // ============================================================
    // UPDATE - Jalankan state yang aktif setiap frame
    // ============================================================
    void Update()
    {
        // Kalau sedang d stun, jangan lakukan apapun (jangan pindah state, jangan bergerak)
        if (IsStunned) return;

        switch (currentState)
        {
            case KuntiState.Idling:
                UpdateIdling();
                break;
            case KuntiState.Wandering:
                UpdateWandering();
                break;
            case KuntiState.FindPlayer:
                UpdateFindPlayer();
                break;
            case KuntiState.Search:
                UpdateSearch();
                break;
        }
    }

    // ============================================================
    // ENTER STATE - Dipanggil saat berpindah ke state baru
    // ============================================================
    private void EnterState(KuntiState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case KuntiState.Idling:
                // Set timer random untuk berapa lama diam
                stateTimer = Random.Range(idleTimeMin, idleTimeMax);
                break;

            case KuntiState.Wandering:
                // Pilih titik random dalam area batas
                wanderTarget = GetRandomPointInBounds();
                break;

            case KuntiState.FindPlayer:
                // Tidak ada setup khusus, langsung kejar
                break;

            case KuntiState.Search:
                // Set timer untuk berapa lama mencari
                stateTimer = searchDuration;
                break;
        }
    }

    // ============================================================
    // STATE: IDLING - Diam di tempat
    // ============================================================
    private void UpdateIdling()
    {
        // Kurangi timer
        stateTimer -= Time.deltaTime;

        // Selalu cek apakah player terdeteksi
        if (CanSeePlayer())
        {
            EnterState(KuntiState.FindPlayer);
            return;
        }

        // Kalau timer habis, pindah ke Wandering
        if (stateTimer <= 0f)
        {
            EnterState(KuntiState.Wandering);
        }
    }

    // ============================================================
    // STATE: WANDERING - Terbang ke titik random
    // ============================================================
    private void UpdateWandering()
    {
        // Selalu cek apakah player terdeteksi
        if (CanSeePlayer())
        {
            EnterState(KuntiState.FindPlayer);
            return;
        }

        // Gerak ke arah target
        MoveTowards(wanderTarget, wanderSpeed);

        // Flip sprite sesuai arah gerak
        FlipSprite(wanderTarget.x);

        // Kalau sudah sampai di target, diam dulu
        if (Vector2.Distance(transform.position, wanderTarget) < arrivalThreshold)
        {
            EnterState(KuntiState.Idling);
        }
    }

    // ============================================================
    // STATE: FIND PLAYER - Mengejar player
    // ============================================================
    private void UpdateFindPlayer()
    {
        if (player == null) return;

        // Simpan posisi terakhir player (untuk Search nanti)
        lastKnownPlayerPos = player.position;

        // Cek apakah player masih terlihat (tidak bersembunyi & dalam jangkauan)
        if (!CanSeePlayer())
        {
            // Player menghilang! Pindah ke Search
            EnterState(KuntiState.Search);
            return;
        }

        // Kejar player
        MoveTowards(player.position, chaseSpeed);

        // Flip sprite sesuai arah player
        FlipSprite(player.position.x);
    }

    // ============================================================
    // STATE: SEARCH - Mencari player di posisi terakhir
    // ============================================================
    private void UpdateSearch()
    {
        // Kurangi timer
        stateTimer -= Time.deltaTime;

        // Kalau player terlihat lagi, langsung kejar!
        if (CanSeePlayer())
        {
            EnterState(KuntiState.FindPlayer);
            return;
        }

        // Gerak ke posisi terakhir player
        float distToLastPos = Vector2.Distance(transform.position, lastKnownPlayerPos);

        if (distToLastPos > arrivalThreshold)
        {
            // Belum sampai, gerak ke sana
            MoveTowards(lastKnownPlayerPos, searchSpeed);
            FlipSprite(lastKnownPlayerPos.x);
        }
        else
        {
            // Sudah sampai, "lihat-lihat" (celingukan) dengan gerakan kecil
            LookAround();
        }

        // Kalau timer habis, kembali ke Wandering
        if (stateTimer <= 0f)
        {
            EnterState(KuntiState.Wandering);
        }
    }

    // ============================================================
    // HELPER: Gerakkan Kuntilanak ke arah target (smooth)
    // ============================================================
    private void MoveTowards(Vector2 target, float speed)
    {
        // Gunakan Vector2.MoveTowards untuk gerakan halus
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, target, speed * Time.deltaTime);

        // Clamp posisi supaya Kunti tidak pernah keluar dari boundary box
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    // ============================================================
    // HELPER: Cek apakah player terlihat
    // ============================================================
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        // Cek jarak antara Kuntilanak dan player
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRadius) return false;

        // Cek apakah player sedang bersembunyi (layer 7 = hiding)
        if (player.gameObject.layer == hidingLayer) return false;

        // Cek apakah player di luar batas area Kuntilanak
        // Kalau player kabur keluar boundary, Kunti berhenti mengejar
        Vector2 playerPos = player.position;
        if (playerPos.x < minX || playerPos.x > maxX ||
            playerPos.y < minY || playerPos.y > maxY)
            return false;

        // Player terlihat!
        return true;
    }

    // ============================================================
    // HELPER: Flip sprite berdasarkan arah target
    // ============================================================
    private void FlipSprite(float targetX)
    {
        if (sr == null) return;

        if (targetX < transform.position.x)
        {
            sr.flipX = true;   // Menghadap kiri
        }
        else if (targetX > transform.position.x)
        {
            sr.flipX = false;  // Menghadap kanan
        }
    }

    // ============================================================
    // HELPER: Pilih titik random di dalam area batas
    // ============================================================
    private Vector2 GetRandomPointInBounds()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector2(x, y);
    }

    // ============================================================
    // HELPER: Efek "celingukan" saat mencari player
    // ============================================================
    private void LookAround()
    {
        // Gerakan kecil bolak-balik menggunakan Mathf.Sin
        // Memberi kesan Kuntilanak sedang mencari/celingukan
        float offset = Mathf.Sin(Time.time * 3f) * 0.3f;
        Vector2 currentPos = transform.position;
        Vector2 swayPos = new Vector2(currentPos.x + offset * Time.deltaTime, currentPos.y);
        transform.position = new Vector3(swayPos.x, swayPos.y, transform.position.z);

        // Flip sprite bolak-balik sesuai arah celingukan
        if (sr != null)
        {
            sr.flipX = Mathf.Sin(Time.time * 2f) > 0;
        }
    }

    // ============================================================
    // GIZMOS: Visualisasi boundary box & detection radius di Scene View
    // ============================================================
    private void OnDrawGizmos()
    {
        // --- Boundary Box (hijau) ---
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);

        // --- Detection Radius (merah) ---
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // --- Wander Target (kuning, hanya saat play mode) ---
        if (Application.isPlaying && currentState == KuntiState.Wandering)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(wanderTarget, 0.2f);
            Gizmos.DrawLine(transform.position, wanderTarget);
        }

        // --- Last Known Player Position (biru, hanya saat search) ---
        if (Application.isPlaying && currentState == KuntiState.Search)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(lastKnownPlayerPos, 0.2f);
            Gizmos.DrawLine(transform.position, lastKnownPlayerPos);
        }
    }

    private IEnumerator StunEnemy()
    {
      wanderSpeed = 0f;  
      chaseSpeed = 0f;
      searchSpeed = 0f;
      yield return new WaitForSeconds(stunDuration);
      IsStunned = false;
      wanderSpeed = 2f;
      chaseSpeed = 4f;
      searchSpeed = 1.5f;
      EnterState(KuntiState.Wandering);
    }

    // ============================================================
    // PUBLIC: Untuk debug di Inspector - tampilkan state saat ini
    // ============================================================
    public string GetCurrentState()
    {
        return currentState.ToString();
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player is Hit!");
        }

        if (collision.CompareTag("Flash"))
        {
            Debug.Log("Kunti Is Blinded");
            StartCoroutine(StunEnemy());
            IsStunned = true;
        }
    }
}
