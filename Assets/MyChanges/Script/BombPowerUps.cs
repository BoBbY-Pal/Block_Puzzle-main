using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Hyperbyte;

public class BombPowerUps : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler, IDragHandler
{
    private bool canClear = false;

    private List<int> highlightRows = new List<int>();
    private List<int> highlightColoum = new List<int>();

    private List<Block> colliderDisabledBlock = new List<Block>();

    private void Reset()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EnableAllCollider();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos = pos + new Vector2(0, 1);
        this.transform.position = pos;
        HighLightBlock();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject != null)
        {
            Transform clickedObject = eventData.pointerCurrentRaycast.gameObject.transform;
            if(clickedObject == this.transform)
            {
                this.transform.localScale = Vector3.one * 1.2f;
                this.transform.position = this.transform.position + new Vector3(0, 1, 0);
            }
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        DiableAllCollider();
        StopHighlighting();
        if(canClear)
        {
            GamePlay.Instance.ClearRows(highlightRows);
            GamePlay.Instance.ClearColumns(highlightColoum);
            canClear = false;
            highlightRows.Clear();
            highlightColoum.Clear();
        }
    }

    private void EnableAllCollider()
    {
        foreach(List<Block> row in GamePlay.Instance.allRows)
        {
            foreach(Block block in row)
            {
                if(block.isFilled)
                {
                    block.thisCollider.enabled = true;
                    colliderDisabledBlock.Add(block);
                }
            }
        } 
    }

    private void DiableAllCollider()
    {
        foreach(Block block in colliderDisabledBlock)
        {
            block.thisCollider.enabled = false;
        }
    }

    private void HighLightBlock()
    {
        Collider2D[] collidedBlocks = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(this.transform.localScale.x / 4, this.transform.localScale.x / 4), 0);
        
        if(collidedBlocks.Length <= 0)
        {
            canClear = false;
            StopHighlighting();
            return;
        }

        for(int i = 0; i < collidedBlocks.Length; i++)
        {
            Block block = collidedBlocks[i].GetComponent<Block>();
            if (block != null)
            {
                int rowId = block.RowId;
                int colId = block.ColumnId;

                if(!highlightRows.Contains(rowId))
                {
                    highlightRows.Add(rowId);
                    if (highlightRows.Count > 2)
                    {
                        while(highlightRows.Count > 2)
                        {
                            StopHighlightingRow(highlightRows[0]);
                            highlightRows.RemoveAt(0);
                        }
                    }
                } 

                if (!highlightColoum.Contains(colId))
                {
                    highlightColoum.Add(colId);
                    if (highlightColoum.Count > 2)
                    {
                        while (highlightColoum.Count > 2)
                        {
                            StopHighlightingColoum(highlightColoum[0]);
                            highlightColoum.RemoveAt(0);
                        }
                    }
                }
            }
        }

        foreach (int row in highlightRows)
        {
            HighlightingRow(row);
        }

        foreach (int col in highlightColoum)
        {
            HighlightingColoum(col);
        }
        canClear = true;
    }

    private void StopHighlightingRow(int rowId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
        {
            rowBlock.Reset();
        }
    }

    private void StopHighlightingColoum(int colId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntirColumn(colId))
        {
            rowBlock.Reset();
        }
    }

    private void HighlightingRow(int rowId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
        {
            rowBlock.Highlight();
        }
    }

    private void HighlightingColoum(int colId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntirColumn(colId))
        {
            rowBlock.Highlight();
        }
    }

    private void StopHighlighting()
    {
        foreach(int rowId in highlightRows)
        {
            StopHighlightingRow(rowId);
        }

        foreach (int colId in highlightColoum)
        {
            StopHighlightingColoum(colId);
        }
    }
}
