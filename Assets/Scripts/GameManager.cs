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
        Node bestNode = FindBestMove((int)state);

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

    private Node FindBestMove(int team)
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

                    int moveVal = Minimax(1, !isMaximizing);

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
