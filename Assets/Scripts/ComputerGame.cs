using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;

public class ComputerGame : MonoBehaviour
{   
    private ChessBoard curBoard;
    private ChessBoard curBoardVirtual;
    public GameObject piecePrefab;
    public GameObject boardPrefab;
    public GameObject tilePrefab;
    public Camera currentCamera;

    private ChessPiece currentPromotionPiece;
    private List<movesScript> availableMoves = new List<movesScript>();
    //private int pressedReleased = 0;

    //public bool disableTurns = false;
    public GameObject victoryScreen;
    //public GameObject promoteScreen;

    //bool computerTeamWhite = true;
    int computerTeam = 0;
    //bool reset = false;
    bool end = false;
    public int depth;
    public movesScript bestMove = new movesScript(0,0,0,0,0);
    

   

    // Start is called before the first frame update
    public void Awake() {
        
       curBoard = new ChessBoard(0, "", piecePrefab, boardPrefab, tilePrefab, 0f, 0f, 5f);
       curBoard.victoryScreen = victoryScreen;   
       curBoardVirtual = new ChessBoard(curBoard);
       //int numMoves = moveGenTest(0, depth, curBoardVirtual);
       //Debug.Log(numMoves);

       
       //int bestEvaluation = search(0, depth, curBoardVirtual, -100000, 100000);
       //Debug.Log(bestEvaluation);
       //Debug.Log(bestMove.currentPositionX);
       //Debug.Log(bestMove.currentPositionY);
       //Debug.Log(bestMove.movePositionX);
       //Debug.Log(bestMove.movePositionY);
    }

    public int moveGenTest(int team, int depth, ChessBoard board){
        if(depth == 0){
            return 1;
        }
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        int numPositions = 0;
        if(team == 0){
            team++;
        }else{
            team--;
        }

        for(int i = 0; i<moves.Count; i++){
            ChessBoard tmpBoard = new ChessBoard(board, true);
            board.movePieceVirtual(moves[i].movePositionX, moves[i].movePositionY, moves[i]);
            numPositions += moveGenTest(team,depth-1, board);
            board = tmpBoard;
        }

        return numPositions;
    }

    public int search(int team, int depth, ChessBoard board, int alpha, int beta, bool top = true){
        if(depth == 0){
            return Evaluation(board);
        }
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        //int numPositions = 0;
        if(moves.Count == 0){
            if(board.isInCheck(team)){
               return -100000; 
            }
            return 0;
        }

        if(team == 0){
            team++;
        }else{
            team--;
        }

        //int bestEval = -100000;

        for(int i = 0; i<moves.Count; i++){
            ChessBoard tmpBoard = new ChessBoard(board, true);
            board.movePieceVirtual(moves[i].movePositionX, moves[i].movePositionY, moves[i]);
            int eval = -search(team,depth-1, board, -beta, -alpha, false);
            //bestEval = Math.Max(eval, bestEval);
            board = tmpBoard;
            if(eval >= beta){
                return beta;
            }

            int tmp = alpha;
            alpha = Math.Max(alpha, eval);
            if(tmp == alpha){
                bestMove = moves[i];
            }
            
        }


        return alpha;
    }

