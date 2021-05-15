# Developer Setup, Requirements and Tools

## Dev Tools
The code provided in this repository does not require any special tools. 
You can use any .NET dev stack, be it command line `dotnet` command or
Visual Studio Code or full Visual Studio 2017 or up.

You can install one of the following for free from Microsoft:
1. Visual Studio 2017 **or** 2019 Community Edition (free) **or** Visual Studio Code (free)
3. Optional: Insomnia - to make API calls **or** Postman
4. Optional: `curl` or `wget` to make calls from command line

## Environment Setup
You may need to setup environment variables.
This can be done on Windows using ControlPanel or "set" command in "dos".
You can use "export" command in Linux bash

```bash
$ export HOME=/home/usr1/tutorial-steps
```

## Windows: Expose Http Listener Port

Run a command window as admin. Run the following command:

```cmd
 > netsh http add urlacl url=http://+:8090/ user=\Everyone
```
Replace "8090" with port number that you are trying to use



