using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool _firtsRun;
    public GameObject PauseMenuUI;
    public GameObject GameMenuUI;
    public GameObject HighScoreMenuUI;

    public GameObject[] PauseMenuButtons;
    public GameObject[] GameMenuButtons;
    public GameObject[] HighScoreMenuButtons;

    public GameObject Pointer;

    public static PauseMenu Instance;

    private int _index = 1;
    private GameObject _leftPointer;
    private GameObject _rightPointer;
    private GameObject[] _buttons;

    public GameObject[] SetUsingButtons
    {
        set { _buttons = value; }
    }

    private void Start()
    {
        _buttons = PauseMenuButtons;
        _leftPointer = Instantiate(Pointer, GetPositionRight(_index, _buttons), transform.rotation, transform);
        _rightPointer = Instantiate(Pointer, GetPositionLeft(_index, _buttons), transform.rotation, transform);
        PauseMenuButtons[0].SetActive(false);
        _firtsRun = true;
        PauseMenuUI.SetActive(true);

        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PauseMenuUI.activeSelf)
            {
                _buttons[_index].GetComponent<Button>().onClick.Invoke();
                return;
            }

            if (GameMenuUI.activeSelf)
            {
                _buttons[_index].GetComponent<Button>().onClick.Invoke();
                return;
            }

            if (HighScoreMenuUI.activeSelf)
            {
                _buttons[_index].GetComponent<Button>().onClick.Invoke();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_index < _buttons.Length - 1)
            {
                _index++;
                ChangePositionOfPointer();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_firtsRun && PauseMenuUI.activeSelf)
            {
                if (_index > 1)
                {
                    _index--;
                    ChangePositionOfPointer();
                }
            }
            else
            {
                if (_index > 0)
                {
                    _index--;
                    ChangePositionOfPointer();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.CanPause && HighScoreMenuManager.Instance.CanPause && !GameManager.Instance.InGame)
        {
            if (PauseMenuUI.activeSelf)
            {
                Resume();
                return;
            }

            if (GameMenuUI.activeSelf)
            {
                Back();
                return;
            }

            Pause();
        }
    }

    public void Resume()
    {
        if (!_firtsRun)
        {
            GameManager.Instance.MenuOpened = false;
            ChangeStateOfPointers(false);
            PauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void Pause()
    {
        if (!_firtsRun)
        {
            GameManager.Instance.MenuOpened = true;
            ChangeStateOfPointers(true);
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeGame()
    {
        _index = 0;
        _buttons = GameMenuButtons;
        ChangePositionOfPointer();
        PauseMenuUI.SetActive(false);
        GameMenuUI.SetActive(true);
    }

    public void Back()
    {
        if (_firtsRun)
        {
            _index = 1;
        }
        else
        {
            _index = 0;
        }

        _buttons = PauseMenuButtons;
        ChangePositionOfPointer();
        GameMenuUI.SetActive(false);
        PauseMenuUI.SetActive(true);
    }

    public void NinjaKeyboard()
    {
        ChangeStateOfPointers(false);
        ActivateResumeButton();
        Back();
        Resume();
        GameManager.Instance.BeginNinjaKeyboard();
    }

    public void TrainingGame()
    {
        ChangeStateOfPointers(false);
        ActivateResumeButton();
        Back();
        Resume();
        GameManager.Instance.BeginTrainingGame();
    }

    public void Calibrate()
    {
        _index = 0;
        ChangeStateOfPointers(false);
        ActivateResumeButton();
        Resume();
        GameManager.Instance.BeginBinding();
    }

    private void ActivateResumeButton()
    {
        _index = 0;
        _firtsRun = false;
        PauseMenuButtons[0].SetActive(true);
    }

    private Vector2 GetPositionLeft(int index, GameObject[] Buttons)
    {
        Vector2 position = new Vector2();
        position.x = Buttons[index].transform.position.x;
        position.y = Buttons[index].transform.position.y;
        position.x -= Buttons[index].GetComponent<RectTransform>().rect.width / 2 + 20;
        return position;
    }

    private Vector2 GetPositionRight(int index, GameObject[] Buttons)
    {
        Vector2 position = new Vector2();
        position.x = Buttons[index].transform.position.x;
        position.y = Buttons[index].transform.position.y;
        position.x += Buttons[index].GetComponent<RectTransform>().rect.width / 2 + 20;
        return position;
    }


    private void ChangePositionOfPointer()
    {
        _leftPointer.transform.position = GetPositionLeft(_index, _buttons);
        _rightPointer.transform.position = GetPositionRight(_index, _buttons);
    }

    public void ChangeStateOfPointers(bool enabled)
    {
        if (enabled)
        {
            ChangePositionOfPointer();
        }

        _leftPointer.SetActive(enabled);
        _rightPointer.SetActive(enabled);
    }
}