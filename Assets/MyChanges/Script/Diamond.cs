using System;
using System.Collections;
using Hyperbyte;
using UnityEngine;
using UnityEngine.UI;


namespace MyChanges.Script
{
    public class Diamond : MonoBehaviour
    {
        // private int _columnId;
        // int _rowId;

        public Block currentBlock;
        public bool canDrop;
        private void OnEnable()
        {
            GamePlayUI.Instance.OnRowCompletedEvent += CheckDiamondCanBeDrop;
        }
        private void OnDisable()
        {
            GamePlayUI.Instance.OnRowCompletedEvent -= CheckDiamondCanBeDrop;
        }
        private void Start()
        {
            BoardSize boardSize = GamePlayUI.Instance.GetBoardSize();

            int rowSize = (int)boardSize;
            int columnSize = (int)boardSize;
        }

        private void Update()
        {

            if (!GamePlay.Instance.isBoardReady)
            {
                return;
            }
            else
            {
                Block block = GetHittingBlock(transform);

                if (block != null )
                {

                    StartCoroutine(Drop(block));
                }
            }
        }
        
        Block GetHittingBlock(Transform draggingBlock)
        {
            RaycastHit2D hit = Physics2D.Raycast(draggingBlock.position, Vector2.down, .5f);
            
            if (hit.collider != null && hit.collider.GetComponent<Block>() != null)
            {
                return hit.collider.GetComponent<Block>();
            }
            return null;
        }
        public void Initialize(Block block)
        {
            // _columnId = columnId;
            // _rowId = rowId;
            currentBlock = block;
        }
        
        private void CheckDiamondCanBeDrop()
        {
            canDrop = true;
            // var row = GamePlay.Instance.allRows[currentBlock.RowId+1];
            //     
            //     Block block = row[currentBlock.ColumnId];
            //     if (block.isFilled == false)
            //     {
            //         Drop(block);
            //     }
            //     
            //     
            //
            //     return;

        }

        private bool CanDrop()
        {
            var row = GamePlay.Instance.allRows[currentBlock.RowId+1];
                
            Block block = row[currentBlock.ColumnId];
            if (block.isFilled == false) 
            { 
                // Drop(block);
                return true;
            }
                
                
                
            return false;
        }

        private IEnumerator Drop(Block block)
        {
            canDrop = false;
            float time = 0;
            
            while (time < 0.20f)
            {
                yield return new WaitForSeconds(0.1f);
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, block.transform.position, time);
            }
            
            currentBlock.hasDiamond = false;
            currentBlock.ClearDiamond();
            currentBlock = block;
            currentBlock.PlaceDiamond();
            // canDrop = CanDrop();
            
           
            
            // transform.position = block.transform.position;
        }
    }
}