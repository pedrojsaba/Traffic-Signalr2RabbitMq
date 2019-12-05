using FluentMigrator.Runner;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Helpers;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NServiceBus;
using NServiceBus.NHibernate.Outbox;
using NServiceBus.Persistence;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Traffic.Application;
using Traffic.Application.Dtos;
using Traffic.Infrastructure.Mappings;
using Traffic.Infrastructure.Migrations.MySQL;
using UpgFisi.Common.Infrastructure.NHibernate;
using Environment = System.Environment;
using NHEnvironment = NHibernate.Cfg.Environment;

namespace Traffic.SignalREndpoint
{
    class Program
    {

        private static ITrafficApplicationService _trafficApplicationService;      

        static async Task Main()
        {
            var serviceProvider = CreateServices();
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
            var endPointName = "Traffic.NSBEndpoint";
            Console.Title = endPointName;
            var endpointConfiguration = new EndpointConfiguration(endPointName);
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            string rabbitmqUrl = Environment.GetEnvironmentVariable("RABBITMQ_TFC_NSB_URL");
            transport.ConnectionString(rabbitmqUrl);
            string mysqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_STRCON_CORE_TRAFFICS");
            var persistence = endpointConfiguration.UsePersistence<NHibernatePersistence>();
            persistence.UseOutboxRecord<Outbox, OutboxMap>();
            var nHibernateConfig = new Configuration();
            nHibernateConfig.SetProperty(NHEnvironment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).FullName);
            nHibernateConfig.SetProperty(NHEnvironment.ConnectionDriver, typeof(NHibernate.Driver.MySqlDataDriver).FullName);
            nHibernateConfig.SetProperty(NHEnvironment.Dialect, typeof(NHibernate.Dialect.MySQLDialect).FullName);
            nHibernateConfig.SetProperty(NHEnvironment.ConnectionString, mysqlConnectionString);
            AddFluentMappings(nHibernateConfig, mysqlConnectionString);
            persistence.UseConfiguration(nHibernateConfig);
            persistence.DisableSchemaUpdate();
            endpointConfiguration.EnableOutbox();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            await endpointInstance.Stop().ConfigureAwait(false);

            var hubConnection = new HubConnection(Environment.GetEnvironmentVariable("SIGNALR_URL_TRAFFIC"));
            IHubProxy hubProxy = hubConnection.CreateHubProxy("performanceHub");
            hubProxy.On<Move>("getMonitorInsert", move => Task.Run(() => StdOut(move)));
            await hubConnection.Start();
            Console.WriteLine("Press any key exit.");
            Console.ReadKey();
        }
        static Configuration AddFluentMappings(Configuration nhConfiguration, string connectionString)
        {
            return Fluently
                .Configure(nhConfiguration)
                .Database(MySQLConfiguration.Standard.ConnectionString(connectionString))
                .Mappings(cfg =>
                {
                    cfg.FluentMappings.AddFromAssembly(typeof(TrafficMap).Assembly);
                    cfg.FluentMappings.Conventions.Add(
                        ForeignKey.EndsWith("_id"),
                        ConventionBuilder.Property.When(criteria => criteria.Expect(x => x.Nullable, Is.Not.Set), x => x.Not.Nullable()));
                    cfg.FluentMappings.Conventions.Add<OtherConversions>();
                    cfg.FluentMappings.Conventions.Add<TableNameConvention>();
                })
                .BuildConfiguration();
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
        private static IServiceProvider CreateServices()
        {
            //"Server=localhost;Database=TrafficsSignalR;Uid=root;";
            string connectionString = Environment.GetEnvironmentVariable("MYSQL_STRCON_CORE_TRAFFICS");
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .WithGlobalCommandTimeout(new TimeSpan(1, 0, 0))
                    .AddMySql5()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(CreateInitialSchema).Assembly)
                    .For.All()
                )
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
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
            PerformTrafficTransferResponseDto response = await _trafficApplicationService.PerformTransfer(dto);
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
