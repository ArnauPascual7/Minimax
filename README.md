# Minimax

## Sense Poda

Si ja hi ha un guanyador retorna la puntuació, i si no hi ha més moviments retorna 0. La profunditat penalitza/premia per preferir **guanyar ràpid** i **perdre tard**.

Si isMax és verdader és maximitza el resultat, per cada casella buida al taulell, crida recursivament a Minimax() fins trobar el millor valor.

Si isMax és fals és minimitza el resultat, per cada casella buida al taulell, crida recursivament a Minimax() fins trobar el millor valor.

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

Si ja hi ha un guanyador retorna la puntuació, i si no hi ha més moviments retorna 0. La profunditat penalitza/premia per preferir **guanyar ràpid** i **perdre tard**.

Amb poda es comporta exactament igual que sense, pero utilitza els valors de alpha i beta per a filtrar resultats que no calen comprovar. Alpha representa el millor valor màxim, mentre que beta el millor valor mínim. La poda es produeix quan beta <= alpha, és a dir, quan el minimitzador ja té garantit un valor igual o pitjor que el que el maximitzador ja pot aconseguir per una altra branca. En aquest cas, continuar explorant no canviarà la decisió final, de manera que la branca s'abandona. Quan maximitzem si el valor trobat V >= beta, el minimitzador mai escollirà aquest camí, podem podar i quan minimitzem si el valor trobat V <= alpha, el maximitzador mai escollirà aquest camí, també podem podar. Cal tenir en compte també que alpha i beta es passen per valor a cada crida recursiva, de manera que s'actualitzen localment a cada nivell i la informació de poda es propaga cap avall però no afecta les branques ja explorades.

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
