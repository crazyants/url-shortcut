# Repository name: url-shortcut
A simple URL shortener web API application.

## Repo Description
I decided to challenge myself by implementing a URL shortener web application.

### Problem Domain
* I assumed the system would be serving lots of requests world-wide. Therefore it has to be fast, available, and expandable.
* Additionally, it has to generate short URL to be able to compete existing options in the market.

### Solution
The first set of requirements are easily satisfiable by using Cassandra database. It's fast, reliable, and expandable. However,the real challenge is to implement an algorithm to provide really short URL or a unique signature identifying that URL. The first idea might be a time-based approach. But that's not sufficient as UUIDs/GUIDs combine time, ip, algorithm version, and emergency code to make a unique identifier. UUIDs are long in characters and they are not the best solution to our problem. Then I thought it might be a great idea to take advantage of base-62 (upper case, lower case, and numbers). This is possible by maintaining a global counter that counts the total number of stored URL in the database. Although Cassandra offers counter table but it does not provide locking mechanism to prevent race condition. Hence I decided to implement a Windows Service in order to maintain a shared memory for the global counter.

## Architecture
The web application receives a shortening request from the user. First, it looks up the database to see if the given URL had been shortened before. If yes, short form will be queried and returned to the user. Otherwise, the application connects to the service and gets the total count. Then, it attempts to convert the number into base-62. The converted number is then used as the short form of that specific URL and it is stored in the database. The process of storing a new URL in database involves incrementing the counter table as well.

On the other hand, the service, upon start, is initialized by reading the counter table and storing the counter value in memory. Upon every socket connection, the service locks the shared memory, it reads the value, increases the value by one, and then it unlocks the shared memory. The service communicates through AsyncSocket. This leads to a situation where each request is on a separats thread. Therefore the locking mechanism is catered perfectly.

Last but not least, the service must read the most updated data from database upon initialization. Hence, it establishes a connection with Consistency Level of ALL. This is an absolute necessity.

***Why Cassandra?** Cassandra offers linear scalability. This means double performance cnould be achieved by doubling the number of nodes in a cluster or a data center. Moreover, all nodes are equally important where there is no master-slave configuration.*

***Note:** The cluster configuration must be consistent. A cluster that is evantually consistent is not recommended.*

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
[AsyncServerSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut%20Service/AsyncServerSocket.cs)
[AsyncClientSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Network/AsyncClientSocket.cs)
[SyncClientSocket.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Network/SyncClientSocket.cs)
[CassandraConnection.cs](https://github.com/kamyar-nemati/url-shortcut/blob/master/URL%20Shortcut/URL%20Shortcut/Utils/Database/CassandraConnection.cs)
