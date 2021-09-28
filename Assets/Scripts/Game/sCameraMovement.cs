using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sCameraMovement : MonoBehaviour
{
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 lerpToPosition;
    [SerializeField] private Vector3 moveToPosition;
    [SerializeField] private float moveSpeed;

    [Header("Level Editor")]
    [SerializeField] private Vector3 LevelEditorTestPosition;
    [SerializeField] private Vector3 levelCompletePosition;
    public IEnumerator toEditorRoutine;
    public IEnumerator toTestingRoutine;
    // Start is called before the first frame update
    void Awake()
    {
        defaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LevelEndMove(float delay, float movesMade, bool isComplete)
    {
        yield return new WaitForSeconds(delay);
        UIManager.instance.SetActiveInGameMenu(false);
        GameManger.instance.tutorialRef.LevelComplete();
        float t = 0f;
        bool triggeredCompleteMenu = false;
        while (t <= 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(defaultPosition, lerpToPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            if (isComplete)
            { 
                if (t >= 0.5f && !triggeredCompleteMenu)
                {
                    triggeredCompleteMenu = true;
                    UIManager.instance.ToggleLevelCompleteMenu(movesMade);
                }
            }
            yield return null;
        }
        transform.position = moveToPosition;
        GameManger.instance.cameraEndRoutine = null;

    }

    public IEnumerator LevelStartMove(bool isComplete)
    {
        //start level complete menu moving off screen
        float t = 0f;
        if (isComplete)
        {
            UIManager.instance.DisableLevelCompleteMenu();
        }
        while (t <= 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(moveToPosition, defaultPosition, 1 - Mathf.Cos(t * Mathf.PI * 0.5f));

            yield return null;
        }
        transform.position = defaultPosition;
        GameManger.instance.cameraStartRoutine = null;
        UIManager.instance.SetActiveInGameMenu(true);
    }

    public IEnumerator LevelComplete()
    {
        float t = 0f;
        Vector3 initial = transform.position;
        while (t <= 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(initial, levelCompletePosition, t);

            yield return null;
        }
        transform.position = levelCompletePosition;
    }

    public IEnumerator LevelEditorToTest()
    {
        float t = 0f;
        Vector3 initial = transform.position;
        while (t <= 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(initial, LevelEditorTestPosition, t);

            yield return null;
        }
        transform.position = LevelEditorTestPosition;
        toTestingRoutine = null;
    }

    public IEnumerator LevelEditorFromTest()
    {
        float t = 0f;
        Vector3 initial = transform.position;

        while (t <= 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(initial, defaultPosition, t);

            yield return null;
        }
        transform.position = defaultPosition;
        toEditorRoutine = null;
    }
}
