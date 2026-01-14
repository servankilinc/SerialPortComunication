using Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using static ConsoleApp1.DispenserEnums;

namespace ConsoleApp1
{
    public class DispenserCRT531 : ICardDispenser, IDisposable
    {
        private SerialPort _serialPort;
        private const int timeoutMs = 5000;
        private const byte ACK = 0x06;
        private const byte ENQ = 0x05;
        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const byte NAK = 0x15;
        private const byte EOT = 0x04;
        private byte[] COMMAND_ENQ = new byte[] { ENQ };
        private bool disposedValue;

        public DispenserCRT531(string comport)
        {
            _serialPort = new SerialPort(comport, 9600, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = timeoutMs,
                WriteTimeout = timeoutMs
            };
        }

        public StatusData<byte[]> SendCommand(Command command)
        {
            try
            {
                PreparePort();

                switch (command)
                {
                    case Command.DispenseDC:
                        HandleSendCommand(DispenserCommands.DispenseDC());
                        break;
                    case Command.CaptureCP:
                        HandleSendCommand(DispenserCommands.CaptureCP());
                        break;
                    case Command.ResetRS:
                        HandleSendCommand(DispenserCommands.ResetRS());
                        break;
                    default:
                        throw new Exception("Dispenser'a tanımsız bir komut gönderildi.");
                }
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Successful,
                    Message = "Komut başarıyla gönderildi"
                };
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.InnerException != null ? $"exception: {ex.Message} innerExceptionex: {ex.InnerException.Message}" : $"exception: {ex.Message}";
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Error,
                    Message = $"Dispenser komut gönderimi sırasında bir sorun oluştu, komut: {command}, detay: {exceptionMessage}"
                };
            }
        }

        public StatusData<byte[]> SendCommand(CommandMoveCardToPosittion command, Position position)
        {
            try
            {
                PreparePort();

                switch (command)
                {
                    case CommandMoveCardToPosittion.DispenseFC:
                        HandleSendCommand(DispenserCommands.DispenseFC(position));
                        break;
                    default:
                        throw new Exception("Pozisyon parametresi ile Dispenser'a tanımsız bir komut gönderildi.");
                }
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Successful,
                    Message = "Komut başarıyla gönderildi"
                };
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.InnerException != null ? $"exception: {ex.Message} innerExceptionex: {ex.InnerException.Message}" : $"exception: {ex.Message}";
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Error,
                    Message = $"Dispenser komut gönderimi sırasında bir sorun oluştu, detay: {exceptionMessage}"
                };
            }
        }

        public StatusData<byte[]> SendCommand(CommandSetDoorStatus command, DoorStatus doorStatus)
        {
            try
            {
                PreparePort();

                switch (command)
                {
                    case CommandSetDoorStatus.DoorSetIN:
                        HandleSendCommand(DispenserCommands.DoorSetIN(doorStatus));
                        break;
                    default:
                        throw new Exception("DoorStatus parametresi ile Dispenser'a tanımsız bir komut gönderildi.");
                }
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Successful,
                    Message = "Komut başarıyla gönderildi"
                };
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.InnerException != null ? $"exception: {ex.Message} innerExceptionex: {ex.InnerException.Message}" : $"exception: {ex.Message}";
                return new StatusData<byte[]>
                {
                    Status = Enums.StatusEnum.Error,
                    Message = $"Dispenser komut gönderimi sırasında bir sorun oluştu, detay: {exceptionMessage}"
                };
            }
        }

        public StatusData<List<(DispenserStatus status, string message)>> SendCommand(CommandSearchStatus command)
        {
            try
            {
                PreparePort();

                switch (command)
                {
                    case CommandSearchStatus.StatusRF:
                        var response_statusRF = HandleSendCommand(DispenserCommands.StatusRF());

                        if (response_statusRF != null && response_statusRF.Length == 6)
                        {
                            string hex = new string(new[]
                            {
                                (char)response_statusRF[2],
                                (char)response_statusRF[3],
                                (char)response_statusRF[4]
                            });
                            var binary = string.Concat(hex.Select(b => Convert.ToString(Convert.ToInt32(b.ToString(), 16), 2).PadLeft(4, '0')));
                            var statusList = new List<(DispenserStatus, string)>();
                            if (binary[0] == '1') statusList.Add((DispenserStatus.CardIsDispensing, "Card Is Dispensing"));
                            if (binary[1] == '1') statusList.Add((DispenserStatus.CardIsCapturing, "Card Is Capturing"));
                            if (binary[2] == '1') statusList.Add((DispenserStatus.CardDispenseError, "Card Dispense Error"));
                            if (binary[3] == '1') statusList.Add((DispenserStatus.CardCaptureError, "Card Capture Error"));
                            if (binary[4] == '1') statusList.Add((DispenserStatus.NoCapture, "No capture"));
                            if (binary[5] == '1') statusList.Add((DispenserStatus.CardOverlapped, "Card overlapped"));
                            if (binary[6] == '1') statusList.Add((DispenserStatus.CardJam, "Card jam"));
                            if (binary[7] == '1') statusList.Add((DispenserStatus.StackerIsPreEmpty, "Stacker Is Pre-Empty"));
                            if (binary[8] == '1') statusList.Add((DispenserStatus.StackerIsEmpty, "Stacker Is Empty"));
                            if (binary[9] == '1') statusList.Add((DispenserStatus.DispSensorStatus, "Disp-Sensor Status"));
                            if (binary[10] == '1') statusList.Add((DispenserStatus.CaptSensor2Status, "Capt-Sensor 2 Status"));
                            if (binary[11] == '1') statusList.Add((DispenserStatus.CaptSensor1Status, "Capt-Sensor 1 Status"));
                            if (!statusList.Any()) statusList.Add((DispenserStatus.StackerIsFullWaitingforDispensing, "Stacker Is Full \nWaiting for Dispensing"));

                            return new StatusData<List<(DispenserStatus, string)>>
                            {
                                Status = Enums.StatusEnum.Successful,
                                Entity = statusList
                            };
                        }
                        else
                        {
                            string resp_statusRF = response_statusRF != null ? Encoding.ASCII.GetString(response_statusRF) : string.Empty;
                            return new StatusData<List<(DispenserStatus, string)>>
                            {
                                Status = Enums.StatusEnum.EmptyData,
                                Message = $"Dispenser StatusRF komutundan beklenen uzunlukta cevap alınamadı. cevap: {resp_statusRF}"
                            };
                        }
                    case CommandSearchStatus.StatusAP:
                        var response_statusAP = HandleSendCommand(DispenserCommands.StatusAP());

                        if (response_statusAP != null && response_statusAP.Length == 6)
                        {
                            string hex = new string(new[]
                            {
                                (char)response_statusAP[2],
                                (char)response_statusAP[3],
                                (char)response_statusAP[4],
                                (char)response_statusAP[5]
                            });
                            var binary = string.Concat(hex.Select(b => Convert.ToString(Convert.ToInt32(b.ToString(), 16), 2).PadLeft(4, '0')));
                            var statusList = new List<(DispenserStatus, string)>();

                            if (binary[2] == '1') statusList.Add((DispenserStatus.FailureAlarm, "Failure alarm (sensor invalid)"));
                            if (binary[3] == '1') statusList.Add((DispenserStatus.ErrorCardBinIsFull, "Error card bin is full")); // AlarmKaydet("Hatalı Kart Haznesi Dolu", "Hatalı Kart Haznesi Dolu.");
                            if (binary[4] == '1') statusList.Add((DispenserStatus.CardIsDispensing, "Card Is Dispensing"));
                            if (binary[5] == '1') statusList.Add((DispenserStatus.CardIsCapturing, "Card Is Capturing"));
                            if (binary[6] == '1') statusList.Add((DispenserStatus.CardDispenseError, "Card Dispense Error"));
                            if (binary[7] == '1') statusList.Add((DispenserStatus.CardCaptureError, "Card Capture Error"));
                            if (binary[8] == '1') statusList.Add((DispenserStatus.NoCapture, "No capture"));
                            if (binary[9] == '1') statusList.Add((DispenserStatus.CardOverlapped, "Card overlapped"));
                            if (binary[10] == '1') statusList.Add((DispenserStatus.CardJam, "Card jam")); // AlarmKaydet("Kart Sıkışması", "Kart sıkıştı.");
                            if (binary[11] == '1') statusList.Add((DispenserStatus.StackerIsPreEmpty, "Stacker Is Pre-Empty")); // AlarmKaydet("Kart Azaldı", "Yeni kart azaldı.");
                            if (binary[12] == '1') statusList.Add((DispenserStatus.StackerIsEmpty, "Stacker Is Empty")); // AlarmKaydet("Kart Bitti", "Yeni kart bitti.");
                            if (binary[13] == '1') statusList.Add((DispenserStatus.DispSensorStatus, "Disp-Sensor Status"));
                            if (binary[14] == '1') statusList.Add((DispenserStatus.CaptSensor2Status, "Capt-Sensor 2 Status"));
                            if (binary[15] == '1') statusList.Add((DispenserStatus.CaptSensor1Status, "Capt-Sensor 1 Status"));


                            if (!statusList.Any()) statusList.Add((DispenserStatus.StackerIsFullWaitingforDispensing, "Stacker Is Full\nWaiting for Dispensing"));

                            return new StatusData<List<(DispenserStatus status, string message)>>
                            {
                                Status = Enums.StatusEnum.Successful,
                                Entity = statusList
                            };
                        }
                        else
                        {
                            string resp_statusAP = response_statusAP != null ? Encoding.ASCII.GetString(response_statusAP) : string.Empty;
                            return new StatusData<List<(DispenserStatus status, string message)>>
                            {
                                Status = Enums.StatusEnum.EmptyData,
                                Message = $"Dispenser StatusAP komutundan beklenen uzunlukta cevap alınamadı. cevap: {resp_statusAP}"
                            };
                        }
                    default:
                        throw new Exception("Dispenser'a tanımsız bir komut gönderildi.");
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.InnerException != null ? $"exception: {ex.Message} innerExceptionex: {ex.InnerException.Message}" : $"exception: {ex.Message}";
                return new StatusData<List<(DispenserStatus status, string message)>>
                {
                    Status = Enums.StatusEnum.Error,
                    Message = $"Dispenser komut gönderimi sırasında bir sorun oluştu, komut: {command}, detay: {exceptionMessage}"
                };
            }
        }

        public StatusData<DoorStatus> SendCommand(CommandSearchDoorStatus command)
        {
            try
            {
                PreparePort();

                switch (command)
                { 
                    case CommandSearchDoorStatus.DoorStatusSI:
                        var response_doorStatusSI = HandleSendCommand(DispenserCommands.DoorStatusSI());

                        if (response_doorStatusSI != null && response_doorStatusSI.Length == 3)
                        {
                            DoorStatus? doorStatus;
                            switch(response_doorStatusSI[2])
                            {
                                case 48:
                                    doorStatus = DoorStatus.Prohibit;
                                    break;
                                case 49:
                                    doorStatus = DoorStatus.IntoErrorCard;
                                    break;
                                case 50:
                                    doorStatus = DoorStatus.ReadWritePosition;
                                    break;
                                default:
                                    return new StatusData<DoorStatus>
                                    {
                                        Status = Enums.StatusEnum.EmptyData,
                                        Message = $"Dispenser DoorStatusSI komutundan beklenmeyen kapı durumu değeri alındı. cevap: {response_doorStatusSI[2]}"
                                    };
                            }
                            
                            return new StatusData<DoorStatus>
                            {
                                Status = Enums.StatusEnum.Successful,
                                Entity = doorStatus.Value
                            };
                        }
                        else
                        {
                            string resp_doorStatusSI = response_doorStatusSI != null ? Encoding.ASCII.GetString(response_doorStatusSI) : string.Empty;
                            return new StatusData<DoorStatus>
                            {
                                Status = Enums.StatusEnum.EmptyData,
                                Message = $"Dispenser DoorStatusSI komutundan beklenen uzunlukta cevap alınamadı. cevap: {resp_doorStatusSI}"
                            };
                        }
                    default:
                        throw new Exception("Dispenser'a tanımsız bir komut gönderildi.");
                }
            }
            catch (Exception ex)
            {
                var exceptionMessage = ex.InnerException != null ? $"exception: {ex.Message} innerExceptionex: {ex.InnerException.Message}" : $"exception: {ex.Message}";
                return new StatusData<DoorStatus>
                {
                    Status = Enums.StatusEnum.Error,
                    Message = $"Dispenser komut gönderimi sırasında bir sorun oluştu, komut: {command}, detay: {exceptionMessage}"
                };
            }
        }


        private void PreparePort()
        {
            if (_serialPort.IsOpen == false)
            {
                _serialPort.Open();
            }
            _serialPort.DiscardInBuffer();
        }

        private byte[]? HandleSendCommand(byte[] command)
        {
            // 1) komutu gönder
            SendToSerialPort(command);

            // 2) ACK/NAK... bekle
            WaitForAck();

            // 3) Cevabı al
            SendToSerialPort(COMMAND_ENQ);

            // 4) Paketi oku
            var responseBytes = ReadFromSerialPort();

            // 5) Cevap varsa paketi doğrula ve mesaj gövdesini al yoksa default dön
            if (responseBytes.Length < 3)
                return responseBytes;

            var result = ValidateAndParseResponse(responseBytes);
            return result;
        }


        #region HELPERS
        private void SendToSerialPort(byte[] commandBytes)
        {
            Thread.Sleep(100);
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
            _serialPort.Write(commandBytes, 0, commandBytes.Length);
            Thread.Sleep(100);
        }

        private byte[] ReadFromSerialPort()
        {
            var buffer = new List<byte>();
            var startTime = DateTime.Now;
            bool stxReceived = false;

            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                // 1) Okunmayı bekleyen byte bilgisi bulunmuyor
                if (_serialPort.BytesToRead == 0)
                    continue;

                byte data = (byte)_serialPort.ReadByte();

                // 2) Gelen byte STX ise önce listeyi temizle
                if (data == STX)
                {
                    buffer.Clear();
                    stxReceived = true;
                }

                // 3) Gelen byte bilgisini listeye al
                buffer.Add(data);

                // 4) [STX, ...,ETX, BCC] paket bütünlüğü sağlandıysa cevap tamamlanmıştır döndürebilirsin
                if (stxReceived && buffer.Count >= 3 && buffer[^2] == ETX)
                {
                    return buffer.ToArray();
                }
            }

            return buffer.ToArray();
        }

        private byte[]? ValidateAndParseResponse(byte[] packet)
        {
            // 1) [STX, ...,ETX, BCC] Paket boyut kontrolü
            if (packet.Length < 3)
                throw new Exception($"Dispenser Serial Port gelen paket boyutu boş veya bekleneden kısa! paket: {packet}");

            // 2) Paket STX/ETX kontrolü
            if (packet[0] != STX || packet[^2] != ETX)
                throw new Exception($"Dispenser Serial Port gelen paket, STX/ETX hatas! paket: {packet}");

            // 3) BCC kontrolü
            byte calculatedBcc = 0x00;
            for (int i = 0; i < packet.Length - 1; i++)
            {
                calculatedBcc ^= packet[i];
            }
            if (calculatedBcc != packet[^1])
                throw new Exception($"Dispenser Serial Port gelen paket BCC doğrulaması başarısız! paket: {packet}");

            // 4) Gövdede gelen mesaj bilgisini dön (cevap içerisinde mesaj gelmeme durumu göz önünde bulundurularak default return edildi)
            return packet.Length > 3 ? packet.Skip(1).Take(packet.Length - 3).ToArray() : default;
        }

        private bool WaitForAck()
        {
            byte response;
            response = (byte)_serialPort.ReadByte();

            switch (response)
            {
                case ACK:
                    return true;
                case NAK:
                    throw new Exception($"Dispenser cihazı NAK döndü. Komut reddedildi. cevap: {response}");
                case EOT:
                    throw new Exception($"Dispenser cihazı EOT gönderdi. İletişim sonlandırıldı. cevap: {response}");
                default:
                    throw new Exception($"Dispenser cihazı beklenmeyen kontrol karakteri döndü. cevap: {response}");
            }
        }
        #endregion

        #region DISPOSE
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        } 
        #endregion
    }
}
