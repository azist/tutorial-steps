# Root class lib

The root class library is used by all assemblies in this solution
It contains solution-wide common types configuration snippets which are 
cross-used by applications in the solution.

In a true business-oriented system you would have some core domain types
such as very base classes.

In this solution this serves no other purpose but create a **build 
step which deploys common config snippets** `cfg/` subdirectory into `out/`