public class DT_Node
{
    public virtual DT_Node GetBranch() { return null; }
    public virtual DT_Node MakeDecision() { return null; }
}

public class DT_Action : DT_Node
{
    public DT_Action_Init init;
    public DT_Action_Update update;
    public DT_Action_End end;

    public DT_Action(DT_Action_Init init, DT_Action_Update update, DT_Action_End end)
    {
        this.init = init;
        this.update = update;
        this.end = end;
    }

    public override DT_Node MakeDecision()
    {
        return this;
    }
}

public class DT_Decision : DT_Node
{
    DT_Decision_Func decisionFunction;
    DT_Node trueNode, falseNode;

    public DT_Decision(DT_Decision_Func decisionFunction, DT_Node trueNode, DT_Node falseNode)
    {
        this.decisionFunction = decisionFunction;
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public override DT_Node GetBranch()
    {
        return decisionFunction() ? trueNode : falseNode;
        /*if (decisionFunction() == true)
            return trueNode;
        else
            return falseNode;*/
    }

    public override DT_Node MakeDecision()
    {
        DT_Node branch = GetBranch();
        return branch.MakeDecision();
    }
}

public delegate void DT_Action_Init();
public delegate void DT_Action_Update();
public delegate void DT_Action_End();
public delegate bool DT_Decision_Func();