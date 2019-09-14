# Test AsyncLocal Middleware Issues
Troubleshooting issues with AsyncLocal when used within a middleware

## What we were trying to achieve
- The TokenAuthenticationMiddleware (simplified here) is supposed to intercept the request, validate the tokens, retrieve corpId and userId value from it and override the values in the `DefaultServiceContext` scoped instance.
   - In this example, to keep things simple, we have replaced the token with alternate `x-auth-` headers that should trump the standard `x-` headers
- There are 2 problems here:
  - By having the `TokenAuthenticationMiddleware` have the following DI dependency chain:
	```
		ILog -> DefaultServiceContext -> ISecurityTokenAccessor
	```
	- We effectively create this conundrum where `DefaulServiceContext` is resolved before the middleware is able to set the values in `ISecurityTokenAccessor` instance
	- We can therefore never overwrite the corp-id and user-id values with the `x-auth-` header values
  - Event without the problem above, AsyncLocal should never keep values from the previous request since they are being set in the middleware, which should be a leaf... (at least that's my understanding...)
    - This is the part that is the most confusing...
	- Interestingly, when running the same scenario using `Microsoft.AspNetCore.TestHost`, the behavior above doesn't occur

## How to reproduce issue
For convenience, use the Postman export located in `artifacts/TestAsyncLocalMiddleware.postman_collection.json` file.
0. Pre-requisite
- Start the service using following command:
```> dotnet run -p ./TestAsyncLocalMiddleware```

1. Make an HTTP Request to service as follows:
- GET /api/values
  - Headers:
    - x-corp-id: 1
    - x-user-id: 5
    - x-auth-corp-id: 10
    - x-auth-user-id: 50
    
- The response returns successfully 1 and 5 (which is already an issue since the dependency chain causes the DefaultServiceContext to be resolved before the middleware overwrites the values)
2. Make a second HTTP request to service as follows:
- GET /api/values
  - Headers:
    - x-corp-id: 2
    - x-user-id: 8
    - x-auth-corp-id: 20
    - x-auth-user-id: 80
 - The middleware crashes because the `SecurityTokenAccessor` keeps items from first request, even though it is backed by AsyncLocal which should guarantee the 2 requests use different logical scopes.
 
## Clues
- The issue is due to the fact the `TokenAuthenticationMiddleware` has a dependency to `ILog` which itself has a dependency to `DefaultServiceContext`, which itself has a dependency to `ISecurityTokenAccessor`.
  - For some reason, this chain of dependency seems to have an effect on the scope of AsyncLocal... It's not very clear why though...
- Interestingly as well, when running the test using `Microsoft.AspNetCore.TestHost`'s `TestServer`, the behavior changes and the values are not being persisted in AsyncLocal between requests...
- Another interesting clue is when running this scenario in IIS Express, the behavior is not occurring as well... Could it be a bug in Kestrel host?

