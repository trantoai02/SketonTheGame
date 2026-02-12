using TMPro;
using UnityEngine;

public class FragmentUI : MonoBehaviour
{
    public TextMeshProUGUI fragmentText;

    private void Start()
    {
        UpdateUI(MirrorFragmentManager.Instance.GetCollectedCount());
        MirrorFragmentManager.Instance.OnFragmentChanged += UpdateUI;
    }

    void UpdateUI(int count)
    {
        fragmentText.text = $"{count} / 3";
    }

    private void OnDestroy()
    {
        if (MirrorFragmentManager.Instance != null)
            MirrorFragmentManager.Instance.OnFragmentChanged -= UpdateUI;
    }
}
