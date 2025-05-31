using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class TrainEventUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public Image eventImage;
    public TextMeshProUGUI descriptionText;
    public List<Button> choiceButtons;

    private Action<int> onChoiceSelected;

    public void ShowEvent(TrainEvent trainEvent, Action<int> onChoiceSelected)
    {
        Time.timeScale = 0f;

        uiPanel.SetActive(true);
        eventImage.sprite = trainEvent.eventImage;
        descriptionText.text = trainEvent.description;
        this.onChoiceSelected = onChoiceSelected;

        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < trainEvent.choices.Count)
            {
                int index = i;
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = trainEvent.choices[i];
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectChoice(int index)
    {
        uiPanel.SetActive(false);
        Time.timeScale = 1f;
        onChoiceSelected?.Invoke(index);
    }
}
