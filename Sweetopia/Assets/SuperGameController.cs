using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SuperGameController : MonoBehaviour
{
    public Button[] cardButtons; // Массив кнопок карт
    public Sprite[] cardSprites; // Массив спрайтов карт
    public GameObject winObject; // Объект для отображения выигрыша
    public GameObject loseObject; // Объект для отображения проигрыша
    public Slider _slider;

    private int firstCardIndex = -1;
    private int secondCardIndex = -1;
    private int cardsMatched = 0;
    private bool canSelect = true;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        ShuffleCards();
        HideCards();
        winObject.SetActive(false);
        loseObject.SetActive(false);
    }

    private void ShuffleCards()
    {
        int n = cardSprites.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // Обмен местами элементов массива
            Sprite temp = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = temp;
        }
    }


    private void HideCards()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            cardButtons[i].interactable = true;
            cardButtons[i].GetComponent<Image>().color = Color.black;
        }
    }

    public void OnCardButtonClick(int cardIndex)
    {
        if (!canSelect) return;

        if (firstCardIndex == -1)
        {
            firstCardIndex = cardIndex;
            ShowCard(cardIndex);
        }
        else if (secondCardIndex == -1 && cardIndex != firstCardIndex)
        {
            secondCardIndex = cardIndex;
            ShowCard(cardIndex);

            // Сравнить две выбранные карты
            StartCoroutine(CheckCardsMatch());
        }
    }

    private void ShowCard(int cardIndex)
    {
        cardButtons[cardIndex].image.sprite = cardSprites[cardIndex];
        cardButtons[cardIndex].GetComponent<Image>().color = Color.white;
    }

    private IEnumerator CheckCardsMatch()
    {
        canSelect = false;

        yield return new WaitForSeconds(1f); // Подождать немного перед проверкой

        if (cardSprites[firstCardIndex] == cardSprites[secondCardIndex])
        {
            cardsMatched++;
            if (cardsMatched == 2)
            {
                winObject.SetActive(true);

                yield return new WaitForSeconds(3f);

                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 10) + (int)_slider.value * 10);

                LoadScene.LoadSceneByRelativeIndex();
            }
        }
        else
        {
            loseObject.SetActive(true);

            yield return new WaitForSeconds(3f);

            LoadScene.LoadSceneByRelativeIndex();
        }

        firstCardIndex = -1;
        secondCardIndex = -1;
        canSelect = true;
    }
}
