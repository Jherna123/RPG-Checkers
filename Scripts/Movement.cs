using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    // setting up the prefab and pieces for board size
    public Piece[,] pieces = new Piece[8, 8];
    public GameObject tielPiecePrefab;
    public GameObject grayPiecePrefab;

    private Vector3 boardOffset = new Vector3(-3.8f, 6.0f, -5.2f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private bool isTeal = true;
    private bool isTealTurn;
    private bool hasKilled;

    private Piece selectedPiece;

    // mouse clicking and movement stuff
    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private List<Piece> forcedPieces;

    //start from somwhere
    void Start()
    {
        isTealTurn = true;
        GenerateBoard();
        forcedPieces = new List<Piece>();
    }

    // Generate all the pieces
    private void GenerateBoard()
    {
        // Makes the tiel pieces
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
        // Makes the gray pieces
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }
    private void GeneratePiece(int x, int y)
    {
        bool isPieceTiel = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceTiel) ? tielPiecePrefab : grayPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    // This does not move a piece, on the contrary this part
    // changes where they spawn.
    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMouseOver();
        if((isTeal)?isTealTurn:!isTealTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if(selectedPiece != null)
            {
                updatePieceDrag(selectedPiece);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }

    void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("unable to find main camera");
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);

        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    void updatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("unable to find main camera");
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForPossibleMove();

        // multiplayer stuff
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        if(x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
            if(selectedPiece != null)
            {
                MovePiece(selectedPiece, x1, y1);
            }

            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if(selectedPiece != null)
        {
            if(endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
            //check if valid move
            if(selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                //did i kill anything
                //if this is a jump
                if(Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        if (isTealTurn)
                        {
                            GetComponent<UIStuff>().POPoints++;
                            GetComponent<UIStuff>().powerpointsO++;
                            if(GetComponent<UIStuff>().POPoints == 3)
                            {
                                GetComponent<UIStuff>().poLevel++;
                            }
                            if (GetComponent<UIStuff>().POPoints == 6)
                            {
                                GetComponent<UIStuff>().poLevel++;
                            }
                            if (GetComponent<UIStuff>().POPoints == 9)
                            {
                                GetComponent<UIStuff>().poLevel++;
                            }
                        }                  
                        else if (!isTealTurn)
                        {
                            GetComponent<UIStuff>().PTPoints++;
                            GetComponent<UIStuff>().powerpointsT++;
                            if (GetComponent<UIStuff>().PTPoints == 3)
                            {
                                GetComponent<UIStuff>().ptLevel++;
                            }
                            if (GetComponent<UIStuff>().PTPoints == 6)
                            {
                                GetComponent<UIStuff>().ptLevel++;
                            }
                            if (GetComponent<UIStuff>().PTPoints == 9)
                            {
                                GetComponent<UIStuff>().ptLevel++;
                            }
                        }
                        hasKilled = true;
                    }
                }

                //was i supposed to kill anything?
                if(forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }
    private void EndTurn()
    {
        //check if there is going to be a king
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // you got a promotion
        if(selectedPiece.isTeal && !selectedPiece.isKing && y == 7)
        {
            selectedPiece.isKing = true;
            selectedPiece.transform.Rotate(Vector3.right * 180);
        }
        else if(!selectedPiece.isTeal && !selectedPiece.isKing && y == 0)
        {
            selectedPiece.isKing = true;
            selectedPiece.transform.Rotate(Vector3.right * 180);
        }

        selectedPiece = null;
        startDrag = Vector2.zero;

        if(ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
        {
            return;
        }

        isTealTurn = !isTealTurn;
        isTeal = !isTeal;
        hasKilled = false;
        CheckVictory();

    }
    //check to see if someone has lost all pieces
    private void CheckVictory()
    {
        if( GetComponent<UIStuff>().POPoints == 12 )
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
        if (GetComponent<UIStuff>().PTPoints == 12)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }
    private void SelectPiece(int x, int y)
    {
        // out of bounds checking
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }
        Piece p = pieces[x, y];
        if(p != null && p.isTeal == isTeal)
        {
            if(forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                //look for the pieces that are being forced to move
                if(forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }

    private List<Piece> ScanForPossibleMove(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();

        if(pieces[x,y].IsForceToMove(pieces, x, y))
        {
            forcedPieces.Add(pieces[x, y]);
        }

        return forcedPieces;
    }

    private List<Piece> ScanForPossibleMove()
    {
        forcedPieces = new List<Piece>();

        //check the pieces
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isTeal == isTealTurn)
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);
        return forcedPieces;
    }
    
}
