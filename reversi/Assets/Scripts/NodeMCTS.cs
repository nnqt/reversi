using System;
using System.Collections.Generic;

public class NodeMCTS
{
    public GameState State { get; }
    public NodeMCTS ParentNode { get; }
    public List<NodeMCTS> ChildNodes { get; }
    public List<Position> UntriedMoves { get; }
    public int Visits { get; set; }
    public float Wins { get; set; }


	public NodeMCTS(GameState state, NodeMCTS parentNode)
	{
        State = state;

        Visits = 0;
        Wins = 0;

        ParentNode = parentNode;
        UntriedMoves = new List<Position>(State.LegalMoves.Keys);
        ChildNodes = new List<NodeMCTS>(UntriedMoves.Count);
	}

}
public static class NodeMCTSExtention
{
    #region Phase 1: SELECTION

    public static NodeMCTS SelectChild(this NodeMCTS node, Player rootPlayer)
	{
        bool isRootPlayer = node.State.CurrentPlayer == rootPlayer;

        NodeMCTS bestChild = null;
        double maxChildUcb = double.MinValue;

        foreach(var childNode in node.ChildNodes)
		{
            double childNodeUcb = childNode.GetUcb();
            if(childNodeUcb > maxChildUcb)
			{
                bestChild = childNode;
                maxChildUcb = childNodeUcb;
			}
		}

        return bestChild;
	}

    private static double GetUcb(this NodeMCTS node)
    { 
        var c = Math.Sqrt(2);
        return node.Wins / node.Visits + c * Math.Sqrt(Math.Log(node.ParentNode.Visits) / node.Visits);
	}

    #endregion

    #region Phase 2: EXPANSION
    public static NodeMCTS ExpandChild(this NodeMCTS node)
	{
        var rand = new Random();
        var i = rand.Next(node.UntriedMoves.Count);
        Position move = node.UntriedMoves[i];
        node.UntriedMoves.RemoveAt(i);

        var newState = node.State.NextState(move);

        var child = new NodeMCTS(newState, node);
        node.ChildNodes.Add(child);

        return child;
	}
    #endregion

    #region Phase3: SIMULATION
    public static float Simulate(this NodeMCTS node)
	{
        var newState = node.State.Clone();

		while (!newState.GameOver)
		{
            var legalMove = new List<Position>(newState.LegalMoves.Keys);
            var rand = new Random();
            var i = rand.Next(legalMove.Count);
            Position move = legalMove[i];

            newState.NextState(move);
        }

        if (newState.Winner == newState.AI)
            return 1.0f;
        else if (newState.Winner == newState.Player1)
            return 0.0f;
        else
            return 0.5f;
	}
    #endregion

    #region Phase 4: BACKPROPAGATION 
    public static void BackPropagate(this NodeMCTS node, float reward)
	{
        while(node != null)
		{
            node.Wins += reward;
            node.Visits++;

            node = node.ParentNode;
		}
	}
	#endregion
	#region
    public static bool IsFullyExpanded (this NodeMCTS node)
	{
        return node.UntriedMoves.Count == 0;
	}

    public static bool HasChildNode (this NodeMCTS node)
	{
        return node.ChildNodes.Count != 0;
	}

    public static bool IsLeaf(this NodeMCTS node)
	{
        return node.IsFullyExpanded() && !node.HasChildNode();
	}
	#endregion
}

