using UnityEngine;
using System;

public class MirrorFragmentManager : MonoBehaviour
{
    public static MirrorFragmentManager Instance;

    public const int TOTAL_FRAGMENTS = 3;
    private bool[] collected = new bool[TOTAL_FRAGMENTS];

    public event Action<int> OnFragmentChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFragments();
    }

    private void Update()
    {
        Debug.Log(GetCollectedCount());
    }

    void LoadFragments()
    {
        for (int i = 0; i < TOTAL_FRAGMENTS; i++)
        {
            collected[i] = PlayerPrefs.GetInt($"Fragment_{i}", 0) == 1;
        }
    }

    public bool IsCollected(int id)
    {
        return collected[id];
    }

    public void CollectFragment(int id)
    {
        if (collected[id]) return;

        collected[id] = true;
        PlayerPrefs.SetInt($"Fragment_{id}", 1);
        PlayerPrefs.Save();

        OnFragmentChanged?.Invoke(GetCollectedCount());
    }

    public int GetCollectedCount()
    {
        int count = 0;
        foreach (bool c in collected)
            if (c) count++;

        return count;
    }

    public bool HasFullSet()
    {
        return GetCollectedCount() == TOTAL_FRAGMENTS;
    }
}
