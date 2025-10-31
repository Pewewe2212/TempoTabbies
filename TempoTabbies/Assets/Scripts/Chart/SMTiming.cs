using System.Collections.Generic;
using UnityEngine;

public static class SMTiming
{
    public struct ParsedNote
    {
        public int lane;
        public float time;
        public char type;
    }

    public static List<ParsedNote> GetNoteTimes(SMFile sm, SMChart chart)
    {
        List<ParsedNote> notes = new();

        if (chart?.Measures == null || chart.Measures.Count == 0)
        {
            Debug.LogWarning("Chart has no measures!");
            return notes;
        }

        // ----- BPM setup -----
        float offset = sm != null ? sm.Offset : 0f;
        float bpm = 120f;
        if (sm?.Bpms != null && sm.Bpms.Count > 0)
        {
            foreach (var kv in sm.Bpms)
            {
                bpm = kv.Value;
                break;
            }
        }

        float secPerBeat = 60f / bpm;
        float currentBeat = 0f;

        // ----- measure loop -----
        foreach (var measure in chart.Measures)
        {
            int rows = measure.Count;
            for (int i = 0; i < rows; i++)
            {
                string row = measure[i];
                if (string.IsNullOrWhiteSpace(row)) continue;

                float rowBeat = currentBeat + (4f * i / rows);
                float time = offset + rowBeat * secPerBeat;

                for (int lane = 0; lane < row.Length; lane++)
                {
                    char c = row[lane];
                    if (c == '0') continue;

                    ParsedNote n = new ParsedNote
                    {
                        lane = lane,
                        time = time,
                        type = c
                    };
                    notes.Add(n);
                }
            }
            currentBeat += 4f;
        }

        Debug.Log($"Generated {notes.Count} notes from chart");
        return notes;
    }
}
