using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public GameObject Text;

    public GameObject LightOnKey;
    public GameObject Floor;
    public GameObject GodParticles;
    public GameObject BadParticles;
    public GameObject CharA;
    public GameObject CharB;
    public GameObject CharC;
    public GameObject CharD;
    public GameObject CharE;
    public GameObject CharF;
    public GameObject CharG;
    public GameObject CharH;
    public GameObject CharI;
    public GameObject CharJ;
    public GameObject CharK;
    public GameObject CharL;
    public GameObject CharM;
    public GameObject CharN;
    public GameObject CharO;
    public GameObject CharP;
    public GameObject CharQ;
    public GameObject CharR;
    public GameObject CharS;
    public GameObject CharT;
    public GameObject CharU;
    public GameObject CharV;
    public GameObject CharW;
    public GameObject CharX;
    public GameObject CharY;
    public GameObject CharZ;
    public GameObject Char0;
    public GameObject Char1;
    public GameObject Char2;
    public GameObject Char3;
    public GameObject Char4;
    public GameObject Char5;
    public GameObject Char6;
    public GameObject Char7;
    public GameObject Char8;
    public GameObject Char9;

    private const String CoeficientsFileName = "coefForCalibration";
    private const int CountOfChars = 36;
    private const float AlphaInitialVelocity = 600f;
    private readonly System.Random _rnd = new System.Random();

    public static GameManager Instance;

    private bool _trainingGame;
    private bool _ninjaKeyboard;
    private bool _binding;

    private bool _spacePressed;
    private GameObject _textForBinding;
    private int _counterChar;

    private bool _inGame;
    private int _fallingCounter;
    private GameObject _startGameText;
    private GameObject _scoreText;
    private GameObject _livesText;
    private float _time;
    private float _timeBetweenFallingAlpha;
    private int _score;
    private int _lives;
    private LinkedList<GameObject> _fallingAlphas;


    private GameObject[] _gameObjects;
    private float[] _xOfChars = new float[CountOfChars];
    private float[] _yOfChars = new float[CountOfChars];

    private GameObject _parent;

    private bool _menuOpened;

    public bool MenuOpened
    {
        set { _menuOpened = value; }
    }

    public bool CanPause
    {
        get { return !_spacePressed; }
    }

    public Camera GameCamera
    {
        get { return Camera; }
    }

    private void Start()
    {
        _menuOpened = true;
        _parent = new GameObject {name = "Game Objects"};
        _gameObjects = new[]
        {
            Char1, Char2, Char3, Char4, Char5, Char6, Char7, Char8, Char9, Char0, CharQ, CharW, CharE, CharR, CharT,
            CharY, CharU, CharI, CharO, CharP, CharA, CharS, CharD, CharF, CharG, CharH, CharJ, CharK, CharL, CharZ,
            CharX, CharC, CharV, CharB, CharN, CharM
        };
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Update()
    {
        if (_ninjaKeyboard)
        {
            NinjaKeyboard();
            return;
        }

        if (_trainingGame)
        {
            TrainingGame();
            return;
        }

        if (_binding)
        {
            BindChars();
            return;
        }
    }

    public void BeginNinjaKeyboard()
    {
        Floor.GetComponent<BoxCollider2D>().isTrigger = true;
        ShowKeyboardTop(false);
        DestroyAllObjects();
        _trainingGame = false;
        _binding = false;
        _ninjaKeyboard = true;
    }

    public void BeginTrainingGame()
    {
        _inGame = false;
        Floor.GetComponent<BoxCollider2D>().isTrigger = false;
        ShowKeyboardTop(false);
        _time = Time.time;
        _lives = 5;
        _score = 0;
        _timeBetweenFallingAlpha = 1f;
        DestroyAllObjects();

        _startGameText = CreateTextObject(Text, 0.5f, 0.5f, "Press SPACE to start game!");

        _startGameText.GetComponent<TextMesh>().anchor = TextAnchor.UpperCenter;
        CreateTextObject(Text, 0, 1, "Score:");
        _scoreText = CreateTextObject(Text, 0, 0.95f, "" + _score);

        CreateTextObject(Text, 1, 1, "Lives:").GetComponent<TextMesh>().anchor = TextAnchor.UpperRight;
        _livesText = CreateTextObject(Text, 1, 0.95f, "" + _lives);
        _livesText.GetComponent<TextMesh>().anchor = TextAnchor.UpperRight;

        _fallingAlphas = new LinkedList<GameObject>();

        _trainingGame = true;
        _binding = false;
        _ninjaKeyboard = false;
    }

    public void BeginBinding()
    {
        Floor.GetComponent<BoxCollider2D>().isTrigger = true;
        ShowKeyboardTop(true);
        DestroyAllObjects();

        _textForBinding = CreateTextObject(Text, 0.05f, 1, "Press SPACE to begin binding");

        GameObject infoText = CreateTextObject(Text, 0.05f, 0.9f, "Set keyboard to center and change");
        infoText.GetComponent<TextMesh>().fontSize = 40;

        GameObject infoText1 = CreateTextObject(Text, 0.05f, 0.85f, "position and scale of floor with arrows");
        infoText1.GetComponent<TextMesh>().fontSize = 40;

        _trainingGame = false;
        _ninjaKeyboard = false;
        _binding = true;
    }

    private void NinjaKeyboard()
    {
        if (Input.anyKey)
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (Input.GetKey(_gameObjects[i].name.ToLower()))
                {
                    GameObject alpha = Instantiate(_gameObjects[i],
                        new Vector2(_xOfChars[i], Floor.transform.position.y + 0.4F), transform.rotation);
                    alpha.GetComponent<Rigidbody2D>().AddForce(new Vector2(_rnd.Next(-100, 100),
                        AlphaInitialVelocity + _rnd.Next(-100, 100)));
                    Instantiate(LightOnKey, new Vector2(_xOfChars[i], _yOfChars[i]), transform.rotation);
                    alpha.transform.parent = _parent.transform;
                }
            }
        }
    }

    private void TrainingGame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _inGame = true;
            Destroy(_startGameText);
        }

        if (_inGame)
        {
            if (Time.time - _time > _timeBetweenFallingAlpha)
            {
                _time = Time.time;
                int numberOfAlphaToFall = _rnd.Next(0, CountOfChars);
                GameObject alpha = Instantiate(_gameObjects[numberOfAlphaToFall],
                    new Vector2(_xOfChars[numberOfAlphaToFall],
                        GameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y),
                    transform.rotation);
                alpha.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
                alpha.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 50);
                alpha.transform.parent = _parent.transform;
                _fallingAlphas.AddLast(alpha);
                _fallingCounter++;
                if (_fallingCounter % 5 == 0 && _timeBetweenFallingAlpha > 0.5)
                {
                    _fallingCounter = 0;
                    _timeBetweenFallingAlpha -= 0.05f;
                }
            }

            if (Input.anyKey)
            {
                foreach (GameObject alpha in _fallingAlphas)
                {
                    if (Input.GetKeyDown(alpha.name.ToLower().Substring(0, 1)))
                    {
                        _score++;
                        _scoreText.GetComponent<TextMesh>().text = "" + _score;
                        Instantiate(LightOnKey, GetPositionOfPressedAlpha(alpha), transform.rotation);
                        RemoveFallingAlpha(alpha, false);
                        break;
                    }
                }
            }
        }
    }

    private void BindChars()
    {
        //Floor settings 
        if(!_menuOpened){
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Space) && !_spacePressed)
                {
                    _spacePressed = true;
                    _textForBinding.GetComponent<TextMesh>().text = "Click on 1.";
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Floor.transform.localScale += new Vector3(0.005F, 0.005F, 0.005F);
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Floor.transform.localScale -= new Vector3(0.005F, 0.005F, 0.005F);
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Floor.transform.position += new Vector3(0, 0.005F, 0);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Floor.transform.position -= new Vector3(0, 0.005F, 0);
                }

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.L))
                {
                    LoadCalibrationFromFile();
                }
            }
        }

        if (_spacePressed)
        {
            if (_counterChar < CountOfChars)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                    _xOfChars[_counterChar] = mousePosition.x;
                    _yOfChars[_counterChar] = mousePosition.y;
                    _counterChar++;
                    if (_counterChar < CountOfChars)
                    {
                        _textForBinding.GetComponent<TextMesh>().text =
                            "Click on " + _gameObjects[_counterChar].name + ".";
                    }
                    else
                    {
                        _textForBinding.GetComponent<TextMesh>().text = "Click on keyboard top.";
                    }
                }
            }
            else
            {
                if (_counterChar == CountOfChars)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                        Floor.transform.position = new Vector2(Floor.transform.position.x, mousePosition.y);
                        _counterChar++;
                        SaveCalibrationToFile();
                    }
                }
                else
                {
                    _textForBinding.GetComponent<TextMesh>().text = "Press SPACE to begin binding";
                    _spacePressed = false;
                    _counterChar = 0;
                }
            }
        }

        if (Input.anyKey)
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (Input.GetKey(_gameObjects[i].name.ToLower()))
                {
                    Instantiate(LightOnKey, new Vector2(_xOfChars[i], _yOfChars[i]), transform.rotation);
                }
            }
        }
    }

    private Vector2 GetPositionOfPressedAlpha(GameObject alpha)
    {
        Vector2 position = new Vector2();

        string alphaToCheck = alpha.name.Substring(0, 1);

        for (int i = 0; i < CountOfChars; i++)
        {
            if (_gameObjects[i].name.Equals(alphaToCheck))
            {
                position.x = _xOfChars[i];
                position.y = _yOfChars[i];
                break;
            }
        }

        return position;
    }

    public void RemoveFallingAlpha(GameObject alpha, bool bad)
    {
        if (bad)
        {
            Instantiate(BadParticles, alpha.transform.position, Quaternion.identity).transform.parent =
                _parent.transform;
        }
        else
        {
            Instantiate(GodParticles, alpha.transform.position, Quaternion.identity).transform.parent =
                _parent.transform;
        }

        Destroy(alpha);
        _fallingAlphas.Remove(alpha);
    }

    private void DestroyAllObjects()
    {
        for (int i = 0; i < _parent.transform.childCount; i++)
        {
            Destroy(_parent.transform.GetChild(i).gameObject);
        }
    }


    public void LoseLife()
    {
        _lives--;
        _livesText.GetComponent<TextMesh>().text = "" + _lives;
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (_lives < 1)
        {
            GameObject scoreText = CreateTextObject(Text, 0.5f, 0.5f, "GameOver!");
            scoreText.GetComponent<TextMesh>().anchor = TextAnchor.UpperCenter;


            _inGame = false;
            DestroyAllObjects();
            GameOverParticles();

            HighScoreMenuManager.Instance.ShowHighScoreTable(_score);
            MenuOpened = true;
        }
    }

    private void GameOverParticles()
    {
        GameObject gameOverParticles = Instantiate(BadParticles, new Vector2(
            GameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).x,
            GameCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).y), Quaternion.identity);
        gameOverParticles.transform.rotation = GodParticles.transform.rotation;
        var main = gameOverParticles.GetComponent<ParticleSystem>().main;
        gameOverParticles.transform.SetParent(_parent.transform);
        main.loop = false;
        main.startLifetime = 5;
        main.startSpeed = 50;
        main.startSize = 20;
    }

    private void ShowKeyboardTop(bool b)
    {
        Color color = Color.clear;
        if (b)
        {
            color = Color.white;
        }

        for (int i = 0; i < Floor.transform.childCount; i++)
        {
            SpriteRenderer sr = Floor.transform.GetChild(i).GetComponent<SpriteRenderer>();
            sr.color = color;
        }
    }

    private void SaveCalibrationToFile()
    {
        string[] tmp = new string[_xOfChars.Length * 2];
        for (int i = 0; i < _xOfChars.Length; i++)
        {
            tmp[2 * i] = _xOfChars[i] + "";
            tmp[2 * i + 1] = _yOfChars[i] + "";
        }

        File.WriteAllLines(CoeficientsFileName, tmp);
    }

    private void LoadCalibrationFromFile()
    {
        string[] tmp = File.ReadAllLines(CoeficientsFileName);

        for (int i = 0; i < _xOfChars.Length; i++)
        {
            _xOfChars[i] = float.Parse(tmp[2 * i]);
            _yOfChars[i] = float.Parse(tmp[2 * i + 1]);
        }
    }

    private GameObject CreateTextObject(GameObject toCreate, float x, float y, string text)
    {
        GameObject obj = Instantiate(toCreate, new Vector2(
            GameCamera.ViewportToWorldPoint(new Vector3(x, y, 0)).x,
            GameCamera.ViewportToWorldPoint(new Vector3(x, y, 0)).y), transform.rotation);
        obj.GetComponent<TextMesh>().text = text;
        obj.transform.parent = _parent.transform;
        return obj;
    }
}