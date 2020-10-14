using Assets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CCTVStatus
{
    NotMonitored,
    Monitored,
    Spotted
}
public class CCTV : Interactable
{
    private Sprite[] _cameraSprites;

    private Vector3 _direction = new Vector3(0f, 0f, 1f);
    private const float RotationSpeed = 10f;
    private Vector3 _originalRotation;
    private Vector3 _currentAngle = Vector3.zero;
    public enum LocationType { Common, EmployeeOnly, Hostile };
    [SerializeField]
    LocationType _locationType = LocationType.Common;


    bool _timerActive = false;
    private SimpleTimer _timer = new SimpleTimer(0.5f);

    [SerializeField]
    bool _isInEmployeeAreaOnly = false;

    [SerializeField] 
    private GameObject _rayCastOrigin;

    [SerializeField] 
    private Building _building;

    private bool _isMonitored;

    // Start is called before the first frame update
    void Start()
    {
        _cameraSprites = Resources.LoadAll<Sprite>("Textures/camerasprites");
        GetComponent<SpriteRenderer>().sprite = _cameraSprites[2];
        _originalRotation = transform.rotation.eulerAngles;
    }

    private bool IsMonitored
    {
        get => _isMonitored;
        set
        {
            _isMonitored = value;
            GetComponent<SpriteRenderer>().sprite = _isMonitored ? _cameraSprites[4] : _cameraSprites[2];
        }
    }

    // Update is called once per frame
    void Update()
    {


        RotateCamera();

        if (_timerActive)
            if (_timer.TickAndReset())
                HostilePlayerNoticed(AlertIntensity.ConfirmedHostile);

        if (IsMonitored != _building.IsSomeoneMonitoringCCTV)
            IsMonitored = _building.IsSomeoneMonitoringCCTV;
    }

    private void SetCameraLookDir(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void RotateCamera()
    {
        _currentAngle += _direction * Time.deltaTime * RotationSpeed;
        transform.rotation = Quaternion.Euler(_originalRotation + _currentAngle);
        if(_direction.z > 0 && _currentAngle.z > 45|| _direction.z < 0 && _currentAngle.z < -45)
        {
            _direction *= -1;
        }
    }

    public override void Cancel()
    {
        
    }

    public override bool Interact(uint itemIndex)
    {
        return false;
    }

    public void OnVisionEnter(Collider2D col)
    {
        if (!_isMonitored || !col.CompareTag("Player"))
            return;

        GetComponent<SpriteRenderer>().sprite = _cameraSprites[3];
        // Check if player is hostile
        // this bool should be kept in some kind of controller

        if (PlayerController.Instance.IsHostile || _locationType == LocationType.Hostile)
        {
            // if the player is hostile in camera vision or if the location type is hostile (maybe inside vault?)
            // report player instantly
            if (_building.IsSomeoneMonitoringCCTV)
                HostilePlayerNoticed(AlertIntensity.ConfirmedHostile);
        }
        else if (/*_building.PlayerReportedAsHostile ||*/ _locationType == LocationType.EmployeeOnly)
        {
            // if player was previously reported at the same building or if the cctv is in a employee only location
            // start the timer so the player can be noticed after noticeDelay seconds
            if (_building.IsSomeoneMonitoringCCTV)
                _timerActive = true;
        }
    }

    public void OnVisionStay(Collider2D col)
    {

    }

    public void OnVisionExit(Collider2D col)
    {
        if (!col.CompareTag("Player")) 
            return;
        _timerActive = false;
        _timer.Reset();
        IsMonitored = _building.IsSomeoneMonitoringCCTV;
    }

    void HostilePlayerNoticed(AlertIntensity alertIntensity)
    {
        _building.OnAlert(PlayerController.Instance.transform.position, AlertType.Guard_CCTV, alertIntensity);
    }

    public override string Name()
    {
        return "CCTV";
    }
}
