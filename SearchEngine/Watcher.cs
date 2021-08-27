using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Hangfire.LiteDB;

class d:IBackgroundProcess{
    public void Execute(BackgroundProcessContext context){
        Console.WriteLine("ayooooo");
        context.Wait(TimeSpan.FromMinutes(1));
    }

}

BackgroundProcessContext
namespace SearchEngine{
public class Watcher
    {
        private Engine engine;
        public Watcher(Engine eng)
        {
            GlobalConfiguration.Configuration.UseLiteDbStorage();

            engine = eng;
        }

        public void Init(){
            var client = new BackgroundJobServer();
            // RecurringJob.AddOrUpdate(() => Console.WriteLine("gojo gojo"), Cron.Minutely);
            new d();
        }
    }
}