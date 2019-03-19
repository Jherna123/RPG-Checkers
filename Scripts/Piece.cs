using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isTeal;
    public bool isKing;

    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        if (isTeal || isKing)
        {
            // top left
            if (x >= 2 && y <= 5)
            {
                // if there is a pieace and not same color
                Piece p = board[x - 1, y + 1];
                if (p != null && p.isTeal != isTeal)
                {
                    // can i land after jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
            //top right
            if (x <= 5 && y <= 5)
            {
                // if there is a pieace and not same color
                Piece p = board[x + 1, y + 1];
                if (p != null && p.isTeal != isTeal)
                {
                    // can i land after jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if(!isTeal || isKing)
        {
            // bottom left
            if (x >= 2 && y >= 2)
            {
                // if there is a pieace and not same color
                Piece p = board[x - 1, y - 1];
                if (p != null && p.isTeal != isTeal)
                {
                    // can i land after jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
            //bottom right
            if (x <= 5 && y >= 2)
            {
                // if there is a pieace and not same color
                Piece p = board[x + 1, y - 1];
                if (p != null && p.isTeal != isTeal)
                {
                    // can i land after jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        if(board[x2, y2] != null)
        {
            return false;
        }

        int deltaMove = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;
        if (isTeal || isKing)
        {
            if(deltaMove == 1)
            {
                if(deltaMoveY == 1)
                {
                    return true;
                }
            }
            else if(deltaMove == 2)
            {
                if(deltaMoveY == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null && p.isTeal != isTeal)
                    {
                        return true;
                    }
                }
            }
        }
        if (!isTeal || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isTeal != isTeal)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
