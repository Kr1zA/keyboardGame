using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreMenuManager : MonoBehaviour
{
    public static HighScoreMenuManager Instance;

    public GameObject HighScoreMenu;
    public GameObject Button;
    public GameObject GameText;
    public GameObject InputField;

    private const String ScoreTableFileName = "scoreTable";

    private GameObject _if;
    private int _actualBestScorePosition = -1;

    private string[] _highScoreNames = new string[10];
    private int[] _highScore = new int[10];
    private bool _enterPressed;

    public bool CanPause
    {
        get { return !HighScoreMenu.activeSelf; }
    }

    // Use this for initialization
    void Start()
    {
        _enterPressed = false;
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && HighScoreMenu.activeSelf && !_enterPressed &&
            _actualBestScorePosition != -1)
        {
            NewHighScoreNameEntered();
        }
    }

    public void ShowHighScoreTable(int score)
    {
        LoadScoreTableFromFile();
        HighScoreMenu.SetActive(true);

        if (score < _highScore[_highScore.Length - 1])
        {
            for (int i = 0; i < _highScore.Length; i++)
            {
                GameObject place = CreateUIGameOverTextObject(GameText, 0.2f, 0.9f - 0.05f * i, (i + 1) + ".");
                place.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

                GameObject names = CreateUIGameOverTextObject(GameText, 0.5f, 0.9f - 0.05f * i, _highScoreNames[i]);
                names.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

                GameObject points = CreateUIGameOverTextObject(GameText, 0.8f, 0.9f - 0.05f * i, "" + _highScore[i]);
                points.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
            }

            GameObject gameOver = CreateUIGameOverTextObject(GameText, 0.5f, 0.35f, "Your score is: " + score);
            gameOver.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            CreateNewGameButton();
            return;
        }

        for (int i = _highScore.Length - 1; i >= 1; i--)
        {
            if (_highScore[i - 1] >= score)
            {
                _actualBestScorePosition = i;
                break;
            }
        }

        if (_actualBestScorePosition == -1)
        {
            _actualBestScorePosition = 0;
        }

        Array.Copy(_highScore, _actualBestScorePosition, _highScore, _actualBestScorePosition + 1,
            _highScore.Length - 1 - _actualBestScorePosition);
        Array.Copy(_highScoreNames, _actualBestScorePosition, _highScoreNames, _actualBestScorePosition + 1,
            _highScore.Length - 1 - _actualBestScorePosition);

        _highScore[_actualBestScorePosition] = score;

        for (int i = 0; i < _highScore.Length; i++)
        {
            GameObject place = CreateUIGameOverTextObject(GameText, 0.3f, 0.9f - 0.05f * i, (i + 1) + ".");
            place.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            if (_actualBestScorePosition == i)
            {
                _if = Instantiate(InputField, new Vector2(
                        GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.9f - 0.05f * i, 0)).x,
                        GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.9f - 0.05f * i, 0))
                            .y),
                    transform.rotation);
                _if.transform.SetParent(HighScoreMenu.transform);
                _if.GetComponent<InputField>().text = "Enter your name!";
                //not working because enter in pause menu script...
//                _if.GetComponent<InputField>().onEndEdit.AddListener(delegate {NewHighScoreNameEntered(); });
                _if.GetComponent<InputField>().Select();
            }
            else
            {
                GameObject names = CreateUIGameOverTextObject(GameText, 0.5f, 0.9f - 0.05f * i, _highScoreNames[i]);
                names.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            }

            GameObject points = CreateUIGameOverTextObject(GameText, 0.7f, 0.9f - 0.05f * i, "" + _highScore[i]);
            points.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        }
    }

    private void NewHighScoreNameEntered()
    {
        _enterPressed = true;
        _highScoreNames[_actualBestScorePosition] = _if.GetComponent<InputField>().text;
        Destroy(_if);

        if (_highScoreNames[_actualBestScorePosition].Equals("Enter your name!"))
        {
            _highScoreNames[_actualBestScorePosition] = "";
        }

        GameObject newName = CreateUIGameOverTextObject(GameText, 0.5f, 0.9f - 0.05f * _actualBestScorePosition,
            _highScoreNames[_actualBestScorePosition]);
        newName.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        SaveScoreTableToFile();
        
        CreateNewGameButton();
    }

    private void CreateNewGameButton()
    {
        GameObject newGame = Instantiate(Button, new Vector2(
            GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.28f, 0)).x,
            GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.28f, 0)).y), transform.rotation);
        newGame.name = "NewGameButton";
        newGame.transform.GetComponentInChildren<Text>().text = "New Game";
        newGame.transform.SetParent(HighScoreMenu.transform);

//#if UNITY_EDITOR
//        UnityEditor.Events.UnityEventTools.RegisterPersistentListener(newGame.GetComponent<Button>().onClick, 0,
//            DestroyHighScoreTable
//        );
//#endif
//
//#if UNITY_EDITOR
//        UnityEditor.Events.UnityEventTools.AddPersistentListener(newGame.GetComponent<Button>().onClick,
//            GameManager.Instance.BeginTrainingGame
//        );
//#endif

        newGame.GetComponent<Button>().onClick.AddListener(DestroyHighScoreTable);
        newGame.GetComponent<Button>().onClick.AddListener(GameManager.Instance.BeginTrainingGame);
        newGame.GetComponent<Button>().onClick.AddListener(ChangeButtons);

        PauseMenu.Instance.HighScoreMenuButtons[0] = newGame;
        PauseMenu.Instance.SetUsingButtons = PauseMenu.Instance.HighScoreMenuButtons;
        PauseMenu.Instance.ChangeStateOfPointers(true);
    }

    private void ChangeButtons()
    {
        PauseMenu.Instance.SetUsingButtons = PauseMenu.Instance.PauseMenuButtons;
    }
    
    private GameObject CreateUIGameOverTextObject(GameObject toCreate, float x, float y, string text)
    {
        GameObject obj = Instantiate(toCreate, new Vector2(
            GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(x, y, 0)).x,
            GameManager.Instance.GameCamera.ViewportToScreenPoint(new Vector3(x, y, 0)).y), transform.rotation);
        obj.GetComponent<Text>().text = text;
        obj.transform.SetParent(HighScoreMenu.transform);
        return obj;
    }

    private void SaveScoreTableToFile()
    {
        string[] tmp = new string[_highScoreNames.Length * 2];
        for (int i = 0; i < _highScoreNames.Length; i++)
        {
            tmp[i] = _highScoreNames[i];
        }

        for (int i = _highScore.Length; i < _highScore.Length * 2; i++)
        {
            tmp[i] = _highScore[i % 10] + "";
        }

        File.WriteAllLines(ScoreTableFileName, tmp);
    }

    private void LoadScoreTableFromFile()
    {
        string[] tmp = File.ReadAllLines(ScoreTableFileName);

        for (int i = 0; i < _highScoreNames.Length; i++)
        {
            _highScoreNames[i] = tmp[i];
        }

        for (int i = _highScore.Length; i < 2 * _highScore.Length; i++)
        {
            _highScore[i % 10] = Int32.Parse(tmp[i]);
        }
    }

    public void DestroyHighScoreTable()
    {
        _enterPressed = false;
        PauseMenu.Instance.ChangeStateOfPointers(false);
        for (int i = 0; i < HighScoreMenu.transform.childCount; i++)
        {
            Destroy(HighScoreMenu.transform.GetChild(i).gameObject);
        }

        HighScoreMenu.SetActive(false);
        GameManager.Instance.MenuOpened = false;
    }
}