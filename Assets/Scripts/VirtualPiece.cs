using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPiece
{
    public int team; //0 white 1 black
    public int xPos;
    public int yPos;
    public int numTimesMoved = 0;
    public ChessPieceType pieceType;
    public ChessBoard belongsToGame;
    
    //Virtual chess piece
    public VirtualPiece(ChessPieceType cpt, int tm, int x, int y, ref ChessBoard btg, int timesMoved)
    {
        pieceType = cpt;
        xPos = x;
        yPos = y;
        team = tm;
        belongsToGame = btg;
        numTimesMoved = timesMoved;
    }

    public List<movesScript> getAvailableMoves(ref VirtualPiece[,] board, ref List<movesScript> prevMoves){

        List<movesScript> arr = new List<movesScript>();
        
        switch(this.pieceType){
            case ChessPieceType.Pawn: arr = getPawnMoves(ref board, ref prevMoves); break;
            case ChessPieceType.Knight: arr = getKnightMoves(ref board); break;
            case ChessPieceType.Bishop: arr = getBishopMoves(ref board); break;
            case ChessPieceType.Rook: arr = getRookMoves(ref board); break;
            case ChessPieceType.Queen: arr = getQueenMoves(ref board); break;
            case ChessPieceType.King: arr = getKingMoves(ref board); break;            
        }
        

        return arr; 
    }

    public List<movesScript> getPawnMoves(ref VirtualPiece[,] board, ref List<movesScript> prevMoves){
        List<movesScript> arr = new List<movesScript>();
        int direction = (team == 0) ? 1 : -1;
        if((!(this.yPos == 6) && !(this.yPos == 7) && this.team == 0) || !(this.yPos == 1) && !(this.yPos == 0) && (this.team == 1)){
            if(board[xPos,yPos+direction] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+direction));
            }
            if(numTimesMoved == 0 && board[xPos,yPos+direction] == null && board[xPos,yPos+2*direction] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+2*direction));
            }
            if(!(this.xPos == 7) && board[xPos+1,yPos+direction] != null && this.team != board[xPos+1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos+direction));
            }
            if(!(this.xPos == 0) && board[xPos-1,yPos+direction] != null && this.team != board[xPos-1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos+direction));
            }
        }else if(this.yPos == 6 && this.team == 0){
            if(board[xPos,yPos+direction] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos, yPos+direction));
            }
            if(!(this.xPos == 7) && board[xPos+1,yPos+direction] != null && this.team != board[xPos+1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos+1, yPos+direction));
            }
            if(!(this.xPos == 0) && board[xPos-1,yPos+direction] != null && this.team != board[xPos-1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos-1, yPos+direction));
            }
        }else if(this.yPos == 1 && this.team == 1){
            if(board[xPos,yPos+direction] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos, yPos+direction));
            }
            if(!(this.xPos == 7) && board[xPos+1,yPos+direction] != null && this.team != board[xPos+1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos+1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos+1, yPos+direction));
            }
            if(!(this.xPos == 0) && board[xPos-1,yPos+direction] != null && this.team != board[xPos-1,yPos+direction].team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Rook, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Bishop, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Knight, xPos-1, yPos+direction));
                arr.Add(new movesScript(xPos, yPos, MoveType.Promote_Queen, xPos-1, yPos+direction));
            }
        }
        
        //Enpassant left
        if(xPos != 0){
            if(team == 0 && yPos == 4){
                if(board[xPos-1,yPos] != null && board[xPos-1,yPos].pieceType == ChessPieceType.Pawn && (prevMoves[prevMoves.Count-1].movePositionX == xPos-1) && (prevMoves[prevMoves.Count-1].movePositionY == yPos) && board[xPos-1,yPos].numTimesMoved == 1){
                    arr.Add(new movesScript(xPos, yPos, MoveType.EnPassant, xPos-1, yPos+1));
                }
            }
            if(team == 1 && yPos == 3){
                if(board[xPos-1,yPos] != null && board[xPos-1,yPos].pieceType == ChessPieceType.Pawn && (prevMoves[prevMoves.Count-1].movePositionX == xPos-1) && (prevMoves[prevMoves.Count-1].movePositionY == yPos) && board[xPos-1,yPos].numTimesMoved == 1){
                    arr.Add(new movesScript(xPos, yPos, MoveType.EnPassant, xPos-1, yPos-1));
                }
            }
        }
        //Enpassant right
        if(xPos != 7){
            if(team == 0 && yPos == 4){
                if(board[xPos+1,yPos] != null && board[xPos+1,yPos].pieceType == ChessPieceType.Pawn && (prevMoves[prevMoves.Count-1].movePositionX == xPos+1) && (prevMoves[prevMoves.Count-1].movePositionY == yPos) && board[xPos+1,yPos].numTimesMoved == 1){
                    arr.Add(new movesScript(xPos, yPos, MoveType.EnPassant, xPos+1, yPos+1));
                }
            }
            if(team == 1 && yPos == 3){
                if(board[xPos+1,yPos] != null && board[xPos+1,yPos].pieceType == ChessPieceType.Pawn && (prevMoves[prevMoves.Count-1].movePositionX == xPos+1) && (prevMoves[prevMoves.Count-1].movePositionY == yPos) && board[xPos+1,yPos].numTimesMoved == 1){
                    arr.Add(new movesScript(xPos, yPos, MoveType.EnPassant, xPos+1, yPos-1));
                }
            }
        }


        return arr; 
    }

    public List<movesScript> getKnightMoves(ref VirtualPiece[,] board){
        List<movesScript> arr = new List<movesScript>();
        if(xPos < 7 && yPos < 6 && ((board[xPos+1,yPos+2] != null && board[xPos+1,yPos+2].team != this.team) || board[xPos+1,yPos+2] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos+2));
        }
        if(xPos > 0 && yPos < 6 && ((board[xPos-1,yPos+2] != null && board[xPos-1,yPos+2].team != this.team) || board[xPos-1,yPos+2] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos+2));
        }
        if(xPos > 0 && yPos > 1 && ((board[xPos-1,yPos-2] != null && board[xPos-1,yPos-2].team != this.team) || board[xPos-1,yPos-2] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos-2));
        }
        if(xPos < 7 && yPos > 1 && ((board[xPos+1,yPos-2] != null && board[xPos+1,yPos-2].team != this.team) || board[xPos+1,yPos-2] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos-2));
        }
        if(xPos < 6 && yPos < 7 && ((board[xPos+2,yPos+1] != null && board[xPos+2,yPos+1].team != this.team) || board[xPos+2,yPos+1] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+2, yPos+1));
        }
        if(xPos > 1 && yPos < 7 && ((board[xPos-2,yPos+1] != null && board[xPos-2,yPos+1].team != this.team) || board[xPos-2,yPos+1] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-2, yPos+1));
        }
        if(xPos > 1 && yPos > 0 && ((board[xPos-2,yPos-1] != null && board[xPos-2,yPos-1].team != this.team) || board[xPos-2,yPos-1] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-2, yPos-1));
        }
        if(xPos < 6 && yPos > 0 && ((board[xPos+2,yPos-1] != null && board[xPos+2,yPos-1].team != this.team) || board[xPos+2,yPos-1] == null)){
            arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+2, yPos-1));
        }
        return arr; 
    }

    public List<movesScript> getBishopMoves(ref VirtualPiece[,] board){
        List<movesScript> arr = new List<movesScript>();
        int i = 1;
        bool blocked = false;
        while(xPos+i < 8 && yPos+i < 8 && !blocked){
            if(board[xPos+i,yPos+i] != null){
                blocked = true;
                if(board[xPos+i,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos+i));
                }
            }
            if(board[xPos+i,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos+i));
            }
            i++;
        }
        i=1;
        blocked = false;

        while(xPos-i > -1 && yPos-i> -1 && !blocked){
            if(board[xPos-i,yPos-i] != null){
                blocked = true;
                if(board[xPos-i,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos-i));
                }
            }
            if(board[xPos-i,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos-i));
            }
            i++;
        }
        i=1;
        blocked = false;

        while(xPos+i < 8 && yPos-i> -1 && !blocked){
            if(board[xPos+i,yPos-i] != null){
                blocked = true;
                if(board[xPos+i,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos-i));
                }
            }
            if(board[xPos+i,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos-i));
            }
            i++;
        }
        i=1;
        blocked = false;

        while(xPos-i > -1 && yPos+i < 8 && !blocked){
            if(board[xPos-i,yPos+i] != null){
                blocked = true;
                if(board[xPos-i,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos+i));
                }
            }
            if(board[xPos-i,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos+i));
            }
            i++;
        }
        return arr; 
    }

    public List<movesScript> getRookMoves(ref VirtualPiece[,] board){
        List<movesScript> arr = new List<movesScript>();
        int i = 1;
        bool blocked = false;
        while(xPos+i < 8 && !blocked){
            if(board[xPos+i,yPos] != null){
                blocked = true;
                if(board[xPos+i,yPos].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos));
                }
            }
            if(board[xPos+i,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(xPos-i > -1 && !blocked){
            if(board[xPos-i,yPos] != null){
                blocked = true;
                if(board[xPos-i,yPos].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos));
                }
            }
            if(board[xPos-i,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(yPos+i < 8 && !blocked){
            if(board[xPos,yPos+i] != null){
                blocked = true;
                if(board[xPos,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+i));
                }
            }
            if(board[xPos,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+i));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(yPos-i > -1 && !blocked){
            if(board[xPos,yPos-i] != null){
                blocked = true;
                if(board[xPos,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-i));
                }
            }
            if(board[xPos,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-i));
            }
            i++;
        }
        return arr; 
    }

    public List<movesScript> getQueenMoves(ref VirtualPiece[,] board){
        List<movesScript> arr = new List<movesScript>();
        int i = 1;
        bool blocked = false;
        while(xPos+i < 8 && yPos+i < 8 && !blocked){
            if(board[xPos+i,yPos+i] != null){
                blocked = true;
                if(board[xPos+i,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos+i));
                }
            }
            if(board[xPos+i,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos+i));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(xPos-i > -1 && yPos-i> -1 && !blocked){
            if(board[xPos-i,yPos-i] != null){
                blocked = true;
                if(board[xPos-i,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos-i));
                }
            }
            if(board[xPos-i,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos-i));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(xPos+i < 8 && yPos-i> -1 && !blocked){
            if(board[xPos+i,yPos-i] != null){
                blocked = true;
                if(board[xPos+i,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos-i));
                }
            }
            if(board[xPos+i,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos-i));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(xPos-i > -1 && yPos+i < 8 && !blocked){
            if(board[xPos-i,yPos+i] != null){
                blocked = true;
                if(board[xPos-i,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos+i));
                }
            }
            if(board[xPos-i,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos+i));
            }
            i++;
        }
        i = 1;
        blocked = false;
        while(xPos+i < 8 && !blocked){
            if(board[xPos+i,yPos] != null){
                blocked = true;
                if(board[xPos+i,yPos].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos));
                }
            }
            if(board[xPos+i,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+i, yPos));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(xPos-i > -1 && !blocked){
            if(board[xPos-i,yPos] != null){
                blocked = true;
                if(board[xPos-i,yPos].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos));
                }
            }
            if(board[xPos-i,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-i, yPos));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(yPos+i < 8 && !blocked){
            if(board[xPos,yPos+i] != null){
                blocked = true;
                if(board[xPos,yPos+i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+i));
                }
            }
            if(board[xPos,yPos+i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+i));
            }
            i++;
        }
        i=1;
        blocked = false;
        while(yPos-i > -1 && !blocked){
            if(board[xPos,yPos-i] != null){
                blocked = true;
                if(board[xPos,yPos-i].team != this.team){
                    arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-i));
                }
            }
            if(board[xPos,yPos-i] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-i));
            }
            i++;
        }
        return arr; 
    }

    public List<movesScript> getKingMoves(ref VirtualPiece[,] board){

        List<movesScript> arr = new List<movesScript>();
        
        //left
        if(xPos > 0){
            if(board[xPos-1,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos));
            }else if(board[xPos-1,yPos].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos));
            }
        }
        //Upleft
        if(xPos > 0 && yPos < 7){
            if(board[xPos-1,yPos+1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos+1));
            }else if(board[xPos-1,yPos+1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos+1));
            }
        }
        //DownLeft
        if(xPos > 0 && yPos > 0){
            if(board[xPos-1,yPos-1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos-1));
            }else if(board[xPos-1,yPos-1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos-1, yPos-1));
            }
        }
        //right
        if(xPos < 7){
            if(board[xPos+1,yPos] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos));
            }else if(board[xPos+1,yPos].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos));
            }
        }
        //UpRight
        if(xPos < 7 && yPos < 7){
            if(board[xPos+1,yPos+1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos+1));
            }else if(board[xPos+1,yPos+1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos+1));
            }
        }
        //DownRight
        if(xPos < 7 && yPos > 0){
            if(board[xPos+1,yPos-1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos-1));
            }else if(board[xPos+1,yPos-1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos+1, yPos-1));
            }
        }
        //up
        if(yPos < 7){
            if(board[xPos,yPos+1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+1));
            }else if(board[xPos,yPos+1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos+1));
            }
        }
        //down
        if(yPos > 0){
            if(board[xPos,yPos-1] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-1));
            }else if(board[xPos,yPos-1].team != team){
                arr.Add(new movesScript(xPos, yPos, MoveType.Default, xPos, yPos-1));
            }
        }

        //Castle Short
        if(this.numTimesMoved == 0){
            if(this.team == 0 && board[7,0] != null && board[7,0].pieceType == ChessPieceType.Rook && board[7,0].numTimesMoved == 0 && board[6,0] == null && board[5,0] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Castle_Short, xPos+2, yPos));
            }
            if(this.team == 1 && board[7,7] != null && board[7,7].pieceType == ChessPieceType.Rook && board[7,7].numTimesMoved == 0 && board[6,7] == null && board[5,7] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Castle_Short, xPos+2, yPos));
            }
        }

        //Castle long
        if(this.numTimesMoved == 0){
            if(this.team == 0 && board[0,0] != null && board[0,0].pieceType == ChessPieceType.Rook && board[0,0].numTimesMoved == 0 && board[1,0] == null && board[2,0] == null && board[3,0] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Castle_Long, xPos-2, yPos));
            }
            if(this.team == 1 && board[0,7] != null && board[0,7].pieceType == ChessPieceType.Rook && board[0,7].numTimesMoved == 0 && board[1,7] == null && board[2,7] == null && board[3,7] == null){
                arr.Add(new movesScript(xPos, yPos, MoveType.Castle_Long, xPos-2, yPos));
            }
        }
        

        return arr; 
    }



   

}
