using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class GameController : MonoBehaviour
{ 
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject[] _prefabs;
    private List<GameObject> _selectedPrefabs = new List<GameObject>();
    [SerializeField] private Sprite[] _availableSprites;
    [SerializeField] private SpriteRenderer[] _spriteRenderers;
    [SerializeField] private Animator _animator;
    [SerializeField] private Slider _betSlider;
    [SerializeField] private TextMeshProUGUI _betAmountText;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject[] _initialColumn1; // Объекты в колонке 1
    [SerializeField] private GameObject[] _initialColumn2; // Объекты в колонке 2
    [SerializeField] private GameObject[] _initialColumn3; // Объекты в колонке 3
    [SerializeField] private GameObject[] _initialColumn4; // Объекты в колонке 4
    [SerializeField] private GameObject _linePrefab, _winScreen, _looseScreen, _superGameScreen;
    [SerializeField] private GameObject _crystalPrefab; // Префаб кристаллика (супер игры)
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip boomAudioClip, fallAudioClip;


    private int _currentSpawnPointIndex = 0;
    private bool _canShake = false;
    private int _coins;
    private List<List<GameObject>> _grid = new List<List<GameObject>>();

    private int _winCombosCount = 0;

    private void Awake()
    {
        InitializeGrid();
        InitializeColumns();

        _betSlider.maxValue = PlayerPrefs.GetInt("Coins", 10);
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < 4; i++)
        {
            _grid.Add(new List<GameObject>());
        }
    }

    private void InitializeColumns()
    {
        AddObjectsToColumn(_initialColumn1, 0);
        AddObjectsToColumn(_initialColumn2, 1);
        AddObjectsToColumn(_initialColumn3, 2);
        AddObjectsToColumn(_initialColumn4, 3);
    }

    private void AddObjectsToColumn(GameObject[] objects, int columnIndex)
    {
        if (objects != null && objects.Length > 0)
        {
            foreach (var obj in objects)
            {
                _grid[columnIndex].Add(obj);
            }
        }
    }

    private void Update()
    {
        if (IsShakeDetected() && _canShake)
        {
            _canShake = false;
            StartExplodeAnimation();
        }
    }

    private bool IsShakeDetected()
    {
        if (Input.acceleration.sqrMagnitude > 2f)
        {
            return true;
        }
        return false;
    }

    private void StartExplodeAnimation()
    {
        _animator.SetBool("Explode", true);
        audioSource.PlayOneShot(boomAudioClip);
        Invoke("SpawnAndMovePrefab", 0.5f);
    }

    private void SpawnAndMovePrefab()
    {
        if (_currentSpawnPointIndex >= _spawnPoints.Length)
        {
            EndGame();
            return;
        }

        AddPrefabToColumn(_currentSpawnPointIndex);

        _currentSpawnPointIndex++;
    }

    private void AddPrefabToColumn(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= _spawnPoints.Length) return;

        GameObject randomPrefab = _selectedPrefabs[Random.Range(0, _selectedPrefabs.Count)];

        GameObject prefab = Instantiate(randomPrefab, _animator.gameObject.transform.position, Quaternion.identity);

        _grid[columnIndex].Add(prefab);

        // Плавное движение префаба к точке спавна с использованием DOTween
        prefab.transform.DOMove(_spawnPoints[columnIndex].position, 1f).OnComplete(() =>
        {
            audioSource.PlayOneShot(fallAudioClip);
            if(randomPrefab == _crystalPrefab){
                _superGameScreen.SetActive(true);

                _canShake = false;

                _selectedPrefabs.Remove(_crystalPrefab);

                ResetExplodeAnimation();
            }
            else{
                CheckForWinningCombinations();

                if (_currentSpawnPointIndex == _spawnPoints.Length)
                {
                    EndGame();
                }
                else
                {
                    ResetExplodeAnimation();
                    _canShake = true;
                }
            }
        });
    }

    public void SetShake(bool toogle){
        _canShake = toogle;
    }

    private void ResetExplodeAnimation()
    {
        _animator.SetBool("Explode", false);
    }

    private void EndGame()
    {
        Debug.Log("Игра окончена");
        // Здесь может быть логика для определения выигрыша или проигрыша
        // Включаем UI элементы для новой игры
        _canShake = false;
        _currentSpawnPointIndex = 0;

        if(_winCombosCount > 0){
            _winScreen.SetActive(true);

            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 10) + (int)_betSlider.value * 3);

            StartCoroutine(delayLoadLevel());
        }

        else{
            _looseScreen.SetActive(true);

            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 10) - (int)_betSlider.value);

            StartCoroutine(delayLoadLevel());
        }
    }

    private IEnumerator delayLoadLevel(){
        yield return new WaitForSeconds(3f);

        LoadScene.LoadSceneByRelativeIndex();
    }

    public void StartGame()
    {
        _canShake = true;
        _coins -= (int)_betSlider.value;
        SelectUniquePrefabs();
        AssignRandomSprites();
        _startButton.gameObject.SetActive(false);
        _betSlider.gameObject.SetActive(false);
    }

    private void SelectUniquePrefabs()
    {
        List<GameObject> tempList = new List<GameObject>(_prefabs);
        _selectedPrefabs.Clear();

        // Добавление обычных префабов
        while (_selectedPrefabs.Count < 3 && tempList.Count > 0)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            _selectedPrefabs.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        // Добавление кристаллика с низкой вероятностью (например, 5% шанс)
        if (Random.Range(0f, 1f) <= 1f)
        {
            _selectedPrefabs.Add(_crystalPrefab);
        }
    }


    private void AssignRandomSprites()
    {
        foreach (var spriteRenderer in _spriteRenderers)
        {
            int randomIndex = Random.Range(0, _availableSprites.Length);
            spriteRenderer.sprite = _availableSprites[randomIndex];
        }
    }

    public void UpdateBetAmount()
    {
        _betAmountText.text = _betSlider.value.ToString();
    }

    private void CheckForWinningCombinations()
    {
        // Проверка по вертикали
        for (int columnIndex = 0; columnIndex < _grid.Count; columnIndex++)
        {
            List<GameObject> column = _grid[columnIndex];
            if (column.Count < 3)
            {
                continue; // Нет смысла проверять меньше 3 объектов в столбце
            }

            Sprite lastSprite = column[0].GetComponent<SpriteRenderer>().sprite;
            int comboStart = 0;

            for (int i = 1; i < column.Count; i++)
            {
                Sprite currentSprite = column[i].GetComponent<SpriteRenderer>().sprite;
                if (currentSprite == lastSprite)
                {
                    // Если текущий спрайт такой же, как и предыдущий, и мы находимся в конце списка или спрайты различаются
                    if (i == column.Count - 1 || column[i + 1].GetComponent<SpriteRenderer>().sprite != currentSprite)
                    {
                        int comboLength = i - comboStart + 1;
                        if (comboLength >= 3)
                        {
                            // Найдена комбинация длиной 3 или более
                            Debug.Log($"Winning combo found from {comboStart} to {i} in column {columnIndex}");
                            DrawLine(column[comboStart].transform.position, column[i].transform.position);

                            _winCombosCount++;
                        }
                        lastSprite = null; // Сброс для следующей проверки
                    }
                }
                else
                {
                    comboStart = i;
                    lastSprite = currentSprite;
                }
            }
        }

        // Проверка по горизонтали
        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
        {
            List<GameObject> row = new List<GameObject>();
            for (int columnIndex = 0; columnIndex < _grid.Count; columnIndex++)
            {
                if (rowIndex < _grid[columnIndex].Count)
                {
                    row.Add(_grid[columnIndex][rowIndex]);
                }
                else
                {
                    break; // Если в столбце нет объекта в данной строке, прекратить собирать ряд
                }
            }

            if (row.Count < 3)
            {
                continue; // Нет смысла проверять меньше 3 объектов в ряду
            }

            Sprite lastSprite = row[0].GetComponent<SpriteRenderer>().sprite;
            int comboStart = 0;

            for (int i = 1; i < row.Count; i++)
            {
                Sprite currentSprite = row[i].GetComponent<SpriteRenderer>().sprite;
                if (currentSprite == lastSprite)
                {
                    // Если текущий спрайт такой же, как и предыдущий, и мы находимся в конце списка или спрайты различаются
                    if (i == row.Count - 1 || row[i + 1].GetComponent<SpriteRenderer>().sprite != currentSprite)
                    {
                        int comboLength = i - comboStart + 1;
                        if (comboLength >= 3)
                        {
                            // Найдена комбинация длиной 3 или более
                            Debug.Log($"Winning combo found from {comboStart} to {i} in row {rowIndex}");
                            DrawLine(row[comboStart].transform.position, row[i].transform.position);

                            _winCombosCount++;
                        }
                        lastSprite = null; // Сброс для следующей проверки
                    }
                }
                else
                {
                    comboStart = i;
                    lastSprite = currentSprite;
                }
            }
        }
    }


    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[] { start, end });
    }
}
