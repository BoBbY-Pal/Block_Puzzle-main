using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Object/Level")]
public class LevelSO : ScriptableObject
{
    [Space(10)]
    public BlockShape BlockShape;

    [Space(10)]
    public Level[] Levels;
}

[System.Serializable]
public class Level
{
    [Space(5)]
    public Mode Mode;

    [Space(5)]
    public SpecialBlock[] SpecialBlockShape;

    [Space(5)]
    public BlockShapeInfo[] FixedBlockShape;

    [Space(5)]
    public LevelGoal[] Goal;

    [Space(5)]
    public Row[] rows;
}

[System.Serializable]
public class SpecialBlock
{
    public bool allowSpecialBlockShape;
    public int probability;
    public SpriteType spriteType;
}

[System.Serializable]
public class Mode
{
    public GameMode GameMode;
    public GameModeSettings GameModeSettings;
}

[System.Serializable]
public class Row
{
    [Space(10)]
    public BlockSprite[] coloum;
}

[System.Serializable]
public class BlockSprite
{
    [Space(10)]
    public SpriteType spriteType;
}

[System.Serializable]
public enum SpriteType
{
    Empty,
    MilkBottle,
    Ice,
    RedWithIce,
    Red,
    Hat,
    Bird,
    MagnetWithYellowAndBubble,
    Yellow,
    Magnet,
    Bubble,
    Panda,
    PandaLevel1
}

[System.Serializable]
public class BlockShape
{
    public BlockShapeInfo[] StandardBlockShape;
    public BlockShapeInfo[] AdvanceBlockShape;
}

[System.Serializable]
public class LevelGoal
{
    public SpriteType spriteType;
    public int target;
}


