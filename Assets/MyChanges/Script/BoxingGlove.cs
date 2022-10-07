using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Hyperbyte;

public class BoxingGlove : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler, IDragHandler
{
    public Animator animator;
    public int maxRowClear;
    public Image gloveProgressBar;

    private bool canClear = false;
    private Vector2 initaialPosition;
    private List<int> highlightRows = new List<int>();

    private List<Block> colliderDisabledBlock = new List<Block>();


    private void Start()
    {
        initaialPosition = transform.localPosition;
    }

    private void Reset()
    {
        this.transform.localPosition = initaialPosition;
        this.transform.localScale = Vector3.one;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EnableAllCollider();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos = pos + new Vector2(-0.25f, 1);
        this.transform.position = pos;
        HighLightBlock();
    }

    public  void OnPointerUp(PointerEventData eventData)
    {
        gameObject.transform.position = new Vector3(-2.5f, transform.position.y , 0);

        DiableAllCollider();
        StopHighlighting();
        if (canClear)
        {
            StartCoroutine(PlayPunchAnimation());

            GamePlay.Instance.ClearRows(highlightRows);
            canClear = false;
            highlightRows.Clear();
        }
        else
        {
            Reset();
        }
    }
  
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Transform clickedObject = eventData.pointerCurrentRaycast.gameObject.transform;
            if (clickedObject == this.transform)
            {
                transform.localScale = Vector3.one * 1.5f;
                this.transform.position = this.transform.position + new Vector3(0, 1, 0);
            }
        }
    }

    IEnumerator PlayPunchAnimation()
    {
        animator.SetTrigger("Punch");

        yield return new WaitForSeconds(0.4f);

        Reset();
    }
    private void EnableAllCollider()
    {
        foreach (List<Block> row in GamePlay.Instance.allRows)
        {
            foreach (Block block in row)
            {
                //if (block.isFilled)
                //{
                    block.thisCollider.enabled = true;
                    //colliderDisabledBlock.Add(block);
                //}
            }
        }
    }

    private void DiableAllCollider()
    {
        /*
        foreach (Block block in colliderDisabledBlock)
        {
            block.thisCollider.enabled = false;
        }
        */

        foreach (List<Block> row in GamePlay.Instance.allRows)
        {
            foreach (Block block in row)
            {
                if (block.isFilled)
                {
                    block.thisCollider.enabled = false;
                    //colliderDisabledBlock.Add(block);
                }
            }
        }
    }

    private void HighLightBlock()
    {
        Collider2D[] collidedBlocks = Physics2D.OverlapBoxAll(this.transform.position, new Vector2(this.transform.localScale.x / 3, this.transform.localScale.y / 3), 0);
        if (collidedBlocks.Length <= 0)
        {
            canClear = false;
            StopHighlighting();
            return;
        }

        for (int i = 0; i < collidedBlocks.Length; i++)
        {
            Block block = collidedBlocks[i].GetComponent<Block>();
            if (block != null)
            {
                int rowId = block.RowId;

                if (!highlightRows.Contains(rowId))
                {
                    highlightRows.Add(rowId);
                    if (highlightRows.Count > maxRowClear)
                    {
                        while (highlightRows.Count > maxRowClear)
                        {
                            StopHighlightingRow(highlightRows[0]);
                            highlightRows.RemoveAt(0);
                        }
                    }
                }
            }
        }

        foreach (int row in highlightRows)
        {
            HighlightingRow(row);
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

    private void HighlightingRow(int rowId)
    {
        foreach (Block rowBlock in GamePlay.Instance.GetEntireRow(rowId))
        {
            rowBlock.Highlight();
        }
    }

    private void StopHighlighting()
    {
        foreach (int rowId in highlightRows)
        {
            StopHighlightingRow(rowId);
        }
    }
}
