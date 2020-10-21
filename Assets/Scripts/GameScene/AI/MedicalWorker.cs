using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalWorker : Civilian
{
    RemoveBody removeBody;
    Sprite[] medicalSprites = new Sprite[4];
    Sprite[] regularSprites = new Sprite[4];
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        removeBody = new RemoveBody(this);
        Actions.Add(ActionE.RemoveBody, removeBody);
        regularSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[12];
        regularSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[13];
        regularSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[14];
        regularSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[15];
        medicalSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[0];
        medicalSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[1];
        medicalSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[2];
        medicalSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[3];
        

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CurrentState == State.Medical)
            return;
        if (removeBody.Bodies.Count > 0 && !BuildingController.Instance.PlayerHostile && !PlayerController.Instance.IsHostile)
        {
            CurrentState = State.Medical;
            CurrentAction = ActionE.FollowPath;
            sprites = medicalSprites;
            removeBody.Bodies.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));
            Debug.Log("Walk to: " + removeBody.Bodies[0].transform.position);
            SetPathToPosition(removeBody.Bodies[0].transform.position);
        }
        if (CurrentState != State.Medical)
        {
            sprites = regularSprites;
            base.Update();
        }
    }

    public void AssignBody(GameObject body)
    {
        if (body.CompareTag("body"))
            removeBody.Bodies.Add(body);
    }
}
