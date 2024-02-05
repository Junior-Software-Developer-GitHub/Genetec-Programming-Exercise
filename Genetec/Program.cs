public sealed class SchedulerTask // this class should not be changed
{
    public TimeSpan Interval { get; }
    public int TaskId { get; }

    private static int nextId = 0;

    public SchedulerTask()
    {
        this.Interval = TimeSpan.FromSeconds(Random.Shared.Next(1, 10)); // run every 1 to 10 seconds
        this.TaskId = nextId++;
    }

    public void Calculate()
    {
        var startTime = DateTime.UtcNow;
        Console.WriteLine($"{startTime.ToLongTimeString()}.{startTime.Millisecond} - ID: {TaskId} - Interval: {Interval.TotalSeconds}");

        var endTime = startTime + TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1500)); // run for at least 0.5, max 1.5 seconds
        while (DateTime.UtcNow < endTime) ; // keep busy
    }

    public async Task CalculateAsync()
    {
        var startTime = DateTime.UtcNow;
        Console.WriteLine($"{startTime.ToLongTimeString()}.{startTime.Millisecond} - ID: {TaskId} - Interval: {Interval.TotalSeconds}");

        var duration = TimeSpan.FromMilliseconds(Random.Shared.Next(500, 1500));

        await Task.Delay(duration);
    }


    static public IEnumerable<SchedulerTask> GetTasks()
    {
        while (true)
        {
            yield return new SchedulerTask();
        }
    }
}

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
        try
        {
            var options = new ParallelOptions()
            {
                CancellationToken = cancellationToken.Token,
                MaxDegreeOfParallelism = 8 //Environment.ProcessorCount
            };
            await Task.Run(() =>
            {
                return Parallel.ForEach(tasks, options, task => PeriodicCalculationAsync(task, task.Interval, cancellationToken.Token));
            });
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Scheduler canceled");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exeption message:{ex.Message}");
        }

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
        if (cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Cancelation Token disposed");

        }
        while (!cancellationToken.IsCancellationRequested)
        {
            //task.Calculate();
            await task.CalculateAsync(); // CPU usage is never > 20% when using CalculateAsync 
            await timer.WaitForNextTickAsync(cancellationToken);
        }
    }

}

public class Program
{
    public static async Task Main()
    {
        // add/modify code here to test with sample data (optional)
        var tasks = SchedulerTask.GetTasks().Take(100).ToList();

        using var scheduler = new Scheduler(tasks);
        await scheduler.Start();

        Console.ReadLine(); // Press the button to stop the program
    }
}

