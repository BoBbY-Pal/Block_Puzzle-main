// ©2019 - 2020 HYPERBYTE STUDIOS LLP
// All rights reserved
// Redistribution of this software is strictly not allowed.
// Copy of this software can be obtained from unity asset store only.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Hyperbyte.Utils;
using Hyperbyte.UITween;
using MyChanges.Script;
using Unity.VisualScripting;

namespace Hyperbyte
{
    /// <summary>
    /// This class component is attached to all blocks in the grid. 
    /// </summary>
	public class Block : MonoBehaviour
    {

        // Returns rowId 
        public int RowId
        {
            get
            {
                return _rowId;
            }
            private set
            {
                _rowId = value;
            }
        }

        //Returns columnId
        public int ColumnId
        {
            get
            {
                return _columnId;
            }
            private set
            {
                _columnId = value;
            }
        }

        // Represents row id of block in the grid.
        private int _rowId;

        // Represents columnId id of block in the grid.
        private int _columnId;

        // Block is filled  with current playing block shape.
        [System.NonSerialized] public bool isFilled = false;

        // Block is available to place block shape or not.
        [System.NonSerialized] public bool isAvailable = true;

        #region Blast Mode Specific
        // Whether Block contains bomb. Applied only to time mode.
        [System.NonSerialized] public bool isBomb = false;
        
        // Instance on bomb on the block. 
        [System.NonSerialized] public Bomb thisBomb = null;
        #endregion

        // Whether Block contains diamond.
        [System.NonSerialized] public bool hasDiamond = false;
        
        // Default sprite tag on the block. Will update runtime.
        public string defaultSpriteTag;

        // Sprite that is assigned on the block. Will update runtime.
        public string assignedSpriteTag;

        //Default sprite that is assigned to block.
        Sprite defaultSprite;

        // Box collide attached to this block.
        public BoxCollider2D thisCollider { get; private set; }

        #pragma warning disable 0649
        // Image component on the block. Assigned from Inspector.
        [SerializeField] public Image blockImage;
        [SerializeField] Image blockLayerImage1;
        [SerializeField] Image blockLayerImage2;
        [SerializeField] Image highlightLayer;
        #pragma warning restore 0649

        public SpriteType spriteType { get; private set; }
        public bool hasStages = false;
        public int stage = 0;
        /// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake()
        {
            /// Initializes the collider component on Awake.
            thisCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Assignes logical position on block on the grid.
        /// </summary>
        public void SetBlockLocation(int rowIndex, int columnIndex)
        {
            RowId = rowIndex;
            ColumnId = columnIndex;
        }

        /// <summary>
        /// Highlights block with given sprite.
        /// </summary>
        public void Highlight(Sprite sprite)
        {
            blockImage.sprite = sprite;
            blockImage.enabled = true;
            isFilled = true;
        }

        // BombPowerUp will call
        public void Highlight()
        {
            highlightLayer.enabled = true;
        }

        /// <summary>
        /// Resets block to its default state.
        /// </summary>
        public void Reset()
        {
            if (!isAvailable)
            {
                blockImage.sprite = defaultSprite;
                isFilled = true;
            }
            else
            {
                blockImage.enabled = false;
                isFilled = false;
            }
            highlightLayer.enabled = false;
        }

        public void PlaceDiamond()
        {
            thisCollider.enabled = false;
            isFilled = false;
            isAvailable = false; 
            hasDiamond = true;
            Debug.Log(name);
        }
        
        /// <summary>
        /// Places block from the block shape. Typically will be called during gameplay.
        /// </summary>
        public void PlaceBlock(Sprite sprite, string spriteTag)
        {
            thisCollider.enabled = false;
            blockImage.enabled = true;
            blockImage.sprite = sprite;
            blockImage.color = blockImage.color.WithNewA(1);
            defaultSprite = sprite;
            isFilled = true;
            isAvailable = false;
            assignedSpriteTag = spriteTag;
        }


        #region My changes
        // will be called when Magnet containing blockshape placed on grid
        public void PlaceBlock(Sprite sprite, string spriteTag, SpriteType spriteType)
        {
            //if(blockLayerImage2.enabled == false)
            //{
            blockLayerImage2.enabled = true;
            blockLayerImage2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            blockLayerImage2.color = blockImage.color.WithNewA(1);
            if(this.spriteType != SpriteType.MagnetWithYellowAndBubble)
            {
                if(this.spriteType == SpriteType.Bubble)
                {
                    this.spriteType = SpriteType.MagnetWithYellowAndBubble;
                }
                else
                {
                    this.spriteType = spriteType;
                }
            }
            Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
            //}
            PlaceBlock(sprite, spriteTag);
        }
        public void ChangeBlockImage()
        {
            blockImage.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
            blockImage.color = blockImage.color.WithNewA(1);
        }
        #endregion
        /// <summary>
        /// Places block from the block shape. Typically will be called when game starting with progress from previos session.
        /// </summary>
        public void PlaceBlock(string spriteTag)
        {
            Sprite sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteTag);
            thisCollider.enabled = false;
            blockImage.enabled = true;
            blockImage.sprite = sprite;
            blockImage.color = blockImage.color.WithNewA(1);
            defaultSprite = sprite;
            isFilled = true;
            isAvailable = false;
            assignedSpriteTag = spriteTag;
        }

