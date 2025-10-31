using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Audio + Timing")]
    public AudioSource Music;
    public float ScrollSpeed = 6f;
    public float SpawnLeadTime = 2f;

    [Header("Lane Setup")]
    public Transform[] Lanes; // Assign all 6 lanes
    public Transform HitLine;

    [Header("Prefabs by Lane Group")]
    public GameObject NotePrefab_TypeA; // Lanes 0,3
    public GameObject NotePrefab_TypeB; // Lanes 1,2
    public GameObject NotePrefab_TypeC; // Lane 4
    public GameObject NotePrefab_TypeD; // Lane 5

    private List<SMTiming.ParsedNote> notes;
    private int nextIndex = 0;

    // --------------------------------------------------------
    // Called by GameManager after parsing .sm file
    // --------------------------------------------------------
    public void LoadChart(SMFile sm, SMChart chart)
    {
        notes = SMTiming.GetNoteTimes(sm, chart);
        notes.Sort((a, b) => a.time.CompareTo(b.time));
        nextIndex = 0;
        Debug.Log($"[NoteSpawner] Loaded chart with {notes.Count} notes");
    }

    void Update()
    {
        if (notes == null || notes.Count == 0) return;
        if (!Music || !HitLine || Lanes == null || Lanes.Length == 0)
            return;

        float songTime = (float)Music.time;

        // Spawn notes ahead of time
        while (nextIndex < notes.Count && notes[nextIndex].time - songTime < SpawnLeadTime)
        {
            var noteData = notes[nextIndex];
            if (noteData.lane >= Lanes.Length)
            {
                Debug.LogWarning($"[NoteSpawner] Invalid lane {noteData.lane}");
                nextIndex++;
                continue;
            }

            // Determine which prefab to use
            GameObject prefabToUse = GetPrefabForLane(noteData.lane);
            if (prefabToUse == null)
            {
                Debug.LogWarning($"[NoteSpawner] No prefab assigned for lane {noteData.lane}");
                nextIndex++;
                continue;
            }

            Transform lane = Lanes[noteData.lane];
            GameObject note = Instantiate(prefabToUse, lane.position, Quaternion.identity, transform);

            // Configure the Note script
            Note n = note.GetComponent<Note>();
            if (n != null)
            {
                n.TargetTime = noteData.time;
                n.ScrollSpeed = ScrollSpeed;
                n.Music = Music;
                n.HitLine = HitLine;
            }

            nextIndex++;
        }
    }

    // --------------------------------------------------------
    // Prefab selection logic
    // --------------------------------------------------------
    private GameObject GetPrefabForLane(int lane)
    {
        switch (lane)
        {
            case 0:
            case 3:
                return NotePrefab_TypeA;

            case 1:
            case 2:
                return NotePrefab_TypeB;

            case 4:
                return NotePrefab_TypeC;

            case 5:
                return NotePrefab_TypeD;

            default:
                return null;
        }
    }
}
