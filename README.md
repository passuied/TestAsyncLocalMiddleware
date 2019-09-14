# Test AsyncLocal Middleware Issues
Troubleshooting issues with AsyncLocal when used within a middleware

## How to reproduce issue
1. Make an HTTP Request to service as follows:
- GET /
  - Headers:
    - x-csod-corp-id: 1
    - x-csod-user-id: 5
    - x-auth-corp-id: 10
    - x-auth-user-id: 50
    
- The response returns successfully 1 and 5 (which is already an issue since the dependency chain causes the DefaultServiceContext to be resolved before the middleware overwrites the values)
2. Make a second HTTP request to service as follows:
- GET /
  - Headers:
    - x-csod-corp-id: 2
    - x-csod-user-id: 8
    - x-auth-corp-id: 20
    - x-auth-user-id: 80
 - The middleware crashes because the `SecurityTokenAccessor` keeps items from first request, even though it is backed by AsyncLocal which should guarantee the 2 requests use different logical scopes.
 
## Clues
- The issue is due to the fact the `TokenAuthenticationMiddleware` has a dependency to `ILog` which itself has a dependency to `DefaultServiceContext`, which itself has a dependency to `ISecurityTokenAccessor`.
 - For some reason, this chain of dependency seems to have an effect on the scope of AsyncLocal... It's not very clear why though...
