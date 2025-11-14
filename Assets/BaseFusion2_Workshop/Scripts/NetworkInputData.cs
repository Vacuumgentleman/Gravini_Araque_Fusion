using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;

    public NetworkButtons buttons; // usa NetworkButtons para ahorrar banda
    public Vector3 direction;
}
