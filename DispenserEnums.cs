namespace ConsoleApp1
{
    public class DispenserEnums
    {
        public enum Command
        {
            DispenseDC = 1,
            CaptureCP,
            ResetRS
        }
        public enum CommandByStatus
        {
            StatusRF,
            StatusAP,
            DoorStatusSI,
        }
        public enum CommandByPosittion
        {
            DispenseFC = 1
        }
        public enum CommandByDoorStatus
        {
            DoorSetIN = 1
        }


        //Dışardan kart alma durumu
        public enum DoorStatus
        {
            //Prohibit cards in
            //Kart Almaz
            Prohibit = 0,
            //Allow cards into error card bin
            //Kartı alır, hatalı kart kutusuna atar
            IntoErrorCard = 1,
            //Allow cards into card read/write position (I.e. card stop location 2)
            //Kartı alır, okuma/yazma pozisyonunda tutar
            ReadWritePosition = 2
        }

        //Kartın çıkma pozisyonu
        public enum Position
        {
            //Dışarı atar
            Out = 0,
            StopAtSensor3 = 1,
            LeaveSensor3 = 2,
            LeaveSensor2 = 3,
            //Yarısını çıkarır
            ToTheDoor = 4,
            StopAtSensor2 = 6,
            StopAtSensor1 = 7,
        }

        public enum DispenserStatus
        {
            CardIsDispensing = 0,
            CardIsCapturing,
            CardDispenseError,
            CardCaptureError,
            NoCapture,
            CardOverlapped,
            CardJam,
            StackerIsPreEmpty,
            StackerIsEmpty,
            DispSensorStatus,
            CaptSensor2Status,
            CaptSensor1Status,
            StackerIsFullWaitingforDispensing,
            FailureAlarm,
            ErrorCardBinIsFull
        }
    }
}
