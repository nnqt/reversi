using System.Collections.Generic;
using System;

public class AI
{
	GameState state;
	public AI(GameState gameState)
	{
		state = gameState;
	}

	public Position randomMove()
	{
		//Dictionary<Position, List<Position>> LegalMoves = state.LegalMoves;

		List<Position> legalMoves = new List<Position>(state.LegalMoves.Keys);
		Random rand = new Random();
		int index = rand.Next(legalMoves.Count); ;
		return legalMoves[index];
	}

	private int score(GameState gameState, Player curr)
	{
		int score = 0;

		int[,] boardHeuristic = {
			{1000, -10, 10, 10, 10, 10, -10, 1000 },
			{-10, -10, 1, 1, 1, 1, -10, -10 },
			{10, 1, 1, 1, 1, 1, 1, 10 },
			{10, 1, 1, 1, 1, 1, 1, 10 },
			{10, 1, 1, 1, 1, 1, 1, 10 },
			{10, 1, 1, 1, 1, 1, 1, 10 },
			{-10, -10, 1, 1, 1, 1, -10, -10 },
			{1000, -10, 1, 1, 1, 1, -10, 1000 }
		};
		if (curr == Player.Black)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (gameState.Board[i, j] == Player.Black)
					{
						score += boardHeuristic[i, j];
					}
				}
			}
		}
		else if (curr == Player.White)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (gameState.Board[i, j] == Player.White)
					{
						score += boardHeuristic[i, j];
					}
				}
			}
		}
		return score;
	}


	int heuristic(GameState gameState, Player curr)
	{
		Player opponent = curr.Opponent();

		int ourScore = score(gameState, curr);
		int opponentScore = score(gameState, opponent);

		return (ourScore - opponentScore);
	}

	public Position minimaxDecision(GameState gameState)
	{
		List<Position> legalMoves = new List<Position>(gameState.LegalMoves.Keys);
		Position pos = new Position(legalMoves[0].Row, legalMoves[0].Col);
		
		if(legalMoves.Count == 0)
		{
			return null;
		}
		else
		{
			int bestScore = -999999;

			for (int i = 0; i < legalMoves.Count; i++){
				GameState newState = gameState.Clone();
				newState.MakeMove(legalMoves[i], out MoveInfo moveInfo);

				int val = minimaxValue(newState, newState.CurrentPlayer, 1);

				if(val > bestScore)
				{
					bestScore = val;
					pos = new Position(legalMoves[i].Row, legalMoves[i].Col);
				}
			}
		}
		return pos;
	}

	int minimaxValue(GameState gameState, Player currTurn, int searchPly)
	{
		if((searchPly == 4) || gameState.GameOver)
		{
			return heuristic(gameState, gameState.AI);
		}

		Player opponent = currTurn.Opponent();

		List<Position> legalMoves = new List<Position>(gameState.LegalMoves.Keys);

		if(legalMoves.Count == 0)
		{
			return minimaxValue(gameState, opponent, searchPly + 1);
		}
		else
		{
			int bestVal = -9999999;
			if (gameState.AI != currTurn)
				bestVal = 9999999;
			for (int i = 0; i < legalMoves.Count; i++)
			{
				GameState newState = gameState.Clone();
				newState.MakeMove(legalMoves[i], out MoveInfo moveInfo);
				int val = minimaxValue(newState, opponent, searchPly + 1);

				if (gameState.AI == currTurn)
				{
					if(val > bestVal)
					{
						bestVal = val;
					}
				}
				else
				{
					if(val < bestVal)
					{
						bestVal = val;
					}
				}
			}
			return bestVal;
		}
	}

}
