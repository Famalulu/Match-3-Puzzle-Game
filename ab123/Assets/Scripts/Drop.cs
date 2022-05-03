using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private Board board;
    private GameObject otherDrop;
    private Vector2 firstPosition = Vector2.zero;
    private Vector2 finalPosition = Vector2.zero;
    private Vector2 tempPosition; 
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }
    void Update()
    {
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column;
        targetY = row;      
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {       //Move towards to target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDrops[column, row] != this.gameObject)
            {
                board.allDrops[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {       //Move towards to target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDrops[column, row] != this.gameObject)
            {
                board.allDrops[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }
    public IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDrop != null)
        {
            if(!isMatched && !otherDrop.GetComponent<Drop>().isMatched)
            {
                otherDrop.GetComponent<Drop>().row = row;
                otherDrop.GetComponent<Drop>().column = column;
                row = previousRow; 
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDrop = null;
        }
    }
    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
        {
            firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    void CalculateAngle()
    {
        if (Mathf.Abs(finalPosition.y - firstPosition.y) > swipeResist || Mathf.Abs(finalPosition.x - firstPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * 180 / Mathf.PI;
            MovePieces();
        }
        else
        {
            board.currentState=GameState.move;
        }
    }
    void MovePiecesActual(Vector2 direction) 
    {
        otherDrop = board.allDrops[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if(otherDrop != null)
        {
        otherDrop.GetComponent<Drop>().column += -1 * (int)direction.x;
        otherDrop.GetComponent<Drop>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(CheckMoveCoroutine());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)    //Right Swipe
        {   
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)    //Up Swipe
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 )   //Left Swipe 
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0 )    //Down Swipe
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
}