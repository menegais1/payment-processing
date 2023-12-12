## Using Exceptions for control flow

Although exceptions present a tradeoff for performance at some points, I believe they lead to cleaner flows for data
validation.

## Same Process for Queue processing

Using the same process for queue processing leads to some strange code, as seen in the Program.cs file.
The best practice would be to use two different processes, one that only publishes messages and the other that only
consume messages. With the implemented way, it can still work and be scalable if the scalability is defined by HTTP
Latencies or queue size. But that would always scale both the publisher and the consumer, as they live currently in a
monolith application. 