        public void ClearDiamond()
        {
            thisCollider.enabled = true;
            isFilled = true;
            isAvailable = true; 
            hasDiamond = false;
        }
        
        /// <summary>
        /// Clears block. Will be called when line containing this block will get completed. This is typical animation effect of how completed block shoudl disappear.
        /// references - 159, 166, 500 (default code)
        /// </summary>
        public void Clear()
        {
            if (hasDiamond)
            {
                return;
            }
            
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            
            // BlockImage will scale down to 0 in 0.35 seconds. and will reset to scale 1 on animation completion.
            UIFeedback.Instance.PlayHapticLight();
            blockImage.transform.LocalScale(Vector3.zero, 0.35F).OnComplete(() =>
            {
                blockImage.transform.localScale = Vector3.one;
                blockImage.sprite = null;
            });

            blockImage.transform.LocalRotationToZ(90, 0.2F).OnComplete(()=> {
                blockImage.transform.localEulerAngles = Vector3.zero;
            });

            transform.GetComponent<Image>().SetAlpha(1, 0.35F).SetDelay(0.3F);

            // Opacity of block image will set to 0 in 0.3 seconds. and will reset to 1 on animation completion.
            blockImage.SetAlpha(0.5F, 0.3F).OnComplete(() =>
            {
                blockImage.enabled = false;
            });


            isFilled = false;
            isAvailable = true;
            thisCollider.enabled = true;
            assignedSpriteTag = defaultSpriteTag;
            SetBlockSpriteType(SpriteType.Empty);

            #region Blast Mode Specific
            if (isBomb)
            {
                isBomb = false;
                ClearBomb();
            }
            #endregion
        }

        #region Blast Mode Specific
        /// <summary>
        /// Place a new bomb on the block.
        /// </summary>
        public void PlaceBomb(int remainCounter)
        {
            GameObject bomb = (GameObject)Instantiate(GamePlay.Instance.bombTemplate);
            thisCollider.enabled = false;
            bomb.transform.SetParent(blockImage.transform);
            bomb.transform.localScale = Vector3.zero;
            bomb.transform.localPosition = Vector3.zero;
            thisBomb = bomb.GetComponent<Bomb>();
            thisBomb.SetCounter(remainCounter);
            bomb.SetActive(true);
            bomb.transform.LocalScale(Vector3.one, 0.25F);
            isFilled = true;
            isAvailable = false;
            isBomb = true;
        }

