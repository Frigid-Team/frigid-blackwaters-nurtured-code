using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ItemEffectsGridSlot : FrigidMonoBehaviour
    {
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private List<Image> borderImages;
        [SerializeField]
        private List<Text> countTexts;

        private FrigidCoroutine fillRoutine;

        public void Populate(ItemStorable storable, Item item)
        {
            this.iconImage.sprite = storable.Icon;
            IEnumerator<FrigidCoroutine.Delay> FillOut()
            {
                while (true)
                {
                    List<AbilityResource> activeAbilityResources = item.ActiveAbilityResources;
                    for (int i = 0; i < this.borderImages.Count; i++)
                    {
                        this.borderImages[i].color = storable.AccentColor * new Color(Mathf.Pow(Color.gray.r, i), Mathf.Pow(Color.gray.g, i), Mathf.Pow(Color.gray.b, i), 1f) * (item.InEffect ? Color.white : Color.gray);
                        this.borderImages[i].enabled = activeAbilityResources.Count > i;
                        if (this.borderImages[i].enabled)
                        {
                            this.borderImages[i].fillAmount = activeAbilityResources[i].Progress;
                        }
                    }
                    for (int i = 0; i < this.countTexts.Count; i++)
                    {
                        this.countTexts[i].color = item.InEffect ? Color.white : Color.gray;
                        this.countTexts[i].enabled = activeAbilityResources.Count > i && activeAbilityResources[i].Quantity >= 0;
                        if (this.countTexts[i].enabled)
                        {
                            this.countTexts[i].text = activeAbilityResources[i].Quantity.ToString();
                        }
                    }
                    this.iconImage.color = item.InEffect ? Color.white : Color.gray;
                    yield return null;
                }
            }
            FrigidCoroutine.Kill(this.fillRoutine);
            this.fillRoutine = FrigidCoroutine.Run(FillOut(), this.gameObject);
        }
    }
}
