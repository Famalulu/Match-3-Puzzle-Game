using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }
    private void AddToListAndMatch(GameObject drop)
    {
        if (!currentMatches.Contains(drop))
        {
            currentMatches.Add(drop);
        }
        drop.GetComponent<Drop>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject drop1, GameObject drop2, GameObject drop3)
    {
        AddToListAndMatch(drop1);
        AddToListAndMatch(drop2);
        AddToListAndMatch(drop3);
    }
    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject currentDrop = board.allDrops[i, j];
                if(currentDrop != null)
                {
                    if (i>0 && i<board.width - 1)
                    {
                        GameObject leftDrop = board.allDrops[i - 1, j];
                        GameObject rightDrop = board.allDrops[i + 1, j];
                        if(leftDrop != null && rightDrop != null)
                        {
                            if (leftDrop.tag == currentDrop.tag && rightDrop.tag == currentDrop.tag)
                            {
                                GetNearbyPieces(leftDrop, currentDrop , rightDrop);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDrop = board.allDrops[i, j + 1];
                        GameObject downDrop = board.allDrops[i, j - 1];
                        if (upDrop != null && downDrop != null)
                        {
                            if (upDrop.tag == currentDrop.tag && downDrop.tag == currentDrop.tag)
                            {
                                GetNearbyPieces(upDrop, currentDrop,downDrop);
                            }
                        }
                    }
                }
            }
        }
    }
}