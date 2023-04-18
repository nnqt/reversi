using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TextMeshProUGUI blackScoreText;

    [SerializeField]
    private TextMeshProUGUI whiteScoreText;

    [SerializeField]
    private TextMeshProUGUI winnerText;

    [SerializeField]
    private Image blackOverlay;

    [SerializeField]
    private RectTransform playAgainButton;
    
    public IEnumerator SetPlayerText(Player currentPlayer)
	{
        if (currentPlayer == Player.Black)
		{
            topText.text = "Black's Turn <sprite name=DiscBlackUp>";
		}
        else if (currentPlayer == Player.White)
		{
            topText.text = "White's Turn <sprite name=DiscWhiteUp>";
		}
        yield return null;
	}

    public IEnumerator SetPlayerText(Player currentPlayer, Player AI)
    {
        if (AI == Player.Black)
        {
            topText.text = "Black's Turn <sprite name=DiscBlackUp> (Computer Calculating)";
        }
        else if (AI == Player.White)
        {
            topText.text = "White's Turn <sprite name=DiscWhiteUp> (Computer Calculating)";
        }
        yield return null;
    }

    public IEnumerator SetPlayerTextOnline(Player currentPlayer, bool isMyTurn)
    {
        if (currentPlayer == Player.Black )
        {
            if (!isMyTurn)
            {
                topText.text = "<sprite name=DiscBlackUp> (Opponent Turn)";
            }
			else
			{
                topText.text = "<sprite name=DiscBlackUp> (Your Turn)";
            }
        }
        else if (currentPlayer == Player.White)
        {
            if (!isMyTurn)
            {
                topText.text = "<sprite name=DiscWhiteUp> (Opponent Turn)";
            }
            else
            {
                topText.text = "<sprite name=DiscWhiteUp> (Your Turn)";
            }
        }
        yield return null;
    }

    public void SetTopText (string message)
	{
        topText.text = message;
	}

    public void SetSkippedText(Player skippedPlayer)
	{
        if (skippedPlayer == Player.Black)
		{
            topText.text = "Black Cannot Move! <sprite name=DiscBlackUp>";
		}
        else if (skippedPlayer == Player.White)
		{
            topText.text = "White Cannot Move! <sprite name=DiscWhiteUp>";
		}
	}

    public void ShowScoreText()
	{
        topText.gameObject.SetActive(false);
        blackScoreText.gameObject.SetActive(true);
        whiteScoreText.gameObject.SetActive(true);
	}

    public void SetBlackScoreText(int score)
	{
        blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private void ShowOverlay()
	{
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.color = Color.clear;
	}

    private IEnumerator HideOverlay()
	{
        yield return new WaitForSeconds(0.2f);
        blackOverlay.gameObject.SetActive(false);
	}

    public void SetWinnerText (Player winner)
	{
        switch (winner)
		{
            case Player.Black:
                winnerText.text = "Black Won!";
                break;
            case Player.White:
                winnerText.text = "White Won!";
                break;
            case Player.None:
                winnerText.text = "It's a Tie";
                break;
		}
	}

    public void ShowEndScreen()
	{
        blackOverlay.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
        winnerText.gameObject.SetActive(true);
	}

    public void HideEndScreen()
	{
        blackOverlay.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        winnerText.gameObject.SetActive(false);
    }
}
