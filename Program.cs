using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //IHost host = Host.CreateDefaultBuilder(args)
            //    .ConfigureServices(services =>
            //    {
            //        services.AddHostedService<DispenserBackgroundService>();
            //        services.AddSingleton<ICardDispenser>(new DispenserCRT531("COM10"));
            //    })
            //    .Build();


            //await host.RunAsync();

            try
            {
                Console.WriteLine("AKTİF COM PORTS :" + SerialPort.GetPortNames());

                var _dispenser = new DispenserCRT531("COM10"); // host.Services.GetRequiredService<ICardDispenser>();

                Console.WriteLine("\nAşağıdaki komutlardan birini çalıştırmak için kodu numarasını girin:\n");
                Console.WriteLine("DispenseDC ======================> 1");
                Console.WriteLine("StatusRF ========================> 2");
                Console.WriteLine("StatusAP ========================> 3");
                Console.WriteLine("DoorStatusSI ====================> 4");
                Console.WriteLine("CaptureCP =====================> * 5"); // 6 referans
                Console.WriteLine("ResetRS =======================> * 6"); // 1 referans
                Console.WriteLine("DispenseFC (ToTheDoor) ========> * 21"); // 1 referans
                Console.WriteLine("DispenseFC (Out) ==============> * 22"); // 5 referans
                Console.WriteLine("DispenseFC (StopAtSensor1) ==============> * 23");
                Console.WriteLine("DispenseFC (StopAtSensor2) ==============> * 24");
                Console.WriteLine("DispenseFC (StopAtSensor3) ==============> * 25");
                Console.WriteLine("DispenseFC (LeaveSensor2) ==============> * 26");
                Console.WriteLine("DispenseFC (LeaveSensor3) ==============> * 27");
                Console.WriteLine("DoorSetIN (ReadWritePosition) ===> 31");
                Console.WriteLine("DoorSetIN (Prohibit) ============> 32");
                Console.WriteLine("DoorSetIN (IntoErrorCard) ============> 33"); 

                while (true)
                {
                    Console.WriteLine("Seçiniz...");
                    var type = Console.ReadLine();
                    bool isThereResult = true;
                    StatusData<byte[]> result = new StatusData<byte[]>();
                    var result_status = new StatusData<List<(DispenserEnums.DispenserStatus, string)>>();

                    switch (type)
                    {
                        case "1":
                            result = _dispenser.SendCommand(DispenserEnums.Command.DispenseDC);
                            break;
                        case "2":
                            result_status = _dispenser.SendCommand(DispenserEnums.CommandSearchStatus.StatusRF);
                            break;
                        case "3":
                            result_status = _dispenser.SendCommand(DispenserEnums.CommandSearchStatus.StatusAP);
                            Console.WriteLine($@"STATUS CONTROL");
                            if(result_status.Entity != null)
                            {
                                result_status.Entity.ForEach(e => Console.WriteLine($"{e.Item2} ==== {e.Item2}"));
                            }
                            Console.WriteLine($@"STATUS CONTROL");
                            break;
                        case "4":
                            var result_door_status = _dispenser.SendCommand(DispenserEnums.CommandSearchDoorStatus.DoorStatusSI);
                            Console.WriteLine($@"DOOR STATUS CONTROL" + result_door_status.Entity);
                            break;
                        case "5": 
                            result = _dispenser.SendCommand(DispenserEnums.Command.CaptureCP);
                            break;
                        case "6": 
                            result = _dispenser.SendCommand(DispenserEnums.Command.ResetRS);
                            break;
                        case "21": 
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.ToTheDoor);
                            break;
                        case "22":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.Out);
                            break;
                        case "23":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.StopAtSensor1);
                            break;
                        case "24":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.StopAtSensor2);
                            break;
                        case "25":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.StopAtSensor3);
                            break;
                        case "26":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.LeaveSensor2);
                            break;
                        case "27":
                            result = _dispenser.SendCommand(DispenserEnums.CommandMoveCardToPosittion.DispenseFC, DispenserEnums.Position.LeaveSensor3);
                            break;
                        case "31": 
                            result = _dispenser.SendCommand(DispenserEnums.CommandSetDoorStatus.DoorSetIN, DispenserEnums.DoorStatus.ReadWritePosition);
                            break;
                        case "32":
                            result = _dispenser.SendCommand(DispenserEnums.CommandSetDoorStatus.DoorSetIN, DispenserEnums.DoorStatus.Prohibit);
                            break;
                        case "33":
                            result = _dispenser.SendCommand(DispenserEnums.CommandSetDoorStatus.DoorSetIN, DispenserEnums.DoorStatus.IntoErrorCard);
                            break;
                        default:
                            isThereResult = false;
                            Console.WriteLine("GEÇERSIZ KOMUT !!! ");
                            break;
                    }
                    //if (isThereResult)
                    //{
                    //    if(result != null)
                    //    {
                    //        Console.WriteLine("\n################### SONUC ################### \n" + JsonSerializer.Serialize(result) + "\n################### SONUC ###################\n");
                    //    }
                    //    else if ( result_status != null)
                    //    {
                    //        Console.WriteLine("\n################### SONUC ################### \n" + JsonSerializer.Serialize(result_status) + "\n################### SONUC ###################\n");
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
            }

        }
    }
}
