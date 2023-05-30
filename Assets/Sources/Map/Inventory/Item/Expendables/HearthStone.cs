using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthStone : ExpendableItem
{
    public HearthStone()
    {
        this.itemId = 5;
        this.itemName = "��ȯ��";
        this.itemDescription = "�������� ������ ���ư��ϴ�.";
        this.isPermanent = false;
        this.canUseFromInventory = true;
        this.price = 100;
        this.itemStock = 5;
        this.stockPrice = 0;
    }

    public override void UseItemFromInventory(ItemUseListener listener)
    {
        listener.GoToSanctuary();
    }
}
