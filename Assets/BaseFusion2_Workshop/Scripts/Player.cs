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

    [Header("Gameplay")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float localFireCooldown = 0.25f; 

    // Networked state
    [Networked] public int Hits { get; set; }
    [Networked] public byte ColorIndex { get; set; } 
    [Networked] private TickTimer delay { get; set; } 

    private NetworkCharacterController _cc;
    private Vector3 _forward;
    private Renderer[] _renderers;
    private List<Color> _defaultColors = new List<Color>();
    private int _lastSeenHits = -1;

    private double _lastLocalFireTime = -9999;

    private static readonly Color[] s_colorPalette = new Color[]
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        new Color(1f, 0.5f, 0f), // orange
        Color.white,
        Color.gray
    };

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();

        if (_followCamera != null)
            _followCamera.gameObject.SetActive(false);

        _renderers = GetComponentsInChildren<Renderer>();
        _defaultColors.Clear();
        foreach (var r in _renderers)
        {
            var mat = r.material;
            _defaultColors.Add(mat.color);
        }

        _forward = transform.forward;
    }

    public override void Spawned()
    {
        if (_followCamera != null)
            _followCamera.gameObject.SetActive(Object.HasInputAuthority);

        Ball.RegisterPlayer(this);

        ApplyColorFromIndex(ColorIndex);

        _lastSeenHits = Hits;
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
            _cc.Move(moveSpeed * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0f)
                _forward = data.direction;

            if (Object.HasInputAuthority)
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    double now = Runner.SimulationTime;
                    if (now - _lastLocalFireTime >= localFireCooldown)
                    {
                        _lastLocalFireTime = now;
                        RPC_RequestSpawnBall();
                    }
                }
            }
        }

        if (ColorIndex >= 0) 
            ApplyColorFromIndex(ColorIndex);
    }

    private void Update()
    {
        if (_lastSeenHits != Hits)
        {
            _lastSeenHits = Hits;
            StartCoroutine(FlashRedCoroutine(0.25f));
        }
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
            Hits++;
    }

    private void ApplyColorFromIndex(byte index)
    {
        int i = Mathf.Clamp(index, 0, s_colorPalette.Length - 1);
        Color target = s_colorPalette[i];

        if (_renderers != null)
        {
            for (int r = 0; r < _renderers.Length; r++)
                _renderers[r].material.color = target;
        }

        for (int j = 0; j < _defaultColors.Count; j++)
            _defaultColors[j] = target;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestSpawnBall()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!delay.ExpiredOrNotRunning(Runner))
            return;

        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        Runner.Spawn(
            _prefabBall,
            (_ballSpawnPoint ? _ballSpawnPoint.position : transform.position),
            Quaternion.LookRotation(_forward),
            Object.InputAuthority,
            (runner, obj) => obj.GetComponent<Ball>().Init()
        );
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateColorIndex(byte index)
    {
        ColorIndex = index;
        ApplyColorFromIndex(index);
    }
}
