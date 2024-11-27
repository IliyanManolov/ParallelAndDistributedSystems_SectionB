using System.Diagnostics;


while (true)
{

    Console.Write("Input number of painters you would like to use: ");
    var paintersStr = Console.ReadLine();

    if (!int.TryParse(paintersStr, out var painters) || painters <= 5)
    {
        Console.Clear();
        Console.WriteLine("Please input a valid number of painters > 5");
        continue;
    }

    Console.Write("Input number of circles you would like to be painted: ");

    var elementsStr = Console.ReadLine();

    if (!int.TryParse(elementsStr, out var elements) || elements <= 1000)
    {
        Console.Clear();
        WriteSeparator();
        Console.WriteLine("Please input a valid number of circles to paint > 1000");
        WriteSeparator();
        continue;
    }

    var circlesToPaint = GetCircles(elements);

    await PaintCircles(painters, circlesToPaint);
}

async Task PaintCircles(int paintersCount, IList<Circle> circlesToPaint)
{
    object lockObj = new object();
    int progressPercentage = 0;
    int paintedCircles = 0;

    var watch = new Stopwatch();

    var tasks = new List<Task>();
    int chunkSize = (int)Math.Ceiling((double)circlesToPaint.Count / paintersCount);

    watch.Start();
    Console.WriteLine($"Beginning to paint '{circlesToPaint.Count}' circles with '{paintersCount}' painters");

    for (int i = 0; i < paintersCount; i++)
    {
        var currentStart = i * chunkSize;

        var currentEnd = Math.Min(currentStart + chunkSize - 1, circlesToPaint.Count - 1);

        tasks.Add(Task.Run(() => Paint(currentStart, currentEnd, circlesToPaint)));
    }

    await Task.WhenAll(tasks);


    watch.Stop();
    WriteSeparator();
    Console.WriteLine($"Finished painting '{circlesToPaint.Count}' circles with '{paintersCount}' painters for in '{watch.Elapsed.TotalSeconds}' seconds");
    WriteSeparator();

    if (circlesToPaint.Any(x => x.IsPainted == false))
        throw new ArgumentException($"FAILURE. Found circles not tagged as painted");

    void Paint(int start, int end, IList<Circle> circlesArr)
    {
        for (int i = start; i <= end; i++)
        {
            var currentCircle = circlesArr[i];

            lock (currentCircle)
            {
                if (currentCircle.IsPainted == true)
                    throw new ArgumentException($"Painter '{Thread.CurrentThread.ManagedThreadId}' got assigned an already painted circle with ID '{currentCircle.ID}'");

                //Console.WriteLine($"Painter '{Thread.CurrentThread.ManagedThreadId}' is starting to paint circle with ID '{currentCircle.ID}'");

                Thread.Sleep(TimeSpan.FromMilliseconds(20));
                currentCircle.IsPainted = true;
                Increment();
                //Console.WriteLine($"Painter '{Thread.CurrentThread.ManagedThreadId}' finished painting circle with ID '{currentCircle.ID}'");
            }
        }

        //Console.WriteLine($"Painter '{Thread.CurrentThread.ManagedThreadId}' finished painting his allocated circles");
    }

    void Increment()
    {
        lock (lockObj)
        {
            paintedCircles++;

            int currentPercentage = (paintedCircles * 100) / circlesToPaint.Count;

            // Progress in console will be updated every X%
            // currently configured to 10
            if (currentPercentage >= progressPercentage + 10)
            {
                progressPercentage = currentPercentage;
                Console.WriteLine($"Progress.... {progressPercentage}%");
            }
        }
    }
}


void WriteSeparator()
{
    Console.WriteLine("===============================================================");
}


IList<Circle> GetCircles(int count)
{
    var result = new List<Circle>(count);

    for (int i = 1; i <= count; i++)
    {
        result.Add(new Circle(i));
    }

    return result;
}

public class Circle
{
    public int ID { get; }
    public bool IsPainted { get; set; }
    public Circle(int Id)
    {
        ID = Id;
        IsPainted = false;
    }
}