        /// <summary>
        /// Remove attached bomb from the block. Will be executes when block gets cleared on completing line.
        /// </summary>
        void ClearBomb()
        {
            if (blockImage.transform.childCount > 0)
            {
                UIFeedback.Instance.PlayHapticHeavy();
                thisBomb.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
                {
                    Destroy(thisBomb.gameObject);
                    thisBomb = null;
                });
            }
        }

        /// <summary>
        /// Remove attached bomb from the block. Will be executes when block gets cleared on completing line.
        /// </summary>
        public void ClearBombExplicitly()
        {
            isFilled = false;
            isAvailable = true;
            thisCollider.enabled = true;
            assignedSpriteTag = defaultSpriteTag;
            isBomb = false;

            if (blockImage.transform.childCount > 0)
            {
                UIFeedback.Instance.PlayHapticHeavy();
                thisBomb.transform.LocalScale(Vector3.zero, 0.25F).OnComplete(() =>
                {
                    Destroy(thisBomb.gameObject);
                    thisBomb = null;
                });
            }
        }

        /// <summary>
        /// Returns Remaining counter on bomb attached to block.static Applies only to blast mode.
        /// </summary>
        public int GetRemainingCounter()
        {
            if (thisBomb != null)
            {
                return thisBomb.remainingCounter;
            }
            return GamePlayUI.Instance.blastModeCounter;
        }
        #endregion

