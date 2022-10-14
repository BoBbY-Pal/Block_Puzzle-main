using System.Collections;
using Hyperbyte;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public Block currentBlock;
    private int rowSize;
    private int columnSize;
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
        rowSize = (int)boardSize;
        columnSize = (int)boardSize;
        StartCoroutine(Drop());
    }

    public void Initialize(Block block)
    {
        currentBlock = block;
        PlaceDiamond();
    }
    
    private void CheckDiamondCanBeDrop()
    {
        StartCoroutine(Drop());
    }

    private IEnumerator Drop()
    {
        var row = GamePlay.Instance.allRows[currentBlock.RowId+1];
        Block block = row[currentBlock.ColumnId];
        if (block.isFilled)
        {
            yield break;
        }

        float time = 0;
        while (time < 0.20f)
        {
            yield return new WaitForSeconds(0.01f);
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, block.transform.position, time);
        }
        
        ClearDiamond();
        currentBlock = block;
        PlaceDiamond();
        

        if (currentBlock.RowId == rowSize-1)
        {
            TargetController.Instance.UpdateTargetText(currentBlock, SpriteType.Diamond);
            ClearDiamond();
            Destroy(gameObject);
            yield break;
        }

        StartCoroutine(Drop());
    }
    
    private void PlaceDiamond()
    {
        currentBlock.thisCollider.enabled = false;
        currentBlock.isFilled = false;
        currentBlock.isAvailable = false; 
        currentBlock.hasDiamond = true;
        currentBlock.assignedSpriteTag = SpriteType.Diamond.ToString();
    }

    private void ClearDiamond()
    {
        currentBlock.thisCollider.enabled = true;
        currentBlock.isFilled = false;
        currentBlock.isAvailable = true; 
        currentBlock.hasDiamond = false;
        currentBlock.assignedSpriteTag = currentBlock.defaultSpriteTag;
        currentBlock.SetBlockSpriteType(SpriteType.Empty);
    }
}