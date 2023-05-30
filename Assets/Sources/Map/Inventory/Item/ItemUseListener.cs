using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ItemUseListener
{
    public void HealPlayer(int heal, bool isPercentage, bool withCoefficient);

    public void GoToSanctuary();
}
