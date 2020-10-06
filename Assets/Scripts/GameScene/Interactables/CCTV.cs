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
    public enum LocationType { BANK, STORE };
    LocationType _locationType;

    private float _timer = 0f;
    [SerializeField]
    float _playerNoticedDelay = 1f;

    bool _timerActive = false;


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
        SetCameraLookDir(new Vector3(0f, 0f, -90f));
        _originalRotation = transform.rotation.eulerAngles;
        _locationType = LocationType.BANK;
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
            _timer += Time.deltaTime;
        else if (_timer > 0)
            _timer -= Time.deltaTime;
        else
        {
            // player hostile and noticed
            HostilePlayerNoticed();
            _timer = 0f;
        }

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

        
        
        var hit = Physics2D.Raycast(_rayCastOrigin.transform.position, col.transform.position - transform.position);


        if (hit.collider == null || !hit.collider.CompareTag("Player"))
            return;

        GetComponent<SpriteRenderer>().sprite = _cameraSprites[3];
        // Check if player is hostile
        // this bool should be kept in some kind of controller
        var playerHasBeenSeenAsHostileBefore = false;
        if (Inventory.Instance.GetCurrentItem().ItemType == ItemType.Weapon)
        {
            if(_building.IsSomeoneMonitoringCCTV)
                _building.OnAlert(AlertType.Guard_CCTV, col.transform.position);
        }
        else if (playerHasBeenSeenAsHostileBefore)
        {
            // start timer
            _timerActive = true;
        }
    }

    public void OnVisionStay(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            
        }
    }

    public void OnVisionExit(Collider2D col)
    {
        if (!col.CompareTag("Player")) 
            return;
        _timerActive = false;
        IsMonitored = _building.IsSomeoneMonitoringCCTV;
    }

    void HostilePlayerNoticed()
    {
        switch (_locationType)
        {
            case LocationType.BANK:
                // Alarm guards
                // Call Police
                // Set off alarm
                break;
        }
    }

    public override string Name()
    {
        return "CCTV";
    }
}
