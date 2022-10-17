using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperbyte
{
    public class MilkShop: MonoBehaviour
    {
        // bottles that milk shop contains.
        public List<GameObject> bottles;
        
        // Grid's blocks that are occupied by the milk shop.
        private List<Block> occupiedBlocks;
        
        //Event action for milk bottle callback.
        public event Action OnMilkShopFoundEvent;
        
        private void OnEnable()
        {
            OnMilkShopFoundEvent += CollectMilkBottle;
        }

        private void OnDisable()
        {
            OnMilkShopFoundEvent -= CollectMilkBottle;
            
        }

        // Blocks that are occupied by the milk shop will be marked as unavailable,
        // so that they can't be used for shape placing. 
        public void MarkOccupiedBlocksUnAvail(Block rootBlock)
        {
            List<Block> row = new List<Block>();
            row = GamePlay.Instance.allRows[rootBlock.RowId];
            List<Block> nextRow = new List<Block>();
            nextRow = GamePlay.Instance.allRows[rootBlock.RowId +1];
            BoardGenerator.rowMilkShopData = new Dictionary<int, MilkShop>();
            BoardGenerator.columnMilkShopData = new Dictionary<int, MilkShop>(); 
            
            occupiedBlocks = new List<Block>
            {
                rootBlock,
                row[rootBlock.ColumnId + 1],
                nextRow[rootBlock.ColumnId],
                nextRow[rootBlock.ColumnId + 1]
            };
            
            if(occupiedBlocks.Count > 0)
            {
                foreach (Block block in occupiedBlocks)
                {
                    block.GetComponent<Collider2D>().enabled = false;
                    block.isAvailable = false;
                    block.isFilled = true;
                    block.hasMilkShop = true;

                    if (!BoardGenerator.rowMilkShopData.ContainsKey(block.RowId) &&
                        !BoardGenerator.columnMilkShopData.ContainsKey(block.ColumnId))
                    {
                        BoardGenerator.rowMilkShopData.Add(block.RowId, this);
                        BoardGenerator.columnMilkShopData.Add(block.ColumnId, this);
                    }
                    
                }
            }
        }
        
        // Blocks that are occupied by the milk shop will be marked as available,
        // so that they can be used for shape placing. 
        private void MarkOccupiedBlocksAvail()
        {
            if(occupiedBlocks.Count > 0)
            {
                foreach (Block block in occupiedBlocks)
                {
                    block.GetComponent<Collider2D>().enabled = true;
                    block.isAvailable = true;
                    block.isFilled = false;
                    block.hasMilkShop = false;

                    if (BoardGenerator.rowMilkShopData.ContainsKey(block.RowId) &&
                        BoardGenerator.columnMilkShopData.ContainsKey(block.ColumnId))
                    {
                        
                        BoardGenerator.rowMilkShopData.Remove(block.RowId);
                        BoardGenerator.columnMilkShopData.Remove(block.ColumnId);
                    }
                }
            }
        }
        
        /// <summary>
        /// Invokes callback for OnMilkShopFound Event.
        /// </summary>
        public void OnMilkShopFound()
        {
            OnMilkShopFoundEvent?.Invoke();
        }
        
        // One milk bottle will be removed from milk shop when line completes.
        private void CollectMilkBottle()
        {
            GameObject bottle = bottles[0];
            bottles.RemoveAt(0); 
            
            Destroy(bottle, .3f);
            TargetController.Instance.UpdateTargetText(bottle.transform, SpriteType.MilkBottle);
            if (bottles.Count <= 0)
            {
                MarkOccupiedBlocksAvail();
                Destroy(gameObject, .2f);
            }
        }
    }
}