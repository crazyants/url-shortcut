# Repository name: url-shortcut
A simple URL shortener Web API.

## Repo Description
A URL shortener web application that provides short hash or code for any given string (URL).

## Problem Domain
* The system shall be capable of serving lots of requests world-wide. Therefore, concurrency is important and it must be fast, reliable, and expandable.
* The system must generate short URL to be able to compete existing options operating in the market.

## Solution
### Architecture and Data Flow
The first requirement is easily satisfiable by using a distributed database such as Cassandra. The real challenge, however, is the second set of the requirements that is to implement an algorithm to provide short unique signature or hash for any given URL. The very first idea I got in mind in order to achive a really-short hash that is guaranteed to be unique was converting unique decimal numbers to another base like 16 (Hexadecimal). Nonetheless, base 16 can get bigger as we have 26 small and another 26 capital letters in our English alphabet, plus 10 numbers ready to serve our need. So now there is a way to conver unique decimals to unique hashes by translating them into base-62. Now we need to find a way to make unique decimal numbers.

I have decided to maintain a *Global Counter* that counts the total number of shortened/stored URLs in the database. As new shortening requests come in, the counter increases. Hence, each request can be provided with a unique decimal number or ID (starting from zero for the very first request as there is no shortened URL in the database at that moment). Although Cassandra offers Counter Table but it does not provide locking mechanism to prevent race condition in situations where there are concurrent requests.

In order to overcome the concurrency problem mentioned above, I have decided to implement a *Service* as a locking mechanism to control the Global Counter. In the following, I have described how the Service handles the Global Counter.

The Service is initialized at the time when the website (Web API) is started by web server. Therefore, the life cycle of the Service is beyond the life cycle of each request that is sent to the website. Upon Service initiation, the value of the Global Counter is read and stored by the Service from the database. Each shortening request requires a *Read* operation that is followed by a *Write* operation. The Read is to read the value of the Global Counter from the Service by the request subroutine and the Write is to increament the counter by one. These two operations are atomic and the Service is locked during this atomic process. The original Global Counter that is stored in the database is updated by each request as well. Therefore, accidental or sudden termination of the Service is safe as it acquires the value of the Global Counter from the database.

Since we now have the unique decimal number, we can convert it into base-62 and the resulting hash can be assigned to the URL for identification purposes. The hash is then returned to the client who made the request in the form of a shorter URL which includes the hash. Using the shorter URL, we can create an API on our website to accept the hash from the short URL and query the database to find the original URL and ultimately redirect the user or client to the designated location.

***Why Cassandra?** Cassandra offers linear scalability. This means double performance cnould be achieved by doubling the number of nodes in a cluster or a data center. Moreover, all nodes are equally important where there is no master-slave configuration.*

***Note:** The cluster configuration must be **Consistent**. A cluster that is **Evantually Consistent** is not recommended.*

## Data Flow Diagram
![DFD: URL Shortcut](https://github.com/kamyar-nemati/url-shortcut/blob/master/DFD%20-%20URL_Shortcut.png?raw=true "DFD: URL Shortcut")

## Entity Diagram
![ED: URL Shortcut](https://raw.githubusercontent.com/kamyar-nemati/url-shortcut/master/ED%20-%20URL_Shortcut.png "ED: URL Shortcut")

## Screenshot
![URL Shortener API](https://user-images.githubusercontent.com/29518086/31230415-86e71f28-aa16-11e7-8876-2a74f2146dc4.PNG "URL Shortener API")

## Projects
1. URL Shortcut _(ASP.NET Core (WebAPI))_
2. URL Shortcut Service _(Windows Service)_

## Dependencies
1. DotNet Core 2.0
2. DotNET 4.7
3. Cassandra database _(cqlsh)_
4. CassandraCSharpDriver 3.3.2

## Future Work
1. URL validation; To validate input long URL before storing in database.
2. Authentication; To implement a procedure to issue authorization before processing user's input.

## My Favorite Classes
* [AsyncServerSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut%20Service/AsyncServerSocket.cs)
* [AsyncClientSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Network/AsyncClientSocket.cs)
* [SyncClientSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Network/SyncClientSocket.cs)
* [CassandraConnection.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Database/CassandraConnection.cs)
