using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiApp : MonoBehaviour
{
    Text _ExperienceText;

    void Start()
    {
        _ExperienceText = GameObject.Find("Experience").GetComponent<Text>();
    }

    void Update()
    {
        if(Session._Session != null)
            _ExperienceText.text = Session._Session._Experience.ToString();
    }
}
