using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/Portrait Database")]
public class PortraitDatabase : ScriptableObject
{
    public List<CharacterPortrait> portraits;

    Dictionary<string, Sprite> cache;

    void BuildCache()
    {
        if (cache != null) return;

        cache = new Dictionary<string, Sprite>();
        foreach (var p in portraits)
        {
            if (!cache.ContainsKey(p.characterName))
                cache.Add(p.characterName, p.portrait);
        }
    }

    public Sprite GetPortrait(string speakerName)
    {
        BuildCache();
        cache.TryGetValue(speakerName, out Sprite sprite);
        return sprite;
    }
}

[System.Serializable]
public class CharacterPortrait
{
    public string characterName;
    public Sprite portrait;
}
