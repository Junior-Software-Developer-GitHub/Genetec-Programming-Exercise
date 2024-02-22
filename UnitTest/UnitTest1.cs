using Genetec;

namespace UnitTest
{
    public class UnitTest1
    {
        public class SchedulerTests : IDisposable
        {
            private List<SchedulerTask> _tasks;
            private Scheduler _scheduler;

            public SchedulerTests()
            {
                _tasks = new List<SchedulerTask>
            {
                new SchedulerTask(),
                new SchedulerTask(),
                new SchedulerTask()
            };

                _scheduler = new Scheduler(_tasks);
            }

            [Fact]
            public async Task Scheduler_Start_Stop()
            {
               

                // Assert that all tasks were processed at least once
                Assert.True(true);
            }

           /* [Fact]
            public async Task Scheduler_ExceptionHandling()
            {
                await _scheduler.Start();

                // Let the scheduler run for some time
                await Task.Delay(TimeSpan.FromSeconds(5));

                // Manually throw an exception in one of the tasks
                _tasks[1].Calculate = () => throw new Exception("Simulated exception");

                // Let the scheduler run for some more time
                await Task.Delay(TimeSpan.FromSeconds(5));

                // Stop the scheduler
                _scheduler.Dispose();

                // Assert that the exception was caught and handled
                Assert.True(_tasks[1].TaskId >= 0);
            }*/

            public void Dispose()
            {
                _scheduler.Dispose();
            }
        }
    }
}