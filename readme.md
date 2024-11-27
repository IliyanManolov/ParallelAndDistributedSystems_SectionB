
## Program structure

The program includes a continious menu for selecting the number of painters (threads) and circles (elements) for painting.
Validations include:
- Painters must be > 5 (as per module book requirements)
- Cirlces must be > 1000 (as per module book requirements)

The task solution utilises local functions for auxiliary functions.
The `Circle` class used in the task contains 2 properties - int `id` and bool `isPainted` used for validating that all circles have been visited by the program.

The task's execution times were tested with arrays of 50 000 elements.
Additionally each execution was tested on a fresh project build in order for caching & optimisations to not sway the results

**NOTE**: the first test is with 6 threads since 5 conflicts with the minimum requirements stated in the module book.

All additional classes are located in Program.cs in order to ease the submission of files.
There are additional Console messages that are commended out due to their large volume when testing with a lot of threads and elements. They can be used to showcase the parallel execution

## Task Results

Thread count : Execution time in seconds using [s,fffffff] format

- 6 : 261,7017192
- 20 : 81,7525128
- 100: 47,6485639

## Task Evalution:

- Is this problem able to be parallelized? 
    - Yes
- How would the problem be partitioned?
    - Divide the array into "chunks" that do not overlap with each other and iterate through every chunk.
- Are communications needed?
    - In this approach, yes. Communication is light and only used for tracking to total number of already painted circles. 
    If we do not track progress % but instead write `X painter finished painting Y circle` to track progress no communication would be needed.
- Are there any data dependencies?
    - In this approach, no. 
- Are there synchronization needs?
    - In this approach, no
- Will load balancing be a concern?
    - In this approach, no. The array is split as evenly as possible and all circle are assumed to take the same amount of time to paint and movement is assumed instantaneous