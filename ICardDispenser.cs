using System.Collections.Generic;
using static ConsoleApp1.DispenserEnums;

namespace ConsoleApp1
{
    public interface ICardDispenser
    {
        StatusData<byte[]> SendCommand(Command command);
        StatusData<byte[]> SendCommand(CommandByDoorStatus command, DoorStatus doorStatus);
        StatusData<byte[]> SendCommand(CommandByPosittion command, Position position);
        StatusData<List<(DispenserStatus status, string message)>> SendCommand(CommandByStatus command);
    }
}
