using System;
using DG.Tweening;
using System.Collections.Generic;
using _Ball;
using UnityEngine;
using LibraryBen.Assembly.Miscellaneous.ExtensionMethods;
using UnityEditor;
using Random = UnityEngine.Random;

[SelectionBase]
public class FallingObject : MonoBehaviour, IPickable
{
    public enum FallingObjectType
    {
        InanimateObject,
        People,
        Vehicle,
        Building,
        Animal
    }

    public enum DMObjectType
    {
        None,

        //city
        Buildings, //173
        Trees, //44

        //Pirates
        Barrels, //41,

        //Western
        Horses, //3
        RockingChair, //5

        //Mall
        Trailers, //11
        Carts, //35

        //office
        Laptops, //8
        Presentations, //6

        //Farm
        Barn, //4
        Greenhouse, //5

        //Winter
        Campfires, //2
        Fountains, // 2

        //Scifi
        Tanks, //10
        Robots, //5,

        //Jurassic
        Volcanos //1
    }

    [SerializeField, Range(1, 20)]
    private int _sizeRequired = 1;

    public int SizeRequired
    {
        get { return _sizeRequired; }
        set { _sizeRequired = value; }
    }

    [SerializeField, Range(1, 20)]
    private int _botSizeRequired = 1;

    public int BotSizeRequired
    {
        get { return _botSizeRequired; }
        set { _botSizeRequired = value; }
    }

    [SerializeField]
    private bool _onlyForPlayer = false;

    public bool OnlyForPlayer
    {
        get { return _onlyForPlayer; }
        set { _onlyForPlayer = value; }
    }

    [SerializeField, Range(0, 20)]
    private int _points = 1;

    public int Points
    {
        get { return _points; }
        set { _points = value; }
    }

    [SerializeField, Range(0, 20)]
    private int _coinsValue = 0;

    public int CoinsValue
    {
        get { return _coinsValue; }
        set { _coinsValue = value; }
    }

    [SerializeField, Range(0, 20)]
    private int _gemsValue = 0;

    public int GemsValue
    {
        get { return _gemsValue; }
        set { _gemsValue = value; }
    }

    private bool _isShrunk = false;

    public bool IsShrunk
    {
        get { return _isShrunk; }
    }

    [SerializeField]
    private bool _canBeTransparent = false;

    public bool CanBeTransparent
    {
        get { return _canBeTransparent; }
    }

    [SerializeField]
    private FallingObjectType _objectType = FallingObjectType.InanimateObject;

    public FallingObjectType ObjectType
    {
        get { return _objectType; }
    }

    [SerializeField]
    private DMObjectType _DMType = DMObjectType.None;

    public DMObjectType DMType
    {
        get { return _DMType; }
    }

    [SerializeField]
    private UniqueObject _uniqueObjectTag = UniqueObject.None;

    public UniqueObject UniqueObjectTag
    {
        get { return _uniqueObjectTag; }
    }

    [SerializeField]
    private string[] _objectTags;

    public string[] ObjectTags
    {
        get { return _objectTags; }
    }

    [SerializeField] AudioClip _soundFeedback;
    public AudioClip SoundFeedback => _soundFeedback;

    [SerializeField] string _narrativeFeedback;
    public string NarrativeFeedback => _narrativeFeedback;

    [SerializeField] string _customFeedbackTag;
    public string CustomFeedbackTag
    {
        get => _customFeedbackTag;
        set => _customFeedbackTag = value;
    }

    [SerializeField]
    private bool _pooledObject = false;

    public bool PooledObject
    {
        get { return _pooledObject; }
        set { _pooledObject = value; }
    }

    [SerializeField]
    private bool setLayersRecursively;

    private List<IFallingStateListener> _fallingStateListeners;
    private bool _isFalling = false;

    public FallingObjectBehaviour Behaviour { get; private set; } = null;

    // Values only filled in BetterEating A/B for now to reduce RAM usage
    public Collider[] Colliders { get; private set; }
    public Rigidbody RB { get; private set; }
    public Vector3 LowestLocalPoint { get; private set; }

