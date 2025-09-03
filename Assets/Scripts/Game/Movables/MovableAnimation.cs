using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface pour gérer le changement d'animation lorsqu'un trou passe en dessous
/// </summary>
public class MovableAnimation : MonoBehaviour, IFallingStateListener
{
    #region REFERENCES

    [SerializeField] public EditorReferences references = new EditorReferences();

    [System.Serializable]
    public class EditorReferences
    {
        public RuntimeAnimatorController[] fallingAnimations;

        public bool HasFallingAnimation()
        {
            return fallingAnimations.Length > 0;
        }

        public RuntimeAnimatorController GetRandomAnimation()
        {
            return fallingAnimations[Random.Range(0, fallingAnimations.Length)];
        }
    }

    #endregion

    private Animator _animator;
    private RuntimeAnimatorController _currentAnimation;
    private RuntimeAnimatorController _fallingAnimation;

    private MovableSplineAdapter _splineAdapter;
    private bool HadSplineAdapter = false;
    private Coroutine _enableAnimationCoroutine;

    private Transform _animatorTransform;
    private Rigidbody _rigidbody;
    private Collider[] _colliders;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Awake()
    {
        _splineAdapter = GetComponentInParent<MovableSplineAdapter>();
        if (_splineAdapter)
            HadSplineAdapter = true;

        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>();

        SetKinematic(_splineAdapter != null);

        _animator = GetComponentInChildren<Animator>();
        if (_animator)
        {
            _currentAnimation = _animator.runtimeAnimatorController;
            _animatorTransform = _animator.transform;

            if (references.HasFallingAnimation())
            {
                _fallingAnimation = references.GetRandomAnimation();
            }

            Renderer rend = GetComponentInChildren<Renderer>();
            rend.gameObject.AddComponent<MovableAnimationEnabler>();
        }
        else
        {
            _animatorTransform = transform;
        }

        FallingObject fo = GetComponent<FallingObject>();
        fo.RegisterFallingStateListener(this);
    }

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void SetAnimation(RuntimeAnimatorController anim)
    {
        _currentAnimation = anim;
        _animator.runtimeAnimatorController = _currentAnimation;
    }

    public void SetKinematic(bool s)
    {
        _rigidbody.isKinematic = s;
        foreach (var item in _colliders)
        {
            item.isTrigger = s;
        }
    }

    public void OnFallingStateChanged(bool b)
    {
        if (_enableAnimationCoroutine != null)
        {
            StopCoroutine(_enableAnimationCoroutine);
            _enableAnimationCoroutine = null;
        }

        if (b)
        {
            OnHoleEnter();
        }
        else
        {
            OnHoleExit();
        }
    }

    void OnHoleEnter()
    {
        if (_splineAdapter)
        {
            SetKinematic(false);
            _splineAdapter.RemoveSpline();
            // Destroy(_splineAdapter);
        }

        if (_fallingAnimation != null)
        {
            _animator.runtimeAnimatorController = _fallingAnimation;
        }
    }

    void OnHoleExit()
    {
        _enableAnimationCoroutine = StartCoroutine(EnableAnimationDelayed());
    }

    // The 1 frame delay is needed to prevent a situation when a moving object is falling slowly into the hole
    // It happens because of the repeated calls of OnHoleEnter() and OnHoleExit() during the same frame
    // SetKinematic(false) disables triggers and that leads to OnHoleExit() call
    IEnumerator EnableAnimationDelayed()
    {
        yield return null;

        // BUG local position tp on change animator
        if (_animatorTransform.transform.position.y > -1f)
        {
            if (_splineAdapter)
            {
                if (!_rigidbody.isKinematic)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }

                SetKinematic(true);
            }

            if (_animator)
            {
                if (_splineAdapter)
                {
                    _splineAdapter.ResetMovablePosition();
                    _splineAdapter.RestoreSpline();
                    //                } else {
                    //                    transform.position = _startPosition;
                    //                    transform.rotation = _startRotation;
                }

                _animator.runtimeAnimatorController = _currentAnimation;
                _animatorTransform.localPosition = Vector3.zero;
                _animatorTransform.localRotation = Quaternion.identity;
            }
            else
            {
                if (_splineAdapter)
                {
                    _splineAdapter.RestoreSpline();
                }
            }
        }
    }
}