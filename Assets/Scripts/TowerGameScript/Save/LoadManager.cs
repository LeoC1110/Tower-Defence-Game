using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class LoadManager : MonoBehaviour
{
    public Transform contentRoot;
    public GameObject saveSlotPrefab;
    public Button loadButton;
    public Button deleteButton;
    public Button backButton;

    private string selectedId = null;

    private void Start()
    {
        RefreshSaveSlots();

        loadButton.onClick.AddListener(() => {
            if (string.IsNullOrEmpty(selectedId)) return;
            SaveData data = SaveSystem.LoadById(selectedId);
            if (data != null)
            {
                PlayerPrefs.SetString("LoadId", data.id);
                SceneManager.LoadScene(data.sceneName);
            }
        });

        deleteButton.onClick.AddListener(() => {
            if (!string.IsNullOrEmpty(selectedId))
            {
                SaveSystem.DeleteSave(selectedId);
                selectedId = null;
                RefreshSaveSlots();
            }
        });

        backButton.onClick.AddListener(() => SceneManager.LoadScene("Main Screen"));
    }

    void RefreshSaveSlots()
    {
        foreach (Transform child in contentRoot) Destroy(child.gameObject);
        List<SaveData> saves = SaveSystem.LoadAllSaves();
        foreach (var save in saves)
        {
            var slot = Instantiate(saveSlotPrefab, contentRoot);
            slot.GetComponent<SaveSlotUI>().Setup(save, (id) => selectedId = id);
        }
    }
}