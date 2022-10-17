using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Hyperbyte;

public class TargetController : Singleton<TargetController>
{
    [SerializeField] private Target targetPrefab;
    [SerializeField] private GameObject destroyingBlockPrefab;
    [SerializeField] private Transform particleParent;
    [SerializeField] private Transform destroyingBlockParent;
    public ParticleSystem particle;
    private List<Target> targetInstanceHolder = new List<Target>();

    public void GenerateTargetInstances(LevelGoal[] goals)
    {
        for (int i = 0; i < goals.Length; i++)
        {
            Target target = Instantiate<Target>(targetPrefab, gameObject.transform);
            target.Initialize(goals[i].target, goals[i].spriteType);
            targetInstanceHolder.Add(target);
        }
    }

    public void UpdateTargetText(Transform block, SpriteType spriteType)
    {
        foreach(Target target in targetInstanceHolder)
        {
            if(spriteType == target.spriteType)
            {
                SpwanDestroyingBlockInstance(block.gameObject.transform, target, spriteType);
                break;
            }
        }
        
        /*
        if(!block.isAvailable || spriteType == SpriteType.Bubble)
        {
            PlayBlockBreakEffect(block.gameObject.transform);
        }
        */
        // if (block.blockImage.enabled == true)
        // {
        //     PlayBlockBreakEffect(block.gameObject.transform);
        // }
    }

    private void PlayBlockBreakEffect(Transform block)
    {
        ParticleSystem blockBreakParticle = GameObject.Instantiate(particle, block.position, Quaternion.identity, particleParent);
        blockBreakParticle.textureSheetAnimation.SetSprite(0, block.gameObject.GetComponent<Block>().blockImage.sprite);
        blockBreakParticle.Play();
        Destroy(blockBreakParticle.gameObject, 2);
    }

    private void SpwanDestroyingBlockInstance(Transform destroyingBlock, Target target, SpriteType spriteType)
    {
        GameObject block = Instantiate(destroyingBlockPrefab, destroyingBlock.position, Quaternion.identity, destroyingBlockParent);

        if(spriteType == SpriteType.Hat)
        {
            block.GetComponent<Image>().sprite = target.image.sprite;
            block.transform.localScale = Vector3.one * 1.5f;
            Animator animator = block.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetTrigger("Fly");
        }
        else
        {
            //block.GetComponent<Image>().sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            block.GetComponent<Image>().sprite = target.image.sprite;
        }

        MoveTowardsTarget(block.transform, target);
    }

    private async void MoveTowardsTarget(Transform destroyingBlock, Target target)
    {
        float time = 0;
        while (time < 0.45)
        {
            await Task.Delay(System.TimeSpan.FromSeconds(0.05f));

            time += Time.deltaTime;
            destroyingBlock.position = Vector3.Lerp(destroyingBlock.position, target.gameObject.transform.position, time);
        }

        target.UpdateTarget();
        Destroy(destroyingBlock.gameObject);
    }
}
