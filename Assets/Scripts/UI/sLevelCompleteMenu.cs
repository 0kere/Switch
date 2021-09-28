using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sLevelCompleteMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI movesMadeText;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float timeToMovesMadeLerp;
    [SerializeField] private float minDelayMovesMade;

    private IEnumerator enableMenu;
    private IEnumerator lerpRoutine;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 centerPos;

    private RectTransform recTransform;
    // Start is called before the first frame update
    void Start()
    {
        recTransform = GetComponent<RectTransform>();
        startPos = recTransform.localPosition;
        endPos = -startPos;
        centerPos = startPos;
        centerPos.x = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableMenu(float movesMade)
    {
        if (lerpRoutine is null)
        {

            lerpRoutine = LerpPosition(startPos, centerPos);
            StartCoroutine(lerpRoutine);
            if (!GameManger.instance.isLevelEditing)
            { 
                StartCoroutine(LerpZeroToValue(movesMade));
            }
        }
    }

    public void DisableMenu()
    {
        if (lerpRoutine is null)
        {
            lerpRoutine = LerpPosition(centerPos, endPos);
            StartCoroutine(lerpRoutine);
        }
    }

    private IEnumerator LerpZeroToValue(float value)
    {
        float temp = 0f;
        float delay = timeToMovesMadeLerp / value;
        delay = Mathf.Clamp(delay, minDelayMovesMade, delay);
        while (temp != value)
        {
            yield return new WaitForSeconds(delay);
            temp++;
            movesMadeText.text = temp.ToString();
        }
    }

    private IEnumerator LerpPosition(Vector3 start, Vector3 end)
    {
        float t = 0f;
        while (t <= 1)
        {
            t += Time.deltaTime * lerpSpeed;
            recTransform.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
        lerpRoutine = null;
        
    }
}
