using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform _ballSpawnPoint;
    [SerializeField] private Camera _followCamera;

    // Networked properties
    [Networked] public int hits { get; set; }
    [Networked] private TickTimer delay { get; set; }

    // Local state
    private NetworkCharacterController _cc;
    private Vector3 _forward;

    private Renderer[] _renderers;
    private List<Color> _defaultColors = new List<Color>();

    // Local cache to detect changes in the networked 'hits' property
    private int _lastSeenHits = -1;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();

        if (_followCamera != null)
            _followCamera.gameObject.SetActive(false);

        _renderers = GetComponentsInChildren<Renderer>();
        _defaultColors.Clear();
        foreach (var r in _renderers)
        {
            // force material instance so color changes don't affect other objects using sharedMaterial
            var mat = r.material;
            _defaultColors.Add(mat.color);
        }

        _forward = transform.forward;
    }

    public override void Spawned()
    {
        if (_followCamera != null)
            _followCamera.gameObject.SetActive(Object.HasInputAuthority);

        if (transform.forward != Vector3.zero)
            _forward = transform.forward;

        Ball.RegisterPlayer(this);

        _lastSeenHits = hits;
    }

    private void OnDisable()
    {
        Ball.UnregisterPlayer(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0.0f)
                _forward = data.direction;

            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_prefabBall,
                        (_ballSpawnPoint ? _ballSpawnPoint.position : transform.position),
                        Quaternion.LookRotation(_forward),
                        Object.InputAuthority,
                        (runner, obj) =>
                        {
                            obj.GetComponent<Ball>().Init();
                        });
                }
            }
        }
    }

    private void Update()
    {
        if (_lastSeenHits != hits)
        {
            _lastSeenHits = hits;
            OnHitsChangedLocal();
        }
    }

    private void OnHitsChangedLocal()
    {
        StartCoroutine(FlashRedCoroutine(0.25f));
    }

    private IEnumerator FlashRedCoroutine(float duration)
    {
        if (_renderers == null || _defaultColors == null || _renderers.Length != _defaultColors.Count)
            yield break;

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = Color.red;

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _defaultColors[i];
    }

    public void ApplyHit()
    {
        if (Object.HasStateAuthority)
            hits++;
    }
}
