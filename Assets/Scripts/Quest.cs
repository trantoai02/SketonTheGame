using System;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public GameObject starObject;

    public string questID;          // ID định danh quest
    public string starID;        
    public string description;      // Nội dung quest
    public bool isCompleted;        // Trạng thái hoàn thành

    // Thêm kill count
    public bool isKillCountQuest;   // có phải kill count quest không
    public int goal;                // số lượng cần giết
    [NonSerialized] public int currentKill;

    // --- Chicken Quest (mới thêm) ---
    public bool isChickenQuest;     // Có phải quest bắt gà không
    [NonSerialized] public int currentChicken; // Số gà hiện tại đã bắt

}
