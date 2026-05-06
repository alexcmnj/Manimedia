using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public TextMeshProUGUI leaderboardText;

    private List<float> times = new List<float>();

    void Awake() => Instance = this;

    public void SaveRun(float time)
    {
        times.Add(time);
        times.Sort();
        UpdateLeaderboardUI();
    }

    void UpdateLeaderboardUI()
    {
        string txt = "🏆 Mejores tiempos\n\n";
        for (int i = 0; i < Mathf.Min(5, times.Count); i++)
        {
            int min = (int)(times[i] / 60f);
            int sec = (int)(times[i] % 60f);
            txt += $"{i + 1}. {min:00}:{sec:00}\n";
        }
        leaderboardText.text = txt;
    }
}

