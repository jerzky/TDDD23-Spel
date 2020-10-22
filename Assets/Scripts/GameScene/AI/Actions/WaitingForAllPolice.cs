
    public class WaitingForAllPolice : Action
    {
        private Police _police;

        public enum ReturnType : uint
        {
            NotFinished,
        };
    public WaitingForAllPolice(Police ai) : base(ai)
    {
        _police = ai;
    }

    public override uint PerformAction()
    {
        if (ai.CurrentState == State.WaitingForAllPolice)
        {
            return (uint) ReturnType.NotFinished;
        }

        _police.Car.ReportWaiting();
        ai.CurrentState = State.WaitingForAllPolice;
        return (uint) ReturnType.NotFinished;
    }

    public override ActionE GetNextAction(State currentState, uint lastActionReturnValue, AlertIntensity alertIntensity)
        {
            throw new System.NotImplementedException("why?");
        }
    }

