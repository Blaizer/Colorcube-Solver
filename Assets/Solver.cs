using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    int[,] board = new int[24, 3];
    bool[,] triangles;
    int[][] pieces;

    int pieceCount;
    int maxPieceCount;
    PieceInfo[] currentPieces;
    bool solved;

    struct PieceInfo
    {
        int index;
        int color;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "Solve"))
        {
            Solve(Triangles.Instance.TrianglesColors, Pieces.Instance.PieceTris);
        }
    }

    void Solve(bool[,] triangles, int[][] pieces)
    {
        this.triangles = triangles;
        this.pieces = pieces;

        ValidateTriangles();

        solved = false;
        maxPieceCount = pieces.Length;
        currentPieces = new PieceInfo[maxPieceCount];

        CheckSolved();
        for (pieceCount = 1; pieceCount < maxPieceCount && !solved; pieceCount++)
        {
            Solve();
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

    void Solve(int depth = 0)
    {
        for (int p = depth; p < maxPieceCount; p++)
        {
            var piece = pieces[p];
            int pieceLength = piece.Length;

            for (int r = 0; r < 24; r += 4)
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int i = 0; i < pieceLength; i++)
                    {
                        int tri = (piece[i] + r) % 24;
                        if (!triangles[tri, c])
                            goto NEXT;
                    }

                    for (int i = 0; i < pieceLength; i++)
                    {
                        int tri = (piece[i] + r) % 24;
                        board[tri, c]++;
                    }

                    if (depth < pieceCount - 1)
                    {
                        Solve(depth + 1);
                    }
                    else
                    {
                        CheckSolved();
                    }

                    for (int i = 0; i < pieceLength; i++)
                    {
                        int tri = (piece[i] + r) % 24;
                        board[tri, c]--;
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

        Debug.Log("Solved: " + pieceCount);
        solved = true;
    }
}
