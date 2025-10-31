using UnityEngine;

public class GameManager : MonoBehaviour
{
    public NoteSpawner Spawner;
    public AudioSource Music;
    public GameObject NotePrefab;
    public Transform LaneParent;
    public Transform HitLine;

    void Start()
    {
        string path = Application.dataPath + "/Songs/zunda/zundasolo.sm";
        SMFile sm = SMParser.Parse(path);

        if (sm.Charts.Count == 0)
        {
            Debug.LogError("No dance-solo chart found in SM file!");
            return;
        }

        SMChart chart = sm.Charts[0];
        Debug.Log($"Loaded dance-solo chart with {chart.Measures.Count} measures.");

        //Auto-create lanes (6 for solo)
        CreateLanes(6);

        // Assign references
        Spawner.Music = Music;
        Spawner.NotePrefab = NotePrefab;
        Spawner.HitLine = HitLine;

        // Get all 6 lanes
        Transform[] lanes = new Transform[LaneParent.childCount];
        for (int i = 0; i < LaneParent.childCount; i++)
            lanes[i] = LaneParent.GetChild(i);
        Spawner.Lanes = lanes;

        Spawner.LoadChart(sm, chart);
        Music.Play();
    }

    void CreateLanes(int count)
    {
        // Clean up old ones
        foreach (Transform t in LaneParent)
            Destroy(t.gameObject);

        float spacing = 1.2f;
        for (int i = 0; i < count; i++)
        {
            GameObject lane = new GameObject("Lane_" + i);
            lane.transform.SetParent(LaneParent);
            lane.transform.localPosition = new Vector3((i - (count - 1) / 2f) * spacing, 0, 0);
        }
    }
}
