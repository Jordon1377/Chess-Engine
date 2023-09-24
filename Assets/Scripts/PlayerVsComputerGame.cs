using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=UnityEngine.Random;

public class PlayerVsComputerGame : MonoBehaviour
{
    private ChessBoard curBoard;
    public GameObject piecePrefab;
    public GameObject boardPrefab;
    public GameObject tilePrefab;
    public Camera currentCamera;
    private Vector2Int currentHover = -Vector2Int.one;

    private ChessPiece currentlyDragging;
    private ChessPiece currentPromotionPiece;
    private List<movesScript> availableMoves = new List<movesScript>();
    //private int pressedReleased = 0;

    public bool disableTurns = false;
    public GameObject victoryScreen;
    public GameObject promoteScreen;

    bool computerTeamWhite = false;
    int computerTeam = 1;
    bool reset = false;
    bool end = false;
    public movesScript bestMove = new movesScript(0,0,0,0,0,0);
    public movesScript currentMoveBoard = new movesScript(0,0,0,0,0, -100000);

    public movesScript alpha1 = new movesScript(0,0,0,0,0, -100000);
    public movesScript beta1 = new movesScript(0,0,0,0,0, 100000);

    private ChessBoard curBoardVirtual;
    public int depth;
    
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

    movesScript alphaBetaMax(int team, int depth, ChessBoard board, movesScript alpha, movesScript beta, movesScript move, bool top = true) {
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        //int numPositions = 0;
        if(moves.Count == 0){
            if(board.isInCheck(team)){
                move.moveEval = -100000;
                return move; 
            }else{
                move.moveEval = 0;
                return move;
            }
        }
        
        if (depth == 0 ){ 
            move.moveEval = Evaluation(board);
            return move;
        }

        for(int i = 0; i < moves.Count; i++){
            ChessBoard tmpBoard = new ChessBoard(board, true);
            board.movePieceVirtual(moves[i].movePositionX, moves[i].movePositionY, moves[i]);

            movesScript move1 = alphaBetaMin(0,depth-1, board, alpha, beta, moves[i], false);
            //int eval = -move1.moveEval;
            //move1.moveEval = eval;
            move.moveEval = move1.moveEval;
            //bestEval = Math.Max(eval, bestEval);
            
            //score = alphaBetaMin( alpha, beta, depthleft - 1 );
            if(move1.moveEval >= beta.moveEval ){
                return beta;   // fail hard beta-cutoff
            }
            if( move1.moveEval > alpha.moveEval ){
                if(top){
                    alpha = move1;
                }else{
                    alpha = move; // alpha acts like max in MiniMax
                }
            }    

            board = tmpBoard;
        }
        return alpha;
        }

    movesScript alphaBetaMin(int team, int depth, ChessBoard board, movesScript alpha, movesScript beta, movesScript move, bool top = true) {
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        //int numPositions = 0;
        if(moves.Count == 0){
            if(board.isInCheck(team)){
                move.moveEval = 100000;
                return move; 
            }else{
                move.moveEval = 0;
                return move;
            }
        }
        
        if (depth == 0 ){ 
            move.moveEval = -Evaluation(board);
            return move;
        }



        for(int i = 0; i < moves.Count; i++){
            ChessBoard tmpBoard = new ChessBoard(board, true);
            board.movePieceVirtual(moves[i].movePositionX, moves[i].movePositionY, moves[i]);

            movesScript move1 = alphaBetaMax(1,depth-1, board, alpha, beta, moves[i], false);
            move.moveEval = move1.moveEval;
            if(move1.moveEval <= alpha.moveEval)
                return alpha; // fail hard alpha-cutoff
            if(move1.moveEval < beta.moveEval){
                if(top){
                    beta = move1;
                }else{
                    beta = move; // alpha acts like max in MiniMax
                }
                //beta = score; // beta acts like min in MiniMax
            }
        }
        return beta;
    }

