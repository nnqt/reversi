using System.Collections.Generic;
using System;

public class GameState
{
    public const int Rows = 8;
    public const int Cols = 8;

    public Player[,] Board { get; private set; }
    public Dictionary<Player, int> DiscCount { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public bool GameOver { get; private set; }
    public Player Winner { get; private set; }
    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    public Player AI { get; private set; }

    public GameState()
    {
        Board = new Player[Rows, Cols];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;

        DiscCount = new Dictionary<Player, int>()
        {
            {Player.Black, 2 },
            {Player.White, 2 }
        };

        CurrentPlayer = Player.Black;
        LegalMoves = FindLegalMoves(CurrentPlayer);

        GameOver = false;

        Random rand = new Random();
        var playFirst = rand.Next(0, 2);
        if (playFirst == 0)
        {
            Player1 = Player.White;
            Player2 = Player.None;
            AI = Player.Black;
        }
        else
        {
            Player1 = Player.Black;
            Player2 = Player.None;
            AI = Player.White;
        }
    }

    public bool MakeMove(Position pos, out MoveInfo moveInfo)
    {
        if (!LegalMoves.ContainsKey(pos))
        {
            moveInfo = null;
            return false;
        }
        Player movePlayer = CurrentPlayer;
        List<Position> outflanked = LegalMoves[pos];

        Board[pos.Row, pos.Col] = movePlayer;
        // Flip Discs
        FlipDiscs(outflanked);
        // Update Disc Counts
        UpdateDiscCount(movePlayer, outflanked.Count);
        // Pass Turn
        PassTurn();

        moveInfo = new MoveInfo { Player = movePlayer, Position = pos, Outflanked = outflanked };
        return true;
    }

    public IEnumerable<Position> OccupiedPosition()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (Board[r, c] != Player.None)
                {
                    yield return new Position(r, c);
                }
            }
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (Position pos in positions)
        {
            Board[pos.Row, pos.Col] = Board[pos.Row, pos.Col].Opponent();
        }
    }

    private void UpdateDiscCount(Player movePlayer, int outflankedCount)
    {
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Opponent()] -= outflankedCount;
    }

    private void ChangePlayer()
    {
        CurrentPlayer = CurrentPlayer.Opponent();
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {
        if (DiscCount[Player.Black] > DiscCount[Player.White])
        {
            return Player.Black;
        }
        if (DiscCount[Player.White] > DiscCount[Player.Black])
        {
            return Player.White;
        }
        return Player.None;
    }

    private void PassTurn()
    {
        ChangePlayer();

        if (LegalMoves.Count > 0)
        {
            return;
        }

        ChangePlayer();

        if (LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None;
            GameOver = true;
            Winner = FindWinner();
        }
    }

    private bool IsInsideBoard(int r, int c)
    {
        return r >= 0 && r < Rows && c >= 0 && c < Cols;
    }

    private List<Position> OutflankedInDir(Position pos, Player player, int rDelta, int cDelta)
    {
        List<Position> outflanked = new List<Position>();
        int r = pos.Row + rDelta;
        int c = pos.Col + cDelta;

        while (IsInsideBoard(r, c) && Board[r, c] != Player.None)
        {
            if (Board[r, c] == player.Opponent())
            {
                outflanked.Add(new Position(r, c));
                r += rDelta;
                c += cDelta;
            }
            else
            {
                return outflanked;
            }
        }
        return new List<Position>();
    }

    private List<Position> Outflanked(Position pos, Player player)
    {
        List<Position> outflanked = new List<Position>();

        for (int rDelta = -1; rDelta <= 1; rDelta++)
        {
            for (int cDelta = -1; cDelta <= 1; cDelta++)
            {
                if (rDelta == 0 && cDelta == 0)
                {
                    continue;
                }

                outflanked.AddRange(OutflankedInDir(pos, player, rDelta, cDelta));
            }
        }

        return outflanked;
    }

    private bool IsMoveLegal(Player player, Position pos, out List<Position> outflanked)
    {
        if (Board[pos.Row, pos.Col] != Player.None)
        {
            outflanked = null;
            return false;
        }

        outflanked = Outflanked(pos, player);
        return outflanked.Count > 0;
    }

    private Dictionary<Position, List<Position>> FindLegalMoves(Player player)
    {
        Dictionary<Position, List<Position>> legalMoves = new Dictionary<Position, List<Position>>();

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                Position pos = new Position(r, c);

                if (IsMoveLegal(player, pos, out List<Position> outflanked))
                {
                    legalMoves[pos] = outflanked;
                }
            }
        }

        return legalMoves;
    }

    public GameState Clone()
	{
        GameState newGameState = new GameState();
        for (int r = 0; r < Rows; r++)
		{
            for (int c = 0; c < Cols; c++)
			{
                //            switch (Board[r, c]){
                //                case Player.Black:
                //                    newGameState.Board[r, c] = Player.Black;
                //                    break;
                //                case Player.White:
                //                    newGameState.Board[r, c] = Player.White;
                //                    break;
                //                case Player.None:
                //                    newGameState.Board[r, c] = Player.None;
                //                    break;
                //}
                newGameState.Board[r, c] = Board[r, c];
			}
		}

        newGameState.DiscCount = new Dictionary<Player, int>()
        {
            {Player.Black, DiscCount[Player.Black] },
            {Player.White, DiscCount[Player.White] }
        };


        newGameState.CurrentPlayer = CurrentPlayer;
        newGameState.GameOver = GameOver;
        newGameState.Winner = Winner;
        newGameState.Player1 = Player1;
        newGameState.Player2 = Player2;
        newGameState.AI = AI;
        newGameState.LegalMoves = FindLegalMoves(CurrentPlayer);

        return newGameState;
    }
}

public static class GameStateExtensions
{
    public static GameState NextState(this GameState gameState, Position pos)
	{
        GameState newState = gameState.Clone();
        newState.MakeMove(pos, out MoveInfo moveInfo);
        return newState;
	}
}