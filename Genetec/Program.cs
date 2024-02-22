using Genetec;

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

