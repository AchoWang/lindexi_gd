using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FeechelgicheCeehanuherechobal.Controllers;
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
            services.AddScoped<Foo>();

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
            return new CCloudConsoleLogger(categoryName);
        }

        class CCloudConsoleLogger : ILogger
        {
            public CCloudConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

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

            private readonly string _categoryName;

            /// <inheritdoc />
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                // [2099-10-19 19:07:45.456][threadName][INFO][��:����][_traceId:realTraceId][_userId:realUserId][tag:custom] ҵ�����־/�쳣��ջ
                string message;
                if (typeof(TState) == typeof(CCloudLogInfo))
                {
                    var logInfo = state as CCloudLogInfo;
                    Debug.Assert(logInfo != null, nameof(logInfo) + " != null");
                    logInfo.CategoryName = _categoryName;
                    message = formatter(state, exception);
                }
                else
                {
                    // [2099-10-19 19:07:45.456][threadName][INFO][��:����][_traceId:realTraceId][_userId:realUserId][tag:custom] ҵ�����־/�쳣��ջ
                    message =
                        $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.sss}][{Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}][{CCloudLogExtension.LogLevelToString(logLevel)}][{_categoryName}][-][-][-][EventId={eventId.Id}:{eventId.Name}] {formatter(state, exception)}";
                }

                Console.WriteLine(message);
            }
        }
    }

    internal class CCloudLogInfo
    {
        public string CategoryName { set; get; }

        public string ThreadName { get; set; }
        public int ThreadId { get; set; }

        public string ClassFile { set; get; }

        public int LineNumber { get; set; }

        public string Message { set; get; }

        public string TraceId { set; get; }

        public string UserId { set; get; }

        public string MemberName { set; get; }

        public string[] Tags { set; get; }

        public LogLevel LogLevel { set; get; }
    }

    public static class CCloudLogExtension
    {
        public static void Error(this ILogger logger,
            string message,
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
            const LogLevel logLevel = LogLevel.Error;
            var logInfo = new CCloudLogInfo()
            {
                ClassFile = Path.GetFileName(sourceFilePath),
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                LineNumber = sourceLineNumber,
                Message = message,
                TraceId = traceId,
                UserId = userId,
                MemberName = memberName,
                Tags = tags,
                LogLevel = logLevel
            };

            logger.Log(logLevel, eventId: EmptyEventId, logInfo, exception, Formatter);
        }

        public static void Warning(this ILogger logger,
            string message,
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
            const LogLevel logLevel = LogLevel.Warning;
            var logInfo = new CCloudLogInfo()
            {
                ClassFile = Path.GetFileNameWithoutExtension(sourceFilePath),
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                LineNumber = sourceLineNumber,
                Message = message,
                TraceId = traceId,
                UserId = userId,
                MemberName = memberName,
                Tags = tags,
                LogLevel = logLevel
            };

            logger.Log(logLevel, eventId: EmptyEventId, logInfo, exception, Formatter);
        }

        // Ϊʲôֻ�� Info ������� Exception �������Ϣ����Ϊ����� Warning �� Error �Ƽ�д���ĸ�ģ��
        public static void Info(this ILogger logger,
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
            // ReSharper disable ExplicitCallerInfoArgument
            Info(logger, null, exception, traceId, userId, memberName, sourceFilePath, sourceLineNumber, tags);
            // ReSharper restore ExplicitCallerInfoArgument
        }

        public static void Info(this ILogger logger, 
            string message,
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

            const LogLevel logLevel = LogLevel.Information;
            var logInfo = new CCloudLogInfo()
            {
                ClassFile = Path.GetFileName(sourceFilePath),
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                LineNumber = sourceLineNumber,
                Message = message,
                TraceId = traceId,
                UserId = userId,
                MemberName = memberName,
                Tags = tags,
                LogLevel = logLevel
            };

            logger.Log(logLevel, eventId: EmptyEventId, logInfo, exception, Formatter);
        }

        public static void Debug(this ILogger logger,
            string message,
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
            var logInfo = new CCloudLogInfo()
            {
                ClassFile = Path.GetFileName(sourceFilePath),
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                LineNumber = sourceLineNumber,
                Message = message,
                TraceId = traceId,
                UserId = userId,
                MemberName = memberName,
                Tags = tags,
                LogLevel = LogLevel.Debug
            };

            logger.Log(logInfo.LogLevel, eventId: EmptyEventId, logInfo, exception, Formatter);
        }

        private static string Formatter(CCloudLogInfo logInfo, Exception exception)
        {
            // honeycomb-log
            // [2099-10-19 19:07:45.456][threadName][INFO][��:����][_traceId:realTraceId][_userId:realUserId][tag:custom] ҵ�����־/�쳣��ջ

            const string empty = "-";

            var traceMessage = string.IsNullOrEmpty(logInfo.TraceId) ? empty : $"_traceId:{logInfo.TraceId}";

            var userMessage = string.IsNullOrEmpty(logInfo.UserId) ? empty : $"_userId:{logInfo.UserId}";

            var logLevelMessage = LogLevelToString(logInfo.LogLevel);

            var logInfoMessage = string.IsNullOrEmpty(logInfo.Message) ? exception?.Message : logInfo.Message;

            return $"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.sss}][{logInfo.ThreadName}:{logInfo.ThreadId}][{logLevelMessage}][{logInfo.ClassFile}:{logInfo.CategoryName}.{logInfo.MemberName}:{logInfo.LineNumber}][{traceMessage}][{userMessage}][tags:{string.Join(";", logInfo.Tags)}] {logInfoMessage} {exception?.ToString()}";
        }

        public static string LogLevelToString(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Trace => "TRACE",
                LogLevel.Debug => "DEBUG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARNING",
                LogLevel.Error => "ERROR",
                LogLevel.Critical => "CRITICAL",
                LogLevel.None => "NONE",
                _ => "NONE"
            };

        private static readonly EventId EmptyEventId = new EventId();
    }
}
