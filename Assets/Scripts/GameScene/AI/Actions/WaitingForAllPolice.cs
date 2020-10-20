
    public class WaitingForAllPolice : Action
    {
        public enum ReturnType : uint
        {
            NotFinished,
        };
    public WaitingForAllPolice(AI ai) : base(ai)
        {
        }

    public override uint PerformAction()
    {
        if (ai.CurrentState == State.WaitingForAllPolice)
        {
            return (uint) ReturnType.NotFinished;
        }

        PoliceController.Instance.ReportWaiting();
        ai.SetCurrentState(State.WaitingForAllPolice);
        return (uint) ReturnType.NotFinished;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
        {
            throw new System.NotImplementedException("why?");
        }
    }