    public movesScript searchbeta(int team, int depth, ChessBoard board, movesScript alpha, movesScript beta, movesScript move, bool top = true){
        if(depth == 0){
            move.moveEval = Evaluation(board);
            return move;
        }
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        //int numPositions = 0;
        if(moves.Count == 0){
            if(board.isInCheck(team)){
                move.moveEval = -100000;
                return move; 
            }else{
                move.moveEval = 0;
                return move;
            }
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

            movesScript NegBeta = beta;
            movesScript NegAlpha = alpha;

            NegBeta.moveEval = -beta.moveEval;
            NegAlpha.moveEval = -alpha.moveEval;
            
            movesScript move1 = searchbeta(team,depth-1, board, NegBeta, NegAlpha, moves[i], false);
            int eval = -move1.moveEval;
            move1.moveEval = eval;
            move.moveEval = eval;
            //bestEval = Math.Max(eval, bestEval);
            board = tmpBoard;

            if(eval >= beta.moveEval){
                //bestMove = moves[i];

                return beta;
            }

            //alpha = Math.Max(alpha, eval);
            if(eval > alpha.moveEval){
                if(top){
                    alpha = move1;
                }else{
                    alpha = move;
                }
            }
            
            
        }

        return alpha;
    }

    public movesScript search(int team, int depth, ChessBoard board, movesScript move, bool tmp = true){
        if(depth == 0){
            move.moveEval = Evaluation(board);
            return move;
        }
        List<movesScript> moves = board.getAllRawMovesVirtual(team);
        moves = board.getValidMovesVirtual(moves);
        //int numPositions = 0;
        if(moves.Count == 0){
            if(board.isInCheck(team)){
                move.moveEval = -100000;
                return move; 
            }else{
                move.moveEval = 0;
                return move;
            }
        }

        if(team == 0){
            team=1;
        }else{
            team=0;
        }

        movesScript bestEvalMove = new movesScript(0,0,0,0,0, -100000);

        for(int i = 0; i<moves.Count; i++){
            ChessBoard tmpBoard = new ChessBoard(board, true);
            board.movePieceVirtual(moves[i].movePositionX, moves[i].movePositionY, moves[i]);
            movesScript move1 = search(team,depth-1, board, moves[i], false);
            int eval = -move1.moveEval;
            move1.moveEval = eval;
            move.moveEval = eval;
        
            //int tmp = bestEval;
            //bestEvalMove = Math.Max(move1.moveEval, bestEvalMove.moveEval); 
            if(move.moveEval > bestEvalMove.moveEval){
                if(tmp){
                    bestEvalMove = move1;
                }else{
                    bestEvalMove = move;
                }
            }

            board = tmpBoard;
        }
        return bestEvalMove;
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

   

    // Start is called before the first frame update
    public void Awake() {
        
       curBoard = new ChessBoard(0, "", piecePrefab, boardPrefab, tilePrefab, 0f, 0f, 5f);
       curBoard.victoryScreen = victoryScreen;  
       curBoardVirtual = new ChessBoard(curBoard); 
    }

    
    public void Update() {
        if(disableTurns){
            return;
        }
        if(!currentCamera){
            currentCamera = Camera.main;
            return;
        }

        if(curBoard.whiteTurn == computerTeamWhite && !end){
            availableMoves = curBoard.getAllRawMoves(computerTeam);
            availableMoves = curBoard.getValidMoves(availableMoves);
            //Random number available moves count
            int index = Random.Range(0, availableMoves.Count-1);
            movesScript move = availableMoves[index];

            curBoardVirtual = new ChessBoard(curBoard);
            disableTurns = true;
            bestMove = search(computerTeam, depth, curBoardVirtual, currentMoveBoard);
            //bestMove = searchbeta(computerTeam, depth, curBoardVirtual, alpha1, beta1, currentMoveBoard);
            Debug.Log(bestMove.moveEval);
            Debug.Log(bestMove.currentPositionX);
            Debug.Log(bestMove.currentPositionY);
            Debug.Log(bestMove.movePositionX);
            Debug.Log(bestMove.movePositionY);
            Debug.Log("------------------------------------");
            if(curBoard.turnNum >= 2){
                move = bestMove;
            }
            
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
            disableTurns = false;
            
        }else if(curBoard.whiteTurn != computerTeamWhite){
            RaycastHit info;
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tiles", "Hover", "Highlight"))){
                Vector2Int hitIndex = new Vector2Int(info.transform.GetComponent<TileScript>().xPos,info.transform.GetComponent<TileScript>().yPos);

                if(currentHover == -Vector2Int.one){
                    currentHover = hitIndex;
                    if(reset){
                        reset = false;
                        return;
                    }
                    info.transform.GetComponent<TileScript>().belongsToBoard.tiles[hitIndex.x,hitIndex.y].transform.gameObject.layer = LayerMask.NameToLayer("Hover");
                } 
                if(currentHover != hitIndex){
                    info.transform.GetComponent<TileScript>().belongsToBoard.tiles[currentHover.x,currentHover.y].transform.gameObject.layer = (containsValidMove(ref availableMoves, currentHover.x, currentHover.y)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tiles");
                    currentHover = hitIndex;
                    info.transform.GetComponent<TileScript>().belongsToBoard.tiles[hitIndex.x,hitIndex.y].transform.gameObject.layer = LayerMask.NameToLayer("Hover");
                }   
                if(Input.GetMouseButtonDown(0) && curBoard.board[hitIndex.x,hitIndex.y] != null){
                    //Is it our turn? add function
                    if((curBoard.board[hitIndex.x, hitIndex.y].team == 0 && curBoard.whiteTurn) || (curBoard.board[hitIndex.x, hitIndex.y].team == 1 && !curBoard.whiteTurn)){
                        currentlyDragging = curBoard.board[hitIndex.x,hitIndex.y];
                        //Highlight move tiles

                        availableMoves = currentlyDragging.getAvailableMoves(ref curBoard.board, ref curBoard.boardMovesMade);
                        availableMoves = curBoard.getValidMoves(availableMoves);
                        highlightMoveTiles();
                    }
                }
                if(currentlyDragging != null && Input.GetMouseButtonUp(0)){
                    //Vector2Int previousPosition = new Vector2Int(currentlyDragging.xPos, currentlyDragging.yPos);
                    bool validMove = false;
                    movesScript move = new movesScript(currentlyDragging.xPos, currentlyDragging.yPos, 0, -1, -1);

                    for(int i = 0; i < availableMoves.Count; i++){
                        if(availableMoves[i].movePositionX == hitIndex.x && availableMoves[i].movePositionY == hitIndex.y){
                            validMove = true;
                            move = availableMoves[i];
                        }
                    }

                    removeHighlightedMoveTiles();
                    
                    if(validMove){
                        if(move.moveType == MoveType.Promote_Queen || move.moveType == MoveType.Promote_Rook || move.moveType == MoveType.Promote_Bishop || move.moveType == MoveType.Promote_Knight){
                            curBoard.movePiece(ref currentlyDragging, hitIndex.x, hitIndex.y, move, true);
                            disableTurns = true;
                            currentPromotionPiece = currentlyDragging;
                            //Get buttons to appear
                            promoteScreen.SetActive(true);
                        }else{
                            curBoard.movePiece(ref currentlyDragging, hitIndex.x, hitIndex.y, move, true);
                        }
                        if(!disableTurns){
                            bool tmp = false;
                            if(currentlyDragging.team == 0){
                                tmp = curBoard.checkForMate(1);
                            }else{
                                tmp = curBoard.checkForMate(0);
                            }
                            if(tmp){
                                end = true;
                            }
                        }
                        
                    }else{
                        movesScript moved = new movesScript(currentlyDragging.xPos, currentlyDragging.yPos, 0, -1, -1);
                        curBoard.movePiece(ref currentlyDragging, currentlyDragging.xPos, currentlyDragging.yPos, moved, false);
                    }
                    currentlyDragging = null;
                }
            }else{
                if (currentHover != -Vector2Int.one){
                    curBoard.tiles[currentHover.x,currentHover.y].transform.gameObject.layer = (containsValidMove(ref availableMoves, currentHover.x, currentHover.y)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tiles");
                    currentHover = -Vector2Int.one;
                }
                if(currentlyDragging != null && Input.GetMouseButtonUp(0)){
                    removeHighlightedMoveTiles();
                    movesScript moved = new movesScript(currentlyDragging.xPos, currentlyDragging.yPos, 0, -1, -1);
                    curBoard.movePiece(ref currentlyDragging, currentlyDragging.xPos, currentlyDragging.yPos, moved, false);
                    currentlyDragging = null;
                }
            }
            //if currently dragging piece
            if(currentlyDragging){
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentlyDragging.setPosition(new Vector3(mousePosition.x, mousePosition.y, -3));
            }
        }
    }

    private void highlightMoveTiles(){
        for(int i = 0; i < availableMoves.Count; i++){
            curBoard.tiles[availableMoves[i].movePositionX, availableMoves[i].movePositionY].gameObject.layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void removeHighlightedMoveTiles(){
        for(int i = 0; i < availableMoves.Count; i++){
            curBoard.tiles[availableMoves[i].movePositionX, availableMoves[i].movePositionY].gameObject.layer = LayerMask.NameToLayer("Tiles");
        }
        availableMoves.Clear();
    }

    private bool containsValidMove(ref List<movesScript> moves, int xPos, int yPos){
        for(int i = 0; i < moves.Count; i++){
            if(moves[i].movePositionX == xPos && moves[i].movePositionY == yPos){
                return true;
            }
        }
        return false;
    }
    
    public void onReset(){
        currentHover = -Vector2Int.one;
        reset = true;
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        currentlyDragging = null;
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

    public void OnPromoteQueen(){
        promoteScreen.SetActive(false);
        currentlyDragging = null;
        availableMoves.Clear();
        currentPromotionPiece.pieceType = ChessPieceType.Queen;
        currentPromotionPiece.changeSprite(ChessPieceType.Queen);
        //curBoard.printBoard();
        if(currentPromotionPiece.team == 0){
            curBoard.checkForMate(1);
        }else{
            curBoard.checkForMate(0);
        }
        disableTurns = false;
    }

    public void OnPromoteRook(){
        promoteScreen.SetActive(false);
        currentlyDragging = null;
        availableMoves.Clear();
        currentPromotionPiece.pieceType = ChessPieceType.Rook;
        currentPromotionPiece.changeSprite(ChessPieceType.Rook);
        if(currentPromotionPiece.team == 0){
            curBoard.checkForMate(1);
        }else{
            curBoard.checkForMate(0);
        }
        disableTurns = false;
    }

    public void OnPromoteKnight(){
        promoteScreen.SetActive(false);
        currentlyDragging = null;
        availableMoves.Clear();
        currentPromotionPiece.pieceType = ChessPieceType.Knight;
        currentPromotionPiece.changeSprite(ChessPieceType.Knight);
        if(currentPromotionPiece.team == 0){
            curBoard.checkForMate(1);
        }else{
            curBoard.checkForMate(0);
        }
        disableTurns = false;
    }

    public void OnPromoteBishop(){
        promoteScreen.SetActive(false);
        currentlyDragging = null;
        availableMoves.Clear();
        currentPromotionPiece.pieceType = ChessPieceType.Bishop;
        currentPromotionPiece.changeSprite(ChessPieceType.Bishop);
        if(currentPromotionPiece.team == 0){
            curBoard.checkForMate(1);
        }else{
            curBoard.checkForMate(0);
        }
        disableTurns = false;
    }
    




}

