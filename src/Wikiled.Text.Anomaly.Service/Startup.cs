using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wikiled.Common.Net.Client;
using Wikiled.Common.Utilities.Resources;
using Wikiled.Server.Core.Errors;
using Wikiled.Server.Core.Helpers;
using Wikiled.Server.Core.Middleware;
using Wikiled.Text.Analysis.NLP.Frequency;
using Wikiled.Text.Analysis.NLP.NRC;
using Wikiled.Text.Analysis.POS;
using Wikiled.Text.Analysis.Word2Vec;
using Wikiled.Text.Analysis.Words;
using Wikiled.Text.Anomaly.Processing;
using Wikiled.Text.Anomaly.Service.Config;
using Wikiled.Text.Anomaly.Service.Logic;
using Wikiled.Text.Anomaly.Supervised;
using Wikiled.Text.Anomaly.Vectors;
using Wikiled.Text.Inquirer.Logic;
using Wikiled.Text.Parser.Api.Service;
using Wikiled.Text.Style.Logic;

namespace Wikiled.Text.Anomaly.Service
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        private ILoggerFactory loggerFactory;

        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<Startup>();
            Configuration.ChangeNlog();
            logger.LogInformation($"Starting: {Assembly.GetExecutingAssembly().GetName().Version}");
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseExceptionHandlingMiddleware();
            app.UseHttpStatusCodeExceptionMiddleware();
            app.UseRequestLogging();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Needed to add this section, and....
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "CorsPolicy",
                        itemBuider => itemBuider.AllowAnyOrigin()
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials());
                });

            // Add framework services.
            services.AddMvc(options => { });

            // needed to load configuration from appsettings.json
            services.AddOptions();
            services.RegisterConfiguration<ServicesConfig>(Configuration.GetSection("Services"));
            services.RegisterConfiguration<StorageConfig>(Configuration.GetSection("Persistency"));

            // Create the container builder.
            var builder = new ContainerBuilder();
            SetupOther(builder);
            SetupAnomaly(builder);
            SetupServices(builder);
            builder.Populate(services);
            var appContainer = builder.Build();

            logger.LogInformation("Ready!");
            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(appContainer);
        }

        private void SetupOther(ContainerBuilder builder)
        {
            builder.RegisterType<IpResolve>().As<IIpResolve>();
            builder.RegisterType<DomainSentimentAnalysisFactory>().As<ISentimentAnalysisFactory>();
            builder.RegisterType<UnsupervisedAnomaly>().As<IAnomalyDetection>();
        }

        private void SetupServices(ContainerBuilder builder)
        {
            logger.LogInformation("Setting up services...");
            var parsingUrl = new Uri(Configuration["Services:Parsing"]);
            logger.LogInformation("Register parsing: {0}", parsingUrl);
            builder.Register(context => new DocumentParser(new ApiClientFactory(new HttpClient {Timeout = TimeSpan.FromMinutes(5)}, parsingUrl))).As<IDocumentParser>();

            var sentimentUrl = new Uri(Configuration["Services:Sentiment"]);
            builder.Register(context =>
                new StreamApiClientFactory(context.Resolve<ILoggerFactory>(),
                    new HttpClient { Timeout = TimeSpan.FromMinutes(10) },
                    sentimentUrl))
                .As<IStreamApiClientFactory>();
            logger.LogInformation("Register parsing: {0}", parsingUrl);
        }

        private void SetupAnomaly(ContainerBuilder builder)
        {
            logger.LogInformation("Setting up lexicon libraries");
            var tagger = new NaivePOSTagger(new BNCList(), WordTypeResolver.Instance);
            builder.RegisterInstance(tagger).As<IPOSTagger>();

            var dictionary = new NRCDictionary();
            dictionary.Load();
            builder.RegisterInstance(dictionary).As<INRCDictionary>();

            var inquirer = new InquirerManager();
            inquirer.Load();
            builder.RegisterInstance(inquirer).As<IInquirerManager>();

            builder.RegisterType<FrequencyListManager>().As<IFrequencyListManager>().SingleInstance();
            builder.RegisterType<StyleFactory>().As<IStyleFactory>();
            builder.RegisterType<AnomalyFactory>().As<IAnomalyFactory>();
            builder.RegisterType<SupervisedAnomaly>().As<ISupervisedAnomaly>();
            builder.RegisterType<DocumentReconstructor>().As<IDocumentReconstructor>();
            builder.RegisterType<DocumentExtractor>().As<IDocumentExtractor>();
            
            builder.RegisterType<EmbeddingVectorSource>().As<IDocumentVectorSource>();
            builder.RegisterType<SvmModelStorageFactory>().As<IModelStorageFactory>();

            logger.LogInformation("Downloading model...");
            var model = new Uri(Configuration["Anomaly:model"]);
            new DataDownloader(loggerFactory).DownloadFile(model, "resources").Wait();
            builder.Register(context => WordModel.Load("Resources/model.bin")).SingleInstance();
        }
    }
}
