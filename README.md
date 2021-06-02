# Azos Framework - a Step by Step Tutorial

A Step-By-Step Guide to using Azos Framework

Start here:
1. [Project Structure Recap](/doc/project-structure.md)
2. [Dev Setup/Requirements/Tools](/doc/dev-setup.md)

All code is under `/src` folder in a single solution. 
The projects are organized in the logical order of conceptual framework feature coverage. 
We recommend that you follow the tutorial in this order:
1. Step 1: **[`s01-console`](/src/s01-console)** - Console application, parse command args, perform some work using console dependency injection/config
2. Step 2: **[`s02-cfglog`](/src/s02-cfglog)** - App Config and Logger
3. Step 3: **[`s03-api`]()** - Simple Http API server
4. Step 4: **[`s04-logic`]()** - Add domain logic module, use DI (Dependency Injection), API protocol
8. Step 5: **[`s05-sec`]()** - Lock API access with security/permissions, add tokens
7. Step 6: Add data store

Additional resource for admin/devops:

[Admin tasks](/doc/admin-tasks.md)