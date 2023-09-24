using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum MoveType{
    Default = 0,
    Castle_Short = 1,
    Castle_Long = 2,
    Promote_Bishop = 3,
    Promote_Knight = 4,
    Promote_Rook = 5,
    Promote_Queen = 6,
    EnPassant = 7
}
public class movesScript
{

    public MoveType moveType;
    public int movePositionX;
    public int movePositionY;
    public int currentPositionX;
    public int currentPositionY;
    public int moveEval;

    public movesScript(int cX, int cY, MoveType mT, int mX, int mY){
        currentPositionX = cX;
        currentPositionY = cY;
        moveType = mT;
        movePositionX = mX;
        movePositionY = mY;
    }

    public movesScript(int cX, int cY, MoveType mT, int mX, int mY, int eval){
        currentPositionX = cX;
        currentPositionY = cY;
        moveType = mT;
        movePositionX = mX;
        movePositionY = mY;
        moveEval = eval;
    }
}
