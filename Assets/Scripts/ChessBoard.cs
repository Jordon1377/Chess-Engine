using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Text;

public class ChessBoard
{
        public int turnNum;
        public bool whiteTurn = true;

        private string startingPositionNotation;
        public string currentPositionNotation;

        public GameObject piecePrefab;
        public GameObject boardPrefab;
        public GameObject tilePrefab;
        public GameObject boardTransform;
        public GameObject victoryScreen;

        public float boardPosX;
        public float boardPosY;
        public float boardSize;
        private float defaultSizeBoard = 1.76f;
        float squareSize;

        private float offsetPieceX = 0.7725f;
        private float offsetPieceY = 0.7765f;
        private float offsetScaledX;
        private float offsetScaledY;

        public ChessPiece[,] board = new ChessPiece[8,8];
        public List<ChessPiece> deadWhite = new List<ChessPiece>();
        public List<ChessPiece> deadBlack = new List<ChessPiece>();
        public TileScript[,] tiles = new TileScript[8,8];
        public List<movesScript> boardMovesMade = new List<movesScript>();
        public VirtualPiece[,] virtualBoard = new VirtualPiece[8,8];
        //public VirtualPiece[] kings = new VirtualPiece[2];

        
        //Physical Board Constructor
        public ChessBoard(int tNum, string sP, GameObject pPrefab, GameObject bPrefab, GameObject tPrefab, float bpx, float bpy, float bSize)
        {
            turnNum = tNum;
            startingPositionNotation = sP;
            currentPositionNotation = sP;
            piecePrefab = pPrefab;
            boardPrefab = bPrefab;
            tilePrefab = tPrefab;
            boardPosX = bpx;
            boardPosY = bpy;
            boardSize = defaultSizeBoard * bSize;
            offsetScaledX = offsetPieceX * bSize;
            offsetScaledY = offsetPieceY * bSize;

            boardTransform = GameObject.Instantiate(boardPrefab, new Vector3(boardPosX,boardPosY,0), Quaternion.identity);
            boardTransform.transform.localScale += new Vector3(bSize-1, bSize-1, bSize-1);

            squareSize = boardSize/8;
            board = generatePieces("", squareSize, bSize);
            changeAllSprites(board);

            tiles = generateAllTiles(squareSize, bSize);
            
        }


