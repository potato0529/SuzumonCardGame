using UnityEngine;

public interface IMatchTransmitter
{
    public void Send(PlayerCommand command);
}
