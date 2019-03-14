# DistributedCircuitBreaker.NET

DistributedCircuitBreaker.NET is a Circuit Breaker that allows developers store data and state of the circuit using an external repository such as SqlServer, Redis, MongoDB Etc. by providing a interface. Redis implementation is included.

# Definition

The circuit will break if, within any timespan of duration(defined below), the proportion of actions resulting in a handled exception exceeds failureThreshold (**failureRateAllowed**), provided also that the number of actions through the circuit in the timespan is at least minimumThroughput (**exceptionsAllowedBeforeBreaking**).

Note that the circuit consider any type of `Exception` to set in counters.

# How to Use

The simplest way to use it:
* Add the dependency to IServiceCollection (located usually in Startup class)
```csharp
            services.AddDistributedRedisCircuitBreaker(options =>
            {
                options.DurationOfBreak = TimeSpan.FromSeconds(300);
                options.WindowDuration = TimeSpan.FromSeconds(60);
                options.RedisConnectionConfiguration = "localhost:6379";
            });
```

* Get an instance of CircuitBreaker using the `ICircuitBreakerFactory` interface
* Call Create method to get the Instance of CircuitBreaker

```csharp
    var circuitBreaker = _circuitBreakerFactory.Create("http://www.example.com.br", exceptionsAllowedBeforeBreaking , failureRateAllowed);
```

## Parameters

### key

`key`: A key used as a unique identifier to perform the operations and compute failures and success

### exceptionsAllowedBeforeBreaking

`exceptionsAllowedBeforeBreaking`: Minimum number of failures before starting considering counters

### failureRateAllowed
`failureRateAllowed`: A proportion of failures that should cause the circuit be opened. A decimal between 0 and 1. Example, if 0.5M is set the circuit should open when the proportion of failures exceeds 50% of the executions for the key

** **Both 2 and 3 rules should be broken to make the circuit to be opened.**

# Syntax

```csharp

public class CatalogService : ICatalogService
{
    private readonly ICircuitBreakerFactory _circuitBreakerFactory;

    public CatalogService(ICircuitBreakerFactory circuitBreakerFactory)
    {
        _circuitBreakerFactory = circuitBreakerFactory;
    }

    public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
    {
        var uri = API.Catalog.GetAllCatalogItems(_remoteServiceBaseUrl, page, take, brand, type);

        int exceptionsAllowedBeforeBreaking = 20;
        decimal failureRateAllowed = 0.5M;

        var circuitBreaker = _circuitBreakerFactory.Create(uri.AbsoluteUri, exceptionsAllowedBeforeBreaking , failureRateAllowed);

        Catalog catalog;
        try
        {
            var responseString = await circuitBreaker.ExecuteActionAsync(() =>  _httpClient.GetStringAsync(uri)); 
            catalog = JsonConvert.DeserializeObject<Catalog>(responseString);
        }
        catch(BrokenCircuitException ex)
        {
            //handle open-state circuit
        }

        return catalog;
    }
}

```

If the circuit is open, a **`BrokenCircuitException`** is thrown with the message *"The circuit is now open and is not allowing calls."*. 



# Nuget Packages

 ## DistributedCircuitBreaker

 This is the Circuit Breaker engine. It depends on a implementation of a public repository interface `IDistributedCircuitBreakerRepository`

 ## DistributedCircuitBreakerRedis

 It is the implementation of the interface `IDistributedCircuitBreakerRepository` using a Redis as repository.