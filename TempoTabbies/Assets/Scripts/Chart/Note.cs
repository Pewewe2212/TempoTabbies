using UnityEngine;

public class Note : MonoBehaviour
{
    public float TargetTime;         // When the note should hit
    public float ScrollSpeed = 6f;   // How fast it scrolls
    public AudioSource Music;        // Music timing reference
    public Transform HitLine;        // Target position

    private bool initialized = false;

    void Start()
    {
        if (Music != null && HitLine != null)
            initialized = true;
        else
            Debug.LogWarning($"Note '{name}' missing Music or HitLine reference.");
    }

    void Update()
    {
        if (!initialized) return;

        // How many seconds until note reaches the hit line
        float timeUntilHit = TargetTime - (float)Music.time;

        // If positive -> note is above the line, negative -> below
        Vector3 pos = HitLine.position + Vector3.up * (timeUntilHit * ScrollSpeed);
        transform.position = pos;

        // Optional: destroy a bit after passing the hit line
        if (timeUntilHit < -0.5f)
            Destroy(gameObject);
    }
}