    private Outline outline;

    public bool HasObjectTag(string tag)
    {
        foreach (var item in _objectTags)
        {
            if (item == tag)
            {
                return true;
            }
        }

        return false;
    }

    private void Start()
    {
        int layer = LayerMask.NameToLayer("OnGround");
        string newTag = "FallingObject";

        if (setLayersRecursively)
        {
            gameObject.SetLayerAndTagRecursive(layer, newTag);
        }
        else
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
            {
                GameObject go = col.gameObject;
                go.tag = newTag;
                go.layer = layer;
            }

            if (ABTestLoader.Instance.parisStudioConfig.BetterEating == true)
            {
                Colliders = colliders;
                RB = GetComponent<Rigidbody>();
            }
        }

        if (_soundFeedback != SoundHelper.Instance.m_Item1 || _soundFeedback != SoundHelper.Instance.m_Item2)
        {
            if (Random.value > 0.5f)
            {
                _soundFeedback = SoundHelper.Instance.m_Item1;
            }
            else
            {
                _soundFeedback = SoundHelper.Instance.m_Item2;
            }
        }
    }

    private void OnEnable()
    {
        if (BaseGameManager.Instance)
            BaseGameManager.Instance.RegisterFallingObject(this);
    }

    private void OnDisable()
    {
        if (BaseGameManager.Instance)
        {
            if (BaseGameManager.Instance._hasGameEnded)
                return;

            BaseGameManager.Instance.UnregisterFallingObject(this);
        }

        if (_pooledObject)
            Destroy(Behaviour);
    }

    public FallingObjectBehaviour AddFallingObjectBehaviour<T>() where T : FallingObjectBehaviour
    {
        Behaviour = gameObject.AddComponent<T>();
        return Behaviour;
    }

    public void RegisterFallingStateListener(IFallingStateListener listener)
    {
        if (_fallingStateListeners == null)
            _fallingStateListeners = new List<IFallingStateListener>();

        _fallingStateListeners.Add(listener);
    }

    public void UnregisterFallingStateListener(IFallingStateListener listener)
    {
        if (_fallingStateListeners == null)
            return;

        _fallingStateListeners.Remove(listener);
    }

    public void RaiseFallingStateChanged(bool state)
    {
        if (_fallingStateListeners == null || _isFalling == state)
            return;

        _isFalling = state;
        foreach (var item in _fallingStateListeners)
        {
            item.OnFallingStateChanged(state);
        }
    }

    public void Shrink()
    {
        if (!_isShrunk)
        {
            _isShrunk = true;
            var scaleTarget = transform.localScale * 0.5f;
            transform.DOScale(scaleTarget, PowerUpShrink.ANIM_TIME);
            _sizeRequired = Mathf.CeilToInt(_sizeRequired / 2);
        }
    }

    public void Attract(Vector3 playerPos, float speed, bool _IgnoreY = false)
    {
        if (_IgnoreY)
            playerPos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, playerPos, speed);
        if (RB != null)
            RB.position = transform.position;
    }

    public void AttractPhysics(Vector3 playerPos, float speed, float torqueSpeed, bool _OnlyDown = true)
    {
        if (RB != null)
        {
            Vector3 velocity = RB.velocity;

            if (_OnlyDown && velocity.y > 0f)
                velocity.y = 0f;

            RB.velocity = velocity;

            Vector3 direction = (playerPos - transform.position).normalized;

            RB.AddForceAtPosition(direction * speed, RB.ClosestPointOnBounds(playerPos), ForceMode.Acceleration);

            if (Mathf.Approximately(0f, torqueSpeed) == false)
            {
                Vector3 rot = Quaternion.FromToRotation(RB.transform.up, Vector3.up).eulerAngles;
                Vector3 currentRot = RB.angularVelocity;
                if (Vector3.Dot(rot, currentRot) < 0f)
                    RB.angularVelocity = Vector3.zero;
                RB.AddTorque(rot * torqueSpeed);
            }
        }
    }

    public int RecalculateSize(Bounds bounds)
    {
        Vector3 size = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);

        int sizeRequired = 1;
        if (size.x > 1 && size.z > 1)
        {
            sizeRequired = Mathf.RoundToInt(Mathf.Max(Mathf.CeilToInt((size.x * 0.65f) / 2) + 1,
                Mathf.CeilToInt((size.z * 0.65f) / 2) + 1));
        }

        SizeRequired = sizeRequired;
        BotSizeRequired = (sizeRequired > 1) ? sizeRequired + 1 : sizeRequired;
        Points = sizeRequired;

        return sizeRequired;
    }

    public void Highlight(bool highlighted, Color color, float thickness = 0f)
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
            if (outline == null)
            {
                outline = gameObject.AddComponent<Outline>();    
            }
        }
        
        outline.enabled = highlighted;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = thickness;
        outline.OutlineColor = color;
    }
    
