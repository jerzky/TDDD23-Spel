using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionWorker : Civilian
{
    private RebuildObject _rebuildObject;
    private readonly Sprite[] _constructionSprites = new Sprite[4];
    private readonly Sprite[] _regularSprites = new Sprite[4];
    private const float TIME_UNTIL_START_WORKING = 120f;

    private readonly SimpleTimer _timer = new SimpleTimer(TIME_UNTIL_START_WORKING);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rebuildObject = new RebuildObject(this);
        Actions.Add(ActionE.Rebuild, _rebuildObject);
        _regularSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[12];
        _regularSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[13];
        _regularSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[14];
        _regularSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[15];
        _constructionSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[16];
        _constructionSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[17];
        _constructionSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[18];
        _constructionSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[19];

        _timer.Reset();
        AiType = AI_Type.Construction_Worker;
        foreach (var v in GameController.Instance.StackedUpBreakables)
            if (v.CompareTag("body"))
                AssignObject(v);

        GameController.Instance.StackedUpBreakables.Clear();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CurrentState == State.Construction)
            return;


        if (_rebuildObject.Objects.Count > 0 && !BuildingController.Instance.PlayerHostile && !PlayerController.Instance.IsHostile)
        {
            if (!_timer.TickAndReset())
                return;
            CurrentState = State.Construction;
            CurrentAction = ActionE.FollowPath;
            Sprites = _constructionSprites;
            _rebuildObject.Objects.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));
            Debug.Log("Walk to: " + _rebuildObject.Objects[0].transform.position);
            SetPathToPosition(_rebuildObject.Objects[0].transform.position);
        }

        if (CurrentState == State.Construction) 
            return;
        Sprites = _regularSprites;
        base.Update();
    }

    public override void OnDeath()
    {
        foreach (var v in _rebuildObject.Objects)
            GameController.Instance.AddBreakable(v);
        base.OnDeath();
    }

    public void AssignObject(GameObject obj)
    {
        if (!obj.CompareTag("broken")) 
            return;
        obj.tag = "brokenandreported";
        _rebuildObject.Objects.Add(obj);
    }
}
