using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public NoteSpawner Spawner;
    public AudioSource Music;

    [Header("Prefabs")]
    public GameObject NotePrefab_TypeA; // lanes 0 & 3
    public GameObject NotePrefab_TypeB; // lanes 1 & 2
    public GameObject NotePrefab_TypeC; // lane 4
    public GameObject NotePrefab_TypeD; // lane 5

    [Header("Layout")]
    public Transform LaneParent;
    public Transform HitLine;

    void Start()
    {
        // Path to your .sm file
        string path = Application.dataPath + "/Songs/zunda/zundasolo.sm";
        SMFile sm = SMParser.Parse(path);

        if (sm.Charts.Count == 0)
        {
            Debug.LogError("No dance-solo chart found in SM file!");
            return;
        }

        SMChart chart = sm.Charts[0];
        Debug.Log($"Loaded dance-solo chart with {chart.Measures.Count} measures.");

        // ?? Lanes are now placed manually in the scene!
        // Just grab their references under LaneParent.
        if (LaneParent == null)
        {
            Debug.LogError("LaneParent is not assigned!");
            return;
        }

        Transform[] lanes = new Transform[LaneParent.childCount];
        for (int i = 0; i < LaneParent.childCount; i++)
            lanes[i] = LaneParent.GetChild(i);

        // Assign to spawner
        Spawner.Music = Music;
        Spawner.HitLine = HitLine;
        Spawner.Lanes = lanes;

        Spawner.NotePrefab_TypeA = NotePrefab_TypeA;
        Spawner.NotePrefab_TypeB = NotePrefab_TypeB;
        Spawner.NotePrefab_TypeC = NotePrefab_TypeC;
        Spawner.NotePrefab_TypeD = NotePrefab_TypeD;

        // Load and start
        Spawner.LoadChart(sm, chart);
        Music.Play();
    }
}
