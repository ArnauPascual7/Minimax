using System.Collections;
using UnityEngine;
public enum States
{
    CanMove = 1,
    CantMove = -1
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private States state = States.CanMove;
    [SerializeField] private bool _usePruning = true;

    public BoxCollider2D collider;
    public GameObject token1, token2;
    public int Size = 3;
    public int[,] Matrix;
    public Camera camera;

    private MouseInput _mouse;

    void Start()
    {
        Instance = this;
        Matrix = new int[Size, Size];
        Calculs.CalculateDistances(collider, Size);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Matrix[i, j] = 0; // 0: desocupat, 1: fitxa jugador 1, -1: fitxa IA;
            }
        }

        _mouse = GetComponent<MouseInput>();
    }
    private void Update()
    {
        if (state == States.CanMove && IsMovesLeft() && Calculs.EvaluateWin(Matrix) == 2)
        {
            Vector3 m = _mouse.Position;
            m.z = 10f;
            if (_mouse.Click)
            {
                Vector3 mousepos = camera.ScreenToWorldPoint(m);
                if (Calculs.CheckIfValidClick((Vector2)mousepos, Matrix))
                {
                    state = States.CantMove;

                    if(Calculs.EvaluateWin(Matrix) == 2)
                    {
                        StartCoroutine(WaitingABit());
                    }
                }
            }
        }
    }

    private IEnumerator WaitingABit()
    {
        yield return new WaitForSeconds(1f);
        //RandomAI();
        Node bestNode = FindBestMove((int)state, _usePruning);

        if (bestNode.X == -1 || bestNode.Y == -1)
        {
            Debug.LogWarning("No s'ha trobat cap moviment vàlid");
            state = States.CanMove;
            yield break;
        }

        DoMove(bestNode.X, bestNode.Y, (int)state);

        state = States.CanMove;
    }

    public void RandomAI()
    {
        int x;
        int y;
        do
        {
            x = Random.Range(0, Size);
            y = Random.Range(0, Size);
        } while (Matrix[x, y] != 0);
        DoMove(x, y, -1);
        state = States.CanMove;
    }

    public void DoMove(int x, int y, int team)
    {
        Matrix[x, y] = team;
        if (team == 1)
            Instantiate(token1, Calculs.CalculatePoint(x, y), Quaternion.identity);
        else
            Instantiate(token2, Calculs.CalculatePoint(x, y), Quaternion.identity);
        int result = Calculs.EvaluateWin(Matrix);
        switch (result)
        {
            case 0:
                Debug.Log("Draw");
                break;
            case 1:
                Debug.Log("You Win");
                break;
            case -1:
                Debug.Log("You Lose");
                break;
            case 2:
                if(state == States.CantMove)
                    state = States.CanMove;
                break;
        }
    }

    /// <summary>
    /// Evaluates the optimal score for the current game state using the Minimax algorithm.
    /// </summary>
    /// <remarks>This method to determine the optimal move in Tic-Tac-Toe. The
    /// score returned reflects the outcome of the game assuming both players play optimally. The method assumes the
    /// current board state is valid and that moves are represented by specific integer values.</remarks>
    /// <param name="depth">The current depth in the game tree. Used to adjust the score based on how many moves have been made.</param>
    /// <param name="isMax">Indicates whether the current move is for the maximizing player. Set to <see langword="true"/> for the
    /// maximizing player; otherwise, <see langword="false"/> for the minimizing player.</param>
    /// <returns>An integer representing the best achievable score from the current game state for the specified player. Positive
    /// values favor the maximizing player; negative values favor the minimizing player.</returns>
    private int Minimax(int depth, bool isMax)
    {
        int result = Calculs.EvaluateWin(Matrix);

        if (result == 1) return 10 - depth;
        if (result == -1) return -10 + depth;
        if (result == 0) return 0;

        if (!IsMovesLeft()) return 0;

        if (isMax)
        {
            int bestVal = int.MinValue;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        Matrix[i, j] = 1;

                        bestVal = Mathf.Max(bestVal, Minimax(depth + 1, !isMax));

                        Matrix[i, j] = 0;
                    }
                }
            }

            return bestVal;
        }
        else
        {
            int bestVal = int.MaxValue;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        Matrix[i, j] = -1;

                        bestVal = Mathf.Min(bestVal, Minimax(depth + 1, !isMax));

                        Matrix[i, j] = 0;
                    }
                }
            }

            return bestVal;
        }
    }

    /// <summary>
    /// Evaluates the optimal score for the current game state using the Minimax algorithm with alpha-beta pruning.
    /// </summary>
    /// <remarks>This method implements the Minimax algorithm with alpha-beta pruning to efficiently determine
    /// the best possible move for a player in Tic-Tac-Toe game. It recursively explores possible moves, updating alpha
    /// and beta values to eliminate branches that cannot affect the final decision. The score returned reflects the
    /// desirability of the game state for the maximizing or minimizing player, adjusted by the depth to prefer faster
    /// wins or slower losses.</remarks>
    /// <param name="depth">The current depth of the recursive search. Used to adjust the score based on how many moves have been made.</param>
    /// <param name="isMax">Indicates whether the current move is for the maximizing player. Set to <see langword="true"/> for the
    /// maximizing player; otherwise, <see langword="false"/> for the minimizing player.</param>
    /// <param name="alpha">The best already explored option along the path to the maximizer. Used to prune branches that cannot improve the
    /// outcome for the maximizing player.</param>
    /// <param name="beta">The best already explored option along the path to the minimizer. Used to prune branches that cannot improve the
    /// outcome for the minimizing player.</param>
    /// <returns>An integer representing the evaluated score of the game state. Positive values favor the maximizing player,
    /// negative values favor the minimizing player, and zero indicates a draw or neutral outcome.</returns>
    private int Minimax(int depth, bool isMax, int alpha, int beta)
    {
        int result = Calculs.EvaluateWin(Matrix);

        if (result == 1) return 10 - depth;
        if (result == -1) return -10 + depth;
        if (result == 0) return 0;

        if (!IsMovesLeft()) return 0;

        if (isMax)
        {
            int bestVal = int.MinValue;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        Matrix[i, j] = 1;

                        int val = Minimax(depth + 1, false, alpha, beta);
                        bestVal = Mathf.Max(bestVal, val);
                        alpha = Mathf.Max(alpha, bestVal);

                        Matrix[i, j] = 0;

                        if (beta <= alpha)
                            break;
                    }
                }

                if (beta <= alpha)
                    break;
            }

            return bestVal;
        }
        else
        {
            int bestVal = int.MaxValue;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        Matrix[i, j] = -1;

                        int val = Minimax(depth + 1, true, alpha, beta);
                        bestVal = Mathf.Min(bestVal, val);
                        beta = Mathf.Min(beta, bestVal);

                        Matrix[i, j] = 0;

                        if (beta <= alpha)
                            break;
                    }
                }

                if (beta <= alpha)
                    break;
            }

            return bestVal;
        }
    }

    /// <summary>
    /// Determines whether there are any available moves remaining on the game board.
    /// </summary>
    /// <remarks>Use this method to check if the game can continue or if the board is full and no further
    /// moves are possible.</remarks>
    /// <returns>Returns <see langword="true"/> if at least one cell on the board is empty and a move can be made; otherwise,
    /// <see langword="false"/>.</returns>
    private bool IsMovesLeft()
    {
        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                if (Matrix[i, j] == 0) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Determines the optimal move for the specified team by evaluating all possible moves using the minimax algorithm,
    /// optionally with alpha-beta pruning.
    /// </summary>
    /// <remarks>If alpha-beta pruning is enabled, the search may be significantly faster, especially in
    /// larger game trees. The returned node's X and Y properties indicate the recommended move position for the
    /// team.</remarks>
    /// <param name="team">The team identifier for which to find the best move. Typically, 1 represents the maximizing player and other
    /// values represent the minimizing player.</param>
    /// <param name="pruning">A value indicating whether alpha-beta pruning should be used to optimize the minimax search. Set to <see
    /// langword="true"/> to enable pruning; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Node"/> representing the best move found for the specified team. The node contains the coordinates
    /// and evaluation of the optimal move.</returns>
    private Node FindBestMove(int team, bool pruning)
    {
        bool isMaximizing = team == 1;
        int bestVal = isMaximizing ? int.MinValue : int.MaxValue;

        Node bestNode = new Node(null, team, int.MinValue, int.MaxValue, -1, -1, Matrix);

        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {
                if (Matrix[i, j] == 0)
                {
                    Matrix[i, j] = team;

                    int moveVal;
                    if (!pruning)
                    {
                        moveVal = Minimax(1, !isMaximizing);
                    }
                    else
                    {
                        moveVal = Minimax(1, !isMaximizing, int.MinValue, int.MaxValue);
                    }
                    
                    Matrix[i, j] = 0;

                    if (isMaximizing ? moveVal > bestVal : moveVal < bestVal)
                    {
                        bestNode.X = i;
                        bestNode.Y = j;
                        bestVal = moveVal;
                    }
                }
            }
        }

        Debug.Log("Best Value Move: " + bestVal);

        return bestNode;
    }
}
