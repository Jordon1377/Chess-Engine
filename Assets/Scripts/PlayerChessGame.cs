using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerChessGame : MonoBehaviour
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

    //bool computerTeamWhite = false;
    

   

    // Start is called before the first frame update
    public void Awake() {
        
       curBoard = new ChessBoard(0, "", piecePrefab, boardPrefab, tilePrefab, 0f, 0f, 5f);
       curBoard.victoryScreen = victoryScreen;
       //Debug.Log(curBoard.turnNum);
       //curBoard.printBoard();

       
    }

    public void Update() {
        if(!currentCamera){
            currentCamera = Camera.main;
            return;
        }
        if(disableTurns){
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tiles", "Hover", "Highlight"))){
            Vector2Int hitIndex = new Vector2Int(info.transform.GetComponent<TileScript>().xPos,info.transform.GetComponent<TileScript>().yPos);

            if(currentHover == -Vector2Int.one){
                currentHover = hitIndex;
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
                        if(currentlyDragging.team == 0){
                            curBoard.checkForMate(1);
                        }else{
                            curBoard.checkForMate(0);
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
