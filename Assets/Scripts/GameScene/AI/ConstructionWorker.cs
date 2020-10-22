using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionWorker : Civilian
{
    RebuildObject rebuildObject;
    Sprite[] constructionSprites = new Sprite[4];
    Sprite[] regularSprites = new Sprite[4];
    const float TIME_UNTIL_START_WORKING = 120f;
    SimpleTimer timer = new SimpleTimer(TIME_UNTIL_START_WORKING);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rebuildObject = new RebuildObject(this);
        Actions.Add(ActionE.Rebuild, rebuildObject);
        regularSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[12];
        regularSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[13];
        regularSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[14];
        regularSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[15];
        constructionSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[16];
        constructionSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[17];
        constructionSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[18];
        constructionSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[19];

        timer.Reset();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CurrentState == State.Construction)
            return;
        if (rebuildObject.Objects.Count > 0 && !BuildingController.Instance.PlayerHostile && !PlayerController.Instance.IsHostile)
        {
            if (!timer.TickAndReset())
                return;
            CurrentState = State.Construction;
            CurrentAction = ActionE.FollowPath;
            sprites = constructionSprites;
            rebuildObject.Objects.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));
            Debug.Log("Walk to: " + rebuildObject.Objects[0].transform.position);
            SetPathToPosition(rebuildObject.Objects[0].transform.position);
        }
        if (CurrentState != State.Construction)
        {
            sprites = regularSprites;
            base.Update();
        }
    }

    public void AssignObject(GameObject obj)
    {
        if (obj.CompareTag("broken"))
        {
            obj.tag = "brokenandreported";
            rebuildObject.Objects.Add(obj);
        }
    }
}
