using UnityEngine;

public class Note : MonoBehaviour
{
    public float TargetTime;         // When the note should hit
    public float ScrollSpeed = 6f;   // How fast it scrolls
    public AudioSource Music;        // Music timing reference
    public Transform HitLine;        // Target Y position reference

    private bool initialized = false;
    private float initialX;          // The lane’s horizontal position
    private float initialZ;          // Keep Z stable too

    void Start()
    {
        if (Music != null && HitLine != null)
        {
            initialized = true;
            initialX = transform.position.x;
            initialZ = transform.position.z;
        }
        else
        {
            Debug.LogWarning($"Note '{name}' missing Music or HitLine reference.");
        }
    }

    void Update()
    {
        if (!initialized) return;

        // How many seconds until note reaches the hit line
        float timeUntilHit = TargetTime - (float)Music.time;

        // Keep X/Z fixed (lane position), move Y based on scroll speed and time
        float newY = HitLine.position.y + (timeUntilHit * ScrollSpeed);
        transform.position = new Vector3(initialX, newY, initialZ);

        // Optional: destroy after passing hit line
        if (timeUntilHit < -0.5f)
            Destroy(gameObject);
    }
}
