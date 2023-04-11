using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Disc discBlackUp;

    [SerializeField]
    private Disc discWhiteUp;

    [SerializeField]
    private GameObject highlightPrefab;

    [SerializeField]
    private UIManager uiManager;

    private Dictionary<Player, Disc> discPrefabs = new Dictionary<Player, Disc>();
    static public GameState gameState = new GameState();
    private Disc[,] discs = new Disc[8, 8];
    private List<GameObject> highlights = new List<GameObject>();

    private bool isAICalculating = false;
    private bool isAnimation = false;

	// Start is called before the first frame update
	private IEnumerator Start()
    {
        discPrefabs[Player.Black] = discBlackUp;
        discPrefabs[Player.White] = discWhiteUp;

        AddStartDiscs();
        ShowLegalMoves();
        yield return uiManager.SetPlayerText(gameState.CurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
		{
            Application.Quit();
		}
        if (Input.GetMouseButtonDown(0) && gameState.CurrentPlayer == gameState.Player1)
		{
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
                    Vector3 impact = hitInfo.point;
                    Position boardPos = SceneToBoardPos(impact);
                    OnBoardClicked(boardPos);
                    StartCoroutine(uiManager.SetPlayerText(gameState.CurrentPlayer, gameState.AI));
            }
		}
        else if (gameState.CurrentPlayer == gameState.AI && !isAICalculating && !isAnimation)
		{
            isAICalculating = true;

            AI computer = new AI(gameState);
            Position boardPos = new Position(-1, -1);

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == "GameSceneEasyAI")
			{
                boardPos = computer.randomMove();
                //Debug.Log("random");
			}
            else if (activeScene.name == "GameSceneMediumAI")
            {
                boardPos = computer.minimaxDecision(gameState);
                //Debug.Log("minimax");

            }
            OnComputerChose(boardPos);
            isAICalculating = false;
            StartCoroutine(uiManager.SetPlayerText(gameState.CurrentPlayer));
		}
    }


    private void ShowLegalMoves()
	{
            foreach(Position boardPos in gameState.LegalMoves.Keys)
		    {
                Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
                GameObject highlight = Instantiate(highlightPrefab, scenePos, Quaternion.identity);
                highlights.Add(highlight);
		    }
	}

    private void HideLegalMoves()
	{
        highlights.ForEach(Destroy);
        highlights.Clear();
	}

    private void OnBoardClicked(Position boardPos)
	{

        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
		{
            StartCoroutine(OnMoveMade(moveInfo));

		}
	}

    private void OnComputerChose(Position boardPos)
    {

        if (gameState.MakeMove(boardPos, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMadeByComputer(moveInfo));

        }
    }

    private IEnumerator OnMoveMadeByComputer(MoveInfo moveInfo)
    {
        HideLegalMoves();
        isAnimation = true;
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLegalMoves();
        isAnimation = false;
    }

    private IEnumerator OnMoveMade (MoveInfo moveInfo)
	{
        HideLegalMoves();
        isAnimation = true;
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        //ShowLegalMoves();
        isAnimation = false;
	}

    private Position SceneToBoardPos(Vector3 scenePos)
	{
        int col = (int)(scenePos.x - 0.25f);
        int row = 7 - (int)(scenePos.z - 0.25f);
        return new Position(row, col);
	}

    private Vector3 BoardToScenePos(Position boardPos)
	{
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.Row + 0.75f);
	}

    private void SpawnDisc(Disc prefab, Position boardPos)
	{
        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.1f;
        discs[boardPos.Row, boardPos.Col] = Instantiate(prefab, scenePos, Quaternion.identity);
	}

    private void AddStartDiscs()
	{
        foreach (Position boardPos in gameState.OccupiedPosition())
		{
            Player player = gameState.Board[boardPos.Row, boardPos.Col];
            SpawnDisc(discPrefabs[player], boardPos);
		}
	}

    private void FlipDiscs(List<Position> positions)
	{
        foreach (Position boardPos in positions)
		{
            discs[boardPos.Row, boardPos.Col].Flip();
		}
	}

	private IEnumerator ShowMove(MoveInfo moveInfo)
	{
		SpawnDisc(discPrefabs[moveInfo.Player], moveInfo.Position);
		yield return new WaitForSeconds(0.33f);
		FlipDiscs(moveInfo.Outflanked);
		yield return new WaitForSeconds(0.83f);
	}

    private IEnumerator ShowTurnSkipped (Player skippedPlayer)
	{
        uiManager.SetSkippedText(skippedPlayer);
        yield return new WaitForSeconds(0.5f);
	}


    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
	{
		if (gameState.GameOver)
		{
            // Show Game Over
            uiManager.ShowScoreText();
            yield return new WaitForSeconds(0.5f);

            yield return ShowCounting();
            uiManager.SetWinnerText(gameState.Winner);
            uiManager.ShowEndScreen();

            yield break;
		}


        Player currentPlayer = gameState.CurrentPlayer;

        if(currentPlayer == moveInfo.Player)
		{
            yield return ShowTurnSkipped(currentPlayer.Opponent());

		}

        uiManager.SetPlayerText(currentPlayer);
	}

    private IEnumerator ShowCounting()
	{
        int black = 0, white = 0;

        foreach (Position pos in gameState.OccupiedPosition())
		{
            Player player = gameState.Board[pos.Row, pos.Col];

            if(player == Player.Black)
			{
                black++;
                uiManager.SetBlackScoreText(black);
			}
            else if (player == Player.White)
			{
                white++;
                uiManager.SetWhiteScoreText(white);
			}

            discs[pos.Row, pos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
		}
	}

    private void RestartGame()
	{
        uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
        gameState = new GameState();
        Start();
	}

    public void OnPlayAgainClicked()
	{
        RestartGame();
	}

    public void OnMainMenuClicked()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}
}