        //Virtual Board contructor
        public ChessBoard(ChessBoard copyBoard)
        {
            turnNum = copyBoard.turnNum;
            whiteTurn = copyBoard.whiteTurn;

            //Board
            for (int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(copyBoard.board[i,j] != null){
                        virtualBoard[i,j] = new VirtualPiece(copyBoard.board[i,j].pieceType, copyBoard.board[i,j].team, copyBoard.board[i,j].xPos, copyBoard.board[i,j].yPos, ref copyBoard, copyBoard.board[i,j].numTimesMoved);
                    }
                }
            }   
            //Moves made  
            for(int i = 0; i < copyBoard.boardMovesMade.Count; i++){
                this.boardMovesMade.Add(copyBoard.boardMovesMade[i]);
            }   
        }

        //Virtual Board from virtual board contructor
        public ChessBoard(ChessBoard copyBoard, bool tmp)
        {
            turnNum = copyBoard.turnNum;
            whiteTurn = copyBoard.whiteTurn;

            //Board
            for (int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(copyBoard.virtualBoard[i,j] != null){
                        virtualBoard[i,j] = new VirtualPiece(copyBoard.virtualBoard[i,j].pieceType, copyBoard.virtualBoard[i,j].team, copyBoard.virtualBoard[i,j].xPos, copyBoard.virtualBoard[i,j].yPos, ref copyBoard, copyBoard.virtualBoard[i,j].numTimesMoved);
                    }
                }
            }   
            //Moves made  
            for(int i = 0; i < copyBoard.boardMovesMade.Count; i++){
                this.boardMovesMade.Add(copyBoard.boardMovesMade[i]);
            }   
        }


        private ChessPiece[,] generatePieces(string positionCode, float squareSize, float bSize){
            ChessPiece[,] chessPieces = new ChessPiece[8,8];

            if(positionCode == ""){

                //White------------------------------------------------------------------------------------------------------------------------
                chessPieces[0,1] = generatePiece(ChessPieceType.Pawn, 0, 0,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[1,1] = generatePiece(ChessPieceType.Pawn, 0, 1,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[2,1] = generatePiece(ChessPieceType.Pawn, 0, 2,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[3,1] = generatePiece(ChessPieceType.Pawn, 0, 3,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[4,1] = generatePiece(ChessPieceType.Pawn, 0, 4,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[5,1] = generatePiece(ChessPieceType.Pawn, 0, 5,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[6,1] = generatePiece(ChessPieceType.Pawn, 0, 6,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[7,1] = generatePiece(ChessPieceType.Pawn, 0, 7,1, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);

                chessPieces[0,0] = generatePiece(ChessPieceType.Rook, 0, 0,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[1,0] = generatePiece(ChessPieceType.Knight, 0, 1,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[2,0] = generatePiece(ChessPieceType.Bishop, 0, 2,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[3,0] = generatePiece(ChessPieceType.Queen, 0, 3,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[4,0] = generatePiece(ChessPieceType.King, 0, 4,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[5,0] = generatePiece(ChessPieceType.Bishop, 0, 5,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[6,0] = generatePiece(ChessPieceType.Knight, 0, 6,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[7,0] = generatePiece(ChessPieceType.Rook, 0, 7,0, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);

                //Black------------------------------------------------------------------------------------------------------------------------
                chessPieces[0,6] = generatePiece(ChessPieceType.Pawn, 1, 0,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[1,6] = generatePiece(ChessPieceType.Pawn, 1, 1,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[2,6] = generatePiece(ChessPieceType.Pawn, 1, 2,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[3,6] = generatePiece(ChessPieceType.Pawn, 1, 3,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[4,6] = generatePiece(ChessPieceType.Pawn, 1, 4,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[5,6] = generatePiece(ChessPieceType.Pawn, 1, 5,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[6,6] = generatePiece(ChessPieceType.Pawn, 1, 6,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[7,6] = generatePiece(ChessPieceType.Pawn, 1, 7,6, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                
                chessPieces[0,7] = generatePiece(ChessPieceType.Rook, 1, 0,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[1,7] = generatePiece(ChessPieceType.Knight, 1, 1,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[2,7] = generatePiece(ChessPieceType.Bishop, 1, 2,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[3,7] = generatePiece(ChessPieceType.Queen, 1, 3,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[4,7] = generatePiece(ChessPieceType.King, 1, 4,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[5,7] = generatePiece(ChessPieceType.Bishop, 1, 5,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[6,7] = generatePiece(ChessPieceType.Knight, 1, 6,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
                chessPieces[7,7] = generatePiece(ChessPieceType.Rook, 1, 7,7, squareSize, offsetPieceX*bSize, offsetPieceY*bSize, bSize);
            }else{
                return null;
            }
            return chessPieces;  
        }

        private ChessPiece generatePiece(ChessPieceType type, int team, int xPos, int yPos, float squareSize, float offsetX, float offsetY, float scale){
            float x = (float)(xPos)*squareSize+boardTransform.transform.position.x - offsetX;
            float y = (float)(yPos)*squareSize+boardTransform.transform.position.y - offsetY;

            ChessPiece cp = GameObject.Instantiate(piecePrefab, new Vector3(x,y,-2), Quaternion.identity).GetComponent<ChessPiece>();
            cp.pieceType = type;
            cp.team = team;
            cp.xPos = xPos;
            cp.yPos = yPos;
            cp.belongsToGame = this;
            //cp.transform.localScale += new Vector3((scale*0.275f)-0.275f, (scale*0.275f)-0.275f, (scale*0.275f)-0.275f);
            return cp;
            
        }

        private void changeAllSprites(ChessPiece[,] pieces){
            for(int i = 0; i<8; i++){
                for(int j = 0; j < 8; j++){
                    if(pieces[i,j] != null){
                        pieces[i,j].changeSprite(pieces[i,j].pieceType);
                    }
                }
            }
        }

        private TileScript[,] generateAllTiles(float squareSize, float scale){
            TileScript[,] tileArray = new TileScript[8,8];

            for(int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    tileArray[i,j] = generateTile(i,j,squareSize,3.851f/5f*scale, 3.853f/5f*scale, scale);
                }
            }
            return tileArray;
        }

        private TileScript generateTile(int xPos, int yPos, float squareSize, float offsetX, float offsetY, float scale){
            float x = (float)(xPos)*squareSize+boardTransform.transform.position.x - offsetX;
            float y = (float)(yPos)*squareSize+boardTransform.transform.position.y - offsetY;

            TileScript tile = GameObject.Instantiate(tilePrefab, new Vector3(x,y,-1), Quaternion.identity).GetComponent<TileScript>();
            tile.xPos = xPos;
            tile.yPos = yPos;
            tile.belongsToBoard = this;
            tile.transform.localScale += new Vector3((scale*0.2195f)-0.2195f, (scale*0.2195f)-0.2195f, (scale*0.2195f)-0.2195f);

            return tile;

        }


        public bool movePieceVirtual(int xPos, int yPos, movesScript move){
            VirtualPiece piece = this.virtualBoard[move.currentPositionX, move.currentPositionY];
            bool isValid = isValidMoveVirtual(piece, xPos, yPos);
            boardMovesMade.Add(move);

            if(isValid){
                if(move.moveType == MoveType.Default){
                    virtualBoard[piece.xPos, piece.yPos] = null;
                    virtualBoard[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;
                    //printBoard();
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.EnPassant){
                    if(piece.team == 0){
                            virtualBoard[xPos,yPos-1] = null;
                        }if(piece.team == 1){
                            virtualBoard[xPos,yPos+1] = null;
                        }
                    
                    virtualBoard[piece.xPos, piece.yPos] = null;
                    virtualBoard[xPos,yPos] = piece;
                    piece.xPos = xPos;
                    piece.yPos = yPos;
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Castle_Short){
                    if(piece.team == 0){
                            virtualBoard[5,0] = virtualBoard[7,0];
                            virtualBoard[7,0] = null;
                            virtualBoard[5,0].xPos = 5;
                            virtualBoard[5,0].yPos = 0;
                            virtualBoard[5,0].numTimesMoved++;
                        }if(piece.team == 1){
                            virtualBoard[5,7] = virtualBoard[7,7];
                            virtualBoard[7,7] = null;
                            virtualBoard[5,7].xPos = 5;
                            virtualBoard[5,7].yPos = 7;
                            virtualBoard[5,7].numTimesMoved++;
                        }
                    
                    virtualBoard[piece.xPos, piece.yPos] = null;
                    virtualBoard[xPos,yPos] = piece;
                    piece.xPos = xPos;
                    piece.yPos = yPos;
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Castle_Long){
                    if(piece.team == 0){
                            virtualBoard[3,0] = virtualBoard[0,0];
                            virtualBoard[0,0] = null;
                            virtualBoard[3,0].xPos = 3;
                            virtualBoard[3,0].yPos = 0;
                            virtualBoard[3,0].numTimesMoved++;
                        }if(piece.team == 1){
                            virtualBoard[3,7] = virtualBoard[0,7];
                            virtualBoard[0,7] = null;
                            virtualBoard[3,7].xPos = 3;
                            virtualBoard[3,7].yPos = 7;
                            virtualBoard[3,7].numTimesMoved++;
                        }
                    
                    virtualBoard[piece.xPos, piece.yPos] = null;
                    virtualBoard[xPos,yPos] = piece;
                    piece.xPos = xPos;
                    piece.yPos = yPos;
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Promote_Queen || move.moveType == MoveType.Promote_Rook || move.moveType == MoveType.Promote_Bishop || move.moveType == MoveType.Promote_Knight){
                    virtualBoard[piece.xPos, piece.yPos] = null;
                    virtualBoard[xPos,yPos] = piece;
                    piece.xPos = xPos;
                    piece.yPos = yPos;
                    //printBoard();
                    //Change piece type
                    switch(move.moveType){
                        case MoveType.Promote_Queen: piece.pieceType = ChessPieceType.Queen; break;
                        case MoveType.Promote_Rook: piece.pieceType = ChessPieceType.Rook; break;
                        case MoveType.Promote_Bishop: piece.pieceType = ChessPieceType.Bishop; break;
                        case MoveType.Promote_Knight: piece.pieceType = ChessPieceType.Knight; break;
                    }
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }
            }
            return true;
        }

        public bool movePiece(ref ChessPiece piece, int xPos, int yPos, movesScript move, bool moved){
            float newX = (float)(xPos)*squareSize+boardTransform.transform.position.x - offsetScaledX;
            float newY = (float)(yPos)*squareSize+boardTransform.transform.position.y - offsetScaledY;

            float prevX = (float)(piece.xPos)*squareSize+boardTransform.transform.position.x - offsetScaledX;
            float prevY = (float)(piece.yPos)*squareSize+boardTransform.transform.position.y - offsetScaledY;

            bool isValid = isValidMove(piece, xPos, yPos);
            if(moved){
                boardMovesMade.Add(move);
            }

            if(isValid){
                if(move.moveType == MoveType.Default){
                    if(board[xPos,yPos] != null){
                        if(board[xPos,yPos].team == 1){
                            deadBlack.Add(board[xPos,yPos]);
                            board[xPos,yPos].setPosition(new Vector3(-1.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((8f - deadBlack.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));

                        }if(board[xPos,yPos].team == 0){
                            deadWhite.Add(board[xPos,yPos]);
                            board[xPos,yPos].setPosition(new Vector3(8.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((-1f + deadWhite.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));

                        }
                        board[xPos,yPos].setScale(new Vector3((boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f)));
                        

                    }

                    board[piece.xPos, piece.yPos] = null;
                    board[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;

                    piece.setPosition(new Vector3(newX,newY, -2));


                    //printBoard();
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.EnPassant){
                    //If checkmate call function
                    if(piece.team == 0){
                            //checkmate(0);
                            deadBlack.Add(board[xPos,yPos-1]);
                            board[xPos,yPos-1].setPosition(new Vector3(-1.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((8f - deadBlack.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));
                            board[xPos,yPos-1].setScale(new Vector3((boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f)));
                            board[xPos,yPos-1] = null;
                        }if(piece.team == 1){
                            //checkmate(1);
                            deadWhite.Add(board[xPos,yPos+1]);
                            board[xPos,yPos+1].setPosition(new Vector3(8.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((-1f + deadWhite.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));
                            board[xPos,yPos+1].setScale(new Vector3((boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f)));
                            board[xPos,yPos+1] = null;
                        }
                    
                    board[piece.xPos, piece.yPos] = null;
                    board[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;

                    piece.setPosition(new Vector3(newX,newY, -2));

                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Castle_Short){
                    //If checkmate call function
                    if(piece.team == 0){
                            board[5,0] = board[7,0];
                            board[7,0] = null;
                            board[5,0].xPos = 5;
                            board[5,0].yPos = 0;
                            board[5,0].numTimesMoved++;
                            board[5,0].setPosition(new Vector3(5*squareSize+boardTransform.transform.position.x - offsetScaledX, boardTransform.transform.position.y - offsetScaledY, -2));
                        }if(piece.team == 1){
                            board[5,7] = board[7,7];
                            board[7,7] = null;
                            board[5,7].xPos = 5;
                            board[5,7].yPos = 7;
                            board[5,7].numTimesMoved++;
                            board[5,7].setPosition(new Vector3(5*squareSize+boardTransform.transform.position.x - offsetScaledX, 7*squareSize+boardTransform.transform.position.y - offsetScaledY, -2));
                        }
                    
                    board[piece.xPos, piece.yPos] = null;
                    board[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;

                    piece.setPosition(new Vector3(newX,newY, -2));

                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Castle_Long){
                    //If checkmate call function
                    if(piece.team == 0){
                            board[3,0] = board[0,0];
                            board[0,0] = null;
                            board[3,0].xPos = 3;
                            board[3,0].yPos = 0;
                            board[3,0].numTimesMoved++;
                            board[3,0].setPosition(new Vector3(3*squareSize+boardTransform.transform.position.x - offsetScaledX, boardTransform.transform.position.y - offsetScaledY, -2));
                        }if(piece.team == 1){
                            board[3,7] = board[0,7];
                            board[0,7] = null;
                            board[3,7].xPos = 3;
                            board[3,7].yPos = 7;
                            board[3,7].numTimesMoved++;
                            board[3,7].setPosition(new Vector3(3*squareSize+boardTransform.transform.position.x - offsetScaledX, 7*squareSize+boardTransform.transform.position.y - offsetScaledY, -2));
                        }
                    
                    board[piece.xPos, piece.yPos] = null;
                    board[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;

                    piece.setPosition(new Vector3(newX,newY, -2));

                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }else if(move.moveType == MoveType.Promote_Queen || move.moveType == MoveType.Promote_Rook || move.moveType == MoveType.Promote_Bishop || move.moveType == MoveType.Promote_Knight){
                    //choosePromotePiece(ref piece);
                    if(board[xPos,yPos] != null){
                        
                        if(board[xPos,yPos].team == 1){
                            deadBlack.Add(board[xPos,yPos]);
                            board[xPos,yPos].setPosition(new Vector3(-1.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((8f - deadBlack.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));

                        }if(board[xPos,yPos].team == 0){
                            deadWhite.Add(board[xPos,yPos]);
                            board[xPos,yPos].setPosition(new Vector3(8.1f*squareSize+boardTransform.transform.position.x - offsetScaledX, ((-1f + deadWhite.Count/1.75f)*squareSize)+boardTransform.transform.position.y - offsetScaledY, -2));

                        }
                        board[xPos,yPos].setScale(new Vector3((boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f), (boardSize/defaultSizeBoard/1.5f*0.275f)));
                        

                    }

                    board[piece.xPos, piece.yPos] = null;
                    board[xPos,yPos] = piece;

                    piece.xPos = xPos;
                    piece.yPos = yPos;

                    piece.setPosition(new Vector3(newX,newY, -2));


                    //printBoard();
                    piece.numTimesMoved++;
                    turnNum++;
                    whiteTurn = !whiteTurn;
                }
            }else{
                piece.setPosition(new Vector3(prevX,prevY, -2));
            }
            return true;
        }

        public bool isValidMove(ChessPiece piece, int xPos, int yPos){

            if(board[xPos, yPos] != null && board[xPos,yPos].team == piece.team){
                return false;
            }           

            return true;
        }

        public bool isValidMoveVirtual(VirtualPiece piece, int xPos, int yPos){

            if(virtualBoard[xPos, yPos] != null && virtualBoard[xPos,yPos].team == piece.team){
                return false;
            }           

            return true;
        }

        public List<movesScript> getAllRawMoves(int team){
            List<movesScript> rawMoves = new List<movesScript>();
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(board[i,j] != null && board[i,j].team == team){
                        rawMoves.AddRange(board[i,j].getAvailableMoves(ref this.board, ref this.boardMovesMade));
                    }
                }
            }
            return rawMoves;
        }

        public List<movesScript> getAllRawMovesVirtual(int team){
            List<movesScript> rawMoves = new List<movesScript>();
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(virtualBoard[i,j] != null && virtualBoard[i,j].team == team){
                        rawMoves.AddRange(virtualBoard[i,j].getAvailableMoves(ref this.virtualBoard, ref this.boardMovesMade));
                    }
                }
            }
            return rawMoves;
        }

        public List<movesScript> getValidMoves(List<movesScript> availableRawMoves){
            List<movesScript> validMoves = new List<movesScript>();
            for(int i = 0; i < availableRawMoves.Count; i++){
                //Make tempboard
                ChessBoard simulateMoveBoard = new ChessBoard(this);
                //simulateMoveBoard.printBoardVirtual();
                bool currentTeamTurn = simulateMoveBoard.whiteTurn;
                //Play the move
                int xMove = availableRawMoves[i].movePositionX;
                int yMove = availableRawMoves[i].movePositionY;

                simulateMoveBoard.movePieceVirtual(xMove, yMove, availableRawMoves[i]);
                //simulateMoveBoard.printBoardVirtual();

                //Check for if move is legal
                List<movesScript> newRawMoves;
                int team;
                if(currentTeamTurn){
                    team = 0;
                    newRawMoves = simulateMoveBoard.getAllRawMovesVirtual(1);
                }else{
                    team = 1;
                    newRawMoves = simulateMoveBoard.getAllRawMovesVirtual(0);
                }
                //Debug.Log(newRawMoves.Count);

                VirtualPiece king = null;
                for(int j = 0; j < 8; j++){
                    for(int k = 0; k < 8; k++){
                        if(simulateMoveBoard.virtualBoard[j,k] != null && simulateMoveBoard.virtualBoard[j,k].pieceType == ChessPieceType.King && simulateMoveBoard.virtualBoard[j,k].team == team){
                            king = simulateMoveBoard.virtualBoard[j,k];
                        }
                    }
                }
                //Debug.Log(king.team);
                //Debug.Log(king.xPos);
                //Debug.Log(king.yPos);

                bool validMove = true;
                if(availableRawMoves[i].moveType != MoveType.Castle_Long && availableRawMoves[i].moveType != MoveType.Castle_Short){
                    //Debug.Log("default");
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if(newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }else if(availableRawMoves[i].moveType == MoveType.Castle_Long){
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if((newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos+1 && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos+2 && newRawMoves[j].movePositionY == king.yPos)){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }else if(availableRawMoves[i].moveType == MoveType.Castle_Short){
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if((newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos-1 && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos-2 && newRawMoves[j].movePositionY == king.yPos)){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }

                //If legal add to list
                if(validMove){
                    validMoves.Add(availableRawMoves[i]);
                }
            }
            //Debug.Log("y");
            return validMoves;
        }

        public List<movesScript> getValidMovesVirtual(List<movesScript> availableRawMoves){
            List<movesScript> validMoves = new List<movesScript>();
            for(int i = 0; i < availableRawMoves.Count; i++){
                //Make tempboard
                ChessBoard simulateMoveBoard = new ChessBoard(this, true);
                //simulateMoveBoard.printBoardVirtual();
                bool currentTeamTurn = simulateMoveBoard.whiteTurn;
                //Play the move
                int xMove = availableRawMoves[i].movePositionX;
                int yMove = availableRawMoves[i].movePositionY;

                simulateMoveBoard.movePieceVirtual(xMove, yMove, availableRawMoves[i]);
                //simulateMoveBoard.printBoardVirtual();

                //Check for if move is legal
                List<movesScript> newRawMoves;
                int team;
                if(currentTeamTurn){
                    team = 0;
                    newRawMoves = simulateMoveBoard.getAllRawMovesVirtual(1);
                }else{
                    team = 1;
                    newRawMoves = simulateMoveBoard.getAllRawMovesVirtual(0);
                }
                //Debug.Log(newRawMoves.Count);

                VirtualPiece king = null;
                for(int j = 0; j < 8; j++){
                    for(int k = 0; k < 8; k++){
                        if(simulateMoveBoard.virtualBoard[j,k] != null && simulateMoveBoard.virtualBoard[j,k].pieceType == ChessPieceType.King && simulateMoveBoard.virtualBoard[j,k].team == team){
                            king = simulateMoveBoard.virtualBoard[j,k];
                        }
                    }
                }
                //Debug.Log(king.team);
                //Debug.Log(king.xPos);
                //Debug.Log(king.yPos);

                bool validMove = true;
                if(availableRawMoves[i].moveType != MoveType.Castle_Long && availableRawMoves[i].moveType != MoveType.Castle_Short){
                    //Debug.Log("default");
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if(newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }else if(availableRawMoves[i].moveType == MoveType.Castle_Long){
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if((newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos+1 && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos+2 && newRawMoves[j].movePositionY == king.yPos)){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }else if(availableRawMoves[i].moveType == MoveType.Castle_Short){
                    for(int j = 0; j < newRawMoves.Count; j++){
                        if((newRawMoves[j].movePositionX == king.xPos && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos-1 && newRawMoves[j].movePositionY == king.yPos) || (newRawMoves[j].movePositionX == king.xPos-2 && newRawMoves[j].movePositionY == king.yPos)){
                            //Debug.Log("invalid");
                            validMove = false;
                        }
                    }
                }

                //If legal add to list
                if(validMove){
                    validMoves.Add(availableRawMoves[i]);
                }
            }
            //Debug.Log("y");
            return validMoves;
        }


        public bool isInCheck(int team){
            int win;
            if(team == 0){
                win = 1;
            }else{
                win = 0;
            }
            List<movesScript> rawMoves = getAllRawMoves(win);
            //List<movesScript> validMoves = getValidMoves(rawMoves);

            int xKing = 0;
            int yKing = 0;
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(this.virtualBoard[i,j] != null && this.virtualBoard[i,j].team == team && this.virtualBoard[i,j].pieceType == ChessPieceType.King){
                        xKing = i;
                        yKing = j;
                    }
                }
            }

            bool tmp = false;
            for(int i = 0; i < rawMoves.Count; i++){
                if(rawMoves[i].movePositionX == xKing && rawMoves[i].movePositionY == yKing){
                    tmp = true; 
                    break;
                }
            }
            return tmp;
        }
        public bool checkForMate(int team){
            List<movesScript> rawMoves = getAllRawMoves(team);
            List<movesScript> validMoves = getValidMoves(rawMoves);
            int win;
            if(team == 0){
                win = 1;
            }else{
                win = 0;
            }
            if(validMoves.Count == 0){
                checkmate(win);
                return true;
            }
            return false;
        }
        public void checkmate(int team){
            displayVictory(team);
        }
        private void displayVictory(int team){
            victoryScreen.SetActive(true);
            victoryScreen.transform.GetChild(team).gameObject.SetActive(true);
        }     
        public void printBoard(){
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if(board[j,i] == null){
                       sb.Append("-");
                       sb.Append(' ');
                    }
                    else{
                        sb.Append((int)board[j,i].pieceType);
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
            Debug.Log("-----------------------------------------");
        }

        public void printBoardVirtual(){
            StringBuilder sb = new StringBuilder();
            for(int i = 7; i >= 0; i--){
                for(int j = 0; j < 8; j++){
                    if(virtualBoard[j,i] == null){
                       sb.Append("-");
                       sb.Append(' ');
                    }
                    else{
                        sb.Append((int)virtualBoard[j,i].pieceType);
                        sb.Append(' ');
                    }
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
            Debug.Log("-----------------------------------------");
        }

}


