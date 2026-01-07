using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class DispenserBackgroundService : BackgroundService
    {
        private readonly ICardDispenser _dispenser;
        public DispenserBackgroundService(ICardDispenser dispenser)
        {
            _dispenser = dispenser;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime _lastCommunicationTime = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                var status = _dispenser.SendCommand(DispenserEnums.CommandByStatus.StatusAP);
                if (status.Status == Enums.StatusEnum.Successful)
                {
                    _lastCommunicationTime = DateTime.Now;
                    if (status.Entity.Any(f => f.status == DispenserEnums.DispenserStatus.ErrorCardBinIsFull))
                    {
                        //AlarmKaydet("Hatalı Kart Haznesi Dolu", "Hatalı Kart Haznesi Dolu.");
                        Console.WriteLine("Hatalı Kart Haznesi Dolu");
                    }
                    if (status.Entity.Any(f => f.status == DispenserEnums.DispenserStatus.CardJam))
                    {
                        //AlarmKaydet("Kart Sıkışması", "Kart sıkıştı.");
                        Console.WriteLine("Kart Sıkışması");
                    }
                    if (status.Entity.Any(f => f.status == DispenserEnums.DispenserStatus.StackerIsPreEmpty))
                    {
                        //AlarmKaydet("Kart Azaldı", "Yeni kart azaldı.");
                        Console.WriteLine("Yeni Kart Azaldı");
                    }
                    if (status.Entity.Any(f => f.status == DispenserEnums.DispenserStatus.StackerIsEmpty))
                    {
                        //AlarmKaydet("Kart Bitti", "Yeni kart bitti.");
                        Console.WriteLine("Yeni Kart Bitti");
                    }
                }
                else
                {
                    // son 3dk içerinde yapılan durum sorgusunda iletişim kurulamadıysa bildirim gönder
                    if ((DateTime.Now - _lastCommunicationTime).TotalMinutes > 5)
                    {
                        // TODO: Alarm tetiklenecek
                        //AlarmKaydet("İletişim Yok", "Haberleşme kesildi.");
                    }
                }
                // 30sn
                await Task.Delay(10000, stoppingToken); // Delay for 5 seconds before the next iteration
            }
        }
    }
}
