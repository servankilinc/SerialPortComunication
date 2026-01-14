using System.Collections.Generic;
using static ConsoleApp1.DispenserEnums;

namespace ConsoleApp1
{
    public interface ICardDispenser
    {
        StatusData<byte[]> SendCommand(Command command);
        StatusData<byte[]> SendCommand(CommandSetDoorStatus command, DoorStatus doorStatus);
        StatusData<byte[]> SendCommand(CommandMoveCardToPosittion command, Position position);
        StatusData<List<(DispenserStatus status, string message)>> SendCommand(CommandSearchStatus command);
    }
}
