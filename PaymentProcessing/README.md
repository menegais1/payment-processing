## Secrets and Environment Variables

For security purposes, in production I would employ a service like **Doppler** for injecting the environment variables
locally, to keep any sensitive data away from the host computer, and localized in the containers.

## Same Process for Queue processing

Using the same process for queue processing leads to some strange code, as seen in the Program.cs file.
The best practice would be to use two different processes, one that only publishes messages and the other that only
consume messages. With the implemented way, it can still work and be scalable if the scalability is defined by HTTP
Latencies or queue size. But that would always scale both the publisher and the consumer, as they live currently in a
monolith application.

## Idempotency Requests Handling

One of the solutions for guaranteeing idempotency between requests is expecting an idempotency key to be provided by the
API user, giving him control over when a request should not be executed twice. Generally, what happens is that when
receiving a request, if an idempotency key is present, the database or a cache is checked against that key, and if
found,
the existing object is returned, or an error, depending on the implementation.

The problem arises when we have a
distributed system, where each container may handle a request separately and with no specific ordering guarantees. If
two or more requests with the same exact idempotency key are fired, there may be a timeframe that no data is inserted in
the database, and thus all request will pass the key present check and try to execute some piece of code. For payment
transactions, that could mean double spending, which is something we wish to avoid.

One of the ways of removing this
error is guaranteeing request ordering through a queue and a single consumer responsible for deduplicating messages,
which has the downside of introducing a Single Point Of Failure and possible bottleneck in the solution.

If the application is using a SQL
database with ACID guarantees, this job can be offloaded to the database system, where instead of checking if a record
exists, an insertion of the data, considering that the provided idempotent key has a unique constraint in the database,
is performed. The database will then handle locking the index bucket (for simplicity), and further inserts will return a
error to the API, thus indicating that a request was already made and an idempotent response should be handled in the
API.

- In this case, where we only have one instance for processing, the simplest solution was chosen, but the tradeoffs for
  distributed systems were discussed above, and for production-grade code would be employed or further explored for edge
  cases.

### Publishing messages in the endpoint calls VS pooling transactions in processable status

Publishing messages in the endpoint call presents a problem, what if the queue is not online or some network error
happens that prevents a message from being published? Do we keep retrying it, which may case a user timeout, do we
consider the endpoint is only successful if the message is published? How to handle the idempotency if what fails is
the message publishing bit, but the data is inserted in the DB?

In this case, we will consider that the message publishing bit will not fail, but in a production setting the pooling
solution is more robust to failure and also covers the possibility of having scheduled messages in the system.

If pooling is used, we must lock the transaction row in the DB or using a distributed lock with some shared storage to
process the transaction, so even if multiple duplicated messages are present in the queue, no duplicated side effects
will occur to that same transaction. 