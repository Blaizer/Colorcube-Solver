using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    Piece[] pieces;

    int[][] pieceTris;

    public static Pieces Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pieces = GetComponentsInChildren<Piece>();

        pieceTris = pieces.Select(p => p.tris).ToArray();
    }

    public int[][] PieceTris
    {
        get { return pieceTris; }
    }
}
