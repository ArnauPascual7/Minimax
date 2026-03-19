# Minimax

## Sense Poda

Si ja hi ha un guanyador retorna la puntuació, i si no hi ha més moviments retorna 0. La profunditat penalitza/premia per preferir **guanyar ràpid** i **perdre tard**.

Si isMax és verdader és maximitza el resultat, per cada casella buida al taulell, crida recursivament a Minimax() fins trobar el millor valor.

Si isMax és fals és minimitza el resultat, per cada casella buida al taulell, crida recursivament a Minimax() fins trobar el millor valor.

### Funció de Minimax

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

### Funció de Minimax detallada

```
    private int Minimax(int depth, bool isMax, int alpha, int beta) // Demana la profunditat del node a comprovar, si estem maximitzant, el valor de alpha i el de beta
    {
        int result = Calculs.EvaluateWin(Matrix); // Comrpova si hi ha un guanyador -> 1 guanya la ia, -1 guanya el jugador i 0 acaba en empat.

        // Utilitzem un 10 en comptes de un 1 per a donar a l'algoritme més marge de diferència, i que la profuncitat es noti menys, volem guanyar el més aviat possible i perdre el més tard possible
        if (result == 1) return 10 - depth;
        if (result == -1) return -10 + depth;
        if (result == 0) return 0;

        // En cas de que no hi hagui un gunyador i no s'hagui retornat el 0 del empat abans retornen 0, empat
        if (!IsMovesLeft()) return 0;

        if (isMax)
        {
            // En el cas de maximitzar fem el següent:
            int bestVal = int.MinValue; // Inicialitzem el millor valor amb el valor mínim possible, perquè qualsevol valor superior a aquest el pugui substituir

            // Recorrem la matriu, el taulell del tres en ratlla
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        // En cas de trobar una casella sense fitxa fem el següent:
                        Matrix[i, j] = 1; // Col·loquem una fitxa temporal per a simular el moviment en aquesta casella i comprovar l'algoritme

                        // Cridem recursivament la funció augmentant la profunditat i girant el valor de isMax per a fer el contrari en la següent iteració, a més de passar els valors de alpha i beta.
                        int val = Minimax(depth + 1, !isMax, alpha, beta);

                        // Del valor que obtenim el comparem amb el millor valor que tenim actualment i ens quedem el que sigui major, MAX, i l'establim com el millor valor trobat
                        bestVal = Mathf.Max(bestVal, val);

                        // Fem el mateix però amb el valor de alpha i el millor valor trobat
                        alpha = Mathf.Max(alpha, bestVal);

                        Matrix[i, j] = 0; // Treiem la fitxa temporal que haviem posat

                        // Si beta <= alpha, el minimitzador ja té garantit un valor igual o pitjor al que el maximitzador pot obtenir per una altra branca, no cal explorar més així que podem la branca
                        if (beta <= alpha)
                            break; // Trenquem el bucle interior (j) per aplicar la poda
                    }
                }
                // Si la poda es va activar al bucle interior també trenquem el bucle exterior (i)
                if (beta <= alpha)
                    break;
            }
            // Retornem el millor valor trobat
            return bestVal;
        }
        else
        {
            // En el cas de minimitzar fem el següent:
            int bestVal = int.MaxValue; // Inicialitzem el millor valor amb el valor màxim possible, perquè qualsevol valor inferior a aquest el pugui substituir

            // Recorrem la matriu, el taulell del tres en ratlla
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == 0)
                    {
                        // En cas de trobar una casella sense fitxa fem el següent:
                        Matrix[i, j] = -1; // Col·loquem una fitxa temporal per a simular el moviment en aquesta casella i comprovar l'algoritme

                        // Cridem recursivament la funció augmentant la profunditat i girant el valor de isMax per a fer el contrari en la següent iteració, a més de passar els valors de alpha i beta.
                        int val = Minimax(depth + 1, !isMax, alpha, beta);

                        // Del valor que obtenim el comparem amb el millor valor que tenim actualment i ens quedem el que sigui menor, MIN, i l'establim com el millor valor trobat
                        bestVal = Mathf.Min(bestVal, val);

                        // Fem el mateix però amb el valor de beta i el millor valor trobat
                        beta = Mathf.Min(beta, bestVal);

                        Matrix[i, j] = 0; // Treiem la fitxa temporal que haviem posat

                        // Si beta <= alpha, el minimitzador ja té garantit un valor igual o pitjor al que el maximitzador pot obtenir per una altra branca, no cal explorar més així que podem la branca
                        if (beta <= alpha)
                            break; // Trenquem el bucle interior (j) per aplicar la poda
                    }
                }
                // Si la poda es va activar al bucle interior també trenquem el bucle exterior (i)
                if (beta <= alpha)
                    break;
            }
            // Retornem el millor valor trobat
            return bestVal;
        }
    }
```
