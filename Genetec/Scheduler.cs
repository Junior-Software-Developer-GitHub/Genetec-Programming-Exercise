namespace Genetec
{
    public class Scheduler : IDisposable
    {
        private List<SchedulerTask> tasks;
        private readonly CancellationTokenSource cancellationToken;

        public Scheduler(List<SchedulerTask> tasks)
        {
            this.tasks = tasks;
            cancellationToken = new CancellationTokenSource();
        }

        public async Task Start() => await RunTasksAsync();

        private async Task RunTasksAsync()
        {
            var options = new ParallelOptions
            {
                CancellationToken = cancellationToken.Token,
                MaxDegreeOfParallelism = 4
            };

            var semaphore = new SemaphoreSlim(4);

            var tasks = this.tasks.Select(async task =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await PeriodicCalculationAsync(task, task.Interval, cancellationToken.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Scheduler canceled");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception message: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }



        public void Dispose()
        {
            cancellationToken.Cancel();
            Console.WriteLine("Cancelation Token disposed");

            cancellationToken.Dispose();
            Console.WriteLine("Scheduler disposed");
        }

        public static async Task PeriodicCalculationAsync(SchedulerTask task, TimeSpan interval, CancellationToken cancellationToken)
        {
            using PeriodicTimer timer = new(interval);
            while (!cancellationToken.IsCancellationRequested)
            {
                task.Calculate();
                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

    }

}
