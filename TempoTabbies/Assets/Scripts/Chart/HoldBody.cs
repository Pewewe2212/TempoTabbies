using UnityEngine;

public class HoldBody : MonoBehaviour
{
    public float StartTime;
    public float EndTime;
    public float ScrollSpeed;
    public AudioSource Music;
    public Transform HitLine;

    void Update()
    {
        if (!Music || !HitLine) return;

        float current = (float)Music.time;
        float startOffset = StartTime - current;
        float endOffset = EndTime - current;

        // Calculate top and bottom positions
        Vector3 startPos = HitLine.position + Vector3.up * (startOffset * ScrollSpeed);
        Vector3 endPos = HitLine.position + Vector3.up * (endOffset * ScrollSpeed);

        // Set position and scale
        transform.position = (startPos + endPos) / 2f;
        transform.localScale = new Vector3(
            transform.localScale.x,
            Mathf.Abs(startPos.y - endPos.y),
            transform.localScale.z
        );

        // Destroy slightly after end
        if (current > EndTime + 0.5f)
            Destroy(gameObject);
    }
}