using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInitialTooltip : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstTimeEditor", 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
            PlayerPrefs.SetInt("FirstTimeEditor", 1);
        }
    }
}
