# Minimax

## Sense Poda

```
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
```

## Amb poda

```
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

                        int val = Minimax(depth + 1, !isMax, alpha, beta);
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

                        int val = Minimax(depth + 1, !isMax, alpha, beta);
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

```

La profunditat penalitza/premia per preferir **guanyar ràpid** i **perdre tard**.
