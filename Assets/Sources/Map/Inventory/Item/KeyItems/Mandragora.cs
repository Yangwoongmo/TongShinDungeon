using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mandragora : StewIngredient
{
    public Mandragora()
    {
        this.itemId = 101;
        this.itemName = "만드라고라";
        this.itemDescription = "맨드레이크라고도 하며 무려 가지과 식물이다.";
        this.isPermanent = true;
        this.isPlantType = true;
        this.price = 250;
        this.recipes.recipeNames = new string[2] { "뿌리만 넣기" , "통으로 넣기" };
        this.recipes.recipeDescriptions = new string[2] { "적의 급소가 더 잘 보이게 된다", "전사의 검술이 노련해진다" };
        this.recipes.effectTexts = new string[2] { "기력 데미지 +20%", "튕겨내기 기력 데미지 50% 증가" };
    }

    public override void UpdatePlayerStatus(Player player, int selectIndex = 0)
    {
        if (selectIndex == 0)
        {
            int playerDex = player.GetDexterity();
            playerDex += 2;
            player.SetDexterity(playerDex);
            
        }
        else if (selectIndex == 1)
        {
            float playerPP = player.GetPlayerPpDamageMultiplier();
            playerPP *= 1.5f;
            player.SetPlayerPpDamageMultiplier(playerPP);
        }

        player.UseItem(itemId);
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        // Not Implemented Yet
    }
}
