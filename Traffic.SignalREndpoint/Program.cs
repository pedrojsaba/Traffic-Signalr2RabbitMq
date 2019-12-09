using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Traffic.Application.Dtos;
using Environment = System.Environment;

namespace Traffic.SignalREndpoint
{
    class Program
    {

        
        static async Task Main()
        {
            var hubConnection = new HubConnection(Environment.GetEnvironmentVariable("SIGNALR_URL_TRAFFIC"));
            IHubProxy hubProxy = hubConnection.CreateHubProxy("performanceHub");
            hubProxy.On<Move>("getMonitorInsert", move => Task.Run(() => StdOut(move)).ConfigureAwait(false));
            await hubConnection.Start();
            Console.WriteLine("Press any key exit.");
            Console.ReadKey();

        }
        static void StdOut(Move move)
        {
            if (move.Id > 0)
            {
                move.FkVia = 1;
                move.Direccion = "SignalR->Firebase";
                move.Fiabilidad = 100;
                Console.WriteLine(move.Matricula);
                try
                {
                    UploadData(move);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        static async void UploadData(Move move)
        {
            var dto = new PerformTrafficTransferRequestDto() {
                TrafficId = move.Id,
                Register = Convert.ToDateTime(move.Fecha),
                Plate = move.Matricula,
                SourceId = move.PorticoId,
                Photo = move.Foto1
            };
            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:44393/traffic";
                var myContent = CreateHttpContent(dto);

                var result = await httpClient.PostAsync(url, myContent);
            }
        }
        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Newtonsoft.Json.Formatting.None })
            {
                var js = new Newtonsoft.Json.JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
        static void ObtenerToken()
        {
            //expireToken = "";
        }
        static void UploadImage(Move move)
        {
           
            var requestContent = new MultipartFormDataContent();

            byte[] imgContext = Url2Byte(string.Format("https://dvwsits.mtc.gob.pe/api/move/download/{0}/{1}/{2}/", move.PorticoId, move.EventTime.ToString("yyyyMMdd"), move.Foto1));
            var imageContext = new ByteArrayContent(imgContext);
            imageContext.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            requestContent.Add(imageContext, move.Foto1, move.Foto1);

            byte[] imgPlate = Url2Byte(string.Format("https://dvwsits.mtc.gob.pe/api/move/download/{0}/{1}/{2}/placa", move.PorticoId, move.EventTime.ToString("yyyyMMdd"), move.Foto2));
            var imagePlate = new ByteArrayContent(imgPlate);
            imagePlate.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            requestContent.Add(imagePlate, move.Foto2, move.Foto2.Replace(".jpg", "_1_plate.jpg"));            
        }

        static byte[] Url2Byte(string url)
        {
            var wc = new WebClient();
            return wc.DownloadData(url);
        }
    }
    class Move
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string Matricula { get; set; }
        public int Velocidad { get; set; }
        public string Foto1 { get; set; }
        public string Foto2 { get; set; }
        public int PorticoId { get; set; }
        public int FkVia { get; set; }
        public string Direccion { get; set; }
        public int Fiabilidad { get; set; }

        public DateTime EventTime
        {
            get
            {
                return Functions.TryConvert2DateTime(Fecha, "yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    static class Functions
    {
        public static string GetGateway(int id)
        {
            switch (id)
            {
                case 1: return "Panamericana Sur km 95";
                case 2: return "Panamericana Sur km 58";
                case 3: return "Gambeta Cuadra 3";
                case 4: return "Panamericana Norte km 44.5";
                case 5: return "Carretera Central km 31.5";
                default:
                    return string.Empty;
            }
        }
        
        public static DateTime TryConvert2DateTime(string dateString, string format)
        {
            if (!string.IsNullOrEmpty(dateString))
                dateString = dateString.Replace("  ", " ");

            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                return new DateTime(2001, 01, 01);
            }
        }
    }

}
