using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public AudioSource Music;
    public GameObject NotePrefab;
    public Transform[] Lanes;
    public Transform HitLine;

    public float ScrollSpeed = 6f;
    public float SpawnLeadTime = 2f;

    private List<SMTiming.ParsedNote> notes;
    private int nextIndex = 0;

    public void LoadChart(SMFile sm, SMChart chart)
    {
        notes = SMTiming.GetNoteTimes(sm, chart);
        if (notes == null) notes = new List<SMTiming.ParsedNote>();
        notes.Sort((a, b) => a.time.CompareTo(b.time));
        Debug.Log($"Loaded chart with {notes.Count} parsed notes");
    }

    void Update()
    {
        if (Music == null || HitLine == null || NotePrefab == null || Lanes == null || Lanes.Length == 0)
            return;
        if (notes == null || notes.Count == 0)
            return;

        float songTime = (float)Music.time;

        while (nextIndex < notes.Count && notes[nextIndex].time - songTime < SpawnLeadTime)
        {
            var noteData = notes[nextIndex];

            if (float.IsNaN(noteData.time))
            {
                Debug.LogWarning($"Skipping note {nextIndex}, invalid time");
                nextIndex++;
                continue;
            }

            if (noteData.lane >= Lanes.Length)
            {
                Debug.LogWarning($"Skipping note {nextIndex}, lane out of range");
                nextIndex++;
                continue;
            }

            Transform lane = Lanes[noteData.lane];
            GameObject note = Instantiate(NotePrefab, lane.position, Quaternion.identity, transform);

            Note n = note.GetComponent<Note>();
            if (n != null)
            {
                n.TargetTime = noteData.time;
                n.ScrollSpeed = ScrollSpeed;
                n.Music = Music;
                n.HitLine = HitLine;
            }

            SpriteRenderer sr = note.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (noteData.type == '2') sr.color = Color.cyan;
                else if (noteData.type == '3') sr.color = Color.magenta;
                else sr.color = Color.white;
            }

            nextIndex++;
        }
    }
}