#if UNITY_EDITOR
    private static Color GizmoColor = Color.yellow;
    private Collider[] _colliders;
    private BoxCollider _boxTemp;
    private SphereCollider _sphereTemp;
    private CapsuleCollider _capsuleTemp;
    private Color _prevColor;

    void OnDrawGizmos()
    {
        _colliders = GetComponentsInChildren<Collider>();

        if (_colliders == null || _colliders.Length == 0)
            return;

        _prevColor = Gizmos.color;
        Gizmos.color = GizmoColor;

        Collider _temp;
        for (int i = 0; i < _colliders.Length; i++)
        {
            _temp = _colliders[i];
            if (!_temp.enabled)
                continue;
            if ((_boxTemp = _temp as BoxCollider) != null)
            {
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                Gizmos.matrix = Matrix4x4.TRS(_boxTemp.transform.TransformPoint(_boxTemp.center),
                    _boxTemp.transform.rotation, _boxTemp.transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, _boxTemp.size);

                Gizmos.matrix = oldGizmosMatrix;
            }
            else if ((_sphereTemp = _temp as SphereCollider) != null)
            {
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                Gizmos.matrix = Matrix4x4.TRS(_sphereTemp.transform.TransformPoint(_sphereTemp.center),
                    _sphereTemp.transform.rotation, _sphereTemp.transform.lossyScale);
                Gizmos.DrawWireSphere(Vector3.zero, _sphereTemp.radius);

                Gizmos.matrix = oldGizmosMatrix;
            }
        }

        Gizmos.color = _prevColor;
    }

    private void OnDrawGizmosSelected()
    {
        _colliders = GetComponents<Collider>();

        if (_colliders == null || _colliders.Length == 0)
            return;

        _prevColor = Gizmos.color;

        BoxCollider _temp;
        for (int i = 0; i < _colliders.Length; i++)
        {
            _temp = _colliders[i] as BoxCollider;
            if (!_temp || !_temp.enabled)
                continue;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_temp.transform.TransformPoint(_temp.center), _sizeRequired);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_temp.transform.TransformPoint(_temp.center), _botSizeRequired);
        }

        Gizmos.color = _prevColor;
    }
    
    [MenuItem(itemName:"Tools/Hole/Bake Outlines")]
    public static void MenuBakeSmoothNormalsInFallingObjects()
    {
        var guids = AssetDatabase.FindAssets("t:prefab");
        var count = 0;
        var addedCount = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var fo = AssetDatabase.LoadAssetAtPath<FallingObject>(path);
            if (fo != null)
            {
                Debug.Log("[FallingObjectOutline] found asset: " + fo.gameObject.name);
                count++;
                if (fo.TryGetComponent(out Outline res))
                {
                    res.PrecomputeOutline = true;
                    res.enabled = false;
                }
                else
                {
                    var outline = fo.gameObject.AddComponent<Outline>();
                    outline.PrecomputeOutline = true;
                    outline.enabled = false;
                    addedCount++;
                }
            }
        }
        Debug.Log($"[FallingObjectOutline] Found {count} falling object prefabs and added Outline to {addedCount} of them.");
        AssetDatabase.SaveAssets();
    }
#endif
}