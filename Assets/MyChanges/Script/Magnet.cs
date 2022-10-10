using Hyperbyte;
using DigitalRuby.LightningBolt;

public class Magnet : Singleton<Magnet>
{
    public LightningBoltScript lightningBoltPrefab;
    public void CheckForRowOrColoumClear(int rowId, int coloumId)
    {
        if(CanRowClear(rowId) && CanColoumClear(coloumId))
        {
            GamePlay.Instance.ClearRow(rowId);
            GamePlay.Instance.ClearColoum(coloumId);
        }
        else if(CanRowClear(rowId))
        {
            GamePlay.Instance.ClearRow(rowId);
        }
        else if(CanColoumClear(coloumId))
        {
            GamePlay.Instance.ClearColoum(coloumId);
        }
    }

    private bool CanRowClear(int rowID)
    {
        int count = 0;
        foreach (Block block in GamePlay.Instance.GetEntireRow(rowID))
        {
            if (block.spriteType == SpriteType.MagnetWithYellowAndBubble || block.spriteType == SpriteType.Magnet)
            {
                count++;
                if(count >= 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanColoumClear(int coloumID)
    {
        int count = 0;
        foreach (Block block in GamePlay.Instance.GetEntirColumn(coloumID))
        {
            if (block.spriteType == SpriteType.MagnetWithYellowAndBubble || block.spriteType == SpriteType.Magnet)
            {
                count++;
                if (count >= 2)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
