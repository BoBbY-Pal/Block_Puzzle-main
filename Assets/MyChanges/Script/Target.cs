using System;
using UnityEngine;
using UnityEngine.UI;
using Hyperbyte;
using TMPro;

public class Target : MonoBehaviour
{
    public SpriteType spriteType { get; private set; }
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Animator animator;
    [SerializeField] public Image image;
    private int target = 0;

    public void Initialize(int target, SpriteType spriteType)
    {
        this.spriteType = spriteType;
        this.target = target;
        UpdateSprite();
        UpdateTargetText();
        SetSpriteType(spriteType);
    }

    private void SetSpriteType(SpriteType spriteType)
    {
        switch(spriteType)
        {
            // case SpriteType.Ice:
            //     this.spriteType = SpriteType.RedWithIce;
            //     break;

            case SpriteType.Bird:
                this.spriteType = SpriteType.Hat;
                break;
            // case SpriteType.Panda:
            //     this.spriteType = SpriteType.PandaLevel1;
            //     break;
            default:
                this.spriteType = spriteType;
                break;
        }
    }

    private void UpdateSprite()
    {
        /*
        if(spriteType == SpriteType.Bird)
        {
            image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            spriteType = SpriteType.Hat;
            return;
        }
        */
        image.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
    }

    public async void UpdateTarget()
    {
        target--;
        target = Mathf.Max(0, target);
        UpdateTargetText();
        PlayShakeAnimation();
        if(target == 0 && spriteType == SpriteType.Hat)
        {
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(0.6f));
            GamePlay.Instance.ClearAllBlockHat();
        }
    }

    private void UpdateTargetText()
    {
        targetText.text = target.ToString();
    }

    private void PlayShakeAnimation()
    {
        animator.SetTrigger("Shake");
    }
}
