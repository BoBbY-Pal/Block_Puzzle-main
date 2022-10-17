using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hyperbyte
{
    public class MilkShop: MonoBehaviour
    {
        [Tooltip("Delay time for bottle destruction.")]
        [SerializeField] private float destroyTime;
        
        // bottles that milk shop contains.
        public List<GameObject> bottles;
        
        // Grid's blocks that are occupied by the milk shop.
        public List<Block> occupiedBlocks;
        
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
        public IEnumerator MarkOccupiedBlocksUnAvail(Block rootBlock)
        {
            yield return new WaitForSeconds(.5f);
            
            List<Block> row = new List<Block>();
            row = GamePlay.Instance.allRows[rootBlock.RowId];
            List<Block> nextRow = new List<Block>();
            nextRow = GamePlay.Instance.allRows[rootBlock.RowId +1];

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
                    block.thisCollider.enabled = false;
                    block.isAvailable = false;
                    block.isFilled = true;
                    block.milkShop = this;
                    block.SetBlockSpriteType(SpriteType.MilkBottle);
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
                    block.Clear();
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
            
            Destroy(bottle, destroyTime);
            TargetController.Instance.UpdateTargetText(bottle.transform, SpriteType.MilkBottle);
            if (bottles.Count <= 0)
            {
                MarkOccupiedBlocksAvail();
                Destroy(gameObject, .2f);
            }
        }
    }
}