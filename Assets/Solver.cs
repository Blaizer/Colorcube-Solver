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
    int[,][] allPieces;
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

        solved = false;
        maxPieceCount = pieces.Length;
        currentPieces = new PieceInfo[maxPieceCount];

        ValidateTriangles();
        GenerateAllPieces();

        pieceCount = 0;
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

    void Solve(int depth = 0)
    {
        for (int p = depth; p < maxPieceCount; p++)
        {
            int pieceLength = pieces[p].Length;

            for (int r = 0; r < 6; r++)
            {
                var piece = allPieces[p, r];

                for (int c = 0; c < 3; c++)
                {
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
                        Solve(depth + 1);
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

        Debug.Log("Solved: " + pieceCount);
        solved = true;
    }
}