    public int Evaluation(ChessBoard board){
        int evaluation = 0;

        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(board.virtualBoard[i,j] != null){
                    int tmp = 0;
                    switch(board.virtualBoard[i,j].pieceType){
                        case ChessPieceType.Pawn: tmp += 1; break; 
                        case ChessPieceType.Bishop: tmp += 3; break;
                        case ChessPieceType.Knight: tmp += 3; break;
                        case ChessPieceType.Queen: tmp += 9; break;
                        case ChessPieceType.Rook: tmp += 5; break;
                    }
                    if(board.virtualBoard[i,j].team == 0){
                        evaluation += tmp;
                    }else{
                        evaluation -= tmp;
                    }
                }
            }
        }

        if(!board.whiteTurn){
            evaluation *= -1;
        }
        return evaluation;
    }

    ///*
    public void Update() {

        if(Time.frameCount % 640 != 0) { return; }

        int numPieces = 0;
        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){
                if(curBoard.board[i,j] != null){
                    numPieces++;
                }
            }
        }
        if(numPieces == 2){
            return;
        }

        if(!currentCamera){
            currentCamera = Camera.main;
            return;
        }

        if(!end){
            availableMoves = curBoard.getAllRawMoves(computerTeam);
            availableMoves = curBoard.getValidMoves(availableMoves);
            //Random number available moves count
            //int bestEvaluation = search(computerTeam, depth, curBoardVirtual, -100000, 100000);
            //Debug.Log(bestEvaluation);
            int index = Random.Range(0, availableMoves.Count-1);
            movesScript move = availableMoves[index];
            /*
            curBoardVirtual = new ChessBoard(curBoard);
            int bestEvaluation = search(computerTeam, depth, curBoardVirtual, -100000, 100000);
            Debug.Log(bestEvaluation);
            //Debug.Log(bestMove.currentPositionX);
            //Debug.Log(bestMove.currentPositionY);
            //Debug.Log(bestMove.movePositionX);
            //Debug.Log(bestMove.movePositionY);
            //Debug.Log("------------------------------------");
            if(curBoard.turnNum >= 2){
            move = bestMove;
            }
            */
            ChessPiece piece = curBoard.board[move.currentPositionX, move.currentPositionY];

            
            if(move.moveType == MoveType.Promote_Queen || move.moveType == MoveType.Promote_Rook || move.moveType == MoveType.Promote_Bishop || move.moveType == MoveType.Promote_Knight){
                
                currentPromotionPiece = curBoard.board[move.currentPositionX, move.currentPositionY];
                curBoard.movePiece(ref piece, move.movePositionX, move.movePositionY, move, true);
                switch(move.moveType){
                    case MoveType.Promote_Queen: currentPromotionPiece.pieceType = ChessPieceType.Queen; currentPromotionPiece.changeSprite(ChessPieceType.Queen); break;
                    case MoveType.Promote_Rook: currentPromotionPiece.pieceType = ChessPieceType.Rook; currentPromotionPiece.changeSprite(ChessPieceType.Rook); break;
                    case MoveType.Promote_Bishop: currentPromotionPiece.pieceType = ChessPieceType.Bishop; currentPromotionPiece.changeSprite(ChessPieceType.Bishop); break;
                    case MoveType.Promote_Knight: currentPromotionPiece.pieceType = ChessPieceType.Knight; currentPromotionPiece.changeSprite(ChessPieceType.Knight); break;
                }
            }else{
                curBoard.movePiece(ref piece, move.movePositionX, move.movePositionY, move, true);
            }
            bool tmp = false;
            if(piece.team == 0){
                tmp = curBoard.checkForMate(1);
            }else{
                tmp = curBoard.checkForMate(0);
            }
            if(tmp){
                end = true;
            }
            availableMoves.Clear();
            if(computerTeam == 0){
                computerTeam = 1;
            }else{
                computerTeam = 0;
            }
        }
    }

    //*/
    
    public void onReset(){
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        availableMoves.Clear();

        for(int i = 0; i < 8; i++){
            for(int j = 0; j < 8; j++){
                if(curBoard.board[i,j]!= null){
                    Destroy(curBoard.board[i,j].gameObject);
                    
                    curBoard.board[i,j] = null;  
                }
                Destroy(curBoard.tiles[i,j].gameObject);
                curBoard.tiles[i,j] = null;
            }

        }

        for(int i = 0; i < curBoard.deadWhite.Count; i++){
            Destroy(curBoard.deadWhite[i].gameObject);
        }
        for(int i = 0; i < curBoard.deadBlack.Count; i++){
            Destroy(curBoard.deadBlack[i].gameObject);
        }

        curBoard.deadWhite.Clear();
        curBoard.deadBlack.Clear();
        Destroy(curBoard.boardTransform);

        curBoard = new ChessBoard(0, "", piecePrefab, boardPrefab, tilePrefab, 0f, 0f, 5f);
        curBoard.victoryScreen = victoryScreen;
        end = true;
        //disableTurns = false;
    }
    public void onExit(){
        Application.Quit();
    }
}