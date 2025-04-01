using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text timeText;
    public Text sceneText;
    public Text goldText;
    public Button selectButton;

    public string saveId { get; private set; }

    public void Setup(SaveData data, System.Action<string> onSelected)
    {
        timeText.text = data.realTimeString;
        sceneText.text = data.sceneName;
        goldText.text = $"Gold: {data.playerGold} | Time: {Mathf.FloorToInt(data.gameTime)}s";
        saveId = data.id;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected?.Invoke(saveId));
    }
}
