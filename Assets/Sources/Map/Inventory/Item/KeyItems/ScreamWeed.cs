using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreamWeed : StewIngredient
{
    public ScreamWeed()
    {
        this.itemId = 102;
        this.itemName = "비명초";
        this.itemDescription = "던파에 나오는 식물로 뽑으면 비명을 지릅니다.";
        this.isPermanent = true;
        this.isPlantType = true;
        this.recipes.recipeNames = new string[2] { "말리기", "데치기" };
        this.recipes.recipeDescriptions = new string[2] { "마법이 자장가처럼 부드러워진다", "마법이 행진곡처럼 시끄러워진다" };
        this.recipes.effectTexts = new string[2] { "기절한 적에게 마법 피해 15% 증가", "꺠어있는 적에게 마법 피해 25% 증가" };
    }

    public override void UpdatePlayerStatus(Player player, int selectIndex = 0)
    {
        if (selectIndex == 0)
        {
            player.SetStunMagicDamageAddition(0.15f);
        }
        else if(selectIndex == 1)
        {
            player.SetAwakeMagicDamageAddition(0.25f);
        }
        player.UseItem(itemId);
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Not Implemented Yet
    }
}
