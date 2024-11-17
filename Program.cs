

using System.Diagnostics;

const int painters = 20;
const int circlesToPaint = 10_000;


await PaintCircles(painters, circlesToPaint);

async Task PaintCircles(int paintersCount, int circlesToPaint)
{

    if (paintersCount < 5)
    {
        Console.WriteLine($"Insufficient painters selected, minimum number is '5'");
        return;
    }

    if (circlesToPaint < 1000)
    {
        Console.WriteLine($"Insufficient number of circles to be painted selected, minimum number if '1000'");
        return;
    }



    var tasks = new List<Task>();
    int paintedCircles = 0;

    object lockObj = new object();
    var watch = new Stopwatch();

    watch.Start();
    Console.WriteLine($"Beginning to paint '{circlesToPaint}' with '{paintersCount}' painters");

    for (int i = 0; i < paintersCount; i++)
    {
        tasks.Add(Task.Run(() => Paint()));
    }

    await Task.WhenAll(tasks);


    watch.Stop();
    Console.WriteLine($"Finished painting '{paintedCircles}' with '{paintersCount}' painter for time '{watch.Elapsed.TotalSeconds}' seconds");


    // TBD: increase inside lock before sleeping or after

    // Increase before sleep = mark that the last circle WILL be painted, otherwise 2+ workers can try to paint and it time is wasted on all but 1
    void Paint()
    {

        Console.WriteLine($"Painter with thread ID {Thread.CurrentThread.ManagedThreadId} is starting....");

        while (true)
        {
            // Lock to check if all circles have been painted and signal that we are painting a new one
            lock (lockObj)
            {
                if (paintedCircles == circlesToPaint)
                {
                    Console.WriteLine($"Painter {Thread.CurrentThread.ManagedThreadId} found that all circles are painted and is stopping work...");
                    return;
                }

                else if (paintedCircles > circlesToPaint)
                    throw new Exception($"Painter {Thread.CurrentThread.ManagedThreadId} found that more circles have been painted than required. Painted - '{paintedCircles}', required - {circlesToPaint}");

                paintedCircles++;
                Console.WriteLine($"Painter {Thread.CurrentThread.ManagedThreadId} is starting to paint circle #{paintedCircles}");
            }

            Thread.Sleep(20);
            //await Task.Delay(TimeSpan.FromMilliseconds(20));
            Console.WriteLine($"Painter {Thread.CurrentThread.ManagedThreadId} finished painting circle....");
        }
    }
} 