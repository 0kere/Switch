using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [System.Serializable]
    public class TutorialContent
    {
        public GameObject tutorialText;
        public int tutorialIndex;
    }
    [SerializeField] string stt;
    [SerializeField] private List<TutorialContent> tutorial;
    private int onIndex;

    public void NewLevelLoaded()
    {
        tutorialPanel.SetActive(false);
        for (int i = 0; i < tutorial.Count; i++)
        {
            if (tutorial[i].tutorialIndex == GameManger.instance.curLevelIndex && GameManger.instance.isPlayingDefaultLevels)
            {
                tutorialPanel.SetActive(true);
                tutorial[i].tutorialText.SetActive(true);
                onIndex = i;
                break;
            }
        }
    }

    public void LevelComplete()
    {
        tutorial[onIndex].tutorialText.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    private void Start()
    {
        GameManger.instance.tutorialRef = this;
        NewLevelLoaded();
    }
}
