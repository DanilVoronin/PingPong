using UnityEngine;
using UnityEngine.UI;

public class GuiApp : MonoBehaviour
{
    GameObject _MenuGlobal;
    GameObject _MenuGameOver;

    GameObject _ExperiencePanal;
    TextMesh _ExperienceText;
    TextMesh _RecordText;

    void Start()
    {
        //MenuGlobal
        _MenuGlobal = GameObject.Find("MenuGlobal");
        GameObject.Find("ButtonStart").GetComponent<Button>().onClick.AddListener(ButtonStart);

        //MenuGameOver
        _MenuGameOver = GameObject.Find("MenuGameOver");
        GameObject.Find("MenuGameOverButtonRestart").GetComponent<Button>().onClick.AddListener(ButtonStart);
        GameObject.Find("MenuGameOverButtonMenu").GetComponent<Button>().onClick.AddListener(MenuGameOverButtonMenu);
        _MenuGameOver.SetActive(false);

        //ExperiencePanal
        _ExperienceText = GameObject.Find("Experience").GetComponent<TextMesh>();
        _RecordText = GameObject.Find("Record").GetComponent<TextMesh>();
        _ExperiencePanal = GameObject.Find("ExperiencePanal");
        _ExperiencePanal.gameObject.SetActive(false);

        Session._Session._StartSession += StartSessin;
        Session._Session._EndSession += EndSession;
    }

    public void StartSessin()
    {
        _MenuGlobal.gameObject.SetActive(false);
        _MenuGameOver.gameObject.SetActive(false);

        _ExperiencePanal.gameObject.SetActive(true);
        _RecordText.text = Session._Session._Record.ToString();
    }
    public void EndSession()
    {
        _ExperiencePanal.gameObject.SetActive(false);
        _MenuGameOver.gameObject.SetActive(true);
    }

    public void ButtonStart()
    {
        Session._Session.StartSession();
    }
    public void MenuGameOverButtonMenu()
    {
        _MenuGlobal.gameObject.SetActive(true);
        _MenuGameOver.gameObject.SetActive(false);
    }

    void Update()
    {
        if(Session._Session != null)
            _ExperienceText.text = Session._Session._Experience.ToString();
    }
}
