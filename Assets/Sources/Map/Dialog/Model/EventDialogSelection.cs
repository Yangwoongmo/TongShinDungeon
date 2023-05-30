using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDialogSelection
{
    private SelectionType type;
    private List<(string, string)> options;

    public EventDialogSelection(string type, List<(string, string)> options)
    {
        switch (type)
        {
            case "OPTION":
                this.type = SelectionType.OPTION;
                break;
            case "CONT":
                this.type = SelectionType.CONT;
                break;
        }

        this.options = new List<(string, string)>();
        this.options.AddRange(options);
    }

    public SelectionType GetSelectionType()
    {
        return type;
    }

    public List<(string, string)> GetOptions()
    {
        return options;
    }

    public enum SelectionType
    {
        OPTION,
        CONT
    }
}
