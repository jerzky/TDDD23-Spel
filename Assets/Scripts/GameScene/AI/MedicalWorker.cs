using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalWorker : Civilian
{
    private RemoveBody _removeBody;
    private readonly Sprite[] _medicalSprites = new Sprite[4];

    private readonly Sprite[] _regularSprites = new Sprite[4];
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _removeBody = new RemoveBody(this);
        Actions.Add(ActionE.RemoveBody, _removeBody);
        _regularSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[12];
        _regularSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[13];
        _regularSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[14];
        _regularSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters2")[15];
        _medicalSprites[0] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[0];
        _medicalSprites[1] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[1];
        _medicalSprites[2] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[2];
        _medicalSprites[3] = Resources.LoadAll<Sprite>("Textures/AI_Characters")[3];
        

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CurrentState == State.Medical)
            return;
        if (_removeBody.Bodies.Count > 0 && !BuildingController.Instance.PlayerHostile && !PlayerController.Instance.IsHostile)
        {
            CurrentState = State.Medical;
            CurrentAction = ActionE.FollowPath;
            Sprites = _medicalSprites;
            _removeBody.Bodies.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));
            Debug.Log("Walk to: " + _removeBody.Bodies[0].transform.position);
            SetPathToPosition(_removeBody.Bodies[0].transform.position);
        }

        if (CurrentState == State.Medical) 
            return;
        
        Sprites = _regularSprites;
        base.Update();
    }

    public void AssignBody(GameObject body)
    {
        if (body.CompareTag("body"))
            _removeBody.Bodies.Add(body);
    }
}