        public void ClearBlock()
        {
            switch(spriteType)
            {
                case SpriteType.RedWithIce:
                    blockLayerImage1.enabled = false;
                    TargetController.Instance.UpdateTargetText(this, spriteType);
                    PlaceBlock(SpriteType.Red.ToString());
                    SetBlockSpriteType(SpriteType.Red);
                    break;

                case SpriteType.Bubble:
                    blockLayerImage1.enabled = false;
                    TargetController.Instance.UpdateTargetText(this, spriteType);
                    Clear();
                    break;

                case SpriteType.Hat:
                    TargetController.Instance.UpdateTargetText(this, spriteType);
                    PlaceBlock(SpriteType.Hat.ToString());
                    break;

                case SpriteType.Magnet:
                    blockLayerImage2.enabled = false;
                    TargetController.Instance.UpdateTargetText(this, SpriteType.Magnet);
                    Clear();
                    break;

                case SpriteType.MagnetWithYellowAndBubble:
                    blockLayerImage2.enabled = false;
                    blockLayerImage1.enabled = false;
                    TargetController.Instance.UpdateTargetText(this, SpriteType.Bubble);
                    Clear();
                    break;

                case SpriteType.MagnetWithIceAndRed:
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            Sprite blockImgSprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString());
                            blockImage.sprite = blockImgSprite;
                            defaultSprite = blockImgSprite;
                            blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice + "Stage" + stage);
                            assignedSpriteTag = spriteType + "Stage" + stage;
                        }
                        else
                        {
                            TargetController.Instance.UpdateTargetText(this, SpriteType.Ice);
                            hasStages = false;
                            stage = 0;
                            blockLayerImage1.enabled = false;
                            Sprite blockImgSprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString());
                            blockImage.sprite = blockImgSprite;
                            defaultSprite = blockImgSprite;
                            spriteType = SpriteType.Magnet;
                            Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
                            assignedSpriteTag = spriteType.ToString();
                        }
                    }
                    else
                    {
                        TargetController.Instance.UpdateTargetText(this, SpriteType.Ice);
                        blockLayerImage1.enabled = false;
                        Sprite blockImgSprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString());
                        blockImage.sprite = blockImgSprite;
                        defaultSprite = blockImgSprite;
                        spriteType = SpriteType.Magnet;
                        Magnet.Instance.CheckForRowOrColoumClear(_rowId, _columnId);
                        assignedSpriteTag = spriteType.ToString();
                    }
                    break;
            
                case SpriteType.Diamond:
                    hasDiamond = true;
                    PlaceBlock(spriteType.ToString());
                    break;
                
                default:
                    if (hasStages)
                    {
                        if (stage > 1)
                        {
                            --stage;
                            Sprite sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType + "Stage" + stage);
                            blockImage.sprite = sprite;
                            defaultSprite = sprite;
                            assignedSpriteTag = spriteType + "Stage" + stage;
                        }
                        else
                        {
                            TargetController.Instance.UpdateTargetText(this, spriteType);
                            hasStages = false;
                            stage = 0;
                            Clear();
                        }
                    }
                    else
                    {
                        TargetController.Instance.UpdateTargetText(this, spriteType);
                        Clear();
                    }
                    break;
            }
        }

        // gets called during board generation
        public void SetBlock(SpriteType spriteType, bool hasStages, int stage)
        {
            this.spriteType = spriteType;
            this.hasStages = hasStages;
            this.stage = stage;
            
            switch(spriteType)
            {
                case SpriteType.Bubble:
                    blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString());
                    blockLayerImage1.enabled = true;
                    blockLayerImage1.color = blockImage.color.WithNewA(1);
                    break;

                case SpriteType.RedWithIce:
                    blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString());
                    blockLayerImage1.enabled = true;
                    blockLayerImage1.color = blockImage.color.WithNewA(1);
                    PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString()), spriteType.ToString());
                    break;

                case SpriteType.MagnetWithYellowAndBubble:
                    blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Bubble.ToString());
                    blockLayerImage2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                    blockLayerImage1.color = blockImage.color.WithNewA(1);
                    blockLayerImage2.color = blockImage.color.WithNewA(1);
                    blockLayerImage2.enabled = true;
                    blockLayerImage1.enabled = true;
                    PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Yellow.ToString()), spriteType.ToString());
                    break;
                
                case SpriteType.MagnetWithIceAndRed:
                    if (hasStages)
                    {
                        blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString() + "Stage" + this.stage);
                        blockLayerImage2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                        blockLayerImage1.color = blockImage.color.WithNewA(1);
                        blockLayerImage2.color = blockImage.color.WithNewA(1);
                        blockLayerImage2.enabled = true;
                        blockLayerImage1.enabled = true;
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString()), spriteType.ToString());
                    }
                    else
                    {
                        blockLayerImage1.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Ice.ToString());
                        blockLayerImage2.sprite = ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Magnet.ToString());
                        blockLayerImage1.color = blockImage.color.WithNewA(1);
                        blockLayerImage2.color = blockImage.color.WithNewA(1);
                        blockLayerImage2.enabled = true;
                        blockLayerImage1.enabled = true;
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(SpriteType.Red.ToString()), spriteType.ToString());
                    }
                    break;
                
                case SpriteType.Diamond:
                    GameObject diamondObject = Instantiate(GamePlay.Instance.diamondPrefab, gameObject.transform.position,
                        Quaternion.identity, GamePlay.Instance.boardGenerator.transform);
                    
                    // diamondObject.transform.localScale = Vector3.one;
                    Diamond diamond = diamondObject.GetComponent<Diamond>();
                
                    diamond.Initialize(this);
                    // diamond.enabled = false;
                    PlaceDiamond();
                    break;
                // case SpriteType.Panda:
                //     if (hasStages)
                //     {
                //         this.hasStages = hasStages;
                //         this.stage = stage;
                //         PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()+ "Stage" + this.stage),
                //                 spriteType.ToString()+ "Stage" + this.stage);
                //     }
                //     else
                //     {
                //         PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()), spriteType.ToString());
                //     }
                //     break;
                
                default:
                    if (hasStages)
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()+ "Stage" + this.stage),
                            spriteType.ToString()+ "Stage" + this.stage);
                    }
                    else
                    {
                        PlaceBlock(ThemeManager.Instance.GetBlockSpriteWithTag(spriteType.ToString()), spriteType.ToString());
                    }
                    break;
            }
        }

        public void SetBlockSpriteType(SpriteType spriteType)
        {
            this.spriteType = spriteType;
        }
    }
}
