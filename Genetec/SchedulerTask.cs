namespace Genetec
{
    public sealed class SchedulerTask // this class should not be changed
    {
        public TimeSpan Interval { get; }
        public int TaskId { get; }

        private static int nextId = 0;

        public SchedulerTask()
        {
            this.Interval = TimeSpan.FromSeconds(Random.Shared.Next(10, 20)); // run every 1 to 10 seconds
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
}
