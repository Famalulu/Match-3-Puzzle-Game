using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait, move
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] drops;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDrops;
    private FindMatches findMatches;
    public float refillDelay = .5f;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width,height];
        allDrops = new GameObject[width,height];
        SetUp();
    }
    private void SetUp()
    {
        for(int i = 0; i<width; i++)
        {
            for(int j=0;j<height; j++)
            {
                Vector2 tempPosition = new Vector2(i,j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + "," + j + " )";               
                int dropToUse = Random.Range(0, drops.Length);
                
                int maxIterations = 0;
                while(MatchesAt(i, j, drops[dropToUse]) && maxIterations < 100)
                {
                    dropToUse = Random.Range(0, drops.Length);
                    maxIterations++;
                }   
                maxIterations = 0;

                GameObject drop = Instantiate(drops[dropToUse], tempPosition, Quaternion.identity);
                drop.GetComponent<Drop>().column = i;
                drop.GetComponent<Drop>().row = j;
                drop.transform.parent = this.transform;
                drop.name = "( " + i + "," + j + " )";
                allDrops[i, j] = drop;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDrops[column - 1, row].tag == piece.tag && allDrops[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDrops[column, row - 1].tag == piece.tag && allDrops[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        } else if (column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDrops[column,row - 1].tag == piece.tag && allDrops[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDrops[column - 1, row].tag == piece.tag && allDrops[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDrops[column, row].GetComponent<Drop>().isMatched)
        {
            findMatches.currentMatches.Remove(allDrops[column, row]);
            Destroy(allDrops[column, row]);
            allDrops[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDrops[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCoroutine());
    }
    private IEnumerator DecreaseRowCoroutine()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for(int j =0; j < height; j++)
            {
                if (allDrops[i, j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allDrops[i,j].GetComponent<Drop>().row -= nullCount;
                    allDrops[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay * .5f);
        StartCoroutine(FillBoardCoroutine());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j =0; j < height; j++)
            {
                if (allDrops[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dropToUse = Random.Range(0, drops.Length);
                    int maxIteration = 0;
                    while (MatchesAt(i,j, drops[dropToUse]) && maxIteration < 100)
                    {
                        maxIteration++;
                        dropToUse = Random.Range(0, drops.Length);
                    }
                    maxIteration = 0;
                    GameObject piece = Instantiate(drops[dropToUse], tempPosition, Quaternion.identity);
                    allDrops[i,j] = piece;
                    piece.GetComponent<Drop>().row = j;
                    piece.GetComponent<Drop>().column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int i=0; i < width; i++)
        {
            for(int j=0; j < height; j++)
            {
                if (allDrops[i, j] != null)
                {
                    if (allDrops[i, j].GetComponent<Drop>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;
    }
}