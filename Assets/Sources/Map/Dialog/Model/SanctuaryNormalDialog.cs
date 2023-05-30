using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanctuaryNormalDialog
{
    private string id;
    private List<DialogModel> sequence;

    public SanctuaryNormalDialog(string id, List<DialogModel> list)
    {
        this.id = id;
        sequence = new List<DialogModel>();
        sequence.AddRange(list);
    }

    public string GetId()
    {
        return id;
    }

    public (DialogModel, int) GetCurrentDialog(int currentSequence)
    {
        DialogModel result = sequence[currentSequence];
        int nextSequence = currentSequence + 1;
        
        if (nextSequence >= sequence.Count)
        {
            nextSequence = 0;
        }

        return (result, nextSequence);
    }
}
