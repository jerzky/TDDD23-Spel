using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public abstract class Action
{
    protected AI ai;
    public Action(AI ai)
    {
        this.ai = ai;
    }

    public abstract bool PerformAction();
}
