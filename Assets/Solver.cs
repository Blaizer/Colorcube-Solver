using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using UnityEngine;

public class Solver : MonoBehaviour
{
    int[,] board = new int[24, 3];
    bool[,] triangles;
    int[][] pieces;

    int pieceCount;
    int maxPieceCount;
    int[,][] allPieces;
    PieceInfo[] currentPieces;
    List<PieceInfo[]> solutions = new List<PieceInfo[]>();
    bool solved;

    Thread solverThread;

    struct PieceInfo
    {
        public int piece;
        public int rotation;
        public int color;
    }

    private void OnDestroy()
    {
        KillSolverThread();
    }

    void OnGUI()
    {
        CheckSolverThread();

        bool enabled = GUI.enabled;

        GUI.enabled = solverThread == null;
        if (GUI.Button(new Rect(0, 0, 200, 20), "Solve"))
        {
            StartSolverThread(Triangles.Instance.TrianglesColors, Pieces.Instance.PieceTris);
        }

        GUI.Label(new Rect(0, 20, 200, 20), pieceCount.ToString(), new GUIStyle() { alignment = TextAnchor.MiddleCenter });

        GUI.enabled = solverThread != null;
        if (GUI.Button(new Rect(200, 0, 200, 20), "Cancel"))
        {
            KillSolverThread();
        }

        GUI.enabled = enabled;
    }

    private void CheckSolverThread()
    {
        if (solverThread != null && !solverThread.IsAlive)
        {
            solverThread.Join();
            solverThread = null;
            if (solved)
            {
                Debug.Log("solved!");
                foreach (var piece in solutions[0])
                {
                    Debug.Log(string.Format("p: {0}, r: {1}, c: {2}", piece.piece, piece.rotation, piece.color));
                }
            }
        }
    }

    private void KillSolverThread()
    {
        if (solverThread != null && solverThread.IsAlive)
        {
            maxPieceCount = 0;
            solved = true;
            solverThread.Join();
            solverThread = null;
        }
    }

    void StartSolverThread(bool[,] triangles, int[][] pieces)
    {
        this.triangles = triangles;
        this.pieces = pieces;
        solverThread = new Thread(Solve);
        solverThread.Start();
    }

    void Solve()
    {
        solved = false;
        solutions.Clear();
        maxPieceCount = pieces.Length;
        currentPieces = new PieceInfo[maxPieceCount];

        ValidateTriangles();
        GenerateAllPieces();

        pieceCount = 0;
        CheckSolved();
        for (pieceCount = 1; pieceCount < maxPieceCount && !solved; pieceCount++)
        {
            Solve(0, 0);

            if (solved)
                break;
        }
    }

    void ValidateTriangles()
    {
        for (int i = 0; i < 24; i++)
        {
            if (!(triangles[i, 0] || triangles[i, 1] || triangles[i, 2]))
            {
                triangles[i, 0] = triangles[i, 1] = triangles[i, 2] = true;
            }
        }
    }

    void GenerateAllPieces()
    {
        allPieces = new int[maxPieceCount, 6][];

        for (int p = 0; p < maxPieceCount; p++)
        {
            var piece = allPieces[p, 0] = pieces[p];
            int pieceLength = piece.Length;

            for (int r = 1; r < 6; r++)
            {
                allPieces[p, r] = new int[pieceLength];

                int offset = r * 4;
                for (int i = 0; i < pieceLength; i++)
                {
                    allPieces[p, r][i] = (piece[i] + offset) % 24;
                }
            }
        }
    }

    void Solve(int depth, int currentPiece)
    {
        int lastPiece = maxPieceCount - (pieceCount - depth);
        for (int p = currentPiece; p <= lastPiece; p++)
        {
            currentPieces[depth].piece = p;
            int pieceLength = pieces[p].Length;

            for (int r = 0; r < 6; r++)
            {
                currentPieces[depth].rotation = r;
                var piece = allPieces[p, r];

                for (int c = 0; c < 3; c++)
                {
                    currentPieces[depth].color = c;

                    for (int i = 0; i < pieceLength; i++)
                    {
                        if (!triangles[piece[i], c])
                            goto NEXT;
                    }

                    for (int i = 0; i < pieceLength; i++)
                    {
                        board[piece[i], c]++;
                    }

                    if (depth < pieceCount - 1)
                    {
                        Solve(depth + 1, p + 1);
                    }
                    else
                    {
                        CheckSolved();
                    }

                    for (int i = 0; i < pieceLength; i++)
                    {
                        board[piece[i], c]--;
                    }

                    NEXT:;
                }
            }
        }
    }

    void CheckSolved()
    {
        for (int i = 0; i < 24; i++)
        {
            if (!(triangles[i, 0] == board[i, 0] > 0 && triangles[i, 1] == board[i, 1] > 0 && triangles[i, 2] == board[i, 2] > 0)
                && !(triangles[i, 0] && triangles[i, 1] && triangles[i, 2] && board[i, 0] + board[i, 1] + board[i, 2] <= 0))
            {
                return;
            }
        }

        SaveSolution();
    }

    void SaveSolution()
    {
        solved = true;
        var solution = new PieceInfo[pieceCount];
        solutions.Add(solution);
        System.Array.Copy(currentPieces, solution, pieceCount);
    }
}
