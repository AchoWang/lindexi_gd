using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeechelgicheCeehanuherechobal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new CCloudConsoleLogProvider());
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class CCloudConsoleLogProvider : ILoggerProvider
    {
        /// <inheritdoc />
        public void Dispose()
        {

        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new CCloudConsoleLogger();
        }

        class CCloudConsoleLogger : ILogger
        {
            class Empty : IDisposable
            {
                /// <inheritdoc />
                public void Dispose()
                {
                }
            }

            /// <inheritdoc />
            public IDisposable BeginScope<TState>(TState state)
            {
                return new Empty();
            }

            /// <inheritdoc />
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            /// <inheritdoc />
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                // [2099-10-19 19:07:45.456][threadName][INFO][��:����][_traceId:realTraceId][_userId:realUserId][tag:custom] ҵ�����־/�쳣��ջ

                var message = formatter(state, exception);
                Console.WriteLine(message);
            }
        }


    }

    public class CCloudLogInfo
    {
        public string ThreadName { get; set; }
        public int ThreadId { get; set; }

        public string ClassName { set; get; }

        public int LineNumber { get; set; }

        public string Message { set; get; }

        public string TraceId { set; get; }

        public string UserId { set; get; }

        public string MemberName { set; get; }

        public string[] Tags { set; get; }
    }

    public static class CCloudLogExtension
    {
        public static void LogInfo(this ILogger logger, string message,
            Exception exception = null,
            string traceId = null,
            string userId = null,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber]
            int sourceLineNumber = 0,
            params string[] tags)
        {
            // �պ��� Linux �¹������� Linux �����У����� Windows �����Ŀ��� Windows ��ִ�С���ʱʹ�� GetFileNameWithoutExtension �ܱ�������·���ͽ�����ͬ
            // �ٶ��� Windows �¹������� Linux �¹�����ֻ����·���䳤���ѣ��������۵���־ϵͳը���ˡ��� ����˵��ը����˵
            // ը�˵Ľ���������� dotnet runtime\src\libraries\System.Private.CoreLib\src\System\IO\Path.cs �� GetFileName �������潫 `PathInternal.IsDirectorySeparator(path[i])` �滻Ϊʵ����Ҫ�� \ �� / ����
            var classFile = Path.GetFileNameWithoutExtension(sourceFilePath);
            var thread = Thread.CurrentThread;

            var logInfo = new CCloudLogInfo()
            {
                ClassName = classFile,
                ThreadId = thread.ManagedThreadId,
                ThreadName = thread.Name,
                LineNumber = sourceLineNumber,
                Message = message,
                TraceId = traceId,
                UserId = userId,
                MemberName = memberName,
                Tags = tags
            };

            logger.Log(LogLevel.Information, eventId: EmptyEventId, logInfo, exception, Formatter);
        }

        private static string Formatter(CCloudLogInfo logInfo, Exception exception)
        {
            // honeycomb-log
            // [2099-10-19 19:07:45.456][threadName][INFO][��:����][_traceId:realTraceId][_userId:realUserId][tag:custom] ҵ�����־/�쳣��ջ

            return
                $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.sss}][{logInfo.ThreadName}:{logInfo.ThreadId}][{logInfo.ClassName}.{logInfo.MemberName}:{logInfo.LineNumber}][{(string.IsNullOrEmpty(logInfo.TraceId) ? "-" : $"_traceId:{logInfo.TraceId}")}][{(string.IsNullOrEmpty(logInfo.UserId) ? "-" : $"_userId:{logInfo.UserId}")}][tags:{string.Join(";", logInfo.Tags)}] {logInfo.Message} {exception?.ToString()}";
        }

        private static readonly EventId EmptyEventId = new EventId();
    }
}
