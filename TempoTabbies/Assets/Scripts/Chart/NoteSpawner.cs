using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Audio + Timing")]
    public AudioSource Music;
    public float ScrollSpeed = 6f;         // units per second
    public float SpawnLeadTime = 2f;       // seconds before hit to spawn

    [Header("Lane Setup")]
    public Transform[] Lanes;
    public Transform HitLine;

    [Header("Prefabs by Lane Group")]
    public GameObject NotePrefab_TypeA; // Lanes 0,3
    public GameObject NotePrefab_TypeB; // Lanes 1,2
    public GameObject NotePrefab_TypeC; // Lane 4
    public GameObject NotePrefab_TypeD; // Lane 5

    [Header("Hold Prefabs (lane-specific head, shared body/end)")]
    public GameObject HoldHeadPrefab_TypeA;
    public GameObject HoldHeadPrefab_TypeB;
    public GameObject HoldHeadPrefab_TypeC;
    public GameObject HoldHeadPrefab_TypeD;
    public GameObject HoldBodyPrefab;    // rectangle sprite
    public GameObject HoldEndPrefab;     // cap sprite

    private List<SMTiming.ParsedNote> notes;
    private int nextIndex = 0;

    // call this from GameManager
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
        if (!Music || !HitLine || Lanes == null || Lanes.Length == 0) return;

        float songTime = (float)Music.time;
        float spawnHeight = SpawnLeadTime * ScrollSpeed; // units above hit line to spawn

        while (nextIndex < notes.Count && notes[nextIndex].time - songTime < SpawnLeadTime)
        {
            var noteData = notes[nextIndex];

            if (noteData.lane < 0 || noteData.lane >= Lanes.Length)
            {
                nextIndex++;
                continue;
            }

            Transform lane = Lanes[noteData.lane];

            // compute spawn position (preserve lane X/Z, spawn at HitLine.y + spawnHeight)
            Vector3 spawnPos = new Vector3(lane.position.x, HitLine.position.y + spawnHeight, lane.position.z);

            // HANDLE HOLD START
            if (noteData.type == '2') // hold start
            {
                var endNote = FindHoldEnd(noteData.lane, nextIndex);
                if (endNote.HasValue)
                {
                    SpawnHold(noteData, endNote.Value, spawnPos);
                    nextIndex++;
                    continue;
                }
            }

            // TAP note
            GameObject tapPrefab = GetTapPrefabForLane(noteData.lane);
            if (tapPrefab == null)
            {
                Debug.LogWarning($"No tap prefab assigned for lane {noteData.lane}");
                nextIndex++;
                continue;
            }

            GameObject head = Instantiate(tapPrefab, spawnPos, Quaternion.identity, transform);

            // configure Note component (keeps initialX, moves vertically)
            Note n = head.GetComponent<Note>();
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

    // Find end matching this lane (type '3'). Search after startIndex.
    private SMTiming.ParsedNote? FindHoldEnd(int lane, int startIndex)
    {
        for (int i = startIndex + 1; i < notes.Count; i++)
        {
            if (notes[i].lane == lane && notes[i].type == '3')
                return notes[i];
        }
        return null;
    }

    private void SpawnHold(SMTiming.ParsedNote start, SMTiming.ParsedNote end, Vector3 spawnPos)
    {
        // pick lane-specific head prefab
        GameObject headPrefab = GetHoldHeadPrefabForLane(start.lane);
        if (headPrefab == null || HoldBodyPrefab == null || HoldEndPrefab == null)
        {
            Debug.LogWarning("[NoteSpawner] Missing hold prefabs for lane " + start.lane);
            return;
        }

        // instantiate head at spawnPos
        GameObject head = Instantiate(headPrefab, spawnPos, Quaternion.identity, transform);

        // add HoldNote and configure
        HoldNote hold = head.AddComponent<HoldNote>();
        hold.StartTime = start.time;
        hold.EndTime = end.time;
        hold.ScrollSpeed = ScrollSpeed;
        hold.Music = Music;
        hold.HitLine = HitLine;

        // instantiate body and end as separate objects but keep them as children of the spawner (we'll anchor by X)
        GameObject body = Instantiate(HoldBodyPrefab, transform);
        GameObject endObj = Instantiate(HoldEndPrefab, transform);

        // ensure their localScale is reset so scaling is deterministic
        body.transform.localScale = Vector3.one;
        endObj.transform.localScale = Vector3.one;

        // assign to hold
        hold.Body = body;
        hold.End = endObj;
    }

    private GameObject GetTapPrefabForLane(int lane)
    {
        return lane switch
        {
            0 or 3 => NotePrefab_TypeA,
            1 or 2 => NotePrefab_TypeB,
            4 => NotePrefab_TypeC,
            5 => NotePrefab_TypeD,
            _ => null,
        };
    }

    private GameObject GetHoldHeadPrefabForLane(int lane)
    {
        return lane switch
        {
            0 or 3 => HoldHeadPrefab_TypeA,
            1 or 2 => HoldHeadPrefab_TypeB,
            4 => HoldHeadPrefab_TypeC,
            5 => HoldHeadPrefab_TypeD,
            _ => null,
        };
    }
}
